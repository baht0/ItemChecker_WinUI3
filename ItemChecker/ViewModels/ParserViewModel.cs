using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.Parser;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemChecker.ViewModels
{
    public partial class ParserViewModel : BaseViewModel<ParserData>
    {
        [ObservableProperty]
        ParserParameters parameters = new();
        [ObservableProperty]
        LoadingBar _loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;

        public ParserViewModel()
        {
            Info = new()
            {
                { "From:", "-" },
                { "To:", "-" },
                { "Date time:", "-" },
                { "Min. price:", 0 },
                { "Max. price:", 0 },
            };
        }

        [RelayCommand]
        private void Start()
        {

        }
        [RelayCommand]
        private void Stop()
        {

        }
        [RelayCommand]
        private void Continue()
        {

        }

        [RelayCommand]
        private void OpenIn(int id)
        {
            string market_hash_name = Uri.EscapeDataString(Items[id].ItemName);
            Edit.OpenUrl("https://steamcommunity.com/market/listings/730/" + market_hash_name);
        }
        [RelayCommand]
        private void SortingAscending(string column)
        {
            List<ParserData> sort = column switch
            {
                "ItemName" => Items.OrderBy(o => o.ItemName).ToList(),
                "Purchase" => Items.OrderBy(o => o.Purchase).ToList(),
                "Price" => Items.OrderBy(o => o.Price).ToList(),
                "Get" => Items.OrderBy(o => o.Get).ToList(),
                "Difference" => Items.OrderBy(o => o.Difference).ToList(),
                _ => Items.OrderBy(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void SortingDescending(string column)
        {
            List<ParserData> sort = column switch
            {
                "ItemName" => Items.OrderByDescending(o => o.ItemName).ToList(),
                "Purchase" => Items.OrderByDescending(o => o.Purchase).ToList(),
                "Price" => Items.OrderByDescending(o => o.Price).ToList(),
                "Get" => Items.OrderByDescending(o => o.Get).ToList(),
                "Difference" => Items.OrderByDescending(o => o.Difference).ToList(),
                _ => Items.OrderByDescending(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
    }
}
