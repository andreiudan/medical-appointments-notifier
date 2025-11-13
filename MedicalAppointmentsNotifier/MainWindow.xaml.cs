using Microsoft.UI.Xaml;

namespace MedicalAppointmentsNotifier
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppWindow.SetIcon("Assets/Square44x44Logo.targetsize-16.ico");
            InitializeComponent();
        }
    }
}
