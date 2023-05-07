using ItemChecker.Views;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;

namespace ItemChecker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            SetWinMinSize();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(TitleBar);
            }
            else
            {
                TitleBar.Visibility = Visibility.Collapsed;
                MainGrid.RowDefinitions.Remove(TitleRow);
            }
        }

        public void StartUpCompletion()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                Logo.Visibility = Visibility.Visible;
                LogoLabel.Visibility = Visibility.Visible;
            }
            if (MicaController.IsSupported())
            {
                MainGrid.Background = null;
                this.SystemBackdrop = new MicaBackdrop();
            }
            MainGrid.Children.Remove(StartUp);
            var navigationPage = new NavigationPage();
            MainGrid.Children.Add(navigationPage);
            Grid.SetRow(navigationPage, 1);
        }

        public async void ExitShowDialogAsync()
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = MainGrid.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Exit",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
                Content = new TextBlock()
                {
                    Text = "Do you really want to quit?"
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                App.HandleClosedEvents = false;
                App.MainWindow.Close();
            }
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            ExitShowDialogAsync();
        }

        #region WindowSize
        IntPtr hWnd = IntPtr.Zero;
        private SUBCLASSPROC SubClassDelegate;
        public void SetWinMinSize()
        {
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bReturn = SetWindowSubclass(hWnd, SubClassDelegate, 0, 0);
        }
        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_GETMINMAXINFO:
                    {
                        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                        mmi.ptMinTrackSize.X = 1000;
                        mmi.ptMinTrackSize.Y = 500;
                        Marshal.StructureToPtr(mmi, lParam, false);
                        return 0;
                    }
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }


        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_GETMINMAXINFO = 0x0024;

        public struct MINMAXINFO
        {
            public System.Drawing.Point ptReserved;
            public System.Drawing.Point ptMaxSize;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Point ptMinTrackSize;
            public System.Drawing.Point ptMaxTrackSize;
        }
        #endregion
    }
}
