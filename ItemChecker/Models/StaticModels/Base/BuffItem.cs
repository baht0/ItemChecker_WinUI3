using ItemChecker.Support;
using Newtonsoft.Json;
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
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("sell_min_price")]
        public double Price
        {
            get => Currencies.ConverterToUsd(price, Currencies.Allow.FirstOrDefault(x => x.Code == "CNY").Id);
            set => price = value;
        }
        private double price;
        [JsonProperty("sell_num")]
        public int Count { get; set; }
        [JsonProperty("buy_max_price")]
        public double BuyOrder
        {
            get => Currencies.ConverterToUsd(buyOrder, Currencies.Allow.FirstOrDefault(x => x.Code == "CNY").Id);
            set => buyOrder = value;
        }
        private double buyOrder;
        [JsonProperty("buy_num")]
        public int OrderCount { get; set; }
        public bool IsHave => price > 0;
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
                    JObject json = JObject.Parse(await Get.RequestAsync("https://buff.163.com/api/market/goods/buying?game=csgo&page_num=" + i + "&search=" + market_hash_name + "&sort_by=price.asc&page_size=80"));

                    pages = Convert.ToInt32(json["data"]["total_page"]);
                    JArray items = json["data"]["items"] as JArray;
                    foreach (JObject item in items.Cast<JObject>())
                    {
                        string serviceItemName = item["market_hash_name"].ToString();
                        if (serviceItemName == itemName && itemName != last_item)
                        {
                            Id = Convert.ToInt32(item["id"]);
                            Price = Convert.ToDouble(item["sell_min_price"]);
                            Count = Convert.ToInt32(item["sell_num"]);
                            BuyOrder = Convert.ToDouble(item["buy_max_price"]);
                            OrderCount = Convert.ToInt32(item["buy_num"]);
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
        internal async Task UpdateHistoryAsync()
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
