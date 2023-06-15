using CommunityToolkit.Mvvm.ComponentModel;
using HtmlAgilityPack;
using ItemChecker.Models.AccountModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ItemChecker.Net.SteamRequest;

namespace ItemChecker.Models.StaticModels.Accounts
{
    public class SteamInventory
    {
        public List<DataInventory> Items { get; private set; } = new();
        public double Commission => 0.869565d;
        public double SumOfItemsSellOrder { get; private set; }
        public double SumOfItemsBuyOrder { get; private set; }

        public async Task<Dictionary<string, object>> Main()
        {
            await GetItemsAsync();
            await GetSumOfItemsAsync();

            return new Dictionary<string, object>()
            {
                { "Inventory price currency:", SteamAccount.Currency.Name },
                { "Sum of items sell order:", SumOfItemsSellOrder },
                { "Sum of items buy order:", SumOfItemsBuyOrder },
            };
        }
        private async Task GetItemsAsync()
        {
            var json = await Get.RequestAsync("http://steamcommunity.com/my/inventory/json/730/2");
            JObject rgInventory = (JObject)JObject.Parse(json)["rgInventory"];
            JObject rgDescriptions = (JObject)JObject.Parse(json)["rgDescriptions"];

            List<DataInventory> inventory = new();
            foreach (var jObject in rgInventory)
            {
                string assetid = jObject.Value["id"].ToString();
                string classid = jObject.Value["classid"].ToString();
                string instanceid = jObject.Value["instanceid"].ToString();

                var jsonItem = (JObject)rgDescriptions[$"{classid}_{instanceid}"];
                if ((int)jsonItem["marketable"] == 0)
                    continue;

                string name = jsonItem["market_name"].ToString();
                bool isNameTag = jsonItem.ContainsKey("fraudwarnings");
                string nameTag = isNameTag ? jsonItem["fraudwarnings"][0].ToString() : "-";
                bool isStickers = jsonItem["descriptions"].ToString().Contains("sticker_info");
                List<DataSticker> stickers = new();
                if (isStickers)
                {
                    var array = JArray.Parse(jsonItem["descriptions"].ToString());
                    var html = ((JObject)array.LastOrDefault())["value"].ToString();
                    HtmlDocument htmlDoc = new();
                    htmlDoc.LoadHtml(html);
                    string[] names = htmlDoc.DocumentNode.SelectSingleNode("//center").InnerText.Trim().Replace("Sticker: ", string.Empty).Split(',');
                    var urls = htmlDoc.DocumentNode.Descendants("img").Select(e => e.GetAttributeValue("src", null)).Where(s => !string.IsNullOrEmpty(s)).ToList();

                    for (int i = 0; i < names.Length; i++)
                    {
                        stickers.Add(new()
                        {
                            Name = names[i],
                            Url = urls[i]
                        });
                    }
                }

                var item = new DataInventory()
                {
                    ItemName = name,
                    IsTradable = (int)jsonItem["tradable"] != 0,
                    TradeLock = jsonItem.ContainsKey("cache_expiration") ? (DateTime)jsonItem["cache_expiration"] : new(),
                    IsStickers = isStickers,
                    Stickers = stickers,
                    IsNameTag = isNameTag,
                    NameTag = nameTag.Replace("Name Tag: ", string.Empty).Replace("''", string.Empty),
                    Data = new()
                    {
                        AssetId = assetid,
                        ClassId = classid,
                        InstanceId = instanceid,
                    },
                };
                inventory.Add(item);
            }
            Items = inventory;
        }
        private async Task GetSumOfItemsAsync()
        {
            SumOfItemsSellOrder = 0;
            SumOfItemsBuyOrder = 0;
            foreach (var item in Items)
            {
                var baseItem = ItemBase.List.FirstOrDefault(x => x.ItemName == item.ItemName);
                await baseItem.UpdateSteamItem(SteamAccount.Currency.Id);

                item.LowestSellOrder = baseItem.Steam.LowestSellOrder;
                item.HighestBuyOrder = baseItem.Steam.HighestBuyOrder;
                SumOfItemsSellOrder += item.LowestSellOrder;
                SumOfItemsBuyOrder += item.HighestBuyOrder;
            }
        }

        public async Task<int> AcceptTradeAsync()
        {
            int accepted = 0;
            var trades = await CheckOfferAsync();
            await Task.Run(async () =>
            {
                while (trades.Any())
                {
                    foreach (var offer in trades)
                    {
                        await Post.AcceptTrade(offer.TradeOfferId, offer.PartnerId);
                        await Task.Delay(1000);
                        accepted++;
                    }
                    trades = await CheckOfferAsync();
                }
            });
            return accepted;
        }
        private async Task<List<DataTradeOffer>> CheckOfferAsync()
        {
            var offers = new List<DataTradeOffer>();
            var json = await Get.TradeOffersAsync();
            var trades = (JArray)json["response"]?["trade_offers_received"];
            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    if (trade["trade_offer_state"]?.ToString() == "2")
                    {
                        offers.Add(new()
                        {
                            TradeOfferId = trade["tradeofferid"]?.ToString(),
                            PartnerId = trade["accountid_other"]?.ToString()
                        });
                    }
                    else
                        continue;
                }
            }
            return offers;
        }

        public async Task SellItemAsync(SellParameters config)
        {
            var items = config.AllMarketable ? Items : Items.Where(x => x.IsChecked);
            foreach (var item in items)
            {
                var baseItem = ItemBase.List.FirstOrDefault(x => x.ItemName == item.ItemName);
                if (baseItem == null)
                    continue;

                await baseItem.UpdateSteamItem(SteamAccount.Currency.Id);
                double price = 0;
                switch (config.SellingPriceId)
                {
                    case 0:
                        price = baseItem.Steam.LowestSellOrder;
                        break;
                    case 1:
                        price = baseItem.Steam.HighestBuyOrder;
                        break;
                }
                int sellPrice = (int)((price * 100 - 0.01d) * Commission);
                await Post.SellItem(item.Data.AssetId, sellPrice);
                await Task.Delay(1000);
            }
        }
    }
    public partial class DataInventory : ObservableObject
    {
        [ObservableProperty]
        bool _isChecked = new();
        public DataInventoryItem Data { get; set; } = new();
        public string ItemName { get; set; } = "Unknown";
        public double LowestSellOrder { get; set; }
        public double HighestBuyOrder { get; set; }
        public DateTime TradeLock { get; set; } = new();
        public bool IsTradable { get; set; }
        public bool IsStickers { get; set; }
        public List<DataSticker> Stickers { get; set; } = new();
        public bool IsNameTag { get; set; }
        public string NameTag { get; set; } = "-";
    }
}
