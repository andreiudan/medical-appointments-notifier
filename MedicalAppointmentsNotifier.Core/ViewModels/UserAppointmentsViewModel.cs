using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UserAppointmentsViewModel : ObservableRecipient, IRecipient<NoteAddedMessage>, IRecipient<AppointmentAddedMessage>
{
    public Guid UserId { get; private set; } = Guid.Empty;

    [ObservableProperty]
    public string userName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NoteModel> notes = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> appointments = new();

    public IAsyncRelayCommand DeleteNotesCommand { get; }
    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }
    private IAsyncRelayCommand LoadCollectionsCommand { get; }

    private IEntityToModelMapper Mapper { get; } = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

    public UserAppointmentsViewModel()
    {
        DeleteNotesCommand = new AsyncRelayCommand(DeleteNotesAsync);
        DeleteAppointmentsCommand = new AsyncRelayCommand(DeleteAppointmentsAsync);
        LoadCollectionsCommand = new AsyncRelayCommand(LoadCollectionsAsync);

        IsActive = true;
    }

    public void LoadUser(Guid userId, string username)
    {
        UserId = userId;
        UserName = username;
        LoadCollectionsCommand.Execute(null);
    }

    private async Task LoadCollectionsAsync()
    {
        await LoadNotesAsync();
        await LoadAppointmentsAsync();
    }

    private async Task LoadNotesAsync()
    {
        IRepository<Note> notesRepository = Ioc.Default.GetRequiredService<IRepository<Note>>();
        IEnumerable<Note> notes = await notesRepository.FindAllAsync(n => n.User.Id == UserId);

        Notes.Clear();
        foreach (Note note in notes)
        {
            Notes.Add(Mapper.Map(note));
        }
    }

    private async Task LoadAppointmentsAsync()
    {
        IRepository<Appointment> repository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();
        IEnumerable<Appointment> appointments = await repository.FindAllAsync(a => a.User.Id == UserId);

        Appointments.Clear();
        foreach (Appointment appointment in appointments)
        {
            Appointments.Add(Mapper.Map(appointment));
        }
    }

    public void Receive(NoteAddedMessage message)
    {
        Notes.Add(Mapper.Map(message.note));
        IsActive = true;
    }

    public void Receive(AppointmentAddedMessage message)
    {
        Appointments.Add(Mapper.Map(message.appointment));
        IsActive = true;
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

        return deletedEntries;
    }
}
