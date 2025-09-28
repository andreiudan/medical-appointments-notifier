using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UserAppointmentsViewModel : ObservableRecipient, IRecipient<NoteAddedMessage>
{
    [ObservableProperty]
    private User user = new();

    public IAsyncRelayCommand DeleteNotesCommand { get; }

    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }

    private List<Note> selectedNoteEntries = new();

    private List<Appointment> selectedAppointmentEntries = new();

    public UserAppointmentsViewModel()
    {
        DeleteNotesCommand = new AsyncRelayCommand(DeleteNotesAsync);
        DeleteAppointmentsCommand = new AsyncRelayCommand(DeleteAppointmentsAsync);

        IsActive = true;
    }

    public void LoadUser(User selectedUser)
    {
        User = selectedUser;
    }

    public void Receive(NoteAddedMessage message)
    {
        if(message == null)
        {
            return;
        }

        User.Notes.Add(message.note);
    }

    private async Task DeleteNotesAsync()
    {
        IEnumerable<Note> deletedNotes = await DeleteEntriesAsync<Note>(selectedNoteEntries);

        foreach(Note note in deletedNotes)
        {
            User.Notes.Remove(note);
        }
    }

    private async Task DeleteAppointmentsAsync()
    {
        IEnumerable<Appointment> deletedAppointments = await DeleteEntriesAsync<Appointment>(selectedAppointmentEntries);

        foreach(Appointment appointment in deletedAppointments)
        {
            User.Appointments.Remove(appointment);
        }
    }

    private async Task<IEnumerable<TModel>> DeleteEntriesAsync<TModel>(List<TModel> selectedEntries) where TModel : class
    {
        IRepository<TModel> repository = Ioc.Default.GetRequiredService<IRepository<TModel>>();
        List<TModel> deletedEntries = new();

        foreach (TModel entry in selectedEntries)
        {
            bool deleted = await repository.DeleteAsync(entry);
            if (deleted)
            {
                selectedEntries.Remove(entry);
                deletedEntries.Add(entry);
            }
        }

        return deletedEntries;
    }
}
