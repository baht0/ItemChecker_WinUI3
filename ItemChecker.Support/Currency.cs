using ItemChecker.Net;
using Newtonsoft.Json.Linq;

namespace ItemChecker.Support
{
    public class Currencies
    {
        static decimal DollarValue;
        public static List<DataCurrency> All { get; set; } = new();
        public static List<DataCurrency> Allow => All.Where(x => x.Value > 0).ToList();

        public static async Task GetSteamCurrenciesAsync()
        {
            var json = await DropboxRequest.Get.ReadAsync("SteamCurrencies.json");
            var array = JArray.Parse(json);
            All = JArray.Parse(json.ToString()).ToObject<List<DataCurrency>>();

            foreach (var currency in All.Where(x => x.Id == 1 || x.Id == 23).ToList())
                currency.Value = await GetCurrencyValueAsync(currency.Id);
        }
        public static async Task<decimal> GetCurrencyValueAsync(int id)
        {
            int item_nameid = 1548540;
            string itemName = "StatTrak™ AK-47 | Fire Serpent (Field-Tested)";

            var json = await SteamRequest.Get.ItemOrdersHistogramAsync(itemName, item_nameid, id);
            decimal price = Convert.ToDecimal(json["highest_buy_order"]);
            DollarValue = id == 1 ? price : DollarValue;

            return price / DollarValue;
        }

        public static decimal ConverterFromUsd(decimal value, int valueCurrencyId) => Math.Round(value * Allow.FirstOrDefault(x => x.Id == valueCurrencyId).Value, 2);
        public static decimal ConverterToUsd(decimal value, int valueCurrencyId) => Math.Round(value / Allow.FirstOrDefault(x => x.Id == valueCurrencyId).Value, 2);
    }
    public class DataCurrency
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name => $"{Code} ({Symbol})";
        public decimal Value { get; set; }
    }
}
