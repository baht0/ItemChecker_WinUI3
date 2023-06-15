using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ItemChecker.Views.Parser;
using ItemChecker.ViewModels;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;

namespace ItemChecker.Views
{
    public sealed partial class ParserPage : Page
    {
        readonly static ParserViewModel ViewModel = new();
        public ParserPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
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
        private void ImportBtn_Click(object sender, RoutedEventArgs e) => ImportPage.Visibility = ImportPage.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

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
    }
}
