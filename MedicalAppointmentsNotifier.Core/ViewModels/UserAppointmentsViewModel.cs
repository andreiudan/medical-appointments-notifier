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

public partial class UserAppointmentsViewModel : ObservableRecipient, IRecipient<NoteAddedMessage>, 
    IRecipient<AppointmentAddedMessage>, IRecipient<NoteUpdatedMessage>, IRecipient<AppointmentUpdatedMessage>, IDisposable
{
    public Guid UserId { get; private set; } = Guid.Empty;

    [ObservableProperty]
    public string userName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NoteModel> notes;

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> appointments;

    public AppointmentStatus[] AppointmentStatuses { get; } = (AppointmentStatus[])Enum.GetValues(typeof(AppointmentStatus));

    public IAsyncRelayCommand DeleteNotesCommand { get; }
    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }
    public IAsyncRelayCommand UpdateAppointmentCommand { get; }
    private IAsyncRelayCommand LoadCollectionsCommand { get; }

    private readonly IRepository<Note> notesRepository;
    private readonly IRepository<Appointment> appointmentRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UserAppointmentsViewModel> logger;

    [ObservableProperty]
    private bool isEditing = false;

    private bool isEdited = false;

    public UserAppointmentsViewModel(IRepository<Note> notesRepository, IRepository<Appointment> appointmentRepository, 
        IEntityToModelMapper mapper, ILogger<UserAppointmentsViewModel> logger)
    {
        this.notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
        this.appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        DeleteNotesCommand = new AsyncRelayCommand(DeleteNotesAsync);
        DeleteAppointmentsCommand = new AsyncRelayCommand(DeleteAppointmentsAsync);
        LoadCollectionsCommand = new AsyncRelayCommand(LoadCollectionsAsync);
        UpdateAppointmentCommand = new AsyncRelayCommand<AppointmentModel>(UpdateAppointmentAsync);

        IsActive = true;
    }

    public UserAppointmentsViewModel()
    {
    }

    public void LoadUser(Guid userId, string firstName, string lastName)
    {
        if(userId == Guid.Empty || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            logger.LogWarning("LoadUser called with invalid parameters.");
            return;
        }

        UserId = userId;
        UserName = string.Format("{0} {1}", firstName, lastName);
        LoadCollectionsCommand.Execute(null);

        logger.LogInformation("Loaded user with Id: {UserId}", UserId);
    }

    private async Task LoadCollectionsAsync()
    {
        await LoadNotesAsync();
        await LoadAppointmentsAsync();
    }

    private async Task LoadNotesAsync()
    {
        IEnumerable<Note> notes = await notesRepository.FindAllAsync(n => n.User.Id == UserId);

        Notes = new ObservableCollection<NoteModel>();
        foreach (Note note in notes)
        {
            Notes.Add(mapper.Map(note));
        }

        logger.LogInformation("Loaded {NoteCount} notes for user with Id: {UserId}", Notes.Count, UserId);
    }

    private async Task LoadAppointmentsAsync()
    {
        IEnumerable<Appointment> appointments = await appointmentRepository.FindAllAsync(a => a.User.Id == UserId);

        Appointments = new ObservableCollection<AppointmentModel>();
        foreach (Appointment appointment in appointments)
        {
            Appointments.Add(mapper.Map(appointment));
        }

        logger.LogInformation("Loaded {AppointmentCount} appointments for user with Id: {UserId}", Appointments.Count, UserId);
    }

    private async Task UpdateAppointmentAsync(AppointmentModel appointment)
    {
        Appointment updateAppointment = mapper.Map(appointment, UserId);

        await appointmentRepository.UpdateAsync(updateAppointment);
        isEdited = true;

        logger.LogInformation("Updated appointment with Id: {AppointmentId} for user with Id: {UserId}", appointment.Id, UserId);
    }

    private async Task DeleteNotesAsync()
    {
        IEnumerable<Guid> deletedNotes = await DeleteEntriesAsync<Note>(Notes.Where(n => n.IsSelected).Select(n => n.Id).ToList());

        foreach (Guid noteId in deletedNotes)
        {
            Notes.Remove(Notes.First(n => n.Id == noteId));
        }
    }

    private async Task DeleteAppointmentsAsync()
    {
        IEnumerable<Guid> deletedAppointments = await DeleteEntriesAsync<Appointment>(Appointments.Where(a => a.IsSelected).Select(a => a.Id).ToList());

        foreach (Guid appointmentId in deletedAppointments)
        {
            Appointments.Remove(Appointments.First(a => a.Id == appointmentId));
        }
    }

    private async Task<IEnumerable<Guid>> DeleteEntriesAsync<TRepository>(List<Guid> selectedEntriesId) where TRepository : class
    {
        IRepository<TRepository> repository = Ioc.Default.GetRequiredService<IRepository<TRepository>>();
        List<Guid> entriesToDelete = new(selectedEntriesId);
        List<Guid> deletedEntries = new();

        foreach (Guid entry in entriesToDelete)
        {
            bool deleted = await repository.DeleteAsync(entry);
            if (deleted)
            {
                selectedEntriesId.Remove(entry);
                deletedEntries.Add(entry);
            }
        }

        if (deletedEntries.Count > 0)
        {
            isEdited = true;
            logger.LogInformation("Deleted {DeletedEntryCount} entries from {RepositoryType} for user with Id: {UserId}",
            deletedEntries.Count, typeof(TRepository).Name, UserId);
        }

        return deletedEntries;
    }

    public void Receive(NoteAddedMessage message)
    {
        Notes.Add(message.note);
        IsActive = true;
        isEdited = true;
    }

    public void Receive(AppointmentAddedMessage message)
    {
        Appointments.Add(message.appointment);
        IsActive = true;
        isEdited = true;
    }

    public void Receive(NoteUpdatedMessage message)
    {
        NoteModel updatedNote = message.note;
        NoteModel originalNote = Notes.First(n => n.Id.Equals(updatedNote.Id));

        int index = Notes.IndexOf(originalNote);
        Notes[index] = updatedNote;

        IsActive = true;
        isEdited = true;
    }

    public void Receive(AppointmentUpdatedMessage message)
    {
        AppointmentModel updatedAppointment = message.appointment;
        AppointmentModel originalAppointment = Appointments.First(a => a.Id.Equals(updatedAppointment.Id));

        int index = Appointments.IndexOf(originalAppointment);
        Appointments[index] = updatedAppointment;

        IsActive = true;
        isEdited = true;
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
            if (isEdited)
            {
                WeakReferenceMessenger.Default.Send<RefreshUserMessage>(new RefreshUserMessage(UserId));
            }
            
            IsActive = false;
            WeakReferenceMessenger.Default.UnregisterAll(this);

            Notes?.Clear();
            Notes = null;
            
            Appointments?.Clear();
            Appointments = null;
        }
    }
}
