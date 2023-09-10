using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ItemChecker.Views.Parser;
using ItemChecker.ViewModels;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.System;
using Microsoft.UI.Xaml.Navigation;
using ItemChecker.Models;

namespace ItemChecker.Views
{
    public sealed partial class ParserPage : Page
    {
        readonly static ParserViewModel ViewModel = new();
        public ParserPage()
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

        private void Check_Click(object sender, RoutedEventArgs e) => CheckShowDialogAsync();
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
                ImportBtn.IsChecked = false;
                ImportPage.Visibility = Visibility.Collapsed;
                ViewModel.StartCommand.Execute(null);
            }
            else
                ViewModel.Parameters = oldParam;
        }
        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            ImportPage.Visibility = ImportPage.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            if (ImportPage.Visibility == Visibility.Visible && ViewModel.OpenImportCommand.CanExecute(null))
                ViewModel.OpenImportCommand.Execute(null);
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
        private void dataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            object[] args = new object[]
            {
                ((DataGrid)sender).CurrentColumn.DisplayIndex,
                ((DataGrid)sender).SelectedItem,
            };
            if (args[1] != null)
                ViewModel.OpenInCommand.Execute(args);
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
        private void AdditionalBtn_Click(object sender, RoutedEventArgs e) => FilterTeachingTip.IsOpen = true;

        private void ListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            ViewModel.ImportDataCommand.Execute(null);
            ImportBtn.IsChecked = false;
            ImportPage.Visibility = ImportPage.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
