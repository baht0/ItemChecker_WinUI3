using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using ItemChecker.Models.StaticModels;
using ItemChecker.Net;
using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ItemChecker.Models.StaticModels.BaseConfig;

namespace ItemChecker.Models.Rare
{
    public partial class DataRare : ObservableObject
    {
        public DataListingItem DataBuy { get; set; } = new();
        public string ItemName { get; set; } = "Unknown";
        public string Image { get; set; }
        public double Price { get; set; }
        public double PriceCompare { get; set; }
        public double Precent { get; set; }
        public double Difference { get; set; }
        public string Link { get; set; }
        public double FloatValue { get; set; }
        public List<DataSticker> Stickers { get; set; } = new();
        public Doppler Phase { get; set; } = Doppler.None;

        [ObservableProperty]
        ActionStatus _isBought = ActionStatus.None;

        [RelayCommand]
        private void InspectInGame() => Edit.OpenUrl(Link);
        [RelayCommand]
        private async void BuyItem()
        {
            IsBought = ActionStatus.None;
            var response = await SteamRequest.Post.BuyListing(ItemName, DataBuy.ListingId, DataBuy.Fee, DataBuy.Subtotal, DataBuy.Total, SteamAccount.Currency.Id);
            IsBought = response.StatusCode == System.Net.HttpStatusCode.OK ? ActionStatus.OK : ActionStatus.Error;
        }

        public DataRare(string itemName, double priceCompare)
        {
            ItemName = itemName;
            Image = ItemBase.List.FirstOrDefault(x => x.ItemName == itemName)?.Image;
            PriceCompare = priceCompare;
        }

        public void SetPrices(JObject json)
        {
            DataBuy.ListingPrice(json);

            Price = DataBuy.Total / 100;
            Precent = Edit.Precent(Price, PriceCompare);
            Difference = Edit.Difference(Price, PriceCompare);
        }
        public void GetStickers(JObject json)
        {
            var list = new List<DataSticker>();

            string ass_id = json["listinginfo"][DataBuy.ListingId]["asset"]["id"].ToString();
            var descriptions = JArray.Parse(json["assets"]["730"]["2"][ass_id]["descriptions"].ToString());
            var value = descriptions.LastOrDefault()["value"].ToString().Trim();
            if (!string.IsNullOrEmpty(value))
            {
                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(value);
                string[] stickers = htmlDoc.DocumentNode.SelectSingleNode("//div").InnerText.Trim().Split(',');
                foreach (string sticker in stickers)
                {
                    string name = "Sticker |" + sticker.Replace("Sticker:", string.Empty);
                    list.Add(new()
                    {
                        Name = name,
                        Url = ItemBase.List.FirstOrDefault(x => x.ItemName == name)?.Image
                    });
                }
            }
            Stickers = list;
        }
        public async void InspectLink(JObject json)
        {
            try
            {
                string ass_id = json["listinginfo"][DataBuy.ListingId]["asset"]["id"].ToString();
                string link = json["listinginfo"][DataBuy.ListingId]["asset"]["market_actions"][0]["link"].ToString();
                link = link.Replace("%listingid%", DataBuy.ListingId);
                link = link.Replace("%assetid%", ass_id);
                Link = link;

                json = await ServicesRequest.InspectLinkDetails(Link);
                FloatValue = Convert.ToDouble(json["floatvalue"].ToString());
                int paintIndex = Convert.ToInt32(json["paintindex"].ToString());

                Phase = paintIndex switch
                {
                    415 => Doppler.Ruby,
                    416 => Doppler.Sapphire,
                    417 => Doppler.BlackPearl,
                    568 or 1119 => Doppler.Emerald,
                    418 or 569 or 1120 => Doppler.Phase1,
                    419 or 570 or 1121 => Doppler.Phase2,
                    420 or 571 or 1122 => Doppler.Phase3,
                    421 or 572 or 1123 => Doppler.Phase4,
                    _ => Doppler.None,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
    public enum Doppler
    {
        None,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Ruby,
        Sapphire,
        BlackPearl,
        Emerald
    }
}
