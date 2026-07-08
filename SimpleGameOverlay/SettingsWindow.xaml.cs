using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleGameOverlay
{
    public partial class SettingsWindow : Window
    {
        private readonly OverlaySettings _settings;
        private bool _isInitializing = true;

        public SettingsWindow(OverlaySettings settings)
        {
            InitializeComponent();
            _settings = settings;

            EnabledCheckBox.IsChecked = _settings.IsOverlayEnabled;
            HotkeyTextBox.Text = _settings.HotkeyDisplay;

            RedSlider.Value = _settings.ColorR;
            GreenSlider.Value = _settings.ColorG;
            BlueSlider.Value = _settings.ColorB;

            FontSizeSlider.Value = _settings.FontSize;

            PositionXSlider.Maximum = SystemParameters.PrimaryScreenWidth;
            PositionYSlider.Maximum = SystemParameters.PrimaryScreenHeight;
            PositionXSlider.Value = _settings.PositionX;
            PositionYSlider.Value = _settings.PositionY;

            EnabledCheckBox.Checked += (s, e) => LiveApply();
            EnabledCheckBox.Unchecked += (s, e) => LiveApply();

            UpdateColorPreview();
            _isInitializing = false;
        }

        private void LiveApply()
        {
            if (_isInitializing) return;

            _settings.IsOverlayEnabled = EnabledCheckBox.IsChecked == true;
            _settings.ColorR = (byte)RedSlider.Value;
            _settings.ColorG = (byte)GreenSlider.Value;
            _settings.ColorB = (byte)BlueSlider.Value;

            _settings.FontSize = FontSizeSlider.Value;
            _settings.PositionX = PositionXSlider.Value;
            _settings.PositionY = PositionYSlider.Value;

            App.Overlay!.ApplySettings(_settings);
            UpdateColorPreview();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin)
                return;

            uint modifiers = 0;
            string modText = "";

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) { modifiers |= OverlaySettings.MOD_CONTROL; modText += "Ctrl+"; }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) { modifiers |= OverlaySettings.MOD_ALT; modText += "Alt+"; }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) { modifiers |= OverlaySettings.MOD_SHIFT; modText += "Shift+"; }

            uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);

            _settings.HotkeyModifiers = modifiers;
            _settings.HotkeyVirtualKey = vk;
            _settings.HotkeyDisplay = modText + key;

            HotkeyTextBox.Text = _settings.HotkeyDisplay;
        }

        private void ColorSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LiveApply();
        }

        private void UpdateColorPreview()
        {
            byte r = (byte)RedSlider.Value;
            byte g = (byte)GreenSlider.Value;
            byte b = (byte)BlueSlider.Value;
            ColorPreview.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }

        private void FontSizeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LiveApply();
        }

        private void PositionSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LiveApply();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            LiveApply();
            _settings.Save();
            App.RefreshHotkey();

            System.Windows.MessageBox.Show("Settings saved!", "SGO",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}