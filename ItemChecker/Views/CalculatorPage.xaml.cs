using ItemChecker.ViewModels;
using Microsoft.UI.Xaml.Controls;

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
    }
}
