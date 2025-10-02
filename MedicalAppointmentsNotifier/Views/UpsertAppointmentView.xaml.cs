using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
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
public sealed partial class UpsertAppointmentView : Window
{
    private readonly SizeInt32 startSize = new(600, 470);

    public UpsertAppointmentView(Guid userId, AppointmentModel appointment = null)
    {
        AppWindow.Resize(startSize);
        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<UpsertAppointmentViewModel>();
        ViewModel.OnAppointmentAdded += CloseWindow;
        ViewModel.LoadUserId(userId);
        ViewModel.LoadAppointment(appointment);
    }

    public UpsertAppointmentViewModel ViewModel => (UpsertAppointmentViewModel)RootGrid.DataContext;

    private void CloseWindow(object? sender, EventArgs e)
    {
        this.Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
