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
    private DateTimeOffset? latestDate = DateTimeOffset .UtcNow;

    [NotifyPropertyChangedFor(nameof(DateIntervalErrorMessage))]
    [ObservableProperty]
    private DateTimeOffset? nextDate = DateTimeOffset.UtcNow.AddDays(1);

    public string DateIntervalErrorMessage { get; private set; } = string.Empty;

    private User user { get; set; }

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
        ValidateDates();
    }

    partial void OnNextDateChanged(DateTimeOffset? value)
    {
        ValidateDates();
    }

    private bool ValidateDates()
    {
        ClearErrors(nameof(LatestDate));
        ClearErrors(nameof(NextDate));
        DateIntervalErrorMessage = string.Empty;

        if (LatestDate.HasValue && NextDate.HasValue && LatestDate.Value >= NextDate.Value)
        {
            DateIntervalErrorMessage = ValidationConstants.DateIntervalErrorMessage;
            return false;
        }

        return true;
    }

    public void LoadUser(User selectedUser)
    {
        user = selectedUser;
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

        IRepository<Appointment> repository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();

        Appointment appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            MedicalSpecialty = Specialty,
            Status = "",
            IntervalDays = DaysInterval,
            NextDate = NextDate,
            LatestDate = LatestDate,
            User = user,
        };

        Appointment addedAppointment =  await repository.AddAsync(appointment);

        WeakReferenceMessenger.Default.Send<AppointmentAddedMessage>(new AppointmentAddedMessage(addedAppointment));

        OnAppointmentAdded?.Invoke(this, EventArgs.Empty);
    }
}
