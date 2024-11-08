using System.Runtime.InteropServices;

namespace API_AutomationFramework.Helpers
{
    public static class WindowsUtility
    {
        private const int WM_CLOSE = 0x10;

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void CloseWindow(string windowId, string windowTitle)
        {
            IntPtr h = FindWindow(windowId, windowTitle);

            try
            {
                SendMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
                SendMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }

        }
    }
}
