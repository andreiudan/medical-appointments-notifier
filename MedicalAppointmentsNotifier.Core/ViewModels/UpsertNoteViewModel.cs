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

    private NoteModel note;
    
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    public string Title
    {
        get => note.Title;
        set => SetProperty(note.Title, value, note, (n, t) => n.Title = t, true);
    }

    public string? Description
    {
        get => note.Description;
        set => SetProperty(note.Description, value, note, (n, d) => n.Description = d);
    }

    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    public DateTimeOffset? From
    {
        get => note.From;
        set
        {
            SetProperty(note.From, value, note, (n, f) => n.From = f, true);
            OnPropertyChanged(nameof(ExpiringDate));
        }
    }

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [Range(0, int.MaxValue, ErrorMessage = ValidationConstants.DaysIntervalErrorMessage)]
    public int MonthsPeriod
    {
        get => note.MonthsPeriod;
        set
        {
            SetProperty(note.MonthsPeriod, value, note, (n, m) => n.MonthsPeriod = m, true);
            OnPropertyChanged(nameof(ExpiringDate));
        }
    }

    public DateTimeOffset? ExpiringDate => From.HasValue ? From.Value.AddMonths(MonthsPeriod) : null;

    public string DateIntervalErrorMessage => GetErrors(nameof(From)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
    public string MonthsPeriodErrorMessage => GetErrors(nameof(MonthsPeriod)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
    public string TitleErrorMessage => GetErrors(nameof(Title)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    public string WindowTitle { get; set; } = "Adauga Mentiune";
    public string UpsertButtonText { get; set; } = "Adauga";

    public IAsyncRelayCommand UpsertNoteCommand;
    public IAsyncRelayCommand LoadNoteCommand;
    public IAsyncRelayCommand LoadUserIdCommand;

    private Guid userId { get; set; }
    private NoteModel originalNote;
    private bool isNewNote = false;

    private readonly IRepository<Note> noteRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertNoteViewModel> logger;

    public UpsertNoteViewModel(IRepository<Note> noteRepository, IEntityToModelMapper mapper, ILogger<UpsertNoteViewModel> logger)
    {
        this.noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        UpsertNoteCommand = new AsyncRelayCommand(UpsertAsync);
        LoadNoteCommand = new AsyncRelayCommand<NoteModel>(LoadNote);
        LoadUserIdCommand = new AsyncRelayCommand<Guid>(LoadUserId);

        ErrorsChanged += UpsertNoteViewModel_ErrorsChanged;
    }

    private void UpsertNoteViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TitleErrorMessage));
        OnPropertyChanged(nameof(MonthsPeriodErrorMessage));
        OnPropertyChanged(nameof(DateIntervalErrorMessage));
    }

    public async Task LoadNote(NoteModel? note)
    {
        this.note = new NoteModel();

        if (note == null)
        {
            isNewNote = true;

            logger.LogWarning("LoadNote called with null note");
            return;
        }

        originalNote = note;

        Title = note.Title;
        Description = note.Description;
        MonthsPeriod = note.MonthsPeriod;
        From = note.From;

        WindowTitle = "Modifica Mentiunea";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded note with Id: {NoteId}", originalNote.Id);
    }

    public async Task LoadUserId(Guid userId)
    {
        if(userId.Equals(Guid.Empty))
        {
            logger.LogWarning("LoadUserId called with empty Guid");
            return;
        }

        this.userId = userId;
    }

    private async Task UpsertAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            logger.LogInformation("Validation failed on note with Title: {Title}, Months Period: {MonthsPeriod}, From: {From}", Title, MonthsPeriod, From);
            return;
        }

        if(isNewNote)
        {
            await InsertAsync(mapper.Map(note, userId));
        }
        else
        {
            note.Id = this.originalNote.Id;
            await UpdateAsync(mapper.Map(note, userId));
        }

        OnNoteAdded?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(Note addedNote)
    {
        _ = await noteRepository.AddAsync(addedNote);

        logger.LogInformation("Inserted new note with Id: {NoteId}", addedNote.Id);
        WeakReferenceMessenger.Default.Send<NoteAddedMessage>(new NoteAddedMessage(mapper.Map(addedNote)));
    }

    private async Task UpdateAsync(Note updatedNote)
    {
        bool updated = await noteRepository.UpdateAsync(updatedNote);
        if (!updated)
        {
            logger.LogWarning("Failed to update note with Id: {NoteId}", updatedNote.Id);
            return;
        }
        
        originalNote.Title = updatedNote.Title;
        originalNote.Description = updatedNote.Description;
        originalNote.From = updatedNote.From;
        originalNote.MonthsPeriod = updatedNote.MonthsPeriod;
        logger.LogInformation("Updated note with Id: {NoteId}", updatedNote.Id);
    }
}
