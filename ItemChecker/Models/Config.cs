using System.IO;
using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ItemChecker.Net;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models
{
    internal class AppConfig
    {
        public static string Path => AppDomain.CurrentDomain.BaseDirectory;
        public static string UpdateFolder => AppDomain.CurrentDomain.BaseDirectory + "\\update";
        public static string DocumentPath
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ItemChecker\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static bool IsUpdate { get; private set; }

        public static async Task AppCheckAsync()
        {
            await CheckUpdateAsync();
        }
        private static async Task CheckUpdateAsync()
        {
            JArray json = JArray.Parse(await DropboxRequest.Get.ReadAsync("Updates.json"));

            var latestVersion = (string)json.LastOrDefault()["version"];
            int latest = Convert.ToInt32(latestVersion.Replace(".", string.Empty));
            int current = Convert.ToInt32(CurrentVersion.Replace(".", string.Empty));
            IsUpdate = latest > current;
        }
    }
    internal class UserConfig
    {
        string UserPath
        {
            get
            {
                string path = AppConfig.DocumentPath + "user\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        string ConfigPath => UserPath + "config.json";

        public string AccountName { get; set; } = string.Empty;
        public int SteamCurrencyId { get; set; } = 0;

        public UserConfig()
        {
            if (File.Exists(ConfigPath))
            {
                var str = File.ReadAllText(ConfigPath);
                var json = JObject.Parse(str);
                AccountName = (string)json[nameof(AccountName)];
                SteamCurrencyId = (int)json[nameof(SteamCurrencyId)];
            }
        }
        public async void SaveAsync()
        {
            var json = JObject.FromObject(this);
            await File.WriteAllTextAsync(ConfigPath, json.ToString());
        }
        public void Delete()
        {
            File.Delete(ConfigPath);
            File.Delete(UserPath + "cookies.json");
        }
    }
}
