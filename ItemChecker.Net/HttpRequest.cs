using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ItemChecker.Net
{
    public class HttpRequest
    {
        static HttpClient HttpClient { get; set; } = new();

        public static async Task<string> RequestGetAsync(string url)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            return await HttpClient.GetStringAsync(url);
        }
        internal static async Task<string> RequestGetAsync(string url, CookieContainer cookies)
        {
            Uri uri = new(url);
            uri = new($"https://{uri.Host}");
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("cookie", cookies.GetCookieHeader(uri));
            return await HttpClient.GetStringAsync(url);
        }
        internal static async Task<string> RequestGetAsync(string url, CookieContainer cookies, Uri uri)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("cookie", cookies.GetCookieHeader(uri));
            return await HttpClient.GetStringAsync(url);
        }
        internal static async Task<string> RequestGetAsync(string url, HttpRequestHeaders headers)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            foreach (var head in headers)
                HttpClient.DefaultRequestHeaders.Add(head.Key, head.Value);

            return await HttpClient.GetStringAsync(url);
        }

        internal static async Task<HttpResponseMessage> RequestPostAsync(string url, Dictionary<string, string> args, HttpRequestHeaders headers)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            foreach (var head in headers)
                HttpClient.DefaultRequestHeaders.Add(head.Key, head.Value);

            string param = $"{string.Join("&", args.Select(x => $"{x.Key}={x.Value}"))}";
            HttpRequestMessage request = new(HttpMethod.Post, url)
            {
                Content = new StringContent(param, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            return await HttpClient.SendAsync(request);
        }

        internal static async Task<HttpResponseMessage> SessionRequestPostAsync(string url, Dictionary<string, string> args)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", Helpers.UserAgent);
            HttpClient.DefaultRequestHeaders.Add("Referer", "https://steamcommunity.com/");
            HttpClient.DefaultRequestHeaders.Add("Origin", "https://steamcommunity.com/");
            HttpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

            string param = $"{string.Join("&", args.Select(x => $"{x.Key}={x.Value}"))}";
            HttpRequestMessage request = new(HttpMethod.Post, $"{url}?{param}")
            {
                Content = new StringContent(param, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            return await HttpClient.SendAsync(request);
        }
        internal static async Task<HttpResponseMessage> OpenIdRequestPostAsync(Dictionary<string, string> args, CookieContainer cookies)
        {
            cookies.Add(new Cookie("sessionidSecureOpenIDNonce", args.FirstOrDefault(x => x.Key == "nonce").Value, "/", "steamcommunity.com"));
            using var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                CookieContainer = cookies
            };
            using var httpClient = new HttpClient(handler);
            HttpContent contentForm = new FormUrlEncodedContent(args);
            using var response = await httpClient.PostAsync("https://steamcommunity.com/openid/login", contentForm);

            return response;
        }
    }
}