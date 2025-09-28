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

    public IAsyncRelayCommand AddNoteCommand { get; }

    public IAsyncRelayCommand AddAppointmentCommand { get; }

    public IAsyncRelayCommand DeleteNotesCommand { get; }

    public IAsyncRelayCommand DeleteAppointmentsCommand { get; }

    public UserAppointmentsViewModel()
    {
        AddNoteCommand = new AsyncRelayCommand<Note>(AddNoteAsync);
        AddAppointmentCommand = new AsyncRelayCommand<Appointment>(AddAppointmentAsync);
        IsActive = true;
    }

    public void LoadUser(User selectedUser)
    {
        User = selectedUser;
    }

    private async Task AddNoteAsync(Note note)
    {
        IRepository<Note> repository = Ioc.Default.GetRequiredService<IRepository<Note>>();

        await repository.AddAsync(note);
    }

    private async Task AddAppointmentAsync(Appointment appointment)
    {
        IRepository<Appointment> repository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();

        await repository.AddAsync(appointment);
    }

    public void Receive(NoteAddedMessage message)
    {
        User.Notes.Add(message.note);
    }

    public void Receive(AppointmentAddedMessage message)
    {
        User.Appointments.Add(message.appointment);
    }
}
