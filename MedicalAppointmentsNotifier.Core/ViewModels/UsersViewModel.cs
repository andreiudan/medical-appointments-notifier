using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Core.Models;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>
{
    [ObservableProperty]
    private ObservableCollection<UserModel> users = new();

    public IRepository<User> UsersRepository { get; set; } = Ioc.Default.GetRequiredService<IRepository<User>>();

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
        var _users = await UsersRepository.GetAllAsync();

        Users.Clear();

        foreach (var user in _users)
        {
            Users.Add(new UserModel(user, false));
        }
    }

    private async Task DeleteSelectedUsersAsync()
    {
        List<UserModel> usersToDelete = Users.Where(u => u.IsSelected).ToList();

        foreach (UserModel entry in usersToDelete)
        {
            bool deleted = await UsersRepository.DeleteAsync(entry.User);
            if (deleted)
            {
                Users.Remove(Users.First(u => u.User.Id == entry.User.Id));
            }
        }
    }

    public void Receive(UserAddedMessage message)
    {
        Users.Add(new UserModel(message.user, false));
    }
}
