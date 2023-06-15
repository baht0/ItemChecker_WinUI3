using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ItemChecker.Views.Account.Inventory;
using ItemChecker.ViewModels.AccountViewModels;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.UI.Controls;

namespace ItemChecker.Views.Account
{
    public sealed partial class InventoryPage : Page
    {
        readonly static InventoryViewModel ViewModel = new();
        public InventoryPage()
        {
            this.InitializeComponent();
            ViewModel.MessageBar = new();
            DataContext = ViewModel;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.MessageBar = null;
            base.OnNavigatingFrom(e);
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

        private void Interval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (BeginCalendar != null && EndCalendar != null)
            {
                EndCalendar.MaxDate = DateTime.Now;
                BeginCalendar.Date = EndCalendar.Date = null;
                BeginCalendar.IsEnabled = EndCalendar.IsEnabled = (string)cmb?.SelectedItem == "Custom";
            }
            ViewModel.SwitchIntervalCommand.Execute(cmb?.SelectedIndex);
        }
        private void BeginCalendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = sender.Date.GetValueOrDefault().DateTime;
            ViewModel.BeginIntervalCommand.Execute(date);
        }
        private void EndCalendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = sender.Date.GetValueOrDefault().DateTime;
            ViewModel.EndIntervalCommand.Execute(date);
        }
        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
        }
        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e) => VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);

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
        private void dataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.Items.Any())
                ViewModel.OpenInCommand.Execute(((DataGrid)sender).SelectedIndex);
        }

        private void MessageBar_Closing(TeachingTip sender, TeachingTipClosingEventArgs args)
        {
            ViewModel.MessageBar.IsOpen = false;
            //ViewModel.MessageBar = null;
            //ViewModel.MessageBar = new();
        }

    }
}
