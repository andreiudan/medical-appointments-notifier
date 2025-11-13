using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;

namespace MedicalAppointmentsNotifier.Views;

/// <summary>
/// A window that can be used to update an existing Appointment or add a new one.
/// </summary>
public sealed partial class UpsertNoteView : Window
{
    private readonly SizeInt32 startSize = new(445, 460);

    public UpsertNoteView(Guid userId, NoteModel noteModel = null)
    {
        AppWindow.Resize(startSize);
        AppWindow.SetIcon("Assets/Square44x44Logo.targetsize-16.ico");

        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<UpsertNoteViewModel>();
        
        ViewModel.OnNoteAdded += CloseWindow;
        ViewModel.LoadUserId(userId);
        ViewModel.LoadNote(noteModel);
    }

    public UpsertNoteViewModel ViewModel => (UpsertNoteViewModel)RootGrid.DataContext;

    private void CloseWindow(object? sender, EventArgs e)
    {
        ViewModel.OnNoteAdded -= CloseWindow;

        this.Bindings.StopTracking();
        RootGrid.DataContext = null;
        
        this.Close();
    }

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.CloseWindow(sender, EventArgs.Empty);
    }
}
