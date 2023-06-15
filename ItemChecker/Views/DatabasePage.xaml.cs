using ItemChecker.ViewModels;
using ItemChecker.Views.Database;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace ItemChecker.Views
{
    public sealed partial class DatabasePage : Page
    {
        DatabaseViewModel ViewModel;
        public DatabasePage() => this.InitializeComponent();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new(new NewTabPage(this));
            DataContext = ViewModel;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Tabs.Clear();
            base.OnNavigatingFrom(e);
        }

        private void TabView_AddButtonClick(TabView sender, object args)
            => ViewModel.NewTabCommand.Execute(new NewTabPage(this));
        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count > 1)
                ViewModel.CloseTabCommand.Execute(args.Tab);
            else if (ViewModel.Tabs.FirstOrDefault() is TabViewItem tab && (string)tab.Header != "New Tab")
            {
                ViewModel.CloseTabCommand.Execute(args.Tab);
                ViewModel.NewTabCommand.Execute(new NewTabPage(this));
            }
        }

        public void OpenItem(string itemName)
        {
            ViewModel.CloseTabCommand.Execute(ViewModel.SelectedTab);

            var tab = ViewModel.Tabs.FirstOrDefault(x => (string)x.Header == itemName);
            if (tab == null)
            {
                tab = new TabViewItem()
                {
                    Header = itemName,
                    IconSource = new SymbolIconSource() { Symbol = Symbol.ShowResults },
                    Content = new ItemPage(itemName)
                };
                ViewModel.AddItemCommand.Execute(tab);
            }
            ViewModel.SelectedTab = tab;
        }

        private void CloseTabs_Click(object sender, RoutedEventArgs e)
            => ViewModel.CloseTabsCommand.Execute(new NewTabPage(this));
    }
}
