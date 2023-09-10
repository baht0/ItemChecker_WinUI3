using CommunityToolkit.WinUI.UI.Controls;
using ItemChecker.Models;
using ItemChecker.ViewModels;
using ItemChecker.Views.BuyOrders;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using Windows.System;

namespace ItemChecker.Views
{
    public sealed partial class BuyOrdersPage : Page
    {
        readonly static BuyOrderViewModel ViewModel = new();
        public BuyOrdersPage()
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

        private void Push_Click(object sender, RoutedEventArgs e) => PushShowDialogAsync();
        public async void PushShowDialogAsync()
        {
            var oldParam = ViewModel.Parameters.Clone();
            var dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Parameters",
                PrimaryButtonText = "Start",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ParametersPage(ViewModel.Parameters),
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ItemsBtn.IsChecked = false;
                ItemsPage.Visibility = Visibility.Collapsed;
                ViewModel.StartCommand.Execute(null);
            }
            else
                ViewModel.Parameters = oldParam;
        }
        private void ItemsToggle_Click(object sender, RoutedEventArgs e)
        {
            ItemsPage.Visibility = ItemsPage.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            if (ItemsPage.Visibility == Visibility.Visible && ViewModel.OpenQueueCommand.CanExecute(null))
                ViewModel.OpenQueueCommand.Execute(null);
        }

        private void InfoBar_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e) => ViewModel.ResetTimeCommand.Execute(null);
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
        private void dataGrid_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F1)
            {
                var item = ((DataGrid)sender).SelectedItem;
                var mainWindow = (MainWindow)App.MainWindow;
                mainWindow?.NavigationInvoke(item);
            }
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

        private void dataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            object[] args = new object[]
            {
                ((DataGrid)sender).CurrentColumn.DisplayIndex,
                ((DataGrid)sender).SelectedItem,
            };
            ViewModel.OpenInCommand.Execute(args);
        }
    }
}
