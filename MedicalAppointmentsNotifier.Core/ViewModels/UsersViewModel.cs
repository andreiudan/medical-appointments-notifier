using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UsersViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<User> users = new();

    public IAsyncRelayCommand LoadUsersCommand { get; }

    public IRepository<User> UsersRepository { get; set; } = Ioc.Default.GetRequiredService<IRepository<User>>();

    public UsersViewModel()
    {
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);
    }

    private async Task LoadUsersAsync()
    {
        var _users = await UsersRepository.GetAll();

        Users.Clear();

        foreach (var user in _users)
        {
            Users.Add(user);
        }
    }

    [RelayCommand]
    private void Add()
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = "New User",
            Appointments = new List<Appointment>(),
            Notes = new List<Note>()
        };

        UsersRepository.Add(user);
    }
}
