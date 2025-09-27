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
    [ObservableProperty]
    private MedicalSpecialty specialty = 0;

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [ObservableProperty]
    private int daysInterval = 30;

    [ObservableProperty]
    private DateTime latestDate = DateTime.UtcNow;

    [ObservableProperty]
    private DateTime nextDate = DateTime.UtcNow.AddDays(30);

    private User user { get; set; }

    private IAsyncRelayCommand AddAppointmentCommand { get; }

    public AddAppointmentViewModel()
    {
        AddAppointmentCommand = new AsyncRelayCommand(AddAsync);
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
        }
        catch
        {
            return false;
        }

        if (DaysInterval <= 0)
        {
            return false;
        }

        if (LatestDate >= NextDate)
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

        WeakReferenceMessenger.Default.Send<AppontmentAddedMessage>(new AppontmentAddedMessage(addedAppointment));
    }
}
