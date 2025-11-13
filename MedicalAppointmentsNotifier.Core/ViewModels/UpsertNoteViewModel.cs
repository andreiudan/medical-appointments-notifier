using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
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

    private readonly IRepository<Note> noteRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertNoteViewModel> logger;

    public UpsertNoteViewModel(IRepository<Note> noteRepository, IEntityToModelMapper mapper, ILogger<UpsertNoteViewModel> logger)
    {
        this.noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        UpsertNoteCommand = new AsyncRelayCommand(UpsertAsync);
    }

    public void LoadNote(NoteModel note)
    {
        if(note == null)
        {
            logger.LogWarning("LoadNote called with null note");
            return;
        }

        NoteId = note.Id;
        Description = note.Description;
        DateFrom = note.From;
        DateTo = note.Until;

        Title = "Modifica Mentiunea";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded note with Id: {NoteId}", NoteId);
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
            logger.LogInformation("Validation failed on note ID:{NoteId}", NoteId);
            return;
        }

        Note note = new Note
        {
            Id = Guid.NewGuid(),
            Description = Description,
            From = DateFrom,
            Until = DateTo,
            UserId = UserId,
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
        _ = await noteRepository.AddAsync(note);

        logger.LogInformation("Inserted new note with Id: {NoteId}", note.Id);
        WeakReferenceMessenger.Default.Send<NoteAddedMessage>(new NoteAddedMessage(mapper.Map(note)));
    }

    private async Task UpdateAsync(Note note)
    {
        bool updated = await noteRepository.UpdateAsync(note);

        if (!updated)
        {
            logger.LogWarning("Failed to update note with Id: {NoteId}", note.Id);
            return;
        }

        logger.LogInformation("Updated note with Id: {NoteId}", note.Id);
        WeakReferenceMessenger.Default.Send<NoteUpdatedMessage>(new NoteUpdatedMessage(mapper.Map(note)));
    }
}
