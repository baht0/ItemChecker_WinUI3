using System.Linq;
using Microsoft.UI.Xaml.Controls;
using ItemChecker.Views.BrowseItem;

namespace ItemChecker.Views
{
    public sealed partial class BrowseItemPage : Page
    {
        public BrowseItemPage()
        {
            this.InitializeComponent();
        }
        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (true)
            {
                CreateNewTab();
            }
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            CreateNewTab();
        }
        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count > 1)
                sender.TabItems.Remove(args.Tab);
            else if (sender.TabItems.FirstOrDefault() is TabViewItem tab && (string)tab.Header != "New Tab")
            {
                sender.TabItems.Remove(args.Tab);
                CreateNewTab();
            }
        }
        private void CreateNewTab()
        {
            TabViewItem newTab = new()
            {
                Header = "New Tab",
                IconSource = new SymbolIconSource() { Symbol = Symbol.Document },
                Content = new NewTabPage(this)
            };
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        public void OpenItem(string itemName)
        {
            var tab = MainTabView.SelectedItem as TabViewItem;
            MainTabView.TabItems.Remove(tab);
            if (!MainTabView.TabItems.Any(x => ((TabViewItem)x).Header.ToString() == itemName))
            {
                var tabItem = new TabViewItem()
                {
                    Header = itemName,
                    IconSource = new SymbolIconSource() { Symbol = Symbol.ShowResults },
                    Content = new ItemPage(itemName)
                };
                MainTabView.TabItems.Add(tabItem);
                MainTabView.SelectedItem = tabItem;
            }
            else
            {
                MainTabView.SelectedItem = MainTabView.TabItems.FirstOrDefault(x => ((TabViewItem)x).Header.ToString() == itemName);
            }
        }

        private void CloseTabs_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainTabView.TabItems.Clear();
            CreateNewTab();
        }
    }
}
