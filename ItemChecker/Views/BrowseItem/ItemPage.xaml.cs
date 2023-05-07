using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views.BrowseItem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        string ItemName { get; set; }
        public ItemPage(string itemName)
        {
            this.InitializeComponent();
            ItemName = itemName;
        }
    }
}
