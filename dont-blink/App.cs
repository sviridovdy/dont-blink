using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace dont_blink
{
    public class AppContext : ApplicationContext
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext());
        }

        public AppContext()
        {
            var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("dont_blink.icon.ico");
            var trayIcon = new NotifyIcon
            {
                Visible = true,
                Icon = new Icon(iconStream)
            };

            const EXECUTION_STATE newState = EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED;
            var originalState = SetThreadExecutionState(newState);

            trayIcon.DoubleClick += (sender, e) =>
            {
                trayIcon.Visible = false;
                SetThreadExecutionState(originalState);
                Application.Exit();
            };
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001,
        }
    }
}