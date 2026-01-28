using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;
using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT;

namespace MedicalAppointmentsNotifier
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public const int WM_SETICON = 0x0080;
        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hInst, string lpszName, uint uType, int cx, int cy, uint fuLoad);

        public const uint IMAGE_ICON = 1;
        public const uint LR_LOADFROMFILE = 0x00000010;

        public MainWindow()
        {
            AppWindow.SetIcon("Assets/Square44x44Logo.altform-unplated_targetsize-256.ico");
            IntPtr hWnd = WindowNative.GetWindowHandle(this);

            string iconPath = "Assets/Square44x44Logo.altform-unplated_targetsize-256.ico"; // Your icon path
            IntPtr hIconSmall = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 16, 16, LR_LOADFROMFILE);
            IntPtr hIconLarge = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);

            SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_SMALL, hIconSmall);
            SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_BIG, hIconLarge);

            InitializeComponent();
        }
    }
}
