using ItemChecker.Views;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using WinRT.Interop;

namespace ItemChecker
{
    public partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();
            m_AppWindow = GetAppWindowForCurrentWindow();
            CenterOfScreen(this);
            Title = "ItemChecker";

            var mainPage = new MainPage();
            Content = mainPage;
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;
                var titleBar = mainPage.CreateAppTitleBar(Title);
                SetTitleBar(titleBar);
            }
            else
            {
                mainPage.RemoveGridRow();
            }
        }
        AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
        void CenterOfScreen(Window window)
        {
            if (DisplayArea.GetFromWindowId(m_AppWindow.OwnerWindowId, DisplayAreaFallback.Nearest) is DisplayArea displayArea)
            {
                PointInt32 CenteredPosition = m_AppWindow.Position;
                CenteredPosition.X = (displayArea.WorkArea.Width - m_AppWindow.Size.Width) / 2;
                CenteredPosition.Y = (displayArea.WorkArea.Height - m_AppWindow.Size.Height) / 2;
                m_AppWindow.Move(CenteredPosition);
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            m_AppWindow.Hide();
            args.Handled = true;
        }
    }
}
