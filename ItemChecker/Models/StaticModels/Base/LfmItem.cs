using Newtonsoft.Json;

namespace ItemChecker.Models.StaticModels.Base
{
    public class LfmItem
    {
        [JsonProperty("price")]
        public double Price
        {
            get => price / 100;
            set => price = value;
        }
        private double price;
        [JsonProperty("have")]
        public int Have { get; set; }
        [JsonProperty("max")]
        public int Limit { get; set; }
        [JsonProperty("res")]
        public int Reservable { get; set; }
        [JsonProperty("tr")]
        public int Tradable { get; set; }
        [JsonProperty("rate")]
        public double SteamPriceRate
        {
            get => steamPriceRate / 100;
            set => steamPriceRate = value;
        }
        private double steamPriceRate;

        public bool IsHave => Tradable > 0;
        public ItemStatus Status => Limit > 0 ? (Have >= Limit ? ItemStatus.Overstock : ItemStatus.Available) : ItemStatus.Unavailable;
    }
}
