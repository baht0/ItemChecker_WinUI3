using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.AccountModel
{
    public partial class Records : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<DataRecord> _items = new();
        [ObservableProperty]
        DataInterval _result = new();
        public List<DataRecord> SavedRecords { get; private set; }
        public List<string> Intervals => new() { "All Time", "1 Day", "7 Days", "30 Days", "3 Months", "6 Months", "1 Year", "Custom" };

        public Records()
        {
            string path = AppConfig.DocumentPath + "user\\records.json";

            if (!File.Exists(path))
            {
                File.Create(path);
                File.WriteAllText(path, "[]");
            }
            var json = File.ReadAllText(path);
            var array = JArray.Parse(json);
            SavedRecords = array.ToObject<List<DataRecord>>();
            Items = new(SavedRecords.OrderByDescending(d => d.DateTime));
        }
        public void SwitchInterval(int index)
        {
            List<DataRecord> list = index switch
            {
                0 => SavedRecords.ToList(),
                1 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddDays(-1)).ToList(),
                2 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddDays(-7)).ToList(),
                3 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddDays(-30)).ToList(),
                4 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddMonths(-3)).ToList(),
                5 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddMonths(-6)).ToList(),
                6 => SavedRecords.Where(x => x.DateTime >= DateTime.Today.AddYears(-1)).ToList(),
                _ => SavedRecords
            };
            Items = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        public void BeginInterval(DateTime date)
        {
            var list = Items.Where(x => x.DateTime >= date).ToList();
            Items = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        public void EndInterval(DateTime date)
        {
            var list = Items.Where(x => x.DateTime <= date).ToList();
            Items = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        private void GetResult(List<DataRecord> list)
        {
            if (list.Any())
            {
                Result = new()
                {
                    AvgBalance = Math.Round(Queryable.Average(Items.Select(x => x.Total).AsQueryable()), 2),
                    StartBalance = list.FirstOrDefault().Total,
                    EndBalance = list.LastOrDefault().Total,
                };
            }
        }

        public async Task RefreshRecords()
        {
            await SteamAccount.GetSteamBalanceAsync();

            await SteamAccount.Csm.GetBalanceAsync();
            await SteamAccount.Csm.GetSumOfItemsAsync();

            await SteamAccount.Lfm.GetBalanceAsync();
            await SteamAccount.Lfm.GetSumOfItemsAsync();

            await SteamAccount.Buff.GetBalanceAsync();

            var steamBalance = Currencies.ConverterToUsd(SteamAccount.Balance, SteamAccount.Currency.Id);
            var steamSumOfItems = Currencies.ConverterToUsd(SteamAccount.Inventory.SumOfItemsBuyOrder, SteamAccount.Currency.Id);
            var data = new DataRecord()
            {
                Balance = steamBalance + SteamAccount.Csm.Balance + SteamAccount.Lfm.Balance + SteamAccount.Buff.Balance,
                SumOfItems = steamSumOfItems + SteamAccount.Csm.SumOfItems + SteamAccount.Lfm.SumOfItems,
            };
            await AddRecord(data);
        }
        private async Task AddRecord(DataRecord data)
        {
            if (!SavedRecords.Any(x => (int)x.Total == (int)data.Total && (int)x.Balance == (int)data.Balance && (int)x.SumOfItems == (int)data.SumOfItems))
            {
                SavedRecords.Add(data);
                Items = new(SavedRecords.OrderByDescending(d => d.DateTime));
                await SaveAsync();
            }
        }
        private async Task SaveAsync()
        {
            var json = JArray.FromObject(SavedRecords);

            string path = AppConfig.DocumentPath + "user\\records.json";
            if (!File.Exists(path))
                File.Create(path);
            await File.WriteAllTextAsync(path, json.ToString());
        }
    }
    public class DataRecord
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public double Total => Balance + SumOfItems;
        public double Balance { get; set; }
        public double SumOfItems { get; set; }
    }
    public class DataInterval
    {
        public double AvgBalance { get; set; }
        public double StartBalance { get; set; }
        public double EndBalance { get; set; }
        public double Precent => Edit.Precent(StartBalance, EndBalance);
        public double Difference => Edit.Difference(EndBalance, StartBalance);
    }
}