using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Entities;
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

            if(e.Parameter is not null && e.Parameter is User)
            {
                ViewModel.LoadUser(e.Parameter as User);
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
            AddNoteView addNoteView = new AddNoteView(ViewModel.User);
            addNoteView.Activate();
        }

        private void btnAddAppointment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            AddAppointmentView addAppointmentView = new AddAppointmentView(ViewModel.User);
            addAppointmentView.Activate();
        }

        private void CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
