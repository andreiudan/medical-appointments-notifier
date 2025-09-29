using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

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
            //if(e.ClickedItem is null || e.ClickedItem is not User)
            //{
            //    return;
            //}

            Tuple<User, bool> clickedUser = e.ClickedItem as Tuple<User, bool>;

            var rootFrame = ((App)App.Current).RootFrame;
            rootFrame.Navigate(typeof(UserAppointmentsView), clickedUser.Item1, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void btnAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddUserView addUserView = new AddUserView();
            addUserView.Activate();
        }
    }
}
