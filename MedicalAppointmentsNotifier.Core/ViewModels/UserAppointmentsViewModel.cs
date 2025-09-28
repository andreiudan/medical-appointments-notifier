using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UserAppointmentsViewModel : ObservableRecipient, IRecipient<NoteAddedMessage>, IRecipient<AppointmentAddedMessage>
{
    [ObservableProperty]
    private User user = new();

    public IAsyncRelayCommand DeleteNotesCommand { get; }

    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }

    private List<Note> selectedNoteEntries = new();

    private List<Appointment> selectedAppointmentEntries = new();

    public UserAppointmentsViewModel()
    {
        AddNoteCommand = new AsyncRelayCommand<Note>(AddNoteAsync);
        AddAppointmentCommand = new AsyncRelayCommand<Appointment>(AddAppointmentAsync);
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
        User.Notes.Add(message.note);
    }
}
