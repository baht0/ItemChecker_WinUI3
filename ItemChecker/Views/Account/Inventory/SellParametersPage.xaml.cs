using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views.Account.Inventory
{
    public sealed partial class SellParametersPage : Page
    {
        public SellParametersPage(object arg)
        {
            this.InitializeComponent();
            DataContext = arg;
        }
    }
}
