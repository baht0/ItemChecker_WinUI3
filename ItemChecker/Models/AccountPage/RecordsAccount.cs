using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.AccountPage
{
    public class RecordsAccount : ObservableObject
    {
        public List<DataRecord> SavedRecords { get; private set; }
        public ObservableCollection<DataRecord> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }
        ObservableCollection<DataRecord> _records = new();
        public DataInterval Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }
        DataInterval _result = new();
        public List<string> Intervals => new()
                {
                    "All Time",
                    "1 Day",
                    "7 Days",
                    "30 Days",
                    "3 Months",
                    "6 Months",
                    "1 Year",
                    "Custom"
                };

        public RecordsAccount()
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
            Records = new(SavedRecords.OrderByDescending(d => d.DateTime));
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
            Records = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        public void BeginInterval(DateTime date)
        {
            var list = Records.Where(x => x.DateTime >= date).ToList();
            Records = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        public void EndInterval(DateTime date)
        {
            var list = Records.Where(x => x.DateTime <= date).ToList();
            Records = new(list.OrderByDescending(d => d.DateTime));
            GetResult(list);
        }
        private void GetResult(List<DataRecord> list)
        {
            if (list.Any())
            {
                Result = new()
                {
                    AvgBalance = Math.Round(Queryable.Average(Records.Select(x => x.Total).AsQueryable()), 2),
                    StartBalance = list.FirstOrDefault().Total,
                    EndBalance = list.LastOrDefault().Total,
                };
            }
        }

        public async Task RefreshRecords()
        {
            await Account.GetSteamBalanceAsync();

            await Account.Csm.GetBalanceAsync();
            await Account.Csm.GetSumOfItemsAsync();

            await Account.Lfm.GetBalanceAsync();
            await Account.Lfm.GetSumOfItemsAsync();
            await Account.Buff.GetBalanceAsync();

            var steamBalance = Currencies.ConverterToUsd(Account.Balance, Account.Currency.Id);
            //var steamInventory = InventoryService.GetSumOfItems();

            var data = new DataRecord()
            {
                Balance = steamBalance + Account.Csm.Balance + Account.Lfm.Balance + Account.Buff.Balance,
                SumOfItems = Account.Csm.SumOfItems + Account.Lfm.SumOfItems,
            };
            await AddRecord(data);
        }
        private async Task<bool> AddRecord(DataRecord data)
        {
            if (!SavedRecords.Any(x => x == data))
            {
                SavedRecords.Add(data);
                Records = new(SavedRecords.OrderByDescending(d => d.DateTime));
                await SaveAsync();
                return true;
            }
            return false;
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
        public decimal Total => Balance + SumOfItems;
        public decimal Balance { get; set; }
        public decimal SumOfItems { get; set; }
    }
    public class DataInterval
    {
        public decimal AvgBalance { get; set; }
        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }
        public decimal Precent => Edit.Precent(StartBalance, EndBalance);
        public decimal Difference => Edit.Difference(EndBalance, StartBalance);
    }
}