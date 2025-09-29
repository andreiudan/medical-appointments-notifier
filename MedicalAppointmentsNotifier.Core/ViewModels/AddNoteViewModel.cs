using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class AddNoteViewModel : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [NotifyPropertyChangedFor(nameof(DescriptionErrorMessage))]
    [NotifyPropertyChangedFor(nameof(DescriptionErrorMessage))]
    private string description = string.Empty;

    public string DescriptionErrorMessage => GetErrors(Description).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    public string DescriptionErrorMessage => GetErrors(Description).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    [ObservableProperty]
    private DateOnly dateFrom = DateOnly.FromDateTime(DateTime.Now);

    [ObservableProperty]
    private DateOnly dateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

    public IAsyncRelayCommand AddNoteCommand;

    private User User { get; set; }

    public AddNoteViewModel()
    {
        AddNoteCommand = new AsyncRelayCommand(AddNoteAsync);
    }

    public void LoadUserId(User user)
    {
        User = user;
    }

    private bool Validate()
    {
        try
        {
            ValidateAllProperties();
        }
        catch
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            return false;
        }

        if (DateFrom >= DateTo)
        {
            return false;
        }

        return true;
    }

    private async Task AddNoteAsync()
    {
        if (!Validate())
        {
            return;
        }

        IRepository<Note> repository = Ioc.Default.GetRequiredService<IRepository<Note>>();

        Note note = new Note
        {
            Id = Guid.NewGuid(),
            Description = Description,
            From = DateFrom,
            Until = DateTo,
            User = this.User
        };

        Note addedNote = await repository.AddAsync(note);

        WeakReferenceMessenger.Default.Send<NoteAddedMessage>(new NoteAddedMessage(addedNote));
    }
}
