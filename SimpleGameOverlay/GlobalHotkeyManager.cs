using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SimpleGameOverlay
{
    public class GlobalHotkeyManager : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;

        private readonly HwndSource _source;
        public event Action? HotkeyPressed;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public GlobalHotkeyManager(Window ownerWindow)
        {
            var helper = new WindowInteropHelper(ownerWindow);
            _source = HwndSource.FromHwnd(helper.Handle)!;
            _source.AddHook(HwndHook);
        }

        public void Register(uint modifiers, uint vk)
        {
            UnregisterHotKey(_source.Handle, HOTKEY_ID);
            RegisterHotKey(_source.Handle, HOTKEY_ID, modifiers, vk);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                HotkeyPressed?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            UnregisterHotKey(_source.Handle, HOTKEY_ID);
            _source.RemoveHook(HwndHook);
        }
    }
}