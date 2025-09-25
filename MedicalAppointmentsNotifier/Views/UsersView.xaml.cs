using MedicalAppointmentsNotifier.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace MedicalAppointmentsNotifier.Views
{
    public sealed partial class UsersView : Page
    {
        public UsersView()
        {
            InitializeComponent();
            this.DataContext = ((App)App.Current).Services.GetService<UsersViewModel>();
        }

        public UsersViewModel ViewModel => (UsersViewModel)DataContext;
    }
}
