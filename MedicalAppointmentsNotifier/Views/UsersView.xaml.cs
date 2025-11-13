using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace MedicalAppointmentsNotifier.Views
{
    public sealed partial class UsersView : Page
    {
        public UsersView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            this.DataContext = ((App)App.Current).Services.GetRequiredService<UsersViewModel>();
        }

        public UsersViewModel ViewModel => (UsersViewModel)DataContext;

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is null || e.ClickedItem is not UserModel)
            {
                return;
            }

            var rootFrame = ((App)App.Current).RootFrame;
            rootFrame.Navigate(typeof(UserAppointmentsView), e.ClickedItem as UserModel, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void btnAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertUserView addUserView = new UpsertUserView();
            addUserView.Activate();
        }

        private void CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            return;
        }

        private void btnEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is null || sender is not Button)
            {
                return;
            }

            if(sender is Button button && button.DataContext is UserModel userModel)
            {
                UpsertUserView addUserView = new UpsertUserView(userModel);
                addUserView.Activate();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        ~UsersView()
        {
            try
            {
                if (ViewModel is not null)
                {
                    ViewModel.Dispose();
                }

                this.Bindings.StopTracking();
                DataContext = null;
            }
            catch { }
        }
    }
}
