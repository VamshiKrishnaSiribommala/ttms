using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TMS
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiFlag);

        [STAThread]
        static void Main()
        {
            try
            {
                // DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4
                if (Environment.OSVersion.Version.Major >= 10)
                {
                    SetProcessDpiAwarenessContext((IntPtr)(-4));
                }
                else
                {
                    SetProcessDPIAware();
                }
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}

