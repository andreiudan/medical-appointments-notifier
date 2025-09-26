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

    public IAsyncRelayCommand AddUserCommand { get; }

    public IRepository<User> UsersRepository { get; set; } = Ioc.Default.GetRequiredService<IRepository<User>>();

    public UsersViewModel()
    {
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        LoadUsersCommand.Execute(null);

        AddUserCommand = new AsyncRelayCommand(AddAsync);
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

    private async Task AddAsync()
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = "New User",
            Appointments = new List<Appointment>(),
            Notes = new List<Note>()
        };

        await UsersRepository.AddAsync(user);
    }
}
