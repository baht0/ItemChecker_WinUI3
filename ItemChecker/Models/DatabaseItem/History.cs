using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.DatabaseItem
{
    public partial class History : ObservableObject
    {
        private Item Item { get; set; }

        [ObservableProperty]
        private bool isBusy = true;
        [ObservableProperty]
        private List<SaleHistory> saleHistory;
        [ObservableProperty]
        private Dictionary<string, object> info = new();

        [ObservableProperty]
        private bool isCount;
        [ObservableProperty]
        private bool isBuyOrder;

        public History(Item item)
        {
            Item = item;
            Info = new();
            IsBusy = true;
        }
        public async Task GetSaleHistory(int id)
        {
            var info = new Dictionary<string, object>();
            string currency = Currencies.Allow[0].Name;
            switch (id)
            {
                case 0 or 1:
                    await Item.UpdateSteamHistoryItem();
                    IsCount = true;
                    IsBuyOrder = false;
                    SaleHistory = Item.Steam.History;

                    info.Add("LastSale:", SaleHistory.FirstOrDefault()?.Date);

                    var last30 = SaleHistory.Where(x => x.Date > DateTime.Today.AddDays(-30)).Select(x => x.Price).ToList();
                    var avg30 = last30.Any() ? Queryable.Average(last30.AsQueryable()) : 0;
                    info.Add($"Last 30 days Avg ({last30.Count}):", Math.Round(avg30, 2));

                    var last60 = SaleHistory.Where(x => x.Date > DateTime.Today.AddDays(-60)).Select(x => x.Price).ToList();
                    var avg60 = last60.Any() ? Queryable.Average(last60.AsQueryable()) : 0;
                    info.Add($"Last 60 days Avg ({last60.Count}):", Math.Round(avg60, 2));

                    currency = SteamAccount.Currency.Name;
                    break;
                case 4 or 5:
                    await Item.UpdateBuffHistoryItem();
                    IsCount = false;
                    IsBuyOrder = true;
                    SaleHistory = Item.Buff.History;

                    info.Add("LastSale:", SaleHistory.FirstOrDefault()?.Date);

                    var avg = SaleHistory.Any() ? Queryable.Average(SaleHistory.Select(x => x.Price).AsQueryable()) : 0;
                    info.Add($"All sale Avg:", Math.Round(avg, 2));

                    avg = SaleHistory.Any() ? Queryable.Average(SaleHistory.Where(x => x.IsBuyOrder != true).Select(x => x.Price).AsQueryable()) : 0;
                    info.Add($"Without 'Supply' sale Avg:", Math.Round(avg, 2));
                    break;
            };
            info.Add("Table price currency:", currency);
            Info = info;
            IsBusy = false;
        }
    }
}
