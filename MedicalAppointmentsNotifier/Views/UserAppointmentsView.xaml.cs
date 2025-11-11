using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MedicalAppointmentsNotifier.Views
{
    public sealed partial class UserAppointmentsView : Page
    {
        private bool ListViewLoaded { get; set; } = false;
        private IServiceScope _scope;

        public UserAppointmentsView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.DataContextChanged += OnDataContextChanged;
        }

        public UserAppointmentsViewModel ViewModel { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not null && e.Parameter is UserModel)
            {
                _scope = ((App)App.Current).Services.CreateScope();

                this.DataContext = _scope.ServiceProvider.GetRequiredService<UserAppointmentsViewModel>();

                UserModel user = e.Parameter as UserModel;

                ViewModel.LoadUser(user.Id, user.FirstName, user.LastName);
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

        private void OnDataContextChanged(FrameworkElement e, DataContextChangedEventArgs args)
        {
            if (args.NewValue is null)
            {
                this.Bindings.StopTracking();
                return;
            }

            ViewModel = (UserAppointmentsViewModel)this.DataContext;
            this.Bindings.Update();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ListViewLoaded = false;

            DataContext = null;

            _scope?.Dispose();
            _scope = null;

            base.OnNavigatedFrom(e);
        }

        private void btnAddNote_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertNoteView addNoteView = new UpsertNoteView(ViewModel.UserId);
            addNoteView.Activate();
        }

        private void btnAddAppointment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpsertAppointmentView addAppointmentView = new UpsertAppointmentView(ViewModel.UserId);
            addAppointmentView.Activate();
        }

        private void CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            return;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ListViewLoaded)
            {
                return;
            }

            if (e.AddedItems.Count == 0)
            {
                return;
            }

            if (e.RemovedItems.Count > 0 && e.AddedItems[0].Equals(e.RemovedItems[0]))
            {
                return;
            }

            if (sender is ComboBox comboBox && comboBox.DataContext is AppointmentModel appointment)
            {
                ViewModel.UpdateAppointmentCommand.ExecuteAsync(appointment);
            }
        }

        private void lvAppointments_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ListViewLoaded = true;
        }

        private void btnNoteEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
            {
                return;
            }

            if (sender is Button button && button.DataContext is NoteModel note)
            {
                UpsertNoteView addNoteView = new UpsertNoteView(ViewModel.UserId, note);
                addNoteView.Activate();
            }
        }

        private void btnAppointmentEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null || sender is not Button)
            {
                return;
            }

            if (sender is Button button && button.DataContext is AppointmentModel appointment)
            {
                UpsertAppointmentView addAppointmentView = new UpsertAppointmentView(ViewModel.UserId, appointment);
                addAppointmentView.Activate();
            }
        }

        ~UserAppointmentsView()
        {
            DataContextChanged -= OnDataContextChanged;
            ViewModel?.Dispose();
            ViewModel = null;
            DataContext = null;
            _scope?.Dispose();
            _scope = null;
        }
    }
}
