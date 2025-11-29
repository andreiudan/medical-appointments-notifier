using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>, 
    IRecipient<UserUpdatedMessage>, IRecipient<RefreshUserMessage>, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<UserModel> shownUsers = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private UserModel selectedUser = new();

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
        SwitchSelectedUser(Users.FirstOrDefault().Id);
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

    private async Task DeleteSelectedUser()
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();

        bool deleted = await usersRepository.DeleteAsync(SelectedUser.Id);
        if (!deleted)
        {
            logger.LogWarning("Deletion of user with Id: {userId} failed.", SelectedUser.Id);
            return;
        }

        UserModel deletedUser = Users.First(u => u.Id.Equals(SelectedUser.Id));
        Users.Remove(deletedUser);
        if (ShownUsers.Contains(deletedUser))
        {
            ShownUsers.Remove(deletedUser);
        }

        logger.LogInformation("Deleted user with Id: {userId}.", SelectedUser.Id);
    }

    public void Receive(UserAddedMessage message)
    {
        Users.Add(message.user);
        IsActive = true;
    }

    public void Receive(UserUpdatedMessage message)
    {
        int index = Users.IndexOf(Users.First(u => u.Id.Equals(message.user.Id)));
        Users[index] = message.user;
    }

    public async void Receive(RefreshUserMessage message)
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();
        
        User originalUser = await usersRepository.FindAsync(u => u.Id.Equals(message.userId));

        int index = Users.IndexOf(Users.First(u => u.Id.Equals(message.userId)));
        Users[index] = mapper.Map(originalUser);

        logger.LogInformation("Refreshed user with ID {UserId}", message.userId);
    }

    private async Task DeleteAppointment(AppointmentModel appointment)
    {
        bool deleted = await appointmentsRepository.DeleteAsync(appointment.Id);
        if (deleted)
        {
            ScheduledAppointments.Remove(appointment);
            ExpiringAppointments.Remove(appointment);
            PastAppointments.Remove(appointment);
            logger.LogInformation("Deleted appointment with Id: {appointmentId}.", appointment.Id);
        }
    }

    private async Task DeleteNote(NoteModel note)
    {
        bool deleted = await notesRepository.DeleteAsync(note.Id);
        if (deleted)
        {
            Notes.Remove(note);
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

    public async Task SwitchSelectedUser(Guid userId)
    {
        if (SelectedUser.Id.Equals(userId))
        {
            return;
        }

        SelectedUser = Users.FirstOrDefault(u => u.Id.Equals(userId));

        LoadPastAppointments();
        LoadExpiringAppointments();
        LoadNotesAppointments();
        LoadUpcomingAppointments();
    }

    private async Task LoadUpcomingAppointments()
    {
        List<Appointment> appointments = await appointmentsRepository.GetUpcomingAppointments(SelectedUser.Id);

        ScheduledAppointments.Clear();
        foreach (var appointment in appointments)
        {
            ScheduledAppointments.Add(mapper.Map(appointment));
        }
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

    private async Task LoadNotesAppointments()
    {
        IEnumerable<Note> notes = await notesRepository.FindAllAsync(n => n.UserId.Equals(SelectedUser.Id));

        Notes.Clear();
        foreach(Note note in notes)
        {
            Notes.Add(mapper.Map(note));
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
