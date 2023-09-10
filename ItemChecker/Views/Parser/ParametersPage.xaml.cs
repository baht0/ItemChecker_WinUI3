using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace ItemChecker.Views.Parser
{
    public sealed partial class ParametersPage : Page
    {
        public ParametersPage(object arg)
        {
            this.InitializeComponent();
            DataContext = arg;
        }

        private void Service1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int cmbId = Service1Cmb.SelectedIndex;
            if (Service1Cmb != null)
            {
                All.Visibility = Info.Visibility = cmbId == 4 || cmbId == 5 ? Visibility.Visible : Visibility.Collapsed;
                NotWeapon.Visibility = cmbId != 4 && cmbId != 5 ? Visibility.Visible : Visibility.Collapsed;
            }
            if (Service2Cmb.SelectedIndex == cmbId)
                Service2Cmb.SelectedIndex = cmbId > 0 ? 0 : 1;
        }
        private void Service2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int cmbId = Service2Cmb.SelectedIndex;
            if (Service1Cmb.SelectedIndex == cmbId)
                Service1Cmb.SelectedIndex = cmbId > 0 ? 0 : 1;
        }
        private void MinPrice_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (sender.Value > MaxPrice.Value)
                MaxPrice.Value = sender.Value;
        }
        private void MaxPrice_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (sender.Value < MinPrice.Value)
                MinPrice.Value = sender.Value;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string name = ((CheckBox)sender).Name.ToString();
            int cmbId = Service1Cmb.SelectedIndex;
            if (cmbId == 4 || cmbId == 5)
            {
                All.IsChecked = name == "All";
                NotWeapon.IsChecked = name == "NotWeapon";
                Normal.IsChecked = name == "Normal";
                Souvenir.IsChecked = name == "Souvenir";
                Unique.IsChecked = name == "Unique";
                Stattrak.IsChecked = name == "Stattrak";
                UniqueStattrak.IsChecked = name == "UniqueStattrak";
            }
        }
    }
}
