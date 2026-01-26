using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;

namespace MedicalAppointmentsNotifier.Views;

/// <summary>
/// A window that can be used to update an existing Note or add a new one.
/// </summary>
public sealed partial class UpsertAppointmentView : Window
{
    private readonly SizeInt32 noScheduleInfoSize = new(625, 545);
    private readonly SizeInt32 scheduleInfoSize = new(625, 750);

    public UpsertAppointmentView(Guid userId, AppointmentModel appointment = null, bool scheduleTrigger = false)
    {
        AppWindow.Resize(noScheduleInfoSize);
        AppWindow.SetIcon("Assets/Square44x44Logo.targetsize-16.ico");

        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<UpsertAppointmentViewModel>();

        ViewModel.OnCompleted += CloseWindow;
        ViewModel.LoadUserIdCommand.ExecuteAsync(userId);
        ViewModel.LoadAppointmentCommand.ExecuteAsync(appointment);

        if (scheduleTrigger)
        {
            ViewModel.TriggerScheduleCommand.ExecuteAsync(scheduleTrigger);
        }
    }

    public UpsertAppointmentViewModel ViewModel => (UpsertAppointmentViewModel)RootGrid.DataContext;

    private void CloseWindow(object? sender, EventArgs e)
    {
        ViewModel.OnCompleted -= CloseWindow;
        ViewModel.Dispose();

        this.Bindings?.StopTracking();
        RootGrid.DataContext = null;

        this.Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.CloseWindow(sender, EventArgs.Empty);
    }

    public void ResizeWindowOnChecked(object sender, RoutedEventArgs e)
    {
        AppWindow.Resize(scheduleInfoSize);
    }

    public void ResizeWindowOnUnchecked(object sender, RoutedEventArgs e)
    {
        AppWindow.Resize(noScheduleInfoSize);
    }
}
