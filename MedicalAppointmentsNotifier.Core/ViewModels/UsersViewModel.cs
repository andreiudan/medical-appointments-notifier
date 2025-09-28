using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<Tuple<User, bool>> users = new();

    public IRepository<User> UsersRepository { get; set; } = Ioc.Default.GetRequiredService<IRepository<User>>();

    public IAsyncRelayCommand LoadUsersCommand { get; }
    public IAsyncRelayCommand DeleteSelectedUsersCommand { get; }

    public UsersViewModel()
    {
        DeleteSelectedUsersCommand = new AsyncRelayCommand(DeleteSelectedUsersAsync);
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);
    }

    private async Task LoadUsersAsync()
    {
        var _users = await UsersRepository.GetAllAsync();

        Users.Clear();

        foreach (var user in _users)
        {
            Users.Add(new Tuple<User, bool>(user, false));
        }
    }

    private async Task DeleteSelectedUsersAsync()
    {
        foreach (Tuple<User, bool> entry in Users.Where(s => s.Item2 == true))
        {
            bool deleted = await UsersRepository.DeleteAsync(entry.Item1);
            if (deleted)
            {
                Users.Remove(Users.First(u => u.Item1.Id == entry.Item1.Id));
            }
        }
    }
}
