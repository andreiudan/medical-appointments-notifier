using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace MedicalAppointmentsNotifier.Views
{
    public sealed partial class UsersView : Page
    {
        private Guid? ClickedUserId = Guid.Empty;

        public UsersView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.DataContext = ((App)App.Current).Services.GetService<UsersViewModel>();
        }

        public UsersViewModel ViewModel => (UsersViewModel)DataContext;

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is null || e.ClickedItem is not UserModel)
            {
                return;
            }

            UserModel ClickedUser = e.ClickedItem as UserModel;
            this.ClickedUserId = ClickedUser.Id;

            var rootFrame = ((App)App.Current).RootFrame;
            rootFrame.Navigate(typeof(UserAppointmentsView), ClickedUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(ClickedUserId is not null && !ClickedUserId.Equals(Guid.Empty))
            {
                ViewModel.RefreshUser(ClickedUserId.Value);
                ClickedUserId = null;
            }

            ((App)App.Current).RootFrame.BackStack.Clear();
            GC.Collect();

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
    }
}
