using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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

    public MedicalSpecialty[] MedicalSpecialties => (MedicalSpecialty[])Enum.GetValues(typeof(MedicalSpecialty));

    [NotifyPropertyChangedFor(nameof(DaysIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [Range(1, int.MaxValue, ErrorMessage = ValidationConstants.DaysIntervalErrorMessage)]
    private int daysInterval = 30;

    public string DaysIntervalErrorMessage => GetErrors(nameof(DaysInterval)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? latestDate = DateTimeOffset.UtcNow;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? nextDate = DateTimeOffset.UtcNow.AddDays(1);

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;

    public DateTimeOffset Today { get; } = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset minNextDate = DateTimeOffset.Now.AddDays(1);

    private Guid UserId { get; set; }
    private Guid AppointmentId { get; set; } = Guid.Empty;
    private AppointmentStatus Status { get; set; } = 0;

    public string Title { get; set; } = "Adauga Scrisoare Medicala";
    public string UpsertButtonText = "Adauga";

    public IAsyncRelayCommand UpsertAppointmentCommand { get; }

    private readonly IRepository<Appointment> appointmentsRepository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertAppointmentViewModel> logger;

    public UpsertAppointmentViewModel(IRepository<Appointment> appointmentsRepository, IEntityToModelMapper mapper, ILogger<UpsertAppointmentViewModel> logger)
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
        DaysInterval = appointment.IntervalDays;
        LatestDate = appointment.LatestDate;
        NextDate = appointment.NextDate;

        Title = "Modifica Scrisoarea Medicala";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded appointment with Id: {AppointmentId}", AppointmentId);
    }

    public void LoadUserId(Guid userId)
    {
        UserId = userId;
    }

    partial void OnDaysIntervalChanged(int value)
    {
        ValidateProperty(value, nameof(DaysInterval));
    }

    partial void OnLatestDateChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(LatestDate));
        ValidateDates();

        if(LatestDate != null)
        {
            MinNextDate = LatestDate.Value.AddDays(1);
        }
    }

    partial void OnNextDateChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(NextDate));
        ValidateDates();
    }

    private bool ValidateDates()
    {
        DateIntervalErrorMessage = GetErrors(nameof(LatestDate)).FirstOrDefault()?.ErrorMessage ?? GetErrors(nameof(NextDate)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

        if(!string.IsNullOrEmpty(DateIntervalErrorMessage))
        {
            return false;
        }

        if (LatestDate.Value >= NextDate.Value)
        {
            DateIntervalErrorMessage = ValidationConstants.DateIntervalErrorMessage;
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
            OnPropertyChanged(nameof(DaysIntervalErrorMessage));

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
        if(!Validate())
        {
            logger.LogInformation("Validation failed for appointment ID:{AppointmentId}", AppointmentId);
            return;
        }

        Appointment appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            MedicalSpecialty = Specialty,
            Status = Status,
            IntervalDays = DaysInterval,
            NextDate = NextDate,
            LatestDate = LatestDate,
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
