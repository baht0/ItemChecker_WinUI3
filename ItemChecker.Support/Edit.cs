using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ItemChecker.Support
{
    public class Edit
    {
        public static void OpenUrl(string url)
        {
            var psi = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(psi);
        }
        public static void OpenCsm(string itemName)
        {
            string market_hash_name = Uri.EscapeDataString(itemName);
            string stattrak = "false";
            string souvenir = "false";
            if (market_hash_name.Contains("StatTrak"))
                stattrak = "true";
            if (market_hash_name.Contains("Souvenir"))
                souvenir = "true";

            string url = "https://cs.money/csgo/trade/?search=" + market_hash_name + "&sort=price&order=asc&hasRareFloat=false&hasRareStickers=false&hasRarePattern=false&hasTradeLock=false&hasTradeLock=true&isStatTrak=" + stattrak + "&isSouvenir=" + souvenir;

            OpenUrl(url);
        }

        public static decimal GetDecimal(string str)
        {
            var mat = Regex.Match(str, @"(\d+(\.\d+)?)|(\.\d+)").Value;
            return Convert.ToDecimal(mat, CultureInfo.InvariantCulture);
        }
        public static decimal SteamAvgPrice(string itemName, JObject items)
        {
            itemName = itemName.Replace("'", "&#39");
            var item = items[itemName] as JObject;
            if (item != null && item["price"] != null)
            {
                var price = item["price"];
                decimal value;
                if (price["24_hours"] != null && decimal.TryParse((string)price["24_hours"]["average"], out value) && value != 0)
                    return value;
                else if (price["7_days"] != null && decimal.TryParse((string)price["7_days"]["average"], out value) && value != 0)
                    return value;
                else if (price["30_days"] != null && decimal.TryParse((string)price["30_days"]["average"], out value) && value != 0)
                    return value;
                else if (price["all_time"] != null && decimal.TryParse((string)price["all_time"]["average"], out value) && value != 0)
                    return value;
            }
            return 0;
        }

        public static decimal Precent(decimal a, decimal b) //from A to B
        {
            if (a != 0)
                return Math.Round((b - a) / a * 100, 2);
            else
                return 0;
        }
        public static decimal Difference(decimal a, decimal b)
        {
            return Math.Round(a - b, 2);
        }

        //time
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp).ToLocalTime();
        }
        public static DateTime ConvertFromUnixTimestampJava(double timestamp)
        {
            DateTime origin = new(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(timestamp).ToLocalTime();
        }
        public static string calcTimeLeft(DateTime start, int count, int i)
        {
            double min = (count - ++i) / calcTimeLeftSpeed(start, i);
            TimeSpan time = TimeSpan.FromMinutes(min);
            if (min > 60)
                return time.ToString("hh'h 'mm'min'");
            else if (min > 1)
                return time.ToString("mm'min 'ss'sec.'");

            return time.ToString("ss'sec.'");
        }
        static double calcTimeLeftSpeed(DateTime start, int i)
        {
            var time_passed = DateTime.Now.Subtract(start).TotalMinutes;
            return Math.Round(++i / time_passed, 2);
        }
    }
}
