using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static ItemChecker.Net.SteamRequest;

namespace ItemChecker.Models.StaticModels.Base
{
    public class SteamItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public int Id { get; set; }
        public double AvgPrice { get; set; }
        public int CurrencyId { get; set; }
        public double LowestSellOrder { get; set; }
        public double HighestBuyOrder { get; set; }
        public bool IsHave { get; set; }
        public List<SaleHistory> History { get; set; } = new();

        internal async Task UpdateAsync(string itemName, int currencyId)
        {
            JObject json = await Get.ItemOrdersHistogramAsync(itemName, Id, currencyId);
            double high = !string.IsNullOrEmpty(json["highest_buy_order"].ToString()) ? Convert.ToDouble(json["highest_buy_order"]) / 100 : 0;
            double low = !string.IsNullOrEmpty(json["lowest_sell_order"].ToString()) ? Convert.ToDouble(json["lowest_sell_order"]) / 100 : 0;

            CurrencyId = currencyId;
            HighestBuyOrder = high;
            LowestSellOrder = low;
            IsHave = low > 0;
        }
        internal async Task UpdateHistoryAsync(string itemName)
        {
            if (History.Any())
                return;

            string json = await Get.RequestAsync("https://steamcommunity.com/market/pricehistory/?appid=730&market_hash_name=" + Uri.EscapeDataString(itemName));
            JArray sales = JArray.Parse(JObject.Parse(json)["prices"].ToString());
            foreach (var sale in sales.Reverse())
            {
                var date = DateTime.Parse(sale[0].ToString()[..11]);
                var price = double.Parse(sale[1].ToString());
                var count = Convert.ToInt32(sale[2]);

                History.Add(new(date, price, count, false));
            }
        }
    }
}
