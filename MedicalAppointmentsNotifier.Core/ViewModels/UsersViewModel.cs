using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>, IRecipient<UserUpdatedMessage>, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<UserModel> users = new();

    private IRepository<User> UsersRepository { get; } = Ioc.Default.GetRequiredService<IRepository<User>>();
    private IEntityToModelMapper mapper { get; } = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

    public IAsyncRelayCommand LoadUsersCommand { get; }
    public IAsyncRelayCommand DeleteSelectedUsersCommand { get; }

    public UsersViewModel()
    {
        DeleteSelectedUsersCommand = new AsyncRelayCommand(DeleteSelectedUsersAsync);
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);

        IsActive = true;
    }

    private async Task LoadUsersAsync()
    {
        List<User> _users = await UsersRepository.GetAllAsync();

        if(_users.Count == 0)
        {
            return;
        }

        Users.Clear();
        foreach (var user in _users)
        {
            Users.Add(mapper.Map(user));
        }
    }

    private async Task DeleteSelectedUsersAsync()
    {
        List<UserModel> usersToDelete = Users.Where(u => u.IsSelected).ToList();

        foreach (UserModel entry in usersToDelete)
        {
            bool deleted = await UsersRepository.DeleteAsync(entry.Id);
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
        UserModel updatedUser = message.user;
        UserModel originalUser = Users.First(u => u.Id.Equals(updatedUser.Id));

        int index = Users.IndexOf(originalUser);
        Users[index] = updatedUser;
    }

    public async Task RefreshUser(Guid userId)
    {
        User originalUser = await UsersRepository.FindAsync(u => u.Id.Equals(userId));
        UserModel originalUserModel = Users.First(u => u.Id.Equals(userId));

        int index = Users.IndexOf(originalUserModel);
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
            Users.Clear();
        }
    }
}
