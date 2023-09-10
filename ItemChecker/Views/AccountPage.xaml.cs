using Microsoft.UI.Xaml.Controls;
using ItemChecker.Views.Account;
using Microsoft.UI.Xaml.Navigation;

namespace ItemChecker.Views
{
    public sealed partial class AccountPage : Page
    {
        readonly static InfoPage InfoPage = new();
        readonly static InventoryPage InventoryPage = new();
        public AccountPage()
        {
            this.InitializeComponent();
            Info.Content = InfoPage;
            Inventory.Content = InventoryPage;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Info.Content = null;
            Inventory.Content = null;
            base.OnNavigatedFrom(e);
        }
    }
}
