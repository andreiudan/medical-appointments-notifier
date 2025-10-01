using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MedicalAppointmentsNotifier.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddUserView : Window
    {
        private readonly Windows.Graphics.SizeInt32 startSize = new(460, 402);

        public AddUserView()
        {
            AppWindow.Resize(startSize);
            InitializeComponent();
            RootGrid.DataContext = ((App)App.Current).Services.GetService<AddUserViewModel>();
            ViewModel.OnUserAdded += CloseWindow;
        }

        public AddUserViewModel ViewModel => (AddUserViewModel)RootGrid.DataContext;

        private void CloseWindow(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
