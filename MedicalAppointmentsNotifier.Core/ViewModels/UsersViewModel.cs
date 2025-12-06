using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>, IRecipient<AppointmentAddedMessage>,
    IRecipient<AppointmentUpdatedMessage>, IRecipient<NoteAddedMessage>, IRecipient<NoteUpdatedMessage>, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<UserModel> shownUsers = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private UserModel selectedUser = new();

    public AppointmentModel? FirstScheduledAppointment => ScheduledAppointments.FirstOrDefault() ?? new();

    [NotifyPropertyChangedFor(nameof(FirstScheduledAppointment))]
    [ObservableProperty]
    private ObservableCollection<AppointmentModel> scheduledAppointments = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> expiringAppointments = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> pastAppointments = new();

    [ObservableProperty]
    private ObservableCollection<NoteModel> notes = new();

    public IAsyncRelayCommand LoadUsersCommand { get; }

    private List<UserModel> Users { get; set; } = new();

    private readonly IAppointmentsRepository appointmentsRepository;
    private readonly IRepository<Note> notesRepository;
    private readonly ILogger<UsersViewModel> logger;
    private readonly IEntityToModelMapper mapper;

    public delegate void SelectedUserSwitchedEventHandler(object sender, int newIndex);
    public event SelectedUserSwitchedEventHandler OnSelectedUserSwitched;

    public UsersViewModel(IAppointmentsRepository appointmentsRepository, IRepository<Note> notesRepository, ILogger<UsersViewModel> logger, IEntityToModelMapper mapper)
    {
        this.appointmentsRepository = appointmentsRepository ?? throw new ArgumentNullException(nameof(appointmentsRepository));
        this.notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);

        IsActive = true;
    }

    public UsersViewModel()
    {
    }

    private async Task LoadUsersAsync()
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();

        logger.LogInformation("Loading users from the database.");
        List<User> _users = await usersRepository.GetAllAsync();

        if (_users.Count == 0)
        {
            logger.LogInformation("No users found in the database.");
            return;
        }

        Users.Clear();
        ShownUsers.Clear();
        foreach (var user in _users)
        {
            Users.Add(await mapper.Map(user));
        }
        ReorderUsersCollection();

        ShownUsers = new ObservableCollection<UserModel>(Users);
        OnSelectedUserSwitched?.Invoke(this, 0);
        logger.LogInformation("Loaded {UserCount} users from the database.", Users.Count);
    }

    partial void OnSearchTextChanging(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ShownUsers = new ObservableCollection<UserModel>(Users);
            return;
        }

        ShownUsers = new ObservableCollection<UserModel>(
            Users.Where(u => u.FirstName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                             u.LastName.Contains(value, StringComparison.OrdinalIgnoreCase))
        );
    }

    public void Receive(UserAddedMessage message)
    {
        Users.Add(message.user);
        ReorderUsersCollection();
        int index = Users.Count;

        if (message.user.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
           message.user.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        {
            ShownUsers.Add(message.user);
            ReorderShownUsersCollection();
            index = ShownUsers.Count;
        }

        OnSelectedUserSwitched?.Invoke(this, index - 1);
    }

    public void Receive(AppointmentAddedMessage message)
    {
        switch(message.appointment.Status)
        {
            case AppointmentStatus.Programat:
                AddAppointmentToSchedluedCollection(message.appointment);
                break;
            case AppointmentStatus.Neprogramat:
                AddAppointmentToExpiringCollection(message.appointment);
                break;
            case AppointmentStatus.Finalizat:
                AddAppointmentToPastCollection(message.appointment);
                break;
            default:
                logger.LogWarning("Received appointment with unknown status {AppointmentStatus}", message.appointment.Status);
                return;
        }

        logger.LogInformation("Added appointment with ID {AppointmentId} to user with ID {UserId}", message.appointment.Id, selectedUser.Id);
    }

    public void Receive(AppointmentUpdatedMessage message)
    {
        AppointmentModel oldAppointment;
        int index;

        if(ScheduledAppointments.Any(a => a.Id.Equals(message.appointment.Id)))
        {
            oldAppointment = ScheduledAppointments.FirstOrDefault(a => a.Id.Equals(message.appointment.Id));
            index = ScheduledAppointments.IndexOf(oldAppointment);
        }
        else if(ExpiringAppointments.Any(a => a.Id.Equals(message.appointment.Id)))
        {
            oldAppointment = ExpiringAppointments.FirstOrDefault(a => a.Id.Equals(message.appointment.Id));
            index = ExpiringAppointments.IndexOf(oldAppointment);
        }
        else if(PastAppointments.Any(a => a.Id.Equals(message.appointment.Id)))
        {
            oldAppointment = PastAppointments.FirstOrDefault(a => a.Id.Equals(message.appointment.Id));
            index = PastAppointments.IndexOf(oldAppointment);
        }
        else
        {
            logger.LogWarning("Received update for appointment with ID {AppointmentId} which does not exist in any collection", message.appointment.Id);
            return;
        }

        if(oldAppointment.Status != message.appointment.Status)
        {
            //Status has changed, move between collections
            switch(oldAppointment.Status)
            {
                case AppointmentStatus.Programat:
                    ScheduledAppointments = RemoveAppointmentFromCollection(ScheduledAppointments, oldAppointment);
                    break;
                case AppointmentStatus.Neprogramat:
                    ExpiringAppointments = RemoveAppointmentFromCollection(ExpiringAppointments, oldAppointment);
                    break;
                case AppointmentStatus.Finalizat:
                    PastAppointments = RemoveAppointmentFromCollection(PastAppointments, oldAppointment);
                    break;
            }
            switch(message.appointment.Status)
            {
                case AppointmentStatus.Programat:
                    AddAppointmentToSchedluedCollection(message.appointment);
                    break;
                case AppointmentStatus.Neprogramat:
                    AddAppointmentToExpiringCollection(message.appointment);
                    break;
                case AppointmentStatus.Finalizat:
                    AddAppointmentToPastCollection(message.appointment);
                    break;
            }
        }
        else
        {
            //Status is the same, just update in place
            switch(message.appointment.Status)
            {
                case AppointmentStatus.Programat:
                    ScheduledAppointments[index] = message.appointment;
                    ReorderScheduledAppointmentsCollection();
                    break;
                case AppointmentStatus.Neprogramat:
                    ExpiringAppointments[index] = message.appointment;
                    ReorderExpiringAppointmentsOrder();
                    break;
                case AppointmentStatus.Finalizat:
                    PastAppointments[index] = message.appointment;
                    ReorderPastAppointmentsCollection();
                    break;
            }
        }

        logger.LogInformation("Updated appointment with ID {AppointmentId}", message.appointment.Id);
    }

    public void Receive(NoteAddedMessage message)
    {
        Notes.Add(message.note);
        logger.LogInformation("Added note with ID {NoteId} to user with ID {UserId}", message.note.Id, SelectedUser.Id);
    }

    public void Receive(NoteUpdatedMessage message)
    {
        int index = Notes.IndexOf(Notes.FirstOrDefault(n => n.Id.Equals(message.note.Id)));
        if(index < 0)
        {
            return;
        }
        Notes[index] = message.note;
        logger.LogInformation("Updated note with ID {AppointmentId}", message.note.Id);
    }

    private void ReorderShownUsersCollection()
    {
        if(ShownUsers is null || ShownUsers.Count <= 0)
        {
            return;
        }

        ShownUsers = new ObservableCollection<UserModel>(ShownUsers.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList());
    }

    private void ReorderUsersCollection()
    {
        if(Users is null || Users.Count <= 0)
        {
            return;
        }

        Users = Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
    }

    private void AddUserToCollections(UserModel user)
    {
        Users.Add(user);
        ReorderUsersCollection();

        if (user.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
           user.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        {
            ShownUsers.Add(user);
            ReorderShownUsersCollection();
        }
    }

    private void ReorderScheduledAppointmentsCollection()
    {
        ScheduledAppointments = new ObservableCollection<AppointmentModel>(ScheduledAppointments.OrderBy(a => a.DaysUntilScheduled).ToList());
    }

    private void ReorderExpiringAppointmentsOrder()
    {
        ExpiringAppointments = new ObservableCollection<AppointmentModel>(ExpiringAppointments.OrderBy(a => a.DaysUntilExpiry).ToList());
    }

    private void ReorderPastAppointmentsCollection()
    {
        PastAppointments = new ObservableCollection<AppointmentModel>(PastAppointments.OrderBy(a => a.MedicalSpecialty).ToList());
    }

    private void AddAppointmentToSchedluedCollection(AppointmentModel appointment)
    {
        ScheduledAppointments.Add(appointment);
        ReorderScheduledAppointmentsCollection();
        UpdateSelectedUserInfo();
    }

    private void AddAppointmentToExpiringCollection(AppointmentModel appointment)
    {
        ExpiringAppointments.Add(appointment);
        ReorderExpiringAppointmentsOrder();
        UpdateSelectedUserInfo();
    }

    private void AddAppointmentToPastCollection(AppointmentModel appointment)
    {
        PastAppointments.Add(appointment);
        ReorderPastAppointmentsCollection();
        UpdateSelectedUserInfo();
    }

    private async Task DeleteSelectedUser()
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();

        bool deleted = await usersRepository.DeleteAsync(SelectedUser.Id);
        if (!deleted)
        {
            logger.LogWarning("Deletion of user with Id: {userId} failed.", SelectedUser.Id);
            return;
        }

        Users.Remove(SelectedUser);

        List<UserModel> shownUsersTemp = new List<UserModel>(ShownUsers);
            
        int indexToRemove = ShownUsers.IndexOf(SelectedUser);
        shownUsersTemp.RemoveAt(indexToRemove);

        ShownUsers = new ObservableCollection<UserModel>(shownUsersTemp);
        
        logger.LogInformation("Deleted user with Id: {userId}.", SelectedUser.Id);

        if(Users.Count <= 0)
        {
            return;
        }

        if (ShownUsers.Count <= 0)
        {
            ShownUsers = new ObservableCollection<UserModel>(Users);
        }

        OnSelectedUserSwitched?.Invoke(this, 0);
    }

    private async Task DeleteAppointment(AppointmentModel appointment)
    {
        bool deleted = await appointmentsRepository.DeleteAsync(appointment.Id);
        if (deleted)
        {
            ScheduledAppointments = RemoveAppointmentFromCollection(ScheduledAppointments, appointment);
            ExpiringAppointments = RemoveAppointmentFromCollection(ExpiringAppointments, appointment);
            PastAppointments = RemoveAppointmentFromCollection(PastAppointments, appointment);
            logger.LogInformation("Deleted appointment with Id: {appointmentId}.", appointment.Id);
        }
    }

    /// <summary>
    /// Provides a safe way to remove an entry from an ObservableCollection
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="appointment"></param>
    /// <returns></returns>
    //Using remove directly on the ObservableCollection was throwing an index error sometimes
    private ObservableCollection<AppointmentModel> RemoveAppointmentFromCollection(ObservableCollection<AppointmentModel> collection, AppointmentModel appointment)
    {
        List<AppointmentModel> temp = new List<AppointmentModel>(collection);
        if (temp.Contains(appointment))
        {
            temp.Remove(appointment);
        }

        UpdateSelectedUserInfo();
        return new ObservableCollection<AppointmentModel>(temp);
    }

    private async Task DeleteNote(NoteModel note)
    {
        bool deleted = await notesRepository.DeleteAsync(note.Id);
        if (deleted)
        {
            List<NoteModel> temp = new List<NoteModel>(Notes);
            temp.Remove(note);
            Notes = new ObservableCollection<NoteModel>(temp);
            logger.LogInformation("Deleted note with Id: {noteId}.", note.Id);
        }
    }

    public async Task DeleteEntry(object sender)
    {
        if(sender is null)
        {
            return;
        }

        if (sender is AppointmentModel appointment)
        {
            DeleteAppointment(appointment);
        }
        else if (sender is NoteModel note)
        {
            DeleteNote(note);
        }
        else if(sender is UsersViewModel)
        {
            DeleteSelectedUser();
        }
    }

    public async Task FinalizeAppointment(AppointmentModel appointment)
    {
        if(appointment is null)
        {
            return;
        }

        appointment.Status = AppointmentStatus.Finalizat;
        bool updated = await appointmentsRepository.UpdateAsync(mapper.Map(appointment, SelectedUser.Id));
        if (updated)
        {
            RemoveAppointmentFromCollection(ScheduledAppointments, appointment);
            AddAppointmentToPastCollection(appointment);
        }
    }

    private void UpdateSelectedUserInfo()
    {
        UserModel user = Users.FirstOrDefault(u => u.Id.Equals(SelectedUser.Id));
        
        if (user == null)
        {
            logger.LogWarning("Could not find user with Id: {userId} to update info.", SelectedUser.Id);
            return;
        }

        SelectedUser.UpcominAppointmentsCount = ScheduledAppointments.Count;
        SelectedUser.ExpiringAppointmentsCount = ExpiringAppointments.Count;

        int index = Users.IndexOf(user);
        Users[index] = SelectedUser;
        index = ShownUsers.IndexOf(user);
        ShownUsers[index].UpcominAppointmentsCount = ScheduledAppointments.Count;
        ShownUsers[index].ExpiringAppointmentsCount = ExpiringAppointments.Count;
    }

    public async Task SwitchSelectedUser(Guid userId)
    {
        if (SelectedUser.Id.Equals(userId))
        {
            return;
        }

        PastAppointments.Clear();
        ExpiringAppointments.Clear();
        ScheduledAppointments.Clear();
        Notes.Clear();

        SelectedUser = Users.FirstOrDefault(u => u.Id.Equals(userId));

        LoadPastAppointments();
        LoadExpiringAppointments();
        LoadNotes();
        await LoadScheduledAppointments();
    }

    private async Task LoadScheduledAppointments()
    {
        List<Appointment> appointments = await appointmentsRepository.GetUpcomingAppointments(SelectedUser.Id);
        List<AppointmentModel> temp = new List<AppointmentModel>();

        ScheduledAppointments.Clear();
        foreach (var appointment in appointments)
        {
            temp.Add(mapper.Map(appointment));
        }

        ScheduledAppointments = new ObservableCollection<AppointmentModel>(temp);
    }

    private async Task LoadExpiringAppointments()
    {
        List<Appointment> appointments = await appointmentsRepository.GetExpiringAppointments(SelectedUser.Id);

        ExpiringAppointments.Clear();
        foreach (var appointment in appointments)
        {
            ExpiringAppointments.Add(mapper.Map(appointment));
        }
    }

    private async Task LoadPastAppointments()
    {
        List<Appointment> appointments = await appointmentsRepository.GetPastAppointments(SelectedUser.Id);

        PastAppointments.Clear();
        foreach (var appointment in appointments)
        {
            PastAppointments.Add(mapper.Map(appointment));
        }

        ReorderPastAppointmentsCollection();
    }

    private async Task LoadNotes()
    {
        IEnumerable<Note> notes = await notesRepository.FindAllAsync(n => n.UserId.Equals(SelectedUser.Id));

        Notes.Clear();
        foreach (Note note in notes)
        {
            Notes.Add(mapper.Map(note));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            IsActive = false;
            WeakReferenceMessenger.Default.UnregisterAll(this);

            Users.Clear();
            Users = null;
        }
    }
}
