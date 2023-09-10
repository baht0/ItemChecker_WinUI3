using HtmlAgilityPack;
using ItemChecker.Models.BuyOrders;
using ItemChecker.Models.StaticModels;
using ItemChecker.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ItemChecker.Views.Database;

namespace ItemChecker.Models
{
    public class BuyOrderTool
    {
        public static string QueueService { get; set; } = "Unknown";
        public static List<string> Queue { get; set; } = new();

        public double MaxOrderAmount => SteamAccount.Balance * 10;
        public double AvailableAmount
        {
            get
            {
                if (Orders.Any())
                    return Math.Round(MaxOrderAmount - Orders.Sum(s => s.OrderPrice * s.Count), 2);
                return MaxOrderAmount;
            }
        }

        private OrdersParameters Parameters { get; set; }
        private List<BuyOrderData> Orders { get; set; } = new();

        public async Task<List<BuyOrderData>> GetOrders(OrdersParameters parameters)
        {
            Parameters = parameters;
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(await SteamRequest.Get.RequestAsync("https://steamcommunity.com/market/"));
            int index = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='my_listing_section market_content_block market_home_listing_table']/h3/span[1]").InnerText.Trim() != "My listings awaiting confirmation" ? 1 : 2;
            HtmlNodeCollection items = htmlDoc.DocumentNode.SelectNodes("//div[@class='my_listing_section market_content_block market_home_listing_table'][" + index + "]/div[@class='market_listing_row market_recent_listing_row']");

            BuyOrderData min = null;
            if (items != null)
            {
                foreach (HtmlNode item in items)
                {
                    var itemName = item.SelectSingleNode(".//div[4]/span/a").InnerText.Trim();
                    var data = Orders.FirstOrDefault(x => x.ItemName == itemName);
                    if (data != null)
                    {
                        data.GetItemData(item, parameters.ServiceId);
                        if (!IsAllow(data))
                            await data.Cancel();
                    }
                    else
                    {
                        data = new BuyOrderData(item, parameters.ServiceId);
                        if (IsAllow(data))
                            Orders.Add(data);
                        else
                            await data.Cancel();
                    }
                    min ??= data;
                    min = data.Precent < min.Precent ? data : min;
                }
                if (min != null && AvailableAmount < (MaxOrderAmount * 0.01d))
                    await min.Cancel();
            }
            else
                Orders = new();
            return Orders;
        }
        private bool IsAllow(BuyOrderData data)
        {
            var item = ItemBase.List.FirstOrDefault(x => x.ItemName == data.ItemName);

            bool isAllow = item != null;
            if (isAllow)
                isAllow = SteamAccount.Balance > data.OrderPrice;
            if (isAllow)
                isAllow = Parameters.ServiceId == 0 || (Parameters.ServiceId != 0 && Parameters.MinPrecent < data.Precent);
            if (isAllow)
            {
                switch (Parameters.ServiceId)
                {
                    case 2:
                        isAllow = item.Csm.Status != ItemStatus.Overstock && item.Csm.Status != ItemStatus.Unavailable;
                        break;
                    case 3:
                        isAllow = item.Lfm.Status != ItemStatus.Overstock && item.Lfm.Status != ItemStatus.Unavailable;
                        break;
                }
            }
            if (isAllow)
                isAllow = data.IsCanceled != BaseConfig.ActionStatus.OK;

            return isAllow;
        }

        public static async Task ExportQueue()
        {
            var itemsArray = JArray.Parse(JsonConvert.SerializeObject(Queue, Formatting.Indented));

            JObject json = new(
                new JProperty("Size", Queue.Count),
                new JProperty("Service", QueueService),
                new JProperty("Items", itemsArray));

            await File.WriteAllTextAsync(AppConfig.DocumentPath + $"queue.json", json.ToString());
        }
        public static async Task ImportQueue()
        {
            string path = AppConfig.DocumentPath + "queue.json";

            if (File.Exists(path))
            {
                var json = JObject.Parse(await File.ReadAllTextAsync(path));

                Queue = json["Items"].ToObject<List<string>>();
                QueueService = json["Service"].ToString();
            }
        }
    }
}
