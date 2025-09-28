using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Messages;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient, IRecipient<UserAddedMessage>
{
    [ObservableProperty]
    private ObservableCollection<User> users = new();

    public IAsyncRelayCommand LoadUsersCommand { get; }

    public IAsyncRelayCommand AddUserCommand { get; }

    public IRepository<User> UsersRepository { get; set; } = Ioc.Default.GetRequiredService<IRepository<User>>();

    public UsersViewModel()
    {
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
            Users.Add(user);
        }
    }

    public void Receive(UserAddedMessage message)
    {
        Users.Add(message.user);
    }
}
