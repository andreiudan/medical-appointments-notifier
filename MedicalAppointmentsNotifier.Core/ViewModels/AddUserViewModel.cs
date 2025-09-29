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
    public event EventHandler OnUserAdded;

    [NotifyPropertyChangedFor(nameof(FirstNameErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    private string firstName;

    public string FirstNameErrorMessage => GetErrors(nameof(FirstName)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    [NotifyPropertyChangedFor(nameof(LastNameErrorMessage))]
    [ObservableProperty]
    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    private string lastName;

    public string LastNameErrorMessage => GetErrors(nameof(LastName)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    public IAsyncRelayCommand AddUserCommand { get; }

    public AddUserViewModel()
    {
        AddUserCommand = new AsyncRelayCommand(AddAsync);
    }

    partial void OnFirstNameChanged(string value)
    {
        ValidateProperty(value, nameof(FirstName));
    }

    partial void OnLastNameChanged(string value)
    {
        ValidateProperty(value, nameof(LastName));
    }

    private bool Validate()
    {
        try
        {
            ValidateAllProperties();

            if(HasErrors)
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
        if (!Validate())
        {
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();

        string userName = nameNormalizer.Normalize(FirstName, LastName);

        IRepository<User> repository = Ioc.Default.GetRequiredService<IRepository<User>>();

        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = userName
        };

        User addedUser = await repository.AddAsync(user);

        WeakReferenceMessenger.Default.Send<UserAddedMessage>(new UserAddedMessage(user));

        OnUserAdded?.Invoke(this, EventArgs.Empty);
    }
}
