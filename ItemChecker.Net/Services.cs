using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ItemChecker.Net
{
    public class ServicesRequest : HttpRequest
    {
        public static async Task<JObject?> InspectLinkDetails(string link)
        {
            string json = await RequestGetAsync(@"https://api.csgofloat.com/?url=" + link);
            return JObject.Parse(json)["iteminfo"] as JObject;
        }

        static Dictionary<string, string> OpenIdParams(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);
            var form = htmlDoc.DocumentNode.SelectSingleNode("//form[@id='openidForm']");
            var inputs = new List<string>()
            {
                "action",
                "openid.mode",
                "openidparams",
                "nonce",
            };
            var args = new Dictionary<string, string>();
            foreach (var i in inputs)
                args[i] = form.SelectSingleNode($"//input[@name='{i}']").GetAttributeValue("value", "");

            return args;
        }
        static async Task<CookieContainer> GetCookieAsync(string authUrl, string cookieHost)
        {
            string html = await RequestGetAsync(authUrl, SteamRequest.Cookies, new("https://steamcommunity.com/"));
            var args = OpenIdParams(html);
            var response = await OpenIdRequestPostAsync(args, SteamRequest.Cookies);

            return Helpers.GetCookieContainer(cookieHost, response);
        }

        public class CsMoney : Helpers
        {
            static string Domain { get; set; } = "cs.money";
            internal static CookieContainer Cookies { get; set; } = new();
            public class Get
            {
                public static async Task<string> RequestAsync(string url) => await RequestGetAsync(url, Cookies);

                public static async Task<JObject> ItemInfoAsync(JObject item)
                {
                    string json = await RequestGetAsync("https://cs.money/skin_info?appId=730&id=" + item["assetId"] + "&isBot=true&botInventory=true");
                    return JObject.Parse(json);
                }
                public static async Task<JObject> ItemStatusAsync(string itemName)
                {
                    string market_hash_name = Uri.EscapeDataString(itemName);
                    string json = await RequestGetAsync("https://cs.money/check_skin_status?appId=730&name=" + market_hash_name);
                    return JObject.Parse(json);
                }
                public static async Task<JArray> LoadBotsInventoryAsync(int offset, int minPrice, int maxPrice)
                {
                    string price = $"maxPrice={maxPrice}&minPrice={minPrice}&";

                    string json = await RequestGetAsync($"https://inventories.cs.money/5.0/load_bots_inventory/730?limit=60&offset={offset}&" + price + "&order=desc&priceWithBonus=40&sort=price&withStack=true");
                    return JObject.Parse(json)["items"] as JArray;
                }
                public static async Task<JArray?> LoadBotsInventoryItemAsync(string itemName)
                {
                    string market_hash_name = Uri.EscapeDataString(itemName);
                    string stattrak = market_hash_name.Contains("StatTrak").ToString().ToLower();
                    string souvenir = market_hash_name.Contains("Souvenir").ToString().ToLower();

                    string json = await RequestGetAsync($"https://inventories.cs.money/5.0/load_bots_inventory/730?isSouvenir=" + souvenir + "&isStatTrak=" + stattrak + "&limit=60&name=" + market_hash_name + "&offset=0&order=asc&priceWithBonus=30&sort=price&withStack=true");
                    return JObject.Parse(json)["items"] as JArray;
                }

                public static async Task<JArray> InventoryItemsAsync()
                {
                    string json = await RequestAsync("https://cs.money/3.0/load_user_inventory/730?isPrime=false&limit=60&noCache=true&offset=0&order=desc&sort=price&withStack=true");
                    var obj = JObject.Parse(json);
                    return obj.ContainsKey("items") ? JArray.Parse(obj["items"].ToString()) : new();
                }

                public static async Task<decimal> BalanceAsync()
                {
                    try
                    {
                        HtmlDocument htmlDoc = new();
                        var html = await RequestAsync("https://cs.money/personal-info/");
                        htmlDoc.LoadHtml(html);
                        string trade = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[2]/div[1]/div/div[6]/div[1]/div[2]/div/div[1]/span[2]").InnerText;
                        html = await RequestAsync("https://cs.money/market/buy/");
                        htmlDoc.LoadHtml(html);
                        string market = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[2]/div[1]/div/div[6]/div[1]/div[2]/div/div[1]/span[2]").InnerText;

                        return GetDecimal(market) + GetDecimal(trade);
                    }
                    catch
                    {
                        return 0;
                    }
                }
                internal static async Task<bool> IsAuthorizedAsync()
                {
                    Cookies = await DeserializeCookie(Domain);
                    if (Cookies == null || Cookies.Count == 0)
                        return false;

                    var json = await RequestAsync("https://cs.money/3.0/load_user_inventory/730");
                    return !JObject.Parse(json).ContainsKey("error");
                }
            }
            public class Post
            {
                public static async Task SignInAsync()
                {
                    if (!await Get.IsAuthorizedAsync())
                    {
                        Cookies = await GetCookieAsync("https://auth.dota.trade/login?redirectUrl=https://cs.money/ru/&callbackUrl=https://cs.money/login", "https://cs.money");
                        SerializeCookieAsync(Cookies, Domain).Wait();
                    }
                }
            }
        }
        public class LootFarm : Helpers
        {
            static string Domain { get; set; } = "loot.farm";
            internal static CookieContainer Cookies { get; set; } = new();
            public class Get
            {
                public static async Task<string> RequestAsync(string url) => await RequestGetAsync(url, Cookies);
                public static async Task<JObject> InventoryItemsAsync()
                {
                    string json = await RequestAsync("https://loot.farm/getReserves.php");
                    var obj = JObject.Parse(json);
                    return obj["result"] as JObject;
                }

                public static async Task<decimal> BalanceAsync()
                {
                    var json = await RequestAsync("https://loot.farm/login_data.php");
                    var balance = Convert.ToInt32(JObject.Parse(json)["balance"]);
                    return Convert.ToDecimal(balance) / 100;
                }
                internal static async Task<bool> IsAuthorizedAsync()
                {
                    Cookies = await DeserializeCookie(Domain);
                    if (Cookies == null || Cookies.Count == 0)
                        return false;

                    var json = await RequestAsync("https://loot.farm/historyJSON.php");

                    return !JObject.Parse(json).ContainsKey("error");
                }
            }
            public class Post
            {
                public static async Task SignInAsync()
                {
                    if (!await Get.IsAuthorizedAsync())
                    {
                        Cookies = await GetCookieAsync("https://authsb.trade/lootlogin.php", "https://loot.farm");
                        SerializeCookieAsync(Cookies, Domain).Wait();
                    }
                }
            }
        }
        public class Buff163 : Helpers
        {
            static string Domain { get; set; } = "buff.163.com";
            internal static CookieContainer Cookies { get; set; } = new();
            public class Get
            {
                public static async Task<string> RequestAsync(string url) => await RequestGetAsync(url, Cookies);
                public static async Task<decimal> BalanceAsync()
                {
                    var json = await RequestAsync("https://buff.163.com/api/asset/get_brief_asset/");
                    var balanceInCny = Convert.ToDecimal(JObject.Parse(json)["data"]["alipay_amount"]);
                    return balanceInCny;
                }

                internal static async Task<bool> IsAuthorizedAsync()
                {
                    Cookies = await DeserializeCookie(Domain);
                    if (Cookies == null || Cookies.Count == 0)
                        return false;

                    var json = await RequestAsync("https://buff.163.com/api/market/goods?game=csgo&page_num=2");
                    var jobject = JObject.Parse(json);

                    return jobject["code"].ToString() != "Login Required";
                }
            }
            public class Post
            {
                public static async Task SignInAsync()
                {
                    if (!await Get.IsAuthorizedAsync())
                    {
                        Cookies = await GetCookieAsync("https://buff.163.com/account/login/steam?back_url=/", "https://buff.163.com");
                        var lan = new Cookie("Locale-Supported", "en", "/", Domain)
                        {
                            Secure = true
                        };
                        Cookies.Add(lan);
                        SerializeCookieAsync(Cookies, Domain).Wait();
                    }
                }
            }
        }
    }
}
