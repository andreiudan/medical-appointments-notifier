using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UpsertUserViewModel : ObservableValidator
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

    private Guid UserId { get; set; } = Guid.Empty;

    public string Title { get; set; } = "Adauga Beneficiar";
    public string UpsertButtonText { get; set; } = "Adauga";

    public IAsyncRelayCommand AddUserCommand { get; }

    private readonly IRepository<User> repository;
    private readonly IEntityToModelMapper mapper;

    public UpsertUserViewModel(IRepository<User> repository, IEntityToModelMapper mapper)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        AddUserCommand = new AsyncRelayCommand(AddAsync);
    }

    public void LoadUser(UserModel user)
    {
        if(user == null)
        {
            return;
        }

        UserId = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;

        Title = "Modifica Beneficiar";
        UpsertButtonText = "Modifica";
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

            OnPropertyChanged(nameof(FirstNameErrorMessage));
            OnPropertyChanged(nameof(LastNameErrorMessage));

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
        if (!Validate())
        {
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();
        User user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = nameNormalizer.Normalize(FirstName),
            LastName = nameNormalizer.Normalize(LastName),
        };

        if (UserId.Equals(Guid.Empty))
        {
            await InsertAsync(user);
        }
        else
        {
            user.Id = UserId;
            await UpdateAsync(user);
        }

        OnUserAdded?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(User user)
    {
        _ = await repository.AddAsync(user);

        WeakReferenceMessenger.Default.Send<UserAddedMessage>(new UserAddedMessage(mapper.Map(user)));
    }

    private async Task UpdateAsync(User user)
    {
        bool updated = await repository.UpdateAsync(user);

        if (!updated)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send<UserUpdatedMessage>(new UserUpdatedMessage(mapper.Map(user)));
    }
}
