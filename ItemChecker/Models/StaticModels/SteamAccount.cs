using HtmlAgilityPack;
using ItemChecker.Models.StaticModels.Accounts;
using ItemChecker.Net;
using ItemChecker.Support;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.StaticModels
{
    public class SteamAccount
    {
        public static string AccountName { get; private set; } = string.Empty;
        public static string UserName { get; private set; } = string.Empty;
        public static string AvatarUrl { get; private set; } = string.Empty;
        public static string ApiKey { get; private set; } = string.Empty;
        public static string ID64 { get; private set; } = string.Empty;
        public static double Balance { get; private set; }
        public static double BalanceUsd => Currencies.ConverterToUsd(Balance, Currency.Id);
        public static DataCurrency Currency { get; private set; } = new();
        public static SteamInventory Inventory { get; set; } = new();
        public static CsmAccount Csm { get; set; } = new();
        public static LfmAccount Lfm { get; set; } = new();
        public static BuffAccount Buff { get; set; } = new();

        public static async Task MainAsync()
        {
            string html = await SteamRequest.Get.RequestAsync("https://steamcommunity.com/market/");
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            var nodes = htmlDoc.DocumentNode.Descendants().Where(n => n.Attributes.Any(a => a.Value.Contains("market_headertip_container market_headertip_container_warning")));
            if (nodes.Any())
            {
                Edit.OpenUrl("https://help.steampowered.com/en/faqs/view/71D3-35C2-AD96-AA3A");
                throw new Exception("Your user accounts are limited.");
            }
            ApiKey = await SteamRequest.Get.ApiKeyAsync();
            if (string.IsNullOrEmpty(ApiKey))
            {
                Edit.OpenUrl("https://steamcommunity.com/dev/apikey");
                throw new Exception("Make sure you have register Steam Web API Key.");
            }

            AccountName = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='persona online']").InnerText.Trim();
            UserName = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='account_pulldown']").InnerText.Trim();
            Balance = Edit.GetDouble(htmlDoc.DocumentNode.SelectSingleNode("//a[@id='header_wallet_balance']").InnerText);
            ID64 = await SteamRequest.Get.Id64();

            var img = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='avatarIcon']/img");
            var scr = img.Attributes["src"].Value;
            AvatarUrl = scr.Replace(".jpg", "_full.jpg");
        }
        public static async Task GetSteamBalanceAsync() => Balance = await SteamRequest.Get.BalanceAsync();
        public static async Task SetCurrencyAsync(int id)
        {
            Currency = Currencies.All.FirstOrDefault(x => x.Id == id);
            Currency.Value = !Currencies.Allow.Any(x => x.Id == id) ? await Currencies.GetCurrencyValueAsync(id) : Currencies.Allow.FirstOrDefault(x => x.Id == id).Value;
        }
    }
}
