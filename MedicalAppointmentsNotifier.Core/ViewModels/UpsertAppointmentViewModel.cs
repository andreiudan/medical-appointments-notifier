using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
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

    public string Title { get; set; } = "Adauga Scrisoare Medicala";
    public string UpsertButtonText = "Adauga";

    public IAsyncRelayCommand UpsertAppointmentCommand { get; }

    public UpsertAppointmentViewModel()
    {
        UpsertAppointmentCommand = new AsyncRelayCommand(UpsertAsync);
    }

    public void LoadAppointment(AppointmentModel appointment)
    {
        if(appointment == null)
        {
            return;
        }

        AppointmentId = appointment.Id;
        Specialty = appointment.MedicalSpecialty ?? 0;
        DaysInterval = appointment.IntervalDays;
        LatestDate = appointment.LatestDate;
        NextDate = appointment.NextDate;

        Title = "Modifica Scrisoarea Medicala";
        UpsertButtonText = "Modifica";
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

    public void LoadUserId(Guid userId)
    {
        UserId = userId;
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
            return;
        }

        IRepository<User> userRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        User user = await userRepository.FindAsync(u => u.Id == UserId);

        Appointment appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            MedicalSpecialty = Specialty,
            Status = 0,
            IntervalDays = DaysInterval,
            NextDate = NextDate,
            LatestDate = LatestDate,
            User = user,
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
        IRepository<Appointment> appointmentsRepository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();
        Appointment addedAppointment = await appointmentsRepository.AddAsync(appointment);

        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

        WeakReferenceMessenger.Default.Send<AppointmentAddedMessage>(new AppointmentAddedMessage(mapper.Map(addedAppointment)));
    }

    private async Task UpdateAsync(Appointment appointment)
    {
        IRepository<Appointment> appointmentsRepository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();
        bool updated = await appointmentsRepository.UpdateAsync(appointment);

        if (!updated)
        {
            return;
        }

        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

        WeakReferenceMessenger.Default.Send<AppointmentUpdatedMessage>(new AppointmentUpdatedMessage(mapper.Map(appointment)));
    }
}
