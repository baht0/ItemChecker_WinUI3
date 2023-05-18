using ItemChecker.Models.Base;
using ItemChecker.Net;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models
{
    public class ItemBase
    {
        public static DateTime Updated { get; set; } = new();
        public static List<Item> List { get; set; } = new();

        public static async Task GetItemsBaseAsync()
        {
            var json = await DropboxRequest.Get.ReadAsync("SteamItemsBase.json");
            var jobject = JObject.Parse(json);

            Updated = Convert.ToDateTime(jobject[nameof(Updated)]);
            List = JArray.Parse(jobject["Items"].ToString()).ToObject<List<Item>>();
        }

        public static async Task UpdateSteamAsync()
        {
            string json = await HttpRequest.RequestGetAsync("https://csgobackpack.net/api/GetItemsList/v2/?no_details=true");
            JObject csgobackpack = (JObject)JObject.Parse(json)["items_list"];
            foreach (var item in List)
            {
                var baseItem = List.FirstOrDefault(x => x.ItemName == item.ItemName).Steam;
                baseItem.AvgPrice = Edit.SteamAvgPrice(item.ItemName, csgobackpack);
            }
        }
        public static async Task UpdateLfmAsync()
        {
            if (!Account.Lfm.IsActive && List.Select(x => x.Lfm.Updated).Max().AddMinutes(30) > DateTime.Now)
                return;

            string json = await HttpRequest.RequestGetAsync("https://loot.farm/fullprice.json");
            JArray array = JArray.Parse(json);
            foreach (JToken item in array)
            {
                string itemName = item["name"].ToString().Replace("(Holo-Foil)", "(Holo/Foil)").Replace("  ", " ");
                if (List.FirstOrDefault(x => x.ItemName == itemName) != null)
                {
                    decimal price = Convert.ToDecimal(item["price"]) / 100;
                    int have = Convert.ToInt32(item["have"]);
                    int max = Convert.ToInt32(item["max"]);

                    List.FirstOrDefault(x => x.ItemName == itemName).Lfm = new()
                    {
                        Updated = DateTime.Now,
                        Price = price,
                        Have = have,
                        Limit = max,
                        Reservable = Convert.ToInt32(item["res"]),
                        Tradable = Convert.ToInt32(item["tr"]),
                        SteamPriceRate = Convert.ToInt32(item["rate"]) / 100,
                        IsHave = Convert.ToInt32(item["tr"]) > 0,
                        Status = have >= max ? ItemStatus.Overstock : ItemStatus.Available
                    };
                }
            }
        }
        //public static async Task UpdateCsmAsync(ParserConfig parserConfig)
        //{
        //    if (List.Select(x => x.Csm.Inventory.Select(x => x.Updated).Max()).Max().AddMinutes(30) > DateTime.Now)
        //        return;

        //    int offset = 0;

        //    while (true)
        //    {
        //        var items = await CsMoney.Get.LoadBotsInventoryAsync(offset, parserConfig.MinPrice, parserConfig.MaxPrice);
        //        if (items != null)
        //        {
        //            string itemName = items[0]["fullName"].ToString();
        //            var itemBase = List.FirstOrDefault(x => x.ItemName == itemName).Csm;
        //            itemBase.Inventory.Clear();
        //            foreach (JObject item in items)
        //            {
        //                InventoryCsm newItem = new()
        //                {
        //                    NameId = Convert.ToInt32(item["nameId"]),
        //                    StackSize = item.ContainsKey("stackSize") ? Convert.ToInt32(item["stackSize"]) : 1,
        //                    Price = Convert.ToDecimal(item["price"]),
        //                    Float = item["float"].Type != JTokenType.Null ? Convert.ToDecimal(item["float"]) : 0,
        //                    Sticker = item["stickers"].Type != JTokenType.Null,
        //                    RareItem = item["overpay"].Type != JTokenType.Null,
        //                    User = item["userId"].Type != JTokenType.Null,
        //                    TradeLock = item.ContainsKey("tradeLock") ? Edit.ConvertFromUnixTimestamp(Convert.ToDouble(item["tradeLock"])) : new(),
        //                };
        //                itemBase.Inventory.Add(newItem);
        //            }
        //            itemBase.IsHave = items.HasValues;
        //            offset += 60;
        //        }
        //        else
        //            break;
        //    }
        //}
        //public static async Task UpdateBuffAsync(ParserConfig parserConfig, int serviceId)
        //{
        //    if (List.Select(x => x.Buff.Updated).Max().AddMinutes(30) > DateTime.Now)
        //        return;

        //    string quality = string.Empty;
        //    if (parserConfig.Normal)
        //        quality = "&quality=normal";
        //    else if (parserConfig.Souvenir)
        //        quality = "&quality=tournament";
        //    else if (parserConfig.Stattrak)
        //        quality = "&quality=strange";
        //    else if (parserConfig.KnifeGlove)
        //        quality = "&quality=unusual";
        //    else if (parserConfig.KnifeGloveStattrak)
        //        quality = "&quality=unusual_strange";

        //    int min = (int)Currencies.ConverterFromUsd(parserConfig.MinPrice, 23);
        //    int max = (int)Currencies.ConverterFromUsd(parserConfig.MaxPrice, 23);
        //    string tab = string.Empty;

        //    switch (serviceId)
        //    {
        //        case 1:
        //            tab = parserConfig.ServiceOne == 4 ? "/buying" : string.Empty;
        //            break;
        //        case 2:
        //            min = (int)(parserConfig.MinPrice * 0.5m);
        //            max = (int)(parserConfig.MaxPrice * 2.5m);
        //            tab = parserConfig.ServiceTwo == 4 ? "/buying" : string.Empty;
        //            break;
        //    }

        //    min = (int)Currencies.ConverterFromUsd(min, 23);
        //    max = (int)Currencies.ConverterFromUsd(max, 23);
        //    int pages = int.MaxValue;
        //    string last_item = string.Empty;
        //    for (int i = 1; i <= pages; i++)
        //    {
        //        try
        //        {
        //            string url = "https://buff.163.com/api/market/goods" + tab + $"?game=csgo&page_num={i}&min_price={min}&max_price={max}{quality}&sort_by=price.asc&page_size=80";
        //            JObject json = JObject.Parse(await Buff163.Get.RequestAsync(url));

        //            pages = Convert.ToInt32(json["data"]["total_page"]);
        //            JArray items = json["data"]["items"] as JArray;
        //            foreach (JObject item in items)
        //            {
        //                string itemName = item["market_hash_name"].ToString();
        //                var itemBase = List.FirstOrDefault(x => x.ItemName == itemName);
        //                if (itemBase != null && itemName != last_item)
        //                {
        //                    decimal price = Currencies.ConverterToUsd(Convert.ToDecimal(item["sell_min_price"]), 23);
        //                    itemBase.Buff = new()
        //                    {
        //                        Updated = DateTime.Now,
        //                        Id = Convert.ToInt32(item["id"]),
        //                        Price = price,
        //                        Count = Convert.ToInt32(item["sell_num"]),
        //                        BuyOrder = Currencies.ConverterToUsd(Convert.ToDecimal(item["buy_max_price"]), 23),
        //                        OrderCount = Convert.ToInt32(item["buy_num"]),
        //                        IsHave = price > 0,
        //                    };
        //                }
        //                last_item = item["market_hash_name"].ToString();
        //            }
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //    }
        //}
    }
    public class Item
    {
        public string ItemName { get; set; } = string.Empty;
        public Type Type { get; set; }
        public Quality? Quality { get; set; }
        public SteamItem Steam { get; set; }
        public CsmItem Csm { get; set; } = new();
        public LfmItem Lfm { get; set; } = new();
        public BuffItem Buff { get; set; } = new();
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
        public decimal Price { get; set; }
        public int Count { get; set; }
        public bool IsBuyOrder { get; set; }

        public SaleHistory(DateTime date, decimal price, int count, bool isBuyOrder)
        {
            Date = date;
            Price = price;
            Count = count;
            IsBuyOrder = isBuyOrder;
        }
    }
}