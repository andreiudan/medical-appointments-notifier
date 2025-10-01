using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class AddAppointmentViewModel : ObservableValidator
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
    private DateTimeOffset? latestDate = DateTimeOffset .UtcNow;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.DatesRequiredErrorMessage)]
    private DateTimeOffset? nextDate = DateTimeOffset.UtcNow.AddDays(1);

    public DateTimeOffset Today { get; } = DateTimeOffset.Now;

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;

    private Guid UserId { get; set; }

    public IAsyncRelayCommand AddAppointmentCommand { get; }

    public AddAppointmentViewModel()
    {
        AddAppointmentCommand = new AsyncRelayCommand(AddAsync);
    }

    partial void OnDaysIntervalChanged(int value)
    {
        ValidateProperty(value, nameof(DaysInterval));
    }

    partial void OnLatestDateChanged(DateTimeOffset? value)
    {
        ValidateProperty(value, nameof(LatestDate));
        ValidateDates();
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

    private async Task AddAsync()
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

        IRepository<Appointment> appointmentsRepository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();

        Appointment addedAppointment =  await appointmentsRepository.AddAsync(appointment);

        WeakReferenceMessenger.Default.Send<AppointmentAddedMessage>(new AppointmentAddedMessage(addedAppointment));

        OnAppointmentAdded?.Invoke(this, EventArgs.Empty);
    }
}
