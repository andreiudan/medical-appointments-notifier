using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void btnAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertUserView addUserView = new UpsertUserView();
            addUserView.Activate();
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

                //this.Bindings.StopTracking();
                DataContext = null;
            }
            catch { }
        }

        private void ItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
        {
            if(sender.SelectedItem is null || sender.SelectedItem is not UserModel)
            {
                return;
            }

            ViewModel.SwitchSelectedUser((sender.SelectedItem as UserModel).Id);
        }

        private void btnAddAppointment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertAppointmentView addAppointmentView = new UpsertAppointmentView(ViewModel.SelectedUser.Id);
            addAppointmentView.Activate();
        }

        private void btnAddNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertNoteView addNoteView = new UpsertNoteView(ViewModel.SelectedUser.Id);
            addNoteView.Activate();
        }

        private void btnEditNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
            {
                return;
            }

            if (sender is Button button && button.DataContext is NoteModel note)
            {
                UpsertNoteView addNoteView = new UpsertNoteView(ViewModel.SelectedUser.Id, note);
                addNoteView.Activate();
            }
        }

        private void btnEditAppointment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
            {
                return;
            }

            if (sender is Button button && button.DataContext is AppointmentModel appointment)
            {
                UpsertAppointmentView addAppointmentView = new UpsertAppointmentView(ViewModel.SelectedUser.Id, appointment);
                addAppointmentView.Activate();
            }
        }

        private async void ShowDeleteDialog(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.RequestedTheme = ElementTheme.Default;
            dialog.Title = "Sigur doresti sa stergi?";
            dialog.Content = "Stergerea duce la pierderea permanenta a datelor inregistrate in aplicatie.";
            dialog.PrimaryButtonText = "Sterge";
            dialog.PrimaryButtonStyle = Application.Current.Resources["DangerButtonStyle"] as Style;
            dialog.CloseButtonText = "Inchide";
            dialog.DefaultButton = ContentDialogButton.Close;

            ContentDialogResult result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                if (sender is null || sender is not Button)
                {
                    return;
                }

                if (sender is not null && sender is Button button)
                {
                    ViewModel.DeleteEntry(button.DataContext);
                }
            }
        }
    }
}
