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
    IRecipient<AppointmentStatusChangedMessage>, IRecipient<NoteAddedMessage>, IDisposable
{
    private List<UserModel> Users { get; set; } = new();

    [ObservableProperty]
    private ObservableCollection<UserModel> shownUsers = new();

    [ObservableProperty]
    private UserModel selectedUser = new();

    [NotifyPropertyChangedFor(nameof(FirstScheduledAppointment))]
    [ObservableProperty]
    private ObservableCollection<AppointmentModel> scheduledAppointments = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> expiringAppointments = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> pastAppointments = new();

    [ObservableProperty]
    private ObservableCollection<NoteModel> notes = new();

    private string searchText = string.Empty;

    public string SearchText
    {
        get => searchText;
        set
        {
            SetProperty(ref searchText, value);
            OnSearchTextChanging(value);
        }
    }

    public AppointmentModel? FirstScheduledAppointment => ScheduledAppointments.FirstOrDefault() ?? new();

    public IAsyncRelayCommand LoadUsersCommand { get; }

    private readonly IAppointmentsRepository appointmentsRepository;
    private readonly IRepository<Note> notesRepository;
    private readonly ILogger<UsersViewModel> logger;
    private readonly IEntityToModelMapper mapper;

    private CancellationTokenSource switchUserCancellationTokenSource;

    public delegate void SelectedUserSwitchedEventHandler(object sender, int newIndex);
    public event SelectedUserSwitchedEventHandler OnSelectedUserSwitched;

    public UsersViewModel(IAppointmentsRepository appointmentsRepository, IRepository<Note> notesRepository, ILogger<UsersViewModel> logger, IEntityToModelMapper mapper)
    {
        this.appointmentsRepository = appointmentsRepository ?? throw new ArgumentNullException(nameof(appointmentsRepository));
        this.notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);

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

    private void OnSearchTextChanging(string value)
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
        switch (message.appointment.Status)
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
    }

    public void Receive(AppointmentStatusChangedMessage message)
    {
        if(message.oldStatus == AppointmentStatus.Neprogramat)
        {
            ExpiringAppointments = RemoveAppointmentFromCollection(ExpiringAppointments, message.appointment);
            AddAppointmentToSchedluedCollection(message.appointment);
        }
        else
        {
            ScheduledAppointments = RemoveAppointmentFromCollection(ScheduledAppointments, message.appointment);
            AddAppointmentToExpiringCollection(message.appointment);
        }

        logger.LogInformation("Appointment with Id:{AppointmentId} changed status from {oldStatus} to {newStatus}.", message.appointment.Id, message.oldStatus, message.appointment.Status);
    }

    public void Receive(NoteAddedMessage message)
    {
        Notes.Add(message.note);
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

            UpdateSelectedUserInfo();
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

        AppointmentModel tempAppointment = temp.FirstOrDefault(a => a.Id.Equals(appointment.Id));
        if(tempAppointment != default)
        {
            temp.Remove(tempAppointment);
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

        try
        {
            if (sender is AppointmentModel appointment)
            {
                await DeleteAppointment(appointment);
            }
            else if (sender is NoteModel note)
            {
                await DeleteNote(note);
            }
            else if (sender is UsersViewModel)
            {
                await DeleteSelectedUser();
            }
            else
            {
                logger.LogWarning("DeleteEntry called with unsupported sender type: {senderType}.", sender.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting an entry.");
        }
    }

    public async Task FinalizeAppointment(AppointmentModel appointment)
    {
        if(appointment is null)
        {
            return;
        }

        AppointmentStatus oldStatus = appointment.Status;

        appointment.Status = AppointmentStatus.Finalizat;

        bool updated = await appointmentsRepository.UpdateAsync(mapper.Map(appointment, SelectedUser.Id));
        if (updated)
        {
            if(oldStatus == AppointmentStatus.Neprogramat)
            {
                ExpiringAppointments = RemoveAppointmentFromCollection(ExpiringAppointments, appointment);
            }
            else
            {
                ScheduledAppointments = RemoveAppointmentFromCollection(ScheduledAppointments, appointment);
            }

            AddAppointmentToPastCollection(appointment);

            logger.LogInformation("Finalized appointment with Id: {appointmentId}.", appointment.Id);
            return;
        }

        logger.LogWarning("Could not finalize appointment with Id: {appointmentId}.", appointment.Id);
    }

    private void UpdateSelectedUserInfo()
    {
        UserModel? user = Users.FirstOrDefault(u => u.Id.Equals(SelectedUser.Id));
        
        if (user == null)
        {
            logger.LogWarning("Could not find user with Id: {userId} to update info.", SelectedUser.Id);
            return;
        }

        SelectedUser.UpcominAppointmentsCount = ScheduledAppointments.Count;
        SelectedUser.ExpiringAppointmentsCount = ExpiringAppointments.Count;

        int index = ShownUsers.IndexOf(user);
        ShownUsers[index].UpcominAppointmentsCount = ScheduledAppointments.Count;
        ShownUsers[index].ExpiringAppointmentsCount = ExpiringAppointments.Count;
    }

    public async Task SwitchSelectedUser(Guid userId)
    {
        if (SelectedUser.Id.Equals(userId))
        {
            return;
        }

        switchUserCancellationTokenSource?.Cancel();
        switchUserCancellationTokenSource?.Dispose();

        switchUserCancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = switchUserCancellationTokenSource.Token;

        PastAppointments.Clear();
        ExpiringAppointments.Clear();
        ScheduledAppointments.Clear();
        Notes.Clear();

        SelectedUser = Users.FirstOrDefault(u => u.Id.Equals(userId));
        try
        {
            var loadTasks = new[]
            {
                LoadPastAppointments(cancellationToken),
                LoadExpiringAppointments(cancellationToken),
                LoadNotes(cancellationToken),
                LoadScheduledAppointments(cancellationToken)
            };

            await Task.WhenAll(loadTasks);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("SwitchSelectedUser operation was canceled.");
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while switching selected user.");
            return;
        }
    }

    private async Task LoadScheduledAppointments(CancellationToken cancellationToken)
    {
        List<Appointment> appointments = await appointmentsRepository.GetUpcomingAppointments(SelectedUser.Id);
        List<AppointmentModel> temp = new List<AppointmentModel>();

        cancellationToken.ThrowIfCancellationRequested();

        ScheduledAppointments.Clear();
        foreach (var appointment in appointments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            temp.Add(mapper.Map(appointment));
        }

        ScheduledAppointments = new ObservableCollection<AppointmentModel>(temp);
    }

    private async Task LoadExpiringAppointments(CancellationToken cancellationToken)
    {
        List<Appointment> appointments = await appointmentsRepository.GetExpiringAppointments(SelectedUser.Id);

        cancellationToken.ThrowIfCancellationRequested();

        ExpiringAppointments.Clear();
        foreach (var appointment in appointments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ExpiringAppointments.Add(mapper.Map(appointment));
        }
    }

    private async Task LoadPastAppointments(CancellationToken cancellationToken)
    {
        List<Appointment> appointments = await appointmentsRepository.GetPastAppointments(SelectedUser.Id);

        cancellationToken.ThrowIfCancellationRequested();

        PastAppointments.Clear();
        foreach (var appointment in appointments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PastAppointments.Add(mapper.Map(appointment));
        }

        ReorderPastAppointmentsCollection();
    }

    private async Task LoadNotes(CancellationToken cancellationToken)
    {
        IEnumerable<Note> notes = await notesRepository.FindAllAsync(n => n.UserId.Equals(SelectedUser.Id));

        cancellationToken.ThrowIfCancellationRequested();

        Notes.Clear();
        foreach (Note note in notes)
        {
            cancellationToken.ThrowIfCancellationRequested();
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

            ShownUsers.Clear();
            ShownUsers = null;

            PastAppointments.Clear();
            PastAppointments = null;

            ExpiringAppointments.Clear();
            ExpiringAppointments = null;

            ScheduledAppointments.Clear();
            ScheduledAppointments = null;

            Notes.Clear();
            Notes = null;

            switchUserCancellationTokenSource?.Cancel();
            switchUserCancellationTokenSource?.Dispose();

            OnSelectedUserSwitched = null;
            (LoadUsersCommand as IDisposable)?.Dispose();
        }
    }
}
