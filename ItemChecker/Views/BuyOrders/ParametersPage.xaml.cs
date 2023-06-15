using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views.BuyOrders
{
    public sealed partial class ParametersPage : Page
    {
        public ParametersPage(object arg)
        {
            this.InitializeComponent();
            DataContext = arg;
        }
    }
}
