﻿using ItemChecker.Support;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest.CsMoney;

namespace ItemChecker.Models.StaticModels.Base
{
    public class CsmItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public int Id { get; set; }
        public double Price { get; set; }
        public int OverstockDifference { get; set; }
        public bool IsHave { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.Unavailable;
        public List<InventoryCsm> Inventory { get; set; } = new();

        internal async Task UpdateAsync(string itemName, bool isInventory)
        {
            if (Price > 0 || (Inventory.Any() && isInventory))
                return;

            var items = await Get.LoadBotsInventoryItemAsync(itemName);
            var status = await Get.ItemStatusAsync(itemName);

            var itemStatus = ItemStatus.Available;
            if (int.TryParse((string)status["status"], out int result) && result == 0)
                itemStatus = ItemStatus.Unavailable;
            else if (result == 1)
                itemStatus = ItemStatus.Overstock;

            if (items != null)
            {
                bool isAdded = false;
                foreach (JObject item in items.Cast<JObject>())
                {
                    string serviceItemName = item["fullName"].ToString();
                    if (serviceItemName == itemName && !isAdded)
                    {
                        Updated = DateTime.Now;
                        Id = Convert.ToInt32(item["nameId"]);
                        var price = await Get.ItemInfoAsync(item);
                        Price = Convert.ToInt32(price["defaultPrice"]);
                        OverstockDifference = Convert.ToInt32(status["overstockDiff"]);
                        Status = itemStatus;

                        isAdded = true;
                    }
                    if (serviceItemName == itemName && isInventory)
                    {
                        Inventory.Add(new()
                        {
                            NameId = Convert.ToInt32(item["nameId"]),
                            StackSize = item.ContainsKey("stackSize") ? Convert.ToInt32(item["stackSize"]) : 1,
                            Price = Convert.ToDouble(item["price"]),
                            Float = item["float"].Type != JTokenType.Null ? Convert.ToDouble(item["float"]) : 0,
                            Sticker = item["stickers"].Type != JTokenType.Null,
                            RareItem = item["overpay"].Type != JTokenType.Null,
                            User = item["userId"].Type != JTokenType.Null,
                            TradeLock = item.ContainsKey("tradeLock") ? Edit.ConvertFromUnixTimestampJava(Convert.ToDouble(item["tradeLock"])) : new(),
                            Updated = DateTime.Now,
                        });
                    }
                    else if (!isInventory && !isAdded)
                        continue;
                    else if (!isInventory && isAdded)
                        break;
                }
                IsHave = items.HasValues;
            }
            else
            {
                Updated = DateTime.Now;
                OverstockDifference = Convert.ToInt32(status["overstockDiff"]);
                Status = itemStatus;
            }
        }
    }
    public class InventoryCsm
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public int NameId { get; set; }
        public int StackSize { get; set; }
        public double Price { get; set; }
        public bool Sticker { get; set; }
        public double Float { get; set; }
        public bool User { get; set; }
        public DateTime TradeLock { get; set; } = new();
        public bool RareItem { get; set; }
    }
}
