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
        private Window upsertView;

        public UsersView()
        {
            this.DataContext = ((App)App.Current).Services.GetRequiredService<UsersViewModel>();
            ViewModel.OnSelectedUserSwitched += SelectedUserChanged;

            InitializeComponent();
        }

        public UsersViewModel ViewModel => (UsersViewModel)DataContext;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void btnAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ActivateNewUpsertView(new UpsertUserView());
        }

        private void btnEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is null || sender is not Button)
            {
                return;
            }

            if(sender is Button button && button.DataContext is UsersViewModel)
            {
                ActivateNewUpsertView(new UpsertUserView(ViewModel.SelectedUser));
            }
        }

        private void ActivateNewUpsertView<TView>(TView view) where TView : Window
        {
            try
            {
                if (upsertView != null)
                {
                    upsertView.Close();
                }

                upsertView = view;
                upsertView.Closed += UpsertView_Closed;
                upsertView.Activate();
            }
            catch(Exception ex)
            {
                
            }        
        }

        private void UpsertView_Closed(object sender, WindowEventArgs args)
        {
            upsertView = null;

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
            ActivateNewUpsertView(new UpsertAppointmentView(ViewModel.SelectedUser.Id));
        }

        private void btnAddNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ActivateNewUpsertView(new UpsertNoteView(ViewModel.SelectedUser.Id));
        }

        private void btnEditNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
            {
                return;
            }

            if (sender is Button button && button.DataContext is NoteModel note)
            {
                ActivateNewUpsertView(new UpsertNoteView(ViewModel.SelectedUser.Id, note));
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
                ActivateNewUpsertView(new UpsertAppointmentView(ViewModel.SelectedUser.Id, appointment));
            }
        }

        private async void ShowDeleteConfirmationDialog(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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

        private void SelectedUserChanged(object sender, int index)
        {
            UsersViewList.Select(index);
        }

        private async void ShowAppointmentConfirmFinalizeDialog(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.RequestedTheme = ElementTheme.Default;
            dialog.Title = "Sigur doresti sa finalizezi scrisoarea medicala?";
            dialog.Content = "Finalizarea scrisorii medicale duce la trecerea acesteia in modul finalizat." +
                "Finalizarea ar trebui realizata dupa ce controlul programat a fost efectuat.";
            dialog.PrimaryButtonText = "Finalizeaza";
            dialog.CloseButtonText = "Inchide";
            dialog.DefaultButton = ContentDialogButton.Primary;

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (sender is null || sender is not Button)
                {
                    return;
                }

                if (sender is Button button && button.DataContext is AppointmentModel appointment)
                {
                    ViewModel.FinalizeAppointment(appointment);
                }
            }
        }
    }
}
