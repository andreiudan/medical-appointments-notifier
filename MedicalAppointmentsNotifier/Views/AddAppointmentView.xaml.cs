using MedicalAppointmentsNotifier.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MedicalAppointmentsNotifier.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddAppointmentView : Window
{
    public AddAppointmentView()
    {
        InitializeComponent();
        RootGrid.DataContext = ((App)App.Current).Services.GetService<AddAppointmentViewModel>();
    }

    public AddAppointmentViewModel ViewModel => (AddAppointmentViewModel)RootGrid.DataContext;

    public void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
