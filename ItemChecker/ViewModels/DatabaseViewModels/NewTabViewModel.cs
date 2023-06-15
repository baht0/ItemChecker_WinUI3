using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using System;
using System.Collections.ObjectModel;

namespace ItemChecker.ViewModels.DatabaseViewModels
{
    public partial class NewTabViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Item> _base = new(ItemBase.List);
        [ObservableProperty]
        DateTime _baseUpdated = ItemBase.Updated;
    }
}
