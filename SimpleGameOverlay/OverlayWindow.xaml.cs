using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace SimpleGameOverlay
{
    public partial class OverlayWindow : Window
    {
        
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int GWL_EXSTYLE = -20;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(System.IntPtr hwnd, int index);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(System.IntPtr hwnd, int index, int newStyle);

        public OverlayWindow()
        {
            InitializeComponent();
            this.Loaded += OverlayWindow_Loaded;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = 20;
            this.Top = 20;

            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE,
                extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
        }

        public void UpdateValues(string gpuLoad, string gpuTemp, string cpuLoad, string cpuTemp)
        {
            GpuLoadValue.Text = gpuLoad;
            GpuTempValue.Text = gpuTemp;
            CpuLoadValue.Text = cpuLoad;
            CpuTempValue.Text = cpuTemp;
        }

        public void ApplySettings(OverlaySettings settings)
        {
            this.Left = settings.PositionX;
            this.Top = settings.PositionY;

            var color = System.Windows.Media.Color.FromRgb(settings.ColorR, settings.ColorG, settings.ColorB);
            var brush = new SolidColorBrush(color);

            GpuLoadValue.Foreground = brush;
            GpuTempValue.Foreground = brush;
            CpuLoadValue.Foreground = brush;
            CpuTempValue.Foreground = brush;

            GpuLoadValue.FontSize = settings.FontSize;
            GpuTempValue.FontSize = settings.FontSize;
            CpuLoadValue.FontSize = settings.FontSize;
            CpuTempValue.FontSize = settings.FontSize;

            this.Visibility = settings.IsOverlayEnabled ? Visibility.Visible : Visibility.Hidden;
        }
    }
}