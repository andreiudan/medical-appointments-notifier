using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace MedicalAppointmentsNotifier.Views
{
    /// <summary>
    /// A window that can be used to update an existing User or add a new one.
    /// </summary>
    public sealed partial class UpsertUserView : Window
    {
        private readonly Windows.Graphics.SizeInt32 startSize = new(460, 435);

        public UpsertUserView(UserModel userModel = null)
        {
            AppWindow.Resize(startSize);
            AppWindow.SetIcon("Assets/Square44x44Logo.targetsize-16.ico");

            InitializeComponent();

            RootGrid.DataContext = ((App)App.Current).Services.GetRequiredService<UpsertUserViewModel>();

            this.Closed += UpsertUserView_Closed;
            ViewModel.OnUserAdded += CloseWindow;
            ViewModel.LoadUser(userModel);
        }

        public UpsertUserViewModel ViewModel => (UpsertUserViewModel)RootGrid.DataContext;

        private void CloseWindow(object? sender, EventArgs e)
        {
            ViewModel.OnUserAdded -= CloseWindow;
            this.Close();
        }

        private void UpsertUserView_Closed(object? sender, WindowEventArgs e)
        {
            try
            {
                if (ViewModel is not null)
                {
                    ViewModel.OnUserAdded -= CloseWindow;

                    if (ViewModel is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch { }

            this.Bindings.StopTracking();
            RootGrid.DataContext = null;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow(sender, EventArgs.Empty);
        }
    }
}
