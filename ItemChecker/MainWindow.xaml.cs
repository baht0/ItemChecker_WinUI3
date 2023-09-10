using ItemChecker.Properties;
using ItemChecker.Views;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using Windows.UI;

namespace ItemChecker
{
    public partial class MainWindow : Window
    {
        private NavigationPage NavigationPage { get; } = new();
        public MainWindow()
        {
            this.InitializeComponent();
            SetWinMinSize();

            if (MicaController.IsSupported() && AppProperties.Settings.IsMicaTheme)
                this.SystemBackdrop = new MicaBackdrop();
            ThemeSwitch();
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(TitleBar);
            }
            else
                MainGrid.Children.Remove(TitleBar);
        }

        public void ThemeSwitch()
        {
            string themeName = AppProperties.Settings.ThemeId switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "Default",
            };
            if (Enum.TryParse(themeName, out ElementTheme theme))
            {
                MainGrid.RequestedTheme = theme;
                if (!MicaController.IsSupported() || !AppProperties.Settings.IsMicaTheme)
                    SetThemeColor();
            }
        }
        public void MicaThemeSwitch(bool isMica)
        {
            if (isMica)
            {
                this.SystemBackdrop = new MicaBackdrop();
                MainGrid.Background = null;
            }
            else
            {
                SetThemeColor();
                this.SystemBackdrop = null;
            }
        }
        private void SetThemeColor()
        {
            MainGrid.Background = MainGrid.RequestedTheme switch
            {
                ElementTheme.Light => new SolidColorBrush(Color.FromArgb(255, 243, 243, 243)),
                ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(255, 32, 32, 32)),
                _ => Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush,
            };
        }

        public void StartUpCompletion()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
                Logo.Visibility = Visibility.Visible;
            if (MicaController.IsSupported() && AppProperties.Settings.IsMicaTheme)
                MainGrid.Background = null;

            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(NavigationPage);
        }

        private void MainGrid_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
                ExitShowDialogAsync();
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
        private void Window_Closed(object sender, WindowEventArgs args) => ExitShowDialogAsync();

        public void NavigationInvoke(object item) => NavigationPage.DatabasePageInvoke(item);

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
