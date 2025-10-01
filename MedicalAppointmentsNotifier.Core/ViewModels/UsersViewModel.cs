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

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>
{
    [ObservableProperty]
    private ObservableCollection<UserModel> users = new();

    private IRepository<User> UsersRepository { get; } = Ioc.Default.GetRequiredService<IRepository<User>>();

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

        IEntityToModelMapper mapper = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

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
}
