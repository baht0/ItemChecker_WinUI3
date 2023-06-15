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
            var cmb = sender as ComboBox;
            if (cmb != null)
                CreateButtons(cmb.SelectedIndex);
        }
        void CreateButtons(int serviceOneId)
        {
            configGrid.Children.Clear();
            string[] bind = new string[] { "Normal", "Souvenir", "Unique", string.Empty, "Stattrak", "UniqueStattrak" };
            string[] content = new string[] { "Normal", "Souvenir", "★", string.Empty, "StatTrak™", "★ StatTrak™" };
            content[3] = bind[3] = serviceOneId == 4 || serviceOneId == 5 ? "All" : "NotWeapon";
            for (int i = 0; i < content.Length; i++)
            {
                dynamic check = serviceOneId == 4 || serviceOneId == 5 ? new RadioButton() : new CheckBox();
                check.Content = content[i];
                var binding = new Binding
                {
                    Path = new PropertyPath(bind[i]),
                    Mode = BindingMode.TwoWay,
                };
                if (serviceOneId == 4 || serviceOneId == 5)
                    check.SetBinding(RadioButton.IsCheckedProperty, binding);
                else
                    check.SetBinding(CheckBox.IsCheckedProperty, binding);

                configGrid.Children.Add(check);
                switch (i)
                {
                    case 0:
                        Grid.SetRow(check, 0);
                        Grid.SetColumn(check, 0);
                        break;
                    case 1:
                        Grid.SetRow(check, 1);
                        Grid.SetColumn(check, 0);
                        break;
                    case 2:
                        Grid.SetRow(check, 2);
                        Grid.SetColumn(check, 0);
                        break;
                    case 3:
                        Grid.SetRow(check, 0);
                        Grid.SetColumn(check, 1);
                        break;
                    case 4:
                        Grid.SetRow(check, 1);
                        Grid.SetColumn(check, 1);
                        break;
                    case 5:
                        Grid.SetRow(check, 2);
                        Grid.SetColumn(check, 1);
                        break;
                }
            }
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
    }
}
