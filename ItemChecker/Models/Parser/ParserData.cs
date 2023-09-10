using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.Parser
{
    public partial class ParserData : ObservableObject
    {
        [ObservableProperty]
        private bool isChecked = false;
        [JsonIgnore]
        public string Image => ItemBase.List.FirstOrDefault(x => x.ItemName == ItemName).Image;
        public string ItemName { get; set; }
        public double Purchase { get; set; }
        public double Price { get; set; }
        public double Get { get; set; }
        [JsonIgnore]
        public double Precent => Edit.Precent(Purchase, Get);
        [JsonIgnore]
        public double Difference => Edit.Difference(Get, Purchase);

        [ObservableProperty]
        private bool isCanCheck = false;

        public ParserData(string itemName) => ItemName = itemName;
        public async Task<ParserData> CheckItem(int servOneId, int servTwoId)
        {
            var Item = ItemBase.List.FirstOrDefault(x => x.ItemName == ItemName);
            switch (servOneId)
            {
                case 0:
                    {
                        await Item.UpdateSteamItem();
                        Purchase = Item.Steam.HighestBuyOrder;
                        IsCanCheck = true;
                        break;
                    }
                case 1:
                    {
                        await Item.UpdateSteamItem();
                        Purchase = Item.Steam.LowestSellOrder;
                        IsCanCheck = true;
                        break;
                    }
                case 2:
                    {
                        Purchase = Item.Csm.Inventory.Select(x => x.Price).DefaultIfEmpty().Min();
                        break;
                    }
                case 3:
                    {
                        Purchase = Math.Round(Item.Lfm.Price * 1.03d, 2);
                        break;
                    }
                case 4:
                    {
                        Purchase = Item.Buff.BuyOrder;
                        break;
                    }
                case 5:
                    {
                        Purchase = Item.Buff.Price;
                        break;
                    }
            }
            switch (servTwoId)
            {
                case 0:
                    {
                        await Item.UpdateSteamItem();
                        Price = Item.Steam.HighestBuyOrder;
                        Get = Math.Round(Price * SteamAccount.Inventory.Commission, 2);
                        break;
                    }
                case 1:
                    {
                        await Item.UpdateSteamItem();
                        Price = Item.Steam.LowestSellOrder;
                        Get = Math.Round(Price * SteamAccount.Inventory.Commission, 2);
                        break;
                    }
                case 2:
                    {
                        await Item.UpdateCsmItem(false);
                        Price = Item.Csm.Price;
                        Get = Math.Round(Price * SteamAccount.Csm.Commission, 2);
                        break;
                    }
                case 3:
                    {
                        Price = Item.Lfm.Price;
                        Get = Math.Round(Price * SteamAccount.Lfm.Commission, 2);
                        break;
                    }
                case 4:
                    {
                        await Item.UpdateBuffItem();
                        Price = Item.Buff.BuyOrder;
                        Get = Math.Round(Price * SteamAccount.Buff.Commission, 2);
                        break;
                    }
                case 5:
                    {
                        await Item.UpdateBuffItem();
                        Price = Item.Buff.Price;
                        Get = Math.Round(Price * SteamAccount.Buff.Commission, 2);
                        break;
                    }
            }

            return this;
        }
    }
    public class ParserList
    {
        public string ItemName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }

        public ParserList(string itemName)
        {
            ItemName = itemName;
            IsChecked = false;
        }
    }
}
