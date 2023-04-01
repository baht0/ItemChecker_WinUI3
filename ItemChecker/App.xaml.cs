using Microsoft.UI.Xaml;
using ItemChecker.Core;
using System;
using System.Diagnostics;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT;
using ItemChecker.Helpers;
using Windows.Graphics;

namespace ItemChecker
{
    public partial class App : Application
    {
        private WindowsSystemDispatcherQueueHelper m_wsqdHelper;
        private Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        private Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;
        private AppWindow appWindow;

        private static Window m_window;
        internal static Window MainWindow => m_window;
        public static bool HandleClosedEvents { get; set; } = true;
        public static bool IsMicaSupported { get; set; }

        public App()
        {
            this.InitializeComponent();
        }

        [STAThread]
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            using (SingleProgramInstance spi = new("x5k6yz"))
            {
                if (spi.IsSingleInstance)
                {
                    m_window = new MainWindow
                    {
                        Title = "ItemChecker"
                    };
                    m_window.Closed += Window_Closed;

                    appWindow = GetAppWindow(m_window);
                    appWindow.SetIcon("/Assets/icon.ico");
                    CenterOfScreen();

                    IsMicaSupported = TrySetMicaBackdrop();

                    m_window.Activate();
                }
                else
                {
                    spi.RaiseOtherProcess();
                    Process.GetCurrentProcess().Kill();
                }
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
        private bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsqdHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsqdHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                m_window.Activated += Window_Activated;
                ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            if (HandleClosedEvents)
            {
                args.Handled = true;
            }
            else if (IsMicaSupported)
            {
                // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
                // use this closed window.
                if (m_micaController != null)
                {
                    m_micaController.Dispose();
                    m_micaController = null;
                }
                m_window.Activated -= Window_Activated;
                m_configurationSource = null;
            }
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }
        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)m_window.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
