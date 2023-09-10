using ItemChecker.Models.Parser;
using ItemChecker.Models.StaticModels.Base;
using ItemChecker.Net;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.StaticModels
{
    public class ItemBase
    {
        public static DateTime Updated { get; set; } = new();
        public static List<Item> List { get; set; } = new();

        public static async Task GetItemsBase()
        {
            var json = await DropboxRequest.Get.ReadAsync("SteamItemsBase.json");
            var jobject = JObject.Parse(json);

            Updated = Convert.ToDateTime(jobject[nameof(Updated)]);
            List = JArray.Parse(jobject["Items"].ToString()).ToObject<List<Item>>();
        }

        public static async Task UpdateSteam()
        {
            string json = await HttpRequest.RequestGetAsync("https://csgobackpack.net/api/GetItemsList/v2/?no_details=true");
            JObject csgobackpack = (JObject)JObject.Parse(json)["items_list"];
            foreach (var item in List)
            {
                var baseItem = List.FirstOrDefault(x => x.ItemName == item.ItemName).Steam;
                baseItem.AvgPrice = Edit.SteamAvgPrice(item.ItemName, csgobackpack);
            }
        }
        public static async Task UpdateLfm()
        {
            var array = await LootFarm.Get.ItemsPriceAsync();
            await Task.Run(() =>
            {
                foreach (JObject item in array.Cast<JObject>())
                {
                    string itemName = item["name"].ToString().Replace("(Holo-Foil)", "(Holo/Foil)").Replace("  ", " ");
                    var itemBase = List.FirstOrDefault(x => x.ItemName == itemName);
                    if (itemBase != null)
                        itemBase.Lfm = item.ToObject<LfmItem>();
                }
            });
        }
        public static async Task UpdateCsm(double minPrice, double maxPrice)
        {
            int offset = 0;

            while (true)
            {
                var items = await CsMoney.Get.LoadBotsInventoryAsync(offset, minPrice, maxPrice);
                if (items != null)
                {
                    string itemName = items[0]["fullName"].ToString();
                    var itemBase = List.FirstOrDefault(x => x.ItemName == itemName).Csm;
                    itemBase.Inventory.Clear();
                    foreach (JObject item in items.Cast<JObject>())
                    {
                        InventoryCsm newItem = new()
                        {
                            NameId = Convert.ToInt32(item["nameId"]),
                            StackSize = item.ContainsKey("stackSize") ? Convert.ToInt32(item["stackSize"]) : 1,
                            Price = Convert.ToDouble(item["price"]),
                            Float = item["float"].Type != JTokenType.Null ? Convert.ToDouble(item["float"]) : 0,
                            Sticker = item["stickers"].Type != JTokenType.Null,
                            RareItem = item["overpay"].Type != JTokenType.Null,
                            User = item["userId"].Type != JTokenType.Null,
                            TradeLock = item.ContainsKey("tradeLock") ? Edit.ConvertFromUnixTimestamp(Convert.ToDouble(item["tradeLock"])) : new(),
                        };
                        itemBase.Inventory.Add(newItem);
                    }
                    itemBase.IsHave = items.HasValues;
                    offset += 60;
                }
                else
                    break;
            }
        }
        public static async Task UpdateBuff(ParserParameters parameters, int serviceId)
        {
            string quality = string.Empty;
            if (parameters.Normal)
                quality = "&quality=normal";
            else if (parameters.Souvenir)
                quality = "&quality=tournament";
            else if (parameters.Stattrak)
                quality = "&quality=strange";
            else if (parameters.Unique)
                quality = "&quality=unusual";
            else if (parameters.UniqueStattrak)
                quality = "&quality=unusual_strange";

            var min = Currencies.ConverterFromUsd(parameters.MinPrice, Currencies.Allow.FirstOrDefault(x => x.Code == "CNY").Id);
            var max = Currencies.ConverterFromUsd(parameters.MaxPrice, Currencies.Allow.FirstOrDefault(x => x.Code == "CNY").Id);
            string tab = string.Empty;

            switch (serviceId)
            {
                case 1:
                    tab = parameters.ServiceOneId == 4 ? "/buying" : string.Empty;
                    break;
                case 2:
                    min *= 0.5d; max *= 2.5d;
                    tab = parameters.ServiceTwoId == 4 ? "/buying" : string.Empty;
                    break;
            }
            int pages = int.MaxValue;
            string last_item = string.Empty;
            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    string url = "https://buff.163.com/api/market/goods" + tab + $"?game=csgo&page_num={i}&min_price={min}&max_price={max}{quality}&sort_by=price.asc&page_size=80";
                    JObject json = JObject.Parse(await Buff163.Get.RequestAsync(url));

                    pages = Convert.ToInt32(json["data"]["total_page"]);
                    JArray items = json["data"]["items"] as JArray;
                    foreach (JObject item in items.Cast<JObject>())
                    {
                        string itemName = item["market_hash_name"].ToString();
                        var itemBase = List.FirstOrDefault(x => x.ItemName == itemName);
                        if (itemBase != null && itemName != last_item)
                        {
                            itemBase.Buff = item.ToObject<BuffItem>();
                            last_item = item["market_hash_name"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
            }
        }
    }
    public class Item
    {
        public string ItemName { get; set; } = string.Empty;
        public Type Type { get; set; }
        public Quality? Quality { get; set; }
        public string? Image
        {
            get => "https://community.akamai.steamstatic.com/economy/image/" + _image;
            set => _image = value;
        }
        private string? _image = string.Empty;
        public SteamItem Steam { get; set; }
        public CsmItem Csm { get; set; } = new();
        public LfmItem Lfm { get; set; } = new();
        public BuffItem Buff { get; set; } = new();

        public async Task UpdateSteamItem(int currencyId = 1) => await Steam.UpdateAsync(ItemName, currencyId);
        public async Task UpdateSteamHistoryItem() => await Steam.UpdateHistoryAsync(ItemName);

        public async Task UpdateCsmItem(bool isInventory = false) => await Csm.UpdateAsync(ItemName, isInventory);

        public async Task UpdateBuffItem() => await Buff.UpdateAsync(ItemName);
        public async Task UpdateBuffHistoryItem() => await Buff.UpdateHistoryAsync();
    }
    public enum Type
    {
        Weapon,
        Knife,
        Gloves,
        Agent,
        Sticker,
        Patch,
        Collectable,
        Key,
        Pass,
        MusicKit,
        Graffiti,
        Container,
        Gift,
        Tool
    }
    public enum Quality
    {
        ConsumerGrade,
        IndustrialGrade,
        MilSpec,
        Restricted,
        Classified,
        Covert,
        Contraband
    }
    public enum ItemStatus
    {
        Available,
        Overstock,
        Unavailable,
    }

    public class SaleHistory
    {
        public DateTime Date { get; set; } = new();
        public double Price { get; set; }
        public int Count { get; set; }
        public bool IsBuyOrder { get; set; }

        public SaleHistory(DateTime date, double price, int count, bool isBuyOrder)
        {
            Date = date;
            Price = price;
            Count = count;
            IsBuyOrder = isBuyOrder;
        }
    }
}