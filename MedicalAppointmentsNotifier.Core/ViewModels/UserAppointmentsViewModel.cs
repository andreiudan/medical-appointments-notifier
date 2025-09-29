using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Models;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UserAppointmentsViewModel : ObservableRecipient, IRecipient<NoteAddedMessage>, IRecipient<AppointmentAddedMessage>
{
    [ObservableProperty]
    private User user = new();

    [ObservableProperty]
    private ObservableCollection<NoteModel> notes = new();

    [ObservableProperty]
    private ObservableCollection<AppointmentModel> appointments = new();

    public IAsyncRelayCommand DeleteNotesCommand { get; }

    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }

    public UserAppointmentsViewModel()
    {
        DeleteNotesCommand = new AsyncRelayCommand(DeleteNotesAsync);
        DeleteAppointmentsCommand = new AsyncRelayCommand(DeleteAppointmentsAsync);

        IsActive = true;
    }

    public void LoadUser(User selectedUser)
    {
        User = selectedUser;
        LoadNotes();
        LoadAppointments();
    }

    public void LoadNotes()
    {
        Notes.Clear();
        foreach (Note note in User.Notes)
        {
            Notes.Add(new NoteModel(note, false));
        }
    }

    public void LoadAppointments()
    {
        Appointments.Clear();
        foreach (Appointment appointment in User.Appointments)
        {
            Appointments.Add(new AppointmentModel(appointment, false));
        }
    }

    public void Receive(NoteAddedMessage message)
    {
        Notes.Add(new NoteModel(message.note, false));
        IsActive = true;
    }

    public void Receive(AppointmentAddedMessage message)
    {
        Appointments.Add(new AppointmentModel(message.appointment, false));
        IsActive = true;
    }

    private async Task DeleteNotesAsync()
    {
        IEnumerable<Note> deletedNotes = await DeleteEntriesAsync<Note>(Notes.Where(n => n.IsSelected).Select(n => n.Note).ToList());

        foreach (Note note in deletedNotes)
        {
            Notes.Remove(Notes.First(n => n.Note.Id == note.Id));
        }
    }

    private async Task DeleteAppointmentsAsync()
    {
        IEnumerable<Appointment> deletedAppointments = await DeleteEntriesAsync<Appointment>(Appointments.Where(a => a.IsSelected).Select(a => a.Appointment).ToList());

        foreach (Appointment appointment in deletedAppointments)
        {
            Appointments.Remove(Appointments.First(a => a.Appointment.Id == appointment.Id));
        }
    }

    private async Task<IEnumerable<TModel>> DeleteEntriesAsync<TModel>(List<TModel> selectedEntries) where TModel : class
    {
        IRepository<TModel> repository = Ioc.Default.GetRequiredService<IRepository<TModel>>();
        List<TModel> entriesToDelete = new(selectedEntries);
        List<TModel> deletedEntries = new();

        foreach (TModel entry in entriesToDelete)
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
