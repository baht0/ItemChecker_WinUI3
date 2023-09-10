using CommunityToolkit.WinUI.UI.Controls;
using ItemChecker.Models;
using ItemChecker.ViewModels;
using ItemChecker.Views.Rare;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace ItemChecker.Views
{
    public sealed partial class RarePage : Page
    {
        readonly static RareViewModel ViewModel = new();
        public RarePage()
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

        private void Start_Click(object sender, RoutedEventArgs e) => CheckShowDialogAsync();
        public async void CheckShowDialogAsync()
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
        private void ItemsToggle_Click(object sender, RoutedEventArgs e) => ItemsPage.Visibility = ItemsPage.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

        private void InfoBar_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => ViewModel.ResetTimeCommand.Execute(null);

        private void dataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.Items.Any())
                ViewModel.OpenInCommand.Execute(((DataGrid)sender).SelectedIndex);
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
        private void FilterItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = (ToggleMenuFlyoutItem)sender;
            var pair = new KeyValuePair<string, bool>((string)btn.Tag, btn.IsChecked);
            ViewModel.FilterCommand.Execute(pair);
        }

        #region RareItems
        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }
        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e) => VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);

        private void addAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            listSuggest.Text = string.Empty;
            switch (btn.IsChecked)
            {
                case true:
                    serviceCmb.IsEnabled = true;
                    listSuggest.ItemsSource = ViewModel.RareItems.AddBase;
                    listSuggest.TextChanged -= listSuggestSearch_TextChanged;
                    listSuggest.TextChanged += listSuggestAdd_TextChanged;
                    listSuggest.QuerySubmitted += listSuggest_QuerySubmitted;
                    listSuggest.SuggestionChosen += listSuggest_SuggestionChosen;
                    break;
                case false:
                    serviceCmb.IsEnabled = false;
                    listSuggest.ItemsSource = null;
                    listSuggest.TextChanged -= listSuggestAdd_TextChanged;
                    listSuggest.TextChanged += listSuggestSearch_TextChanged;
                    listSuggest.QuerySubmitted -= listSuggest_QuerySubmitted;
                    listSuggest.SuggestionChosen -= listSuggest_SuggestionChosen;
                    break;
            }
        }
        private void listSuggestSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                var results = ViewModel.RareItems.Items.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                itemsListView.ItemsSource = results;
            }
        }
        private bool suggestionChosen = false;
        private void listSuggestAdd_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                if (!string.IsNullOrEmpty(term))
                {
                    var results = ViewModel.RareItems.AddBase.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                    listSuggest.ItemsSource = results;
                    listSuggest.IsSuggestionListOpen = true;
                }
                else if (suggestionChosen)//unfocus searchSuggest
                {
                    ItemsPage.Focus(FocusState.Programmatic);
                    suggestionChosen = false;
                }
            }
        }
        private void listSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var term = args.QueryText.ToLower();
            var results = ViewModel.RareItems.AddBase.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
            listSuggest.ItemsSource = results;
            listSuggest.IsSuggestionListOpen = true;
        }
        private void listSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            suggestionChosen = true;
            listSuggest.IsSuggestionListOpen = false;
            listSuggest.Text = string.Empty;
            ViewModel.AddToListCommand.Execute(args.SelectedItem);
            AddListCommand();
        }

        private void itemsListView_Loaded(object sender, RoutedEventArgs e) => AddListCommand();
        private async void AddListCommand()
        {
            var deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
            deleteCommand.ExecuteRequested += DeleteCommand;

            await Task.Run(() =>
            {
                foreach (var i in ViewModel.RareItems.Items)
                    i.Command = deleteCommand;
            });
        }
        public void DeleteCommand(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
                ViewModel.DeleteItemCommand.Execute(args.Parameter);
        }
        #endregion
    }
}
