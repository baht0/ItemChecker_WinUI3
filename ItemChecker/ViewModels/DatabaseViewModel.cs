using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.StaticModels;
using System;
using System.Collections.ObjectModel;

namespace ItemChecker.ViewModels
{
    public partial class DatabaseViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Item> baseItems = new(ItemBase.List);
        [ObservableProperty]
        DateTime baseUpdated = ItemBase.Updated;

        [ObservableProperty]
        ObservableCollection<object> tabs = new(Database.Items);
        [ObservableProperty]
        int selectedId = 0;

        [RelayCommand]
        private void AddPage(object[] obj)
        {
            var id = (int)obj[0];

            if (id == -1)
            {
                Tabs.Add(obj[1]);
                Database.Items.Add(obj[1]);
                id = Database.Items.Count - 1;
            }
            SelectedId = id;
        }
        [RelayCommand]
        private void ClosePage(object page)
        {
            Tabs.Remove(page);
            Database.Items.Remove(page);
        }
        [RelayCommand]
        private void CloseAllPage()
        {
            Tabs.Clear();
            Database.Items.Clear();
        }
    }
}
