using MedicalAppointmentsNotifier.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MedicalAppointmentsNotifier.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddNoteView : Window
{
    private readonly SizeInt32 startSize = new(460, 555);

    public AddNoteView()
    {
        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<AddNoteViewModel>();
        AppWindow.Resize(startSize);
    }

    public AddNoteViewModel ViewModel => (AddNoteViewModel)RootGrid.DataContext;

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
