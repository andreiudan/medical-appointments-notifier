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

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>, 
    IRecipient<UserUpdatedMessage>, IRecipient<AppointmentAddedMessage>,
    IRecipient<AppointmentUpdatedMessage>, IRecipient<NoteAddedMessage>, IRecipient<NoteUpdatedMessage>, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<UserModel> shownUsers = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private UserModel selectedUser = new();

    public AppointmentModel FirstScheduledAppointment => ScheduledAppointments.FirstOrDefault();

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

        if(_users.Count == 0)
        {
            logger.LogInformation("No users found in the database.");
            return;
        }

        Users.Clear();
        ShownUsers.Clear();
        foreach (var user in _users)
        {
            Users.Add(mapper.Map(user));
        }

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
        int index = Users.Count;

        if (message.user.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
           message.user.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        {
            ShownUsers.Add(message.user);
            index = ShownUsers.Count;
        }

        OnSelectedUserSwitched?.Invoke(this, index - 1);
    }

    public void Receive(UserUpdatedMessage message)
    {
        SelectedUser = message.user;

        UserModel originalUser = Users.First(u => u.Id.Equals(message.user.Id));

        int index = Users.IndexOf(originalUser);
        Users[index] = message.user;

        index = ShownUsers.IndexOf(originalUser);
        if(index < 0)
        {
            return;
        }

        ShownUsers[index] = message.user;
    }

    public void Receive(AppointmentAddedMessage message)
    {
        switch(message.appointment.Status)
        {
            case AppointmentStatus.Programat:
                ScheduledAppointments.Add(message.appointment);
                break;
            case AppointmentStatus.Neprogramat:
                ExpiringAppointments.Add(message.appointment);
                break;
            case AppointmentStatus.Finalizat:
                PastAppointments.Add(message.appointment);
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
            List<AppointmentModel> temp = new List<AppointmentModel>(ScheduledAppointments);

            int index = temp.IndexOf(appointment);
            if (index < 0)
            {
                return;
            }

            temp.RemoveAt(index);
            ScheduledAppointments = new ObservableCollection<AppointmentModel>(temp);
            ExpiringAppointments.Add(appointment);
        }
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
        LoadUpcomingAppointments();
    }

    private async Task LoadUpcomingAppointments()
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
