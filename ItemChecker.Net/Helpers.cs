using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ItemChecker.Net
{
    public class Helpers
    {
        internal static string UserAgent => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36";
        internal static string CookiesPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ItemChecker\\user\\cookies.json";

        internal static async Task<Cookie> CreateSessionIdAsync()
        {
            var rnd = new Random();
            Byte[] bytes = new Byte[12];
            rnd.NextBytes(bytes);
            string sessionId = Convert.ToHexString(bytes).ToLower();

            var cookie = new Cookie("sessionid", sessionId, "/", "steamcommunity.com")
            {
                Secure = true,
                HttpOnly = false,
            };

            return cookie;
        }
        internal static CookieContainer GetCookieContainer(string cookieHost, HttpResponseMessage response)
        {
            Uri uri = new(cookieHost);
            var initialContainer = new CookieContainer();
            foreach (var header in response.Headers.GetValues("Set-Cookie"))
            {
                var cookie = header.Replace("CEST", "GMT");
                initialContainer.SetCookies(uri, cookie);
            }
            var collection = initialContainer.GetAllCookies();
            foreach (Cookie cookie in collection.Cast<Cookie>())
            {
                cookie.Secure = true;
                cookie.Expires = cookie.Expires != new DateTime() ? cookie.Expires.AddYears(1) : DateTime.Now.AddYears(1);
            }
            var cookies = new CookieContainer();
            cookies.Add(collection);
            return cookies;
        }
        internal static async Task<string> ToResponseStringAsync(HttpResponseMessage response) => await response.Content.ReadAsStringAsync();

        private static JObject GetCookieJson()
        {
            if (!File.Exists(CookiesPath))
                return new JObject();

            var file = File.ReadAllText(CookiesPath);
            var json = JObject.Parse(file);
            return json;
        }
        internal static async Task SerializeCookieAsync(CookieContainer cookies, string host)
        {
            var jsonFile = GetCookieJson();
            string json = JsonSerializer.Serialize(cookies.GetAllCookies());
            var array = JArray.Parse(json);
            if (jsonFile[host] != null)
                jsonFile[host] = array;
            else
                jsonFile.Add(host, array);

            await File.WriteAllTextAsync(CookiesPath, jsonFile.ToString());
        }
        internal static async Task<CookieContainer?> DeserializeCookie(string host)
        {
            var jsonFile = GetCookieJson();
            var json = jsonFile[host]?.ToString();
            if (json == null) return null;

            var cookieCollection = JsonSerializer.Deserialize<CookieCollection>(json);
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(cookieCollection);
            return cookieContainer;
        }

        internal static decimal GetDecimal(string str)
        {
            var mat = Regex.Match(str, @"(\d+(\.\d+)?)|(\.\d+)").Value;
            return Convert.ToDecimal(mat, CultureInfo.InvariantCulture);
        }
    }
}