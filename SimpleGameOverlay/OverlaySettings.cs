using System;
using System.IO;
using System.Text.Json;

namespace SimpleGameOverlay
{
    public class OverlaySettings
    {
        public bool IsOverlayEnabled { get; set; } = true;
        public uint HotkeyModifiers { get; set; } = MOD_CONTROL | MOD_ALT;
        public uint HotkeyVirtualKey { get; set; } = 0x4F; // 'O'
        public string HotkeyDisplay { get; set; } = "Ctrl+Alt+O";

        public byte ColorR { get; set; } = 255;
        public byte ColorG { get; set; } = 255;
        public byte ColorB { get; set; } = 255;

        public double FontSize { get; set; } = 16;

        public double PositionX { get; set; } = 20;
        public double PositionY { get; set; } = 20;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        private static string SettingsFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleGameOverlay");

        private static string SettingsPath => Path.Combine(SettingsFolder, "settings.json");

        public static OverlaySettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var loaded = JsonSerializer.Deserialize<OverlaySettings>(json);
                    if (loaded != null) return loaded;
                }
            }
            catch
            {
                
            }
            return new OverlaySettings();
        }

        public void Save()
        {
            Directory.CreateDirectory(SettingsFolder);
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
    }
}