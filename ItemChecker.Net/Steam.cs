using HtmlAgilityPack;
using ItemChecker.Net.Session;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace ItemChecker.Net
{
    public class SteamRequest : HttpRequest
    {
        internal static CookieContainer Cookies { get; set; } = new();
        static string ApiKey { get; set; } = string.Empty;
        static Cookie SessionId
        {
            get
            {
                if (Cookies != null)
                {
                    CookieCollection cookieCollection = Cookies.GetAllCookies();
                    _sessionId = cookieCollection.FirstOrDefault(x => x.Name == "sessionid");
                }
                return _sessionId == null ? Helpers.CreateSessionIdAsync().Result : _sessionId;
            }
        }
        static Cookie _sessionId = new();

        public class Get : Helpers
        {
            internal static DateTime ifModifiedSince { get; set; } = DateTime.Now.ToUniversalTime();

            public static async Task<string> RequestAsync(string url) => await RequestGetAsync(url, Cookies);
            public static async Task<double> BalanceAsync()
            {
                var html = await RequestAsync("https://steamcommunity.com/market/");
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                return GetDouble(htmlDoc.DocumentNode.SelectSingleNode("//a[@id='header_wallet_balance']").InnerText);
            }
            public static async Task<string> ApiKeyAsync()
            {
                string html = await RequestAsync("https://steamcommunity.com/dev/apikey");
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                string apiKey = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='bodyContents_ex']/p").InnerText;

                if (apiKey.Contains("Key: "))
                    ApiKey = apiKey.Replace("Key: ", string.Empty);
                return ApiKey;
            }
            public static async Task<string> Id64()
            {
                CookieCollection cookieCollection = Cookies.GetAllCookies();
                var cookie = cookieCollection.FirstOrDefault(x => x.Name == "steamLoginSecure");
                return cookie.Value[..17];
            }
            public static async Task<JObject> GameServersStatusAsync()
            {
                string json = await RequestGetAsync("https://api.steampowered.com/ICSGOServers_730/GetGameServersStatus/v1/?key=" + ApiKey);
                return JObject.Parse(json);
            }
            public static async Task<JObject> TradeOffersAsync()
            {
                string json = await RequestGetAsync(@"http://api.steampowered.com/IEconService/GetTradeOffers/v1/?key=" + ApiKey + "&get_received_offers=1&active_only=100");
                return JObject.Parse(json);
            }

            static HttpRequestHeaders MarketHeaders(string market_hash_name)
            {
                var headers = new HttpClient().DefaultRequestHeaders;
                headers.Add("Accept", "*/*");
                headers.Add("User-Agent", UserAgent);
                headers.Add("Referer", "https://steamcommunity.com/market/listings/730/" + market_hash_name);
                headers.Add("Origin", "https://steamcommunity.com/");
                headers.Add("sec-ch-ua", "Google Chrome\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24");
                headers.Add("sec-ch-ua-mobile", "ooooo");
                headers.Add("sec-ch-ua-platform", "Windows");
                headers.Add("Sec-Fetch-Dest", "empty");
                headers.Add("Sec-Fetch-Mode", "cors");
                headers.Add("Sec-Fetch-Site", "same-origin");

                return headers;
            }
            public static async Task<JObject> ItemListingsAsync(string itemName)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                string url = "https://steamcommunity.com/market/listings/730/" + market_hash_name + "/render?start=0&count=100&currency=1&language=english&format=json";
                var headers = MarketHeaders(market_hash_name);
                var json = await RequestGetAsync(url, headers);
                return JObject.Parse(json);
            }
            public static async Task<JObject> PriceOverviewAsync(string itemName, int currencyId)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                string url = "https://steamcommunity.com/market/priceoverview/?country=RU&currency=" + currencyId + "&appid=730&market_hash_name=" + market_hash_name;
                var headers = MarketHeaders(market_hash_name);
                var json = await RequestGetAsync(url, headers);
                return JObject.Parse(json);
            }
            public static async Task<JObject> ItemOrdersHistogramAsync(string itemName, int item_nameid, int currencyId)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                string url = "https://steamcommunity.com/market/itemordershistogram?country=RU&language=english&currency=" + currencyId + "&item_nameid=" + item_nameid + "&two_factor=0";
                var headers = MarketHeaders(market_hash_name);
                headers.IfModifiedSince = ifModifiedSince.AddMilliseconds(-7000);
                var json = await RequestGetAsync(url, headers);
                return JObject.Parse(json);
            }
        }
        public class Post : Helpers
        {
            static async Task<HttpResponseMessage> RequestAsync(string url, string referer, Dictionary<string, string> args)
            {
                Uri uriAddress = new(url);
                uriAddress = new($"https://{uriAddress.Host}");

                var headers = new HttpClient().DefaultRequestHeaders;
                headers.Add("Accept", "*/*");
                headers.Add("Referer", referer);
                headers.Add("User-Agent", UserAgent);
                headers.Add("Cookie", Cookies.GetCookieHeader(uriAddress));

                return await RequestPostAsync(url, args, headers);
            }

            public static async Task<HttpResponseMessage> CreateBuyOrder(string itemName, double highest_buy_order, int currencyId, int count = 1)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"currency", currencyId.ToString()},
                    {"appid", "730"},
                    {"market_hash_name", market_hash_name},
                    {"price_total", ((int)(highest_buy_order * 100 + 1)).ToString()},
                    {"quantity", count.ToString()},
                    {"billing_state", string.Empty},
                    {"save_my_address", "0"},
                };
                string url = "https://steamcommunity.com/market/createbuyorder/";
                string referer = "https://steamcommunity.com/market/listings/730/" + market_hash_name;

                return await RequestAsync(url, referer, args);
            }
            public static async Task<HttpResponseMessage> CancelBuyOrder(string itemName, string buyOrderId)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"buy_orderid", buyOrderId},
                };

                string url = "https://steamcommunity.com/market/cancelbuyorder/";
                string referer = "https://steamcommunity.com/market/listings/730/" + market_hash_name;

                return await RequestAsync(url, referer, args);
            }
            public static async Task<HttpResponseMessage> BuyListing(string itemName, string listingId, double fee, double subtotal, double total, int currencyId)
            {
                string market_hash_name = Uri.EscapeDataString(itemName);
                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"currency", currencyId.ToString()},
                    {"fee", ((int)fee).ToString()},
                    {"subtotal", ((int)subtotal).ToString()},
                    {"total", ((int)total).ToString()},
                    {"quantity", "1"},
                    {"first_name", string.Empty},
                    {"last_name", string.Empty},
                    {"billing_address", string.Empty},
                    {"billing_address_two", string.Empty},
                    {"billing_country", string.Empty},
                    {"billing_city", string.Empty},
                    {"billing_state", string.Empty},
                    {"billing_postal_code", string.Empty},
                    {"save_my_address", "1"},
                };

                string url = "https://steamcommunity.com/market/buylisting/" + listingId;
                string referer = "https://steamcommunity.com/market/listings/730/" + market_hash_name;

                return await RequestAsync(url, referer, args);
            }

            public static async Task<HttpResponseMessage> SellItem(string assetId, int price)
            {
                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"appid", "730"},
                    {"contextid", "2"},
                    {"assetid", assetId},
                    {"amount", "1"},
                    {"price", price.ToString()},
                };
                string url = "https://steamcommunity.com/market/sellitem/";
                string referer = "https://steamcommunity.com/my/inventory/";

                return await RequestAsync(url, referer, args);
            }
            public static async Task<HttpResponseMessage> AcceptTrade(string tradeOfferId, string partnerId)
            {
                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"serverid", "1"},
                    {"tradeofferid", tradeOfferId},
                    {"partner", partnerId},
                    {"captcha", string.Empty},
                };

                string url = "https://steamcommunity.com/tradeoffer/" + tradeOfferId + "/accept";
                string referer = "https://steamcommunity.com/tradeoffer/" + tradeOfferId;

                return await RequestAsync(url, referer, args);
            }
        }
        public class Session : Helpers
        {
            public static bool IsSetToken { get; private set; }
            private static LoginPassResponse PassResponse { get; set; }
            private static LoginQrResponse QrResponse { get; set; }

            public static async Task<bool> IsAuthorizedAsync()
            {
                Cookies = await DeserializeCookie("steamcommunity.com");
                if (Cookies == null)
                    return false;

                var html = await Get.RequestAsync("https://steamcommunity.com/login/home/?goto=my/profile");
                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(html);
                string title = htmlDoc.DocumentNode.SelectSingleNode("html/head/title").InnerText;

                return !title.Contains("Sign In");
            }

            //Login with password
            public static async Task<bool> SubmitSignIn(string accountName, string password)
            {
                var rsa = new RsaPassword();
                var rsaPass = await rsa.GetEncryptedPasswordAsync(accountName, password);
                await BeginAuthSessionViaCredentials(accountName, rsaPass);

                return PassResponse != null;
            }
            static async Task BeginAuthSessionViaCredentials(string accountName, RsaPassword rsaPassword)
            {
                string url = "https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaCredentials/v1/";

                Dictionary<string, string> args = new()
                {
                    {"access_token", string.Empty},
                    {"device_friendly_name", Uri.EscapeDataString(UserAgent)},
                    {"account_name", accountName},
                    {"encrypted_password", Uri.EscapeDataString(rsaPassword.EncryptedPassword)},
                    {"encryption_timestamp", rsaPassword.TimeStamp},
                    {"remember_login", "true"},
                    {"platform_type", "2"},
                    {"persistence", "1"},
                    {"website_id", "Community"},
                };
                var response = await SessionRequestPostAsync(url, args);
                string json = await ToResponseStringAsync(response);

                var jobject = JObject.Parse(json)["response"] as JObject;
                if (jobject.ContainsKey("steamid"))
                {
                    PassResponse = new LoginPassResponse()
                    {
                        ClientId = jobject["client_id"].ToString(),
                        RequestId = jobject["request_id"].ToString(),
                        Interval = jobject["interval"].ToString(),
                        AllowedConfirmations = LoginPassResponse.GetAllowedConfirmations(JArray.Parse(jobject["allowed_confirmations"].ToString())),
                        SteamId = jobject["steamid"].ToString(),
                        WeakToken = jobject["weak_token"].ToString(),
                        ExtendedErrorMessage = jobject["extended_error_message"].ToString()
                    };
                }
            }
            public static async Task SubmitCode(string code) => await UpdateAuthSessionWithSteamGuardCode(code);
            static async Task UpdateAuthSessionWithSteamGuardCode(string code)
            {
                string url = "https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithSteamGuardCode/v1/";
                Dictionary<string, string> args = new()
                {
                    {"access_token", string.Empty},
                    {"client_id", PassResponse.ClientId},
                    {"steamid", PassResponse.SteamId},
                    {"code", code},
                    {"code_type", "3"},
                };
                await SessionRequestPostAsync(url, args);
            }

            //Login with QR code
            public static async Task<string> BeginAuthSessionViaQR()
            {
                string url = "https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaQR/v1/";

                Dictionary<string, string> args = new()
                {
                    {"device_friendly_name", Uri.EscapeDataString(UserAgent)},
                    {"platform_type", "2"},
                    {"website_id", "Community"},
                };
                var response = await SessionRequestPostAsync(url, args);
                string json = await ToResponseStringAsync(response);

                var jobject = JObject.Parse(json)["response"] as JObject;
                QrResponse = new LoginQrResponse()
                {
                    ClientId = jobject["client_id"].ToString(),
                    ChallengeUrl = jobject["challenge_url"].ToString(),
                    RequestId = jobject["request_id"].ToString(),
                    Interval = jobject["interval"].ToString(),
                    AllowedConfirmations = LoginQrResponse.GetAllowedConfirmations(JArray.Parse(jobject["allowed_confirmations"].ToString())),
                    Version = Convert.ToInt32(jobject["version"]),
                };

                return "https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=?x=" + QrResponse.ChallengeUrl;
            }

            //Check AuthSessionStatus
            public static async void CheckAuthStatus()
            {
                PollResponse pollResponse = null;
                while (pollResponse == null)
                {
                    pollResponse = await PollAuthSessionStatus();
                    if (pollResponse != null)
                        break;
                    await Task.Delay(5000);
                }

                var finalizeResponse = await FinalizeLogin(pollResponse);
                var httpResponse = await SetToken(finalizeResponse);

                Cookies = GetCookieContainer("https://steamcommunity.com/", httpResponse);
                Cookies.Add(SessionId);
                await SerializeCookieAsync(Cookies, "steamcommunity.com");

                IsSetToken = true;
            }
            static async Task<PollResponse> PollAuthSessionStatus()
            {
                dynamic data = null;
                if (PassResponse != null)
                    data = PassResponse;
                else if (QrResponse != null)
                    data = QrResponse;
                else
                    return data;

                string url = "https://api.steampowered.com/IAuthenticationService/PollAuthSessionStatus/v1/";
                Dictionary<string, string> args = new()
                {
                    {"access_token", string.Empty},
                    {"client_id", data.ClientId},
                    {"request_id", Uri.EscapeDataString(data.RequestId)},
                };
                var response = await SessionRequestPostAsync(url, args);
                string json = await ToResponseStringAsync(response);

                var jobject = JObject.Parse(json)["response"] as JObject;
                if (jobject.ContainsKey("refresh_token"))
                {
                    return new PollResponse()
                    {
                        RefreshToken = jobject["refresh_token"].ToString(),
                        AccessToken = jobject["access_token"].ToString(),
                        HadRemoteInteraction = Convert.ToBoolean(jobject["had_remote_interaction"]),
                        AccountName = jobject["account_name"].ToString()
                    };
                }
                return null;
            }
            static async Task<FinalizeResponse> FinalizeLogin(PollResponse data)
            {
                string url = "https://login.steampowered.com/jwt/finalizelogin/";

                Dictionary<string, string> args = new()
                {
                    {"sessionid", SessionId.Value},
                    {"redir", "https://steamcommunity.com/"},
                    {"nonce", Uri.EscapeDataString(data.RefreshToken)},
                };
                var response = await SessionRequestPostAsync(url, args);
                string json = await ToResponseStringAsync(response);

                var jobject = JObject.Parse(json);
                if (jobject.ContainsKey("steamID"))
                {
                    return new FinalizeResponse()
                    {
                        SteamID = jobject["steamID"].ToString(),
                        Redir = jobject["redir"].ToString(),
                        TransferInfo = JArray.Parse(jobject["transfer_info"].ToString()).Select(x => new Transfer
                        {
                            Url = x["url"].ToString(),
                            Params = JObject.Parse(x["params"].ToString()).ToObject<Param>(),
                        }).ToList(),
                        PrimaryDomain = jobject["primary_domain"].ToString()
                    };
                }
                return new();
            }
            static async Task<HttpResponseMessage> SetToken(FinalizeResponse data)
            {
                Dictionary<string, string> args = new()
                {
                    {"nonce", data.TransferInfo[1].Params.Nonce},
                    {"auth", data.TransferInfo[1].Params.Auth},
                    {"steamID", data.SteamID},
                };

                return await SessionRequestPostAsync(data.TransferInfo[1].Url, args);
            }
        }
    }
}
