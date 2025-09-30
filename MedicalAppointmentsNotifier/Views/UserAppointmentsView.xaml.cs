using MedicalAppointmentsNotifier.Core.Models;
using MedicalAppointmentsNotifier.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MedicalAppointmentsNotifier.Views
{
    public sealed partial class UserAppointmentsView : Page
    {
        public UserAppointmentsView()
        {
            InitializeComponent();
            this.DataContext = ((App)App.Current).Services.GetService<UserAppointmentsViewModel>();
        }

        public UserAppointmentsViewModel ViewModel => (UserAppointmentsViewModel)DataContext;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is not null && e.Parameter is UserModel)
            {
                UserModel user = e.Parameter as UserModel;

                ViewModel.LoadUser(user.Id, user.Name);
            }
        }

        private void btnBack_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var rootFrame = ((App)App.Current).RootFrame;

            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void btnAddNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddNoteView addNoteView = new AddNoteView(ViewModel.UserId);
            addNoteView.Activate();
        }

        private void btnAddAppointment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddAppointmentView addAppointmentView = new AddAppointmentView(ViewModel.UserId);
            addAppointmentView.Activate();
        }

        private void CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
