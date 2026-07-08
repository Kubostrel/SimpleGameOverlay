using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms; 
using System.Drawing;       

namespace SimpleGameOverlay
{
    public partial class App : System.Windows.Application
    {
        public static OverlayWindow? Overlay;
        public static OverlaySettings Settings = OverlaySettings.Load();

        private HardwareMonitorService? _hardwareService;
        private DispatcherTimer? _timer;
        private NotifyIcon? _trayIcon;
        private static GlobalHotkeyManager? _hotkeyManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Overlay = new OverlayWindow();
            Overlay.Show();
            Overlay.ApplySettings(Settings);

            _hardwareService = new HardwareMonitorService();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            SetupTrayIcon();
            SetupHotkey();
        }

        private void SetupTrayIcon()
        {
            string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "icon.ico");

            _trayIcon = new NotifyIcon
            {
                Icon = System.IO.File.Exists(iconPath)
                    ? new System.Drawing.Icon(iconPath)
                    : System.Drawing.SystemIcons.Application,

                Visible = true,
                Text = "Simple Game Overlay"
            };

            var menu = new ContextMenuStrip();

            menu.Items.Add("Settings", null, (s, e) => OpenSettings());
            menu.Items.Add("Exit", null, (s, e) =>
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            });

            _trayIcon.ContextMenuStrip = menu;
            _trayIcon.DoubleClick += (s, e) => OpenSettings();
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(Settings);
            settingsWindow.ShowDialog();
        }

        private void SetupHotkey()
        {
            _hotkeyManager = new GlobalHotkeyManager(Overlay!);
            _hotkeyManager.HotkeyPressed += () =>
            {
                Settings.IsOverlayEnabled = !Settings.IsOverlayEnabled;
                Overlay!.Visibility = Settings.IsOverlayEnabled ? Visibility.Visible : Visibility.Hidden;
                Settings.Save();
            };
            RefreshHotkey();
        }

        public static void RefreshHotkey()
        {
            _hotkeyManager?.Register(Settings.HotkeyModifiers, Settings.HotkeyVirtualKey);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var data = _hardwareService!.GetData();

            Overlay!.UpdateValues(
                gpuLoad: $"{data.GpuLoad:F0}%",
                gpuTemp: $"{data.GpuTemp:F0}",
                cpuLoad: $"{data.CpuLoad:F0}%",
                cpuTemp: $"{data.CpuTemp:F0}"
            );
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hardwareService?.Close();
            _timer?.Stop();
            _trayIcon?.Dispose();
            _hotkeyManager?.Dispose();
            base.OnExit(e);
        }
    }
}