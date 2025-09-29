using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MedicalAppointmentsNotifier.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddAppointmentView : Window
{
    private readonly SizeInt32 startSize = new(545, 570);

    public AddAppointmentView(User user)
    {
        AppWindow.Resize(startSize);
        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<AddAppointmentViewModel>();
        ViewModel.OnAppointmentAdded += CloseWindow;
        ViewModel.LoadUser(user);
    }

    public AddAppointmentViewModel ViewModel => (AddAppointmentViewModel)RootGrid.DataContext;

    private void CloseWindow(object? sender, EventArgs e)
    {
        this.Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
