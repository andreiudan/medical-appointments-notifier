using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class AddUserViewModel : ObservableValidator
{
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    [ObservableProperty]
    private string firstName;

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    [ObservableProperty]
    private string lastName;

    private IAsyncRelayCommand AddUserCommand { get; }

    public AddUserViewModel()
    {
        AddUserCommand = new AsyncRelayCommand(AddAsync);
    }

    private bool Validate()
    {
        try
        {
            ValidateAllProperties();
        }
        catch 
        {
            return false;
        }

        return true;
    }

    private async Task AddAsync()
    {
        if (!Validate())
        {
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();

        string userName = nameNormalizer.Normalize(firstName, lastName);

        IRepository<User> repository = Ioc.Default.GetRequiredService<IRepository<User>>();

        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = userName
        };

        User addedUser = await repository.AddAsync(user);

        WeakReferenceMessenger.Default.Send<UserAddedMessage>(new UserAddedMessage(user));
    }
}
