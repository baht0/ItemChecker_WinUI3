using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;

namespace ItemChecker.Models.DatabaseItem
{
    public partial class DataService : ObservableObject
    {
        [ObservableProperty]
        Compare _compare;
        [ObservableProperty]
        private Dictionary<string, object> rowInfo = new();
        public bool IsRowInfo { get; set; }
        public bool IsHistory { get; set; }

        public int ServiceId { get; set; }
        public string Service { get; set; }
        public double Price { get; set; }
        public double Get { get; set; }
        public bool Have { get; set; }
        public bool Available { get; set; } = true;

        public DataService() => IsHistory = false;
        public DataService UpdateData(Item item, string service)
        {
            ServiceId = BaseConfig.Services.IndexOf(service);
            Service = service;
            switch (ServiceId)
            {
                case 0:
                    IsHistory = true;
                    Price = item.Steam.HighestBuyOrder;
                    Get = Math.Round(Price * SteamAccount.Inventory.Commission, 2);
                    Have = Price > 0;
                    break;
                case 1:
                    IsHistory = true;
                    Price = item.Steam.LowestSellOrder;
                    Get = Math.Round(Price * SteamAccount.Inventory.Commission, 2);
                    Have = Price > 0;
                    break;
                case 2:
                    Price = item.Csm.Price;
                    Get = Math.Round(Price * SteamAccount.Csm.Commission, 2);
                    Have = item.Csm.IsHave;
                    Available = item.Csm.Status == ItemStatus.Available;
                    break;
                case 3:
                    IsRowInfo = true;
                    CreateRowInfo(item);
                    var price = item.Lfm.Price;
                    Price = Math.Round(price * 1.03d, 2);
                    Get = Math.Round(price * SteamAccount.Lfm.Commission, 2);
                    Have = item.Lfm.IsHave;
                    Available = item.Lfm.Status == ItemStatus.Available;
                    break;
                case 4:
                    IsHistory = true;
                    Price = item.Buff.BuyOrder;
                    Get = Math.Round(Price * SteamAccount.Buff.Commission, 2);
                    Have = Price > 0;
                    break;
                case 5:
                    IsHistory = true;
                    Price = item.Buff.Price;
                    Get = Math.Round(Price * SteamAccount.Buff.Commission, 2);
                    Have = item.Buff.IsHave;
                    break;
            }
            Compare = new(this);
            return this;
        }
        private void CreateRowInfo(Item item)
        {
            var info = new Dictionary<string, object>();
            switch (ServiceId)
            {
                case 3:
                    info.Add("Price ($):", item.Lfm.Price);
                    info.Add("Have:", item.Lfm.Have);
                    info.Add("Tradeable:", item.Lfm.Tradable);
                    info.Add("Steam price rate:", item.Lfm.SteamPriceRate);
                    info.Add("Limit:", item.Lfm.Limit);
                    info.Add("Reservable:", item.Lfm.Reservable);
                    break;
            }
            RowInfo = info;
        }
    }
    public partial class Compare : ObservableObject
    {
        readonly DataService Service;
        public List<DataService> DataServices;
        public List<string> Services => BaseConfig.Services;
        [ObservableProperty]
        int _selected;
        [ObservableProperty]
        double _get = new();
        [ObservableProperty]
        double _precent = new();
        [ObservableProperty]
        double _difference = new();

        partial void OnSelectedChanged(int value)
        {
            if (value != -1)
            {
                Get = DataServices[value].Get;
                Precent = Edit.Precent(Service.Price, DataServices[value].Get);
                Difference = Edit.Difference(DataServices[value].Get, Service.Price);
            }
        }

        public Compare(DataService data) => Service = data;
    }
}
