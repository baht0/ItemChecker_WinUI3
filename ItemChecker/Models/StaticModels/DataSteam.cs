using Newtonsoft.Json.Linq;
using System;

namespace ItemChecker.Models.StaticModels
{
    public class DataSticker
    {
        public string Name { get; set; }
        public string? Url { get; set; }
    }
    public class DataInventoryItem
    {
        public string AssetId { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string InstanceId { get; set; } = string.Empty;
    }
    public class DataTradeOffer
    {
        public string TradeOfferId { get; set; }
        public string PartnerId { get; set; }
    }
    public class DataListingItem
    {
        public string ListingId { get; set; } = "0";
        public double Fee { get; set; }
        public double Subtotal { get; set; }
        public double Total { get; set; }

        public void ListingPrice(JObject json)
        {
            Subtotal = Convert.ToDouble(json["listinginfo"][ListingId]["converted_price"]);
            var fee_steam = Convert.ToDouble(json["listinginfo"][ListingId]["converted_steam_fee"]);
            var fee_csgo = Convert.ToDouble(json["listinginfo"][ListingId]["converted_publisher_fee"]);
            Fee = fee_steam + fee_csgo;
            Total = Subtotal + Fee;
        }
    }
}
