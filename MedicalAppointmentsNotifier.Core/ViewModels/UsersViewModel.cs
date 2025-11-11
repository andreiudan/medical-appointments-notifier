using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>, 
    IRecipient<UserUpdatedMessage>, IRecipient<RefreshUserMessage>, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<UserModel> users = new();

    public IAsyncRelayCommand LoadUsersCommand { get; }
    public IAsyncRelayCommand DeleteSelectedUsersCommand { get; }

    private readonly ILogger<UsersViewModel> logger;

    public UsersViewModel(ILogger<UsersViewModel> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        DeleteSelectedUsersCommand = new AsyncRelayCommand(DeleteSelectedUsersAsync);
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);

        IsActive = true;
    }

    public UsersViewModel()
    {
    }

    private async Task LoadUsersAsync()
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

        logger.LogInformation("Loading users from the database.");
        List<User> _users = await usersRepository.GetAllAsync();

        if(_users.Count == 0)
        {
            return;
        }

        Users.Clear();
        foreach (var user in _users)
        {
            Users.Add(mapper.Map(user));
        }

        logger.LogInformation("Loaded {UserCount} users from the database.", Users.Count);
    }

    private async Task DeleteSelectedUsersAsync()
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        List<UserModel> usersToDelete = Users.Where(u => u.IsSelected).ToList();

        foreach (UserModel entry in usersToDelete)
        {
            bool deleted = await usersRepository.DeleteAsync(entry.Id);
            if (deleted)
            {
                Users.Remove(Users.First(u => u.Id == entry.Id));
            }
        }
    }

    public void Receive(UserAddedMessage message)
    {
        Users.Add(message.user);
        IsActive = true;
    }

    public void Receive(UserUpdatedMessage message)
    {
        int index = Users.IndexOf(Users.First(u => u.Id.Equals(message.user.Id)));
        Users[index] = message.user;
    }

    public async void Receive(RefreshUserMessage message)
    {
        IRepository<User> usersRepository = Ioc.Default.GetRequiredService<IRepository<User>>();
        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();
        
        User originalUser = await usersRepository.FindAsync(u => u.Id.Equals(message.userId));

        int index = Users.IndexOf(Users.First(u => u.Id.Equals(message.userId)));
        Users[index] = mapper.Map(originalUser);
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
            IsActive = false;
            WeakReferenceMessenger.Default.UnregisterAll(this);

            Users.Clear();
            Users = null;
        }
    }
}
