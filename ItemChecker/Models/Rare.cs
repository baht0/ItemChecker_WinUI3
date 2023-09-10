using ItemChecker.Models.Rare;
using ItemChecker.Models.StaticModels;
using ItemChecker.Net;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models
{
    public class RareTool
    {
        private string ItemName { get; set; }
        private RareParameters Parameters { get; set; }

        public async Task<List<DataRare>> CheckItemAsync(string itemName, RareParameters parameters)
        {
            ItemName = itemName;
            Parameters = parameters;
            Parameters.MinPrecent *= -1;

            var items = new List<DataRare>();
            var priceCompare = await PriceCompareAsync();

            var json = await SteamRequest.Get.ItemListingsAsync(itemName);
            var attributes = json["listinginfo"].ToList<JToken>();
            foreach (JToken attribute in attributes)
            {
                try
                {
                    var data = new DataRare(itemName, priceCompare);

                    var jProperty = attribute.ToObject<JProperty>();
                    data.DataBuy.ListingId = jProperty?.Name;

                    data.SetPrices(json);
                    if (data.Precent < Parameters.MinPrecent)
                        break;

                    data.GetStickers(json);
                    data.InspectLink(json);

                    switch (Parameters.ParameterId)
                    {
                        case 0://float
                            if (data.FloatValue < MaxFloat())
                                items.Add(data);
                            break;
                        case 1://sticker
                            if (AllowStickers(data))
                                items.Add(data);
                            break;
                        case 2://doppler
                            if (AllowDopplerPhase(data.Phase))
                                items.Add(data);
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return items;
        }
        private async Task<double> PriceCompareAsync()
        {
            var steamPrices = await SteamRequest.Get.PriceOverviewAsync(ItemName, 1);

            var lowest_price = steamPrices.ContainsKey("lowest_price") ? Edit.GetDouble(steamPrices["lowest_price"].ToString()) : 0d;
            var median_price = steamPrices.ContainsKey("median_price") ? Edit.GetDouble(steamPrices["median_price"].ToString()) : 0d;

            return (object)Parameters.CompareId switch
            {
                0 => lowest_price,
                1 => median_price,
                _ => 0,
            };
        }
        //float
        private double MaxFloat()
        {
            double maxFloat = 0;
            if (ItemName.Contains("Factory New")) maxFloat = Parameters.FactoryNew;
            else if (ItemName.Contains("Minimal Wear")) maxFloat = Parameters.MinimalWear;
            else if (ItemName.Contains("Field-Tested")) maxFloat = Parameters.FieldTested;
            else if (ItemName.Contains("Well-Worn")) maxFloat = Parameters.WellWorn;
            else if (ItemName.Contains("Battle-Scarred")) maxFloat = Parameters.BattleScarred;

            return maxFloat;
        }
        //sticker
        private bool AllowStickers(DataRare data)
        {
            if (!String.IsNullOrEmpty(Parameters.NameContains) && !data.Stickers.Any(x => x.Name.Contains(Parameters.NameContains)))
                return false;
            if (data.Stickers.Count >= Parameters.MinSticker)
            {
                foreach (var sticker in data.Stickers)
                {
                    var baseItem = ItemBase.List.FirstOrDefault(x => x.ItemName == sticker.Name);
                    if (baseItem == null)
                        return false;
                    switch (baseItem.Quality)
                    {
                        case Quality.MilSpec:
                            if (Parameters.Normal)
                                return Parameters.Normal;
                            break;
                        case Quality.Restricted:
                            if ((Parameters.Holo && data.ItemName.Contains("Holo")) || (Parameters.Glitter && data.ItemName.Contains("Glitter")))
                                return true;
                            break;
                        case Quality.Classified:
                            if (Parameters.Foil)
                                return true;
                            break;
                        case Quality.Covert:
                            if (Parameters.Gold)
                                return true;
                            break;
                        case Quality.Contraband:
                            if (Parameters.Contraband)
                                return true;
                            break;
                        default:
                            return true;
                    }
                    if (!Parameters.Normal && !Parameters.Holo && !Parameters.Glitter &&
                        !Parameters.Foil && !Parameters.Gold && !Parameters.Contraband)
                        return true;
                }
            }
            return false;
        }
        //doppler
        private bool AllowDopplerPhase(Doppler phase)
        {
            return phase switch
            {
                Doppler.Ruby => Parameters.Ruby,
                Doppler.Sapphire => Parameters.Sapphire,
                Doppler.BlackPearl => Parameters.BlackPearl,
                Doppler.Emerald => Parameters.Emerald,
                Doppler.Phase1 => Parameters.Phase1,
                Doppler.Phase2 => Parameters.Phase2,
                Doppler.Phase3 => Parameters.Phase3,
                Doppler.Phase4 => Parameters.Phase4,
                _ => false,
            };
        }
    }
}
