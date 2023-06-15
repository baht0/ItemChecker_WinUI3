﻿using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static ItemChecker.Net.ServicesRequest.Buff163;

namespace ItemChecker.Models.StaticModels.Base
{
    public class BuffItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public int Id { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double BuyOrder { get; set; }
        public int OrderCount { get; set; }
        public bool IsHave { get; set; }
        public List<SaleHistory> History { get; set; } = new();

        internal async Task UpdateAsync(string itemName)
        {
            if (!SteamAccount.Buff.IsActive)
                return;

            string market_hash_name = HttpUtility.UrlEncode(itemName);
            int pages = int.MaxValue;
            string last_item = string.Empty;
            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    string url = "https://buff.163.com/api/market/goods/buying?game=csgo&page_num=" + i + "&search=" + market_hash_name + "&sort_by=price.asc&page_size=80";
                    JObject json = JObject.Parse(await Get.RequestAsync(url));

                    pages = Convert.ToInt32(json["data"]["total_page"]);
                    JArray items = json["data"]["items"] as JArray;
                    foreach (JObject item in items)
                    {
                        string serviceItemName = item["market_hash_name"].ToString();
                        if (serviceItemName == itemName && itemName != last_item)
                        {
                            Updated = DateTime.Now;
                            Id = Convert.ToInt32(item["id"]);
                            var price = Currencies.ConverterToUsd(Convert.ToDouble(item["sell_min_price"]), 23);
                            Price = price;
                            Count = Convert.ToInt32(item["sell_num"]);
                            BuyOrder = Currencies.ConverterToUsd(Convert.ToDouble(item["buy_max_price"]), 23);
                            OrderCount = Convert.ToInt32(item["buy_num"]);
                            IsHave = price > 0;
                        }
                        last_item = serviceItemName;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        internal async Task UpdateHistoryAsync(string itemName)
        {
            if (!SteamAccount.Buff.IsActive || History.Any() || Id == 0)
                return;

            var url = "https://buff.163.com/api/market/goods/bill_order?game=csgo&goods_id=" + Id;
            var json = JObject.Parse(await Get.RequestAsync(url));
            var items = (JArray)json["data"]["items"];
            foreach (JObject item in items)
            {
                double time = Convert.ToDouble(item["buyer_pay_time"]);
                DateTime date = Edit.ConvertFromUnixTimestamp(time).AddHours(3);
                double price = Currencies.ConverterToUsd(Convert.ToDouble(item["price"]), 23);
                bool isBuyOrder = Convert.ToInt32(item["type"]) == 2;
                History.Add(new(date, price, 1, isBuyOrder));
            }
        }
    }
}