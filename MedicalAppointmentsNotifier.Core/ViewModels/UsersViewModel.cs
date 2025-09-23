using CommunityToolkit.Mvvm.ComponentModel;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using System.Collections.ObjectModel;

namespace MedicalAppointmentsNotifier.Core.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<User> users = new();

        private IRepository<User> usersRepository;

        public UsersViewModel(IRepository<User> repository)
        {
            usersRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            GetAll();
        }

        private void GetAll()
        {
            users = new ObservableCollection<User>(usersRepository.GetAll());
        }
    }
}
