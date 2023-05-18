using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static ItemChecker.Net.SteamRequest;

namespace ItemChecker.Models.Base
{
    public class SteamItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public int Id { get; set; }
        public decimal AvgPrice { get; set; }
        public int CurrencyId { get; set; }
        public decimal LowestSellOrder { get; set; }
        public decimal HighestBuyOrder { get; set; }
        public bool IsHave { get; set; }
        public List<SaleHistory> History { get; set; } = new();

        public async Task UpdateSteamItemAsync(string itemName, int currencyId = 1)
        {
            if (Updated.AddMinutes(30) > DateTime.Now && CurrencyId == currencyId)
                return;

            JObject json = await Get.ItemOrdersHistogramAsync(itemName, Id, currencyId);
            decimal high = !string.IsNullOrEmpty(json["highest_buy_order"].ToString()) ? Convert.ToDecimal(json["highest_buy_order"]) / 100 : 0;
            decimal low = !string.IsNullOrEmpty(json["lowest_sell_order"].ToString()) ? Convert.ToDecimal(json["lowest_sell_order"]) / 100 : 0;

            HighestBuyOrder = high;
            LowestSellOrder = low;
            IsHave = low > 0;
        }
        public async Task UpdateSteamItemHistoryAsync(string itemName)
        {
            if (History.Any())
                return;

            string json = await Get.RequestAsync("https://steamcommunity.com/market/pricehistory/?appid=730&market_hash_name=" + Uri.EscapeDataString(itemName));
            JArray sales = JArray.Parse(JObject.Parse(json)["prices"].ToString());
            foreach (var sale in sales.Reverse())
            {
                var date = DateTime.Parse(sale[0].ToString()[..11]);
                var price = decimal.Parse(sale[1].ToString());
                var count = Convert.ToInt32(sale[2]);

                History.Add(new(date, price, count, false));
            }
        }
    }
}
