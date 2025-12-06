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

public partial class UpsertAppointmentViewModel : ObservableValidator
{
    public event EventHandler OnAppointmentAdded;

    [ObservableProperty]
    private MedicalSpecialty specialty = 0;

    [NotifyPropertyChangedFor(nameof(MonthsIntervalErrorMessage))]
    [NotifyPropertyChangedFor(nameof(ExpiringDate))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [Range(0, int.MaxValue, ErrorMessage = ValidationConstants.DaysIntervalErrorMessage)]
    private int monthsInterval = 3;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [NotifyPropertyChangedFor(nameof(ExpiringDate))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? issuedOn = DateTimeOffset.Now;

    public DateTimeOffset? ExpiringDate => IssuedOn.HasValue ? IssuedOn.Value.AddMonths(MonthsInterval) : DateTimeOffset.Now;

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;
    public string MonthsIntervalErrorMessage => GetErrors(nameof(MonthsInterval)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    public MedicalSpecialty[] MedicalSpecialties => (MedicalSpecialty[])Enum.GetValues(typeof(MedicalSpecialty));

    private Guid UserId { get; set; }
    private Guid AppointmentId { get; set; } = Guid.Empty;
    private AppointmentStatus Status { get; set; } = 0;

    [ObservableProperty]
    public bool isScheduled = false;
    public string? ScheduledLocation { get; set; } = string.Empty;
    public DateTimeOffset? ScheduledDate { get; set; }
    public TimeSpan ScheduledTime { get; set; }

    public string Title { get; set; } = "Adauga Scrisoare Medicala";
    public string UpsertButtonText = "Adauga";

    public IAsyncRelayCommand UpsertAppointmentCommand { get; }

    private readonly IRepository<Appointment> appointmentsRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertAppointmentViewModel> logger;

    public UpsertAppointmentViewModel(IRepository<Appointment> appointmentsRepository, 
        IEntityToModelMapper mapper, ILogger<UpsertAppointmentViewModel> logger)
    {
        this.appointmentsRepository = appointmentsRepository ?? throw new ArgumentNullException(nameof(appointmentsRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        UpsertAppointmentCommand = new AsyncRelayCommand(UpsertAsync);
    }

    public void LoadAppointment(AppointmentModel appointment)
    {
        if(appointment == null)
        {
            logger.LogWarning("LoadAppointment called with null appointment");
            return;
        }

        AppointmentId = appointment.Id;
        Specialty = appointment.MedicalSpecialty ?? 0;
        Status = appointment.Status;
        MonthsInterval = appointment.MonthsInterval;
        IssuedOn = appointment.IssuedOn;

        if(Status == AppointmentStatus.Programat || Status == AppointmentStatus.Finalizat)
        {
            IsScheduled = true;
            if(appointment.ScheduledOn.HasValue)
            {
                ScheduledDate = appointment.ScheduledOn.Value;
                ScheduledTime = new TimeSpan(appointment.ScheduledOn.Value.Hour, appointment.ScheduledOn.Value.Minute, appointment.ScheduledOn.Value.Second);
            }
            ScheduledLocation = appointment.ScheduledLocation;
        }

        Title = "Modifica Scrisoarea Medicala";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded appointment with Id: {AppointmentId}", AppointmentId);
    }

    public void LoadUserId(Guid userId)
    {
        UserId = userId;
    }

    partial void OnMonthsIntervalChanged(int value)
    {
        ValidateProperty(value, nameof(MonthsInterval));
    }

    partial void OnIssuedOnChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(IssuedOn));
        ValidateDates();
    }

    private bool ValidateDates()
    {
        DateIntervalErrorMessage = GetErrors(nameof(IssuedOn)).FirstOrDefault()?.ErrorMessage ?? GetErrors(nameof(ExpiringDate)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

        if(!string.IsNullOrEmpty(DateIntervalErrorMessage))
        {
            return false;
        }

        return true;
    }

    public bool Validate()
    {
        try
        {
            ValidateAllProperties();
            if (!ValidateDates())
            {
                return false;
            }

            OnPropertyChanged(nameof(DateIntervalErrorMessage));
            OnPropertyChanged(nameof(MonthsIntervalErrorMessage));

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

    private AppointmentStatus GetAppointmentStatus()
    {
        if (IsScheduled)
        {
            return AppointmentStatus.Programat;
        }
        
        if(ExpiringDate.HasValue && ExpiringDate.Value.Date < DateTimeOffset.Now.AddMonths(1).Date)
        {
            return AppointmentStatus.Finalizat;
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

    private async Task UpsertAsync()
    {
        if(!Validate())
        {
            logger.LogInformation("Validation failed for appointment ID:{AppointmentId}", AppointmentId);
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();
        Appointment appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            MedicalSpecialty = Specialty,
            Status = GetAppointmentStatus(),
            MonthsInterval = MonthsInterval,
            ScheduledLocation = nameNormalizer.Normalize(ScheduledLocation ?? string.Empty),
            ScheduledOn = GetScheduledOn(),
            IssuedOn = IssuedOn,
            UserId = UserId,
        };

        if(AppointmentId.Equals(Guid.Empty))
        {
            await InsertAsync(appointment);
        }
        else
        {
            appointment.Id = AppointmentId;
            await UpdateAsync(appointment);
        }

        OnAppointmentAdded?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(Appointment appointment)
    {
        Appointment addedAppointment = await appointmentsRepository.AddAsync(appointment);

        logger.LogInformation("Inserted new appointment with ID {AppointmentId}", appointment.Id);
        WeakReferenceMessenger.Default.Send<AppointmentAddedMessage>(new AppointmentAddedMessage(mapper.Map(addedAppointment)));
    }

    private async Task UpdateAsync(Appointment appointment)
    {
        bool updated = await appointmentsRepository.UpdateAsync(appointment);

        if (!updated)
        {
            logger.LogWarning("Failed to update appointment with ID {AppointmentId}", appointment.Id);
            return;
        }

        logger.LogInformation("Updated appointment with ID {AppointmentId}", appointment.Id);
        WeakReferenceMessenger.Default.Send<AppointmentUpdatedMessage>(new AppointmentUpdatedMessage(mapper.Map(appointment)));
    }
}
