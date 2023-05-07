using Microsoft.UI.Xaml;
using ItemChecker.Core;
using System;
using System.Diagnostics;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using Microsoft.UI.Xaml.Media;

namespace ItemChecker
{
    public partial class App : Application
    {
        private AppWindow appWindow;

        private static Window m_window;
        internal static Window MainWindow => m_window;
        public static bool HandleClosedEvents { get; set; } = true;

        public App()
        {
            this.InitializeComponent();
        }

        [STAThread]
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            using SingleProgramInstance spi = new("x5k6yz");
            if (!spi.IsSingleInstance)
            {
                spi.RaiseOtherProcess();
                Process.GetCurrentProcess().Kill();
            }

            m_window = new MainWindow
            {
                Title = "ItemChecker"
            };
            m_window.Closed += Window_Closed;

            appWindow = GetAppWindow(m_window);
            appWindow.SetIcon("Assets/icon.ico");
            CenterOfScreen();

            m_window.Activate();
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            if (HandleClosedEvents)
            {
                args.Handled = true;
            }
        }

        private void CenterOfScreen()
        {
            if (DisplayArea.GetFromWindowId(appWindow.OwnerWindowId, DisplayAreaFallback.Nearest) is DisplayArea displayArea)
            {
                PointInt32 CenteredPosition = appWindow.Position;
                CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                appWindow.Move(CenteredPosition);
            }
        }
        private AppWindow GetAppWindow(Window window)
        {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }
    }
}
