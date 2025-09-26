using CommunityToolkit.Mvvm.ComponentModel;
using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Core.ViewModels;

public partial class UserAppointmentsViewModel : ObservableObject
{
    [ObservableProperty]
    private User user = new();

    public UserAppointmentsViewModel()
    {
        
    }

    public void LoadUser(User selectedUser)
    {
        user = selectedUser;
    }
}
