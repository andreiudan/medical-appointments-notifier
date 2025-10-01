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
    public event EventHandler OnNoteAdded;

    [NotifyPropertyChangedFor(nameof(DescriptionErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    private string description = string.Empty;

    public string DescriptionErrorMessage => GetErrors(Description).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? dateFrom = DateTimeOffset.UtcNow;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? dateTo = DateTimeOffset.UtcNow.AddDays(1);

    public DateTimeOffset Today { get; } = DateTimeOffset.Now;

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;

    public IAsyncRelayCommand AddNoteCommand;

    private Guid UserId { get; set; }

    public AddNoteViewModel()
    {
        AddNoteCommand = new AsyncRelayCommand(AddNoteAsync);
    }

    public void LoadUserId(Guid userId)
    {
        UserId = userId;
    }

    partial void OnDescriptionChanged(string value)
    {
        ValidateProperty(value, nameof(Description));
    }

    partial void OnDateFromChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(DateFrom));
        ValidateDates();
    }

    partial void OnDateToChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(DateTo));
        ValidateDates();
    }

    private bool ValidateDates()
    {
        DateIntervalErrorMessage = GetErrors(nameof(DateFrom)).FirstOrDefault()?.ErrorMessage ?? GetErrors(nameof(DateTo)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

        if (!string.IsNullOrEmpty(DateIntervalErrorMessage))
        {
            return false;
        }

        if (DateFrom.Value > DateTo.Value)
        {
            DateIntervalErrorMessage = ValidationConstants.DateIntervalErrorMessage;
            return false;
        }

        return true;
    }

    private bool Validate()
    {
        try
        {
            ValidateAllProperties();

            if (!ValidateDates())
            {
                return false;
            }

            if (HasErrors)
            {
                return false;
            }
        }
        catch
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

        IRepository<User> userRepository = Ioc.Default.GetRequiredService<IRepository<User>>();

        User user = await userRepository.FindAsync(u => u.Id == UserId);

        if(user == null)
        {
            return;
        }

        Note note = new Note
        {
            Id = Guid.NewGuid(),
            Description = Description,
            From = DateFrom,
            Until = DateTo,
            User = user,
        };

        IRepository<Note> noteRepository = Ioc.Default.GetRequiredService<IRepository<Note>>();

        Note addedNote = await noteRepository.AddAsync(note);

        WeakReferenceMessenger.Default.Send<NoteAddedMessage>(new NoteAddedMessage(note));

        OnNoteAdded?.Invoke(this, EventArgs.Empty);
    }
}
