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
    private IEntityToModelMapper Mapper { get; } = Ioc.Default.GetRequiredService<IEntityToModelMapper>();

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
            Users.Add(Mapper.Map(user));
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
        Users.Add(Mapper.Map(message.user));
        IsActive = true;
    }
}
