using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ItemChecker.Views.Account.Inventory;
using ItemChecker.ViewModels.AccountViewModels;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.System;
using ItemChecker.Models;

namespace ItemChecker.Views.Account
{
    public sealed partial class InventoryPage : Page
    {
        readonly static InventoryViewModel ViewModel = new();
        public InventoryPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            ViewModel.MessageEvent += MessageShow_Handler;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.MessageEvent -= MessageShow_Handler;
            base.OnNavigatedFrom(e);
        }

        private void MessageShow_Handler(object sender, EventArgs e)
        {
            var args = (MessageEventArgs)e;
            MessageBar.Severity = (InfoBarSeverity)args.Icon;
            MessageBar.Title = args.Title;
            MessageBar.Message = args.Message;
            MessageTip.IsOpen = true;
        }

        private void Sell_Click(object sender, RoutedEventArgs e) => SellShowDialogAsync();
        public async void SellShowDialogAsync()
        {
            ViewModel.Parameters = new();
            var dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Parameters",
                PrimaryButtonText = "Start",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new SellParametersPage(ViewModel.Parameters),
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
                ViewModel.SellItemsCommand.Execute(null);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                var results = ViewModel.Items.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                dataGrid.ItemsSource = results;
            }
        }
        private void dataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
            {
                ViewModel.SortingAscendingCommand.Execute(e.Column.Tag.ToString());
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                ViewModel.SortingDescendingCommand.Execute(e.Column.Tag.ToString());
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }
            foreach (var dgColumn in dataGrid.Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }
        private void dataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F1)
            {
                var item = ((DataGrid)sender).SelectedItem;
                var mainWindow = (MainWindow)App.MainWindow;
                mainWindow?.NavigationInvoke(item);
            }
        }
        private void dataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.Items.Any())
                ViewModel.OpenInCommand.Execute(((DataGrid)sender).SelectedIndex);
        }
    }
}
