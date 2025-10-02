using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UpsertNoteViewModel : ObservableValidator
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

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;

    public DateTimeOffset Today { get; } = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset minNextDate = DateTimeOffset.Now.AddDays(1);

    public string Title { get; set; } = "Adauga Mentiune";

    public string UpsertButtonText { get; set; } = "Adauga";

    public IAsyncRelayCommand UpsertNoteCommand;

    private Guid UserId { get; set; }
    private Guid NoteId { get; set; } = Guid.Empty;

    public UpsertNoteViewModel()
    {
        UpsertNoteCommand = new AsyncRelayCommand(UpsertAsync);
    }

    public void LoadNote(NoteModel note)
    {
        if(note == null)
        {
            return;
        }

        NoteId = note.Id;
        Description = note.Description;
        DateFrom = note.From;
        DateTo = note.Until;

        Title = "Modifica Mentiunea";
        UpsertButtonText = "Modifica";
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

        if(DateFrom != null)
        {
            MinNextDate = DateFrom.Value.AddDays(1);
        }
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

            OnPropertyChanged(nameof(DateIntervalErrorMessage));
            OnPropertyChanged(nameof(DescriptionErrorMessage));

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

    private async Task UpsertAsync()
    {
        if (!Validate())
        {
            return;
        }

        IRepository<User> userRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        User user = await userRepository.FindAsync(u => u.Id == UserId);

        Note note = new Note
        {
            Id = Guid.NewGuid(),
            Description = Description,
            From = DateFrom,
            Until = DateTo,
            User = user,
        };

        if(NoteId.Equals(Guid.Empty))
        {
            await InsertAsync(note);
        }
        else
        {
            note.Id = NoteId;
            await UpdateAsync(note);
        }

        OnNoteAdded?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(Note note)
    {
        IRepository<Note> noteRepository = Ioc.Default.GetRequiredService<IRepository<Note>>();
        _ = await noteRepository.AddAsync(note);

        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

        WeakReferenceMessenger.Default.Send<NoteAddedMessage>(new NoteAddedMessage(mapper.Map(note)));
    }

    private async Task UpdateAsync(Note note)
    {
        IRepository<Note> noteRepository = Ioc.Default.GetRequiredService<IRepository<Note>>();
        bool updated = await noteRepository.UpdateAsync(note);

        if (!updated)
        {
            return;
        }

        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

        WeakReferenceMessenger.Default.Send<NoteUpdatedMessage>(new NoteUpdatedMessage(mapper.Map(note)));
    }
}
