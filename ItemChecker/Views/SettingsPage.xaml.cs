using Microsoft.UI.Xaml.Controls;
using ItemChecker.ViewModels;
using Newtonsoft.Json.Linq;

namespace ItemChecker.Views
{
    public sealed partial class SettingsPage : Page
    {
        readonly static SettingsViewModel ViewModel = new();
        public SettingsPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var win = (MainWindow)App.MainWindow;
            win.ThemeSwitch();
        }

        private void ToggleSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var value = ((ToggleSwitch)sender).IsOn;
            var win = (MainWindow)App.MainWindow;
            win.MicaThemeSwitch(value);
        }
    }
}
