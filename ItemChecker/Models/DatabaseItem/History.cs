using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemChecker.Models.DatabaseItem
{
    public partial class History : ObservableObject
    {
        private Item _item;
        [ObservableProperty]
        bool _isBusy = true;
        [ObservableProperty]
        List<SaleHistory> _saleHistory;
        [ObservableProperty]
        Dictionary<string, object> _info = new();

        [ObservableProperty]
        bool _isCount;
        [ObservableProperty]
        bool _isBuyOrder;

        public History(Item item, int id)
        {
            _item = item;
            Info = new();
            GetSaleHistory(id);
            IsBusy = false;
        }
        private async void GetSaleHistory(int id)
        {
            string currency = Currencies.Allow[0].Name;
            switch (id)
            {
                case 0 or 1:
                    await _item.UpdateSteamHistoryItem();
                    IsCount = true;
                    IsBuyOrder = false;
                    SaleHistory = _item.Steam.History;

                    Info.Add("LastSale:", SaleHistory.FirstOrDefault()?.Date);

                    var last30 = SaleHistory.Where(x => x.Date > DateTime.Today.AddDays(-30)).Select(x => x.Price).ToList();
                    var avg30 = last30.Any() ? Queryable.Average(last30.AsQueryable()) : 0;
                    Info.Add($"Last 30 days Avg ({last30.Count}):", Math.Round(avg30, 2));

                    var last60 = SaleHistory.Where(x => x.Date > DateTime.Today.AddDays(-60)).Select(x => x.Price).ToList();
                    var avg60 = last60.Any() ? Queryable.Average(last60.AsQueryable()) : 0;
                    Info.Add($"Last 60 days Avg ({last60.Count}):", Math.Round(avg60, 2));

                    currency = SteamAccount.Currency.Name;
                    break;
                case 4 or 5:
                    await _item.UpdateBuffHistoryItem();
                    IsCount = false;
                    IsBuyOrder = true;
                    SaleHistory = _item.Buff.History;

                    Info.Add("LastSale:", SaleHistory.FirstOrDefault()?.Date);

                    var avg = SaleHistory.Any() ? Queryable.Average(SaleHistory.Select(x => x.Price).AsQueryable()) : 0;
                    Info.Add($"All sale Avg:", Math.Round(avg, 2));

                    avg = SaleHistory.Any() ? Queryable.Average(SaleHistory.Where(x => x.IsBuyOrder != true).Select(x => x.Price).AsQueryable()) : 0;
                    Info.Add($"Without 'Supply' sale Avg:", Math.Round(avg, 2));
                    break;
            };
            Info.Add("Table price currency:", currency);
        }
    }
}
