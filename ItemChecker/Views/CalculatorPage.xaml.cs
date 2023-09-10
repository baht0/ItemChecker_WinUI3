using ItemChecker.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.Views
{
    public sealed partial class CalculatorPage : Page
    {
        readonly static CalculatorViewModel ViewModel = new();
        public CalculatorPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => commission.IsEnabled = ((ComboBox)sender).SelectedItem.ToString() == "Custom";

        private void Copy_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var str = ((TextBlock)sender).Text;
            var dataPackage = new DataPackage();
            dataPackage.SetText(str);
            Clipboard.SetContent(dataPackage);
        }
    }
}
