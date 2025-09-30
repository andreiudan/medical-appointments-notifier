using MedicalAppointmentsNotifier.Core.Models;
using MedicalAppointmentsNotifier.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

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

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is null || e.ClickedItem is not UserModel)
            {
                return;
            }

            UserModel clickedUser = e.ClickedItem as UserModel;

            var rootFrame = ((App)App.Current).RootFrame;
            rootFrame.Navigate(typeof(UserAppointmentsView), clickedUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void btnAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddUserView addUserView = new AddUserView();
            addUserView.Activate();
        }

        private void CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            return;
        }
    }
}
