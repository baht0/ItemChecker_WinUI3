using CommunityToolkit.WinUI.UI.Controls;
using ItemChecker.ViewModels.DatabaseViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views.Database
{
    public sealed partial class ItemPage : Page
    {
        static ItemViewModel ViewModel;
        public ItemPage(string itemName)
        {
            this.InitializeComponent();
            ViewModel = new(itemName);
            DataContext = ViewModel;
        }

        private void dataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
            => ViewModel.OpenInCommand.Execute(((DataGrid)sender).SelectedIndex);
    }
}
