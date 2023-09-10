using ItemChecker.ViewModels;
using ItemChecker.Views.Database;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace ItemChecker.Views
{
    public sealed partial class DatabasePage : Page
    {
        private readonly DatabaseViewModel ViewModel = new();
        public DatabasePage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Tabs.Clear();
            base.OnNavigatingFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            OpenItem(e.Parameter);
            base.OnNavigatedTo(e);
        }

        private async void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Tab",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
                Content = new StackPanel()
                {
                    Children =
                    {
                        new TextBlock()
                        {
                            Text = "Do you really want to close?"
                        },
                        new TextBlock()
                        {
                            Style = Application.Current.Resources["BodyStrongTextBlockStyle"] as Style,
                            Text = args.Tab.Header.ToString()
                        },
                    }
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.ClosePageCommand.Execute(args.Tab);
            }
        }
        private void CloseTabs_Click(object sender, RoutedEventArgs e) => ViewModel.CloseAllPageCommand.Execute(null);

        #region Search
        private bool suggestionChosen = false;
        private void searchSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                if (!string.IsNullOrEmpty(term))
                {
                    var results = ViewModel.BaseItems.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                    searchSuggest.ItemsSource = results;
                    searchSuggest.IsSuggestionListOpen = true;
                }
                else if (suggestionChosen)//unfocus searchSuggest
                {
                    MainTabView.Focus(FocusState.Programmatic);
                    suggestionChosen = false;
                }
            }
        }
        private void searchSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var term = args.QueryText.ToLower();
            var results = ViewModel.BaseItems.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
            searchSuggest.ItemsSource = results;
            searchSuggest.IsSuggestionListOpen = true;
        }
        private void searchSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            suggestionChosen = true;
            searchSuggest.Text = string.Empty;
            OpenItem(args.SelectedItem);
        }
        #endregion

        public void OpenItem(object item)
        {
            if (item == null)
                return;

            string itemName = item is string str ? str : item.GetType().GetProperty("ItemName").GetValue(item, null).ToString();
            var page = MainTabView.TabItems.FirstOrDefault(x => ((TabViewItem)x).Header.ToString() == itemName);

            var obj = new object[]
            {
                MainTabView.TabItems.IndexOf(page),
                new TabViewItem()
                {
                    Header = itemName,
                    Content = new ItemPage(itemName),
                }
            };
            ViewModel.AddPageCommand.Execute(obj);
        }
    }
}
