using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UpsertUserViewModel : ObservableValidator, IDisposable
{
    public event EventHandler OnCompleted;

    private string firstName;

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    public string FirstName
    {
        get => firstName;
        set
        {
            SetProperty(ref firstName, value, true);
            SetDirtyForm();
        }
    }

    public string FirstNameErrorMessage => GetErrors(nameof(FirstName)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    private string lastName;

    [Required(ErrorMessage = ValidationConstants.RequiredErrorMessage)]
    [RegularExpression(ValidationConstants.NameRegex, ErrorMessage = ValidationConstants.NameErrorMessage)]
    public string LastName
    {
        get => lastName;
        set
        {
            SetProperty(ref lastName, value, true);
            SetDirtyForm();
        }
    }

    public string LastNameErrorMessage => GetErrors(nameof(LastName)).FirstOrDefault()?.ErrorMessage ?? string.Empty;

    private UserModel user;
    private bool isDirty = false;

    public string Title { get; set; } = "Adauga Beneficiar";
    public string UpsertButtonText { get; set; } = "Adauga";

    public IAsyncRelayCommand UpsertUserCommand { get; }
    public IAsyncRelayCommand LoadUserCommand { get; }

    private readonly IRepository<User> repository;
    private readonly IEntityToModelMapper mapper;
    private readonly ILogger<UpsertUserViewModel> logger;

    public UpsertUserViewModel(IRepository<User> repository, IEntityToModelMapper mapper, ILogger<UpsertUserViewModel> logger)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadUserCommand = new AsyncRelayCommand<UserModel>(LoadUser);
        UpsertUserCommand = new AsyncRelayCommand(UpsertAsync);

        this.ErrorsChanged += UpsertUserViewModel_ErrorsChanged;
    }

    private void UpsertUserViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged(nameof(FirstNameErrorMessage));
        OnPropertyChanged(nameof(LastNameErrorMessage));
    }

    public async Task LoadUser(UserModel? user)
    {
        if(user == null)
        {
            logger.LogWarning("LoadUser called with null user.");
            return;
        }

        this.user = user;
        FirstName = user.FirstName;
        LastName = user.LastName;

        Title = "Modifica Beneficiar";
        UpsertButtonText = "Modifica";

        logger.LogInformation("Loaded user with Id: {UserId}", user.Id);
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
            logger.LogInformation("Validation failed on user upsert.");
            return;
        }

        INameNormalizer nameNormalizer = new NameNormalizer();
        User newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = nameNormalizer.Normalize(FirstName),
            LastName = nameNormalizer.Normalize(LastName),
        };

        if (isDirty)
        {
            if (user is null)
            {
                await InsertAsync(newUser);
            }
            else
            {
                newUser.Id = user.Id;
                await UpdateAsync(newUser);
            }
        }

        OnCompleted?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertAsync(User newUser)
    {
        _ = await repository.AddAsync(newUser);

        logger.LogInformation("Inserted new user with Id: {UserId}", newUser.Id);
        WeakReferenceMessenger.Default.Send<UserAddedMessage>(new UserAddedMessage(await mapper.Map(newUser)));
    }

    private async Task UpdateAsync(User updatedUser)
    {
        bool updated = await repository.UpdateAsync(updatedUser);
        if (!updated)
        {
            logger.LogWarning("Failed to update user with Id: {UserId}", updatedUser.Id);
            return;
        }

        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        logger.LogInformation("Updated user with Id: {UserId}", updatedUser.Id);
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
            ErrorsChanged -= UpsertUserViewModel_ErrorsChanged;
            OnCompleted = null;
            (LoadUserCommand as IDisposable)?.Dispose();
            (UpsertUserCommand as IDisposable)?.Dispose();
        }
    }
}
