using System.IO;
using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ItemChecker.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ItemChecker.Models.StaticModels
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
        public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName()?.Version?.ToString();
        public static bool IsUpdate { get; private set; }

        public static async Task AppCheckAsync() => await CheckUpdateAsync();
        private static async Task CheckUpdateAsync()
        {
            JArray json = JArray.Parse(await DropboxRequest.Get.ReadAsync("Updates.json"));

            var latestVersion = (string)json.LastOrDefault()["version"];
            int latest = Convert.ToInt32(latestVersion.Replace(".", string.Empty));
            int current = Convert.ToInt32(CurrentVersion.Replace(".", string.Empty));
            IsUpdate = latest > current;
        }
    }
    internal class BaseConfig
    {
        public static List<string> ServicesShort => new() { "SteamMarket", "Cs.Money", "Loot.Farm", "Buff163" };
        public static List<string> Services => new() { "SteamMarket(A)", "SteamMarket", "Cs.Money(T)", "Loot.Farm", "Buff163(A)", "Buff163" };
        public static List<string> RareParameters => new() { "Float", "Sticker", "Doppler" };
    }
    public class GroupInfoCollection<T> : ObservableCollection<T>
    {
        public object Key { get; set; }
        public new IEnumerator<T> GetEnumerator() => (IEnumerator<T>)base.GetEnumerator();
    }
}
