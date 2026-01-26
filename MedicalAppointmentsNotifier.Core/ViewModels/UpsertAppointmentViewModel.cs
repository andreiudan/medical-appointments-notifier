using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UpsertAppointmentViewModel : ObservableValidator, IDisposable
{
    public event EventHandler OnCompleted;
    public MedicalSpecialty[] MedicalSpecialties => (MedicalSpecialty[])Enum.GetValues(typeof(MedicalSpecialty));

    private AppointmentModel appointment;

    public MedicalSpecialty? Specialty
    {
        get => appointment.MedicalSpecialty ?? 0;
        set
        {
            SetProperty(appointment.MedicalSpecialty, value, appointment, (a, m) => a.MedicalSpecialty = m);
            SetDirtyForm();
        }
    }

    public AppointmentStatus Status
    {
        get => appointment.Status;
        set
        {
            SetProperty(appointment.Status, value, appointment, (a, s) => a.Status = s);
            SetDirtyForm();
        }
    }

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [Range(0, int.MaxValue, ErrorMessage = ValidationConstants.DaysIntervalErrorMessage)]
    public int MonthsInterval
    {
        get => appointment.MonthsInterval;
        set
        {
            SetProperty(appointment.MonthsInterval, value, appointment, (a, m) => a.MonthsInterval = m, true);
            OnPropertyChanged(nameof(ExpiringDate));
            OnPropertyChanged(nameof(MonthsIntervalErrorMessage));
            SetDirtyForm();
        }
    }

    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    public DateTimeOffset? IssuedOn
    {
        get => appointment.IssuedOn;
        set
        {
            SetProperty(appointment.IssuedOn, value, appointment, (a, i) => a.IssuedOn = i, true);
            OnPropertyChanged(nameof(DateIntervalErrorMessage));
            OnPropertyChanged(nameof(ExpiringDate));
            SetDirtyForm();
        }
    }

    public DateTimeOffset? ExpiringDate => IssuedOn.HasValue ? IssuedOn.Value.AddMonths(MonthsInterval) : null;

    public string? ScheduledLocation
    {
        get => appointment.ScheduledLocation;
        set
        {
            SetProperty(appointment.ScheduledLocation, value, appointment, (a, s) => a.ScheduledLocation = s);
            SetDirtyForm();
        }
    }
    public DateTimeOffset? ScheduledDate
    {
        get => appointment.ScheduledOn;
        set
        {
            SetProperty(appointment.ScheduledOn, value, appointment, (a, s) => a.ScheduledOn = s);
            SetDirtyForm();
        }
    }

    private TimeSpan scheduledTime;

    public TimeSpan ScheduledTime
    {
        get => scheduledTime;
        set
        {
            SetProperty(ref scheduledTime, value);
            SetDirtyForm();
        }
    }

    private bool isScheduled = false;

    public bool IsScheduled
    {
        get => isScheduled;
        set
        {
            SetProperty(ref isScheduled, value);
            SetDirtyForm();
        }
    }

    public string DateIntervalErrorMessage => GetErrors(nameof(IssuedOn)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
    public string MonthsIntervalErrorMessage => GetErrors(nameof(MonthsInterval)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
    
    private Guid userId { get; set; }
    private AppointmentModel originalAppointment;
    private bool isNewAppointment = false;
    private bool isDirty = false;

    public string Title { get; set; } = "Adauga Scrisoare Medicala";
    public string UpsertButtonText = "Adauga";

    public IAsyncRelayCommand LoadAppointmentCommand { get; }
    public IAsyncRelayCommand LoadUserIdCommand { get; }
    public IAsyncRelayCommand UpsertAppointmentCommand { get; }
    public IAsyncRelayCommand TriggerScheduleCommand { get; }

    private readonly IRepository<Appointment> appointmentsRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertAppointmentViewModel> logger;

    public UpsertAppointmentViewModel(IRepository<Appointment> appointmentsRepository, 
        IEntityToModelMapper mapper, ILogger<UpsertAppointmentViewModel> logger)
    {
        this.appointmentsRepository = appointmentsRepository ?? throw new ArgumentNullException(nameof(appointmentsRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadAppointmentCommand = new AsyncRelayCommand<AppointmentModel>(LoadAppointmentAsync);
        LoadUserIdCommand = new AsyncRelayCommand<Guid>(LoadUserIdAsync);
        TriggerScheduleCommand = new AsyncRelayCommand<bool>(TriggerScheduleAsync);
        UpsertAppointmentCommand = new AsyncRelayCommand(UpsertAsync);

        ErrorsChanged += UpsertAppointmentViewModel_ErrorsChanged;
    }

    private void UpsertAppointmentViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged(nameof(DateIntervalErrorMessage));
        OnPropertyChanged(nameof(MonthsIntervalErrorMessage));
    }

    public async Task LoadAppointmentAsync(AppointmentModel? loadedAppointment)
    {
        this.appointment = new AppointmentModel();

        if (loadedAppointment == null)
        {
            isNewAppointment = true;

            logger.LogWarning("LoadAppointment called with null appointment");
            return;
        }

        originalAppointment = loadedAppointment;

        Specialty = loadedAppointment.MedicalSpecialty;
        Status = loadedAppointment.Status;
        MonthsInterval = loadedAppointment.MonthsInterval;
        IssuedOn = loadedAppointment.IssuedOn;
        ScheduledLocation = loadedAppointment.ScheduledLocation;

        if (loadedAppointment.Status == AppointmentStatus.Programat)
        {
            IsScheduled = true;
        }

        if (loadedAppointment.ScheduledOn.HasValue)
        {
            ScheduledDate = loadedAppointment.ScheduledOn;
            ScheduledTime = new TimeSpan(loadedAppointment.ScheduledOn.Value.Hour, loadedAppointment.ScheduledOn.Value.Minute, loadedAppointment.ScheduledOn.Value.Second);
        }

        Title = "Modifica Scrisoarea Medicala";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded appointment with Id: {AppointmentId}", loadedAppointment.Id);
    }

    public async Task LoadUserIdAsync(Guid userId)
    {
        if(userId.Equals(Guid.Empty))
        {
            logger.LogWarning("LoadUserId called with empty GUID");
            return;
        }

        this.userId = userId;
    }

    public async Task TriggerScheduleAsync(bool isScheduled)
    {
        IsScheduled = isScheduled;
    }

    private AppointmentStatus GetAppointmentStatus()
    {
        if (IsScheduled)
        {
            return AppointmentStatus.Programat;
        }

        return AppointmentStatus.Neprogramat;
    }

    private DateTimeOffset? GetScheduledOn()
    {
        if (ScheduledDate.HasValue)
        {
            return new DateTimeOffset(
                ScheduledDate.Value.Year,
                ScheduledDate.Value.Month,
                ScheduledDate.Value.Day,
                ScheduledTime.Hours,
                ScheduledTime.Minutes,
                ScheduledTime.Seconds,
                ScheduledDate.Value.Offset);
        }
        return null;
    }

    private void SetDirtyForm()
    {
        isDirty = true;
    }

    private async Task UpsertAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();
        appointment.Status = GetAppointmentStatus();
        appointment.ScheduledLocation = nameNormalizer.Normalize(appointment.ScheduledLocation ?? string.Empty);
        appointment.ScheduledOn = GetScheduledOn();

        if (isDirty)
        {
            if (isNewAppointment)
            {
                await InsertAsync(mapper.Map(appointment, userId));
            }
            else
            {
                appointment.Id = originalAppointment.Id;
                await UpdateAsync(mapper.Map(appointment, userId));
            }
        }

        OnCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(Appointment newAppointment)
    {
        Appointment addedAppointment = await appointmentsRepository.AddAsync(newAppointment);

        logger.LogInformation("Inserted new appointment with ID {AppointmentId}", newAppointment.Id);
        WeakReferenceMessenger.Default.Send<AppointmentAddedMessage>(new AppointmentAddedMessage(mapper.Map(addedAppointment)));
    }

    private async Task UpdateAsync(Appointment updatedAppointment)
    {
        bool updated = await appointmentsRepository.UpdateAsync(updatedAppointment);
        if (!updated)
        {
            logger.LogWarning("Failed to update appointment with ID {AppointmentId}", updatedAppointment.Id);
            return;
        }

        bool statusChanged = !originalAppointment.Status.Equals(updatedAppointment.Status);
        AppointmentStatus oldStatus = originalAppointment.Status;

        originalAppointment.MedicalSpecialty = updatedAppointment.MedicalSpecialty;
        originalAppointment.MonthsInterval = updatedAppointment.MonthsInterval;
        originalAppointment.Status = updatedAppointment.Status;
        originalAppointment.IssuedOn = updatedAppointment.IssuedOn;
        originalAppointment.ScheduledOn = updatedAppointment.ScheduledOn;
        originalAppointment.ScheduledLocation = updatedAppointment.ScheduledLocation ?? string.Empty;

        logger.LogInformation("Updated appointment with ID {AppointmentId}", updatedAppointment.Id);
        if (statusChanged)
        {
            WeakReferenceMessenger.Default.Send<AppointmentStatusChangedMessage>(new AppointmentStatusChangedMessage(mapper.Map(updatedAppointment), oldStatus));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ErrorsChanged -= UpsertAppointmentViewModel_ErrorsChanged;
            OnCompleted = null;
            (LoadUserIdCommand as IDisposable)?.Dispose();
            (LoadAppointmentCommand as IDisposable)?.Dispose();
            (UpsertAppointmentCommand as IDisposable)?.Dispose();
            (TriggerScheduleCommand as IDisposable)?.Dispose();
        }
    }
}
