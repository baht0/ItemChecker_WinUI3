using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItemChecker.ViewModels
{
    public partial class DatabaseViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<TabViewItem> _tabs = new(Database.Items);
        [ObservableProperty]
        TabViewItem _selectedTab = Database.SelectedItem;

        public DatabaseViewModel(object page)
        {
            if (Tabs.Count == 0)
                NewTab(page);
            if (SelectedTab.Header == null)
                SelectedTab = Tabs.LastOrDefault();
        }

        [RelayCommand]
        private void NewTab(object page) => AddNewTab(page);
        private void AddNewTab(object page)
        {
            var tab = new TabViewItem()
            {
                Header = "New Tab",
                IconSource = new SymbolIconSource() { Symbol = Symbol.Document },
                Content = page
            };
            Tabs.Add(tab);
            SelectedTab = tab;
        }
        [RelayCommand]
        private void AddItem(TabViewItem item)
        {
            Tabs.Add(item);
            Database.Items.Add(item);
            SelectedTab = item;
        }
        [RelayCommand]
        private void CloseTab(TabViewItem tab)
        {
            Tabs.Remove(tab);
            Database.Items.Remove(tab);
        }
        [RelayCommand]
        private void CloseTabs(object page)
        {
            Tabs.Clear();
            Database.Items.Clear();
            AddNewTab(page);
        }
    }
}
