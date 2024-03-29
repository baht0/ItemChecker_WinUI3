﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using ItemChecker.Models.StaticModels;
using ItemChecker.Net;
using ItemChecker.Support;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ItemChecker.Models.StaticModels.BaseConfig;

namespace ItemChecker.Models.BuyOrders
{
    public partial class BuyOrderData : ObservableObject
    {
        private int ServiceId { get; set; }

        public string ItemName { get; set; }
        public string Image { get; set; }
        public string Id { get; set; }
        [ObservableProperty]
        private double orderPrice;
        [ObservableProperty]
        private double servicePrice;
        [ObservableProperty]
        private double serviceGive;
        [ObservableProperty]
        private double precent = -100;
        [ObservableProperty]
        private double difference;
        public int Count { get; set; }

        [ObservableProperty]
        private int pushed;
        [ObservableProperty]
        private bool impossible;

        public BuyOrderData(HtmlNode item, int serviceId) => GetItemData(item, serviceId);
        public async void GetItemData(HtmlNode item, int serviceId)
        {
            ServiceId = serviceId;
            ItemName = item.SelectSingleNode(".//div[4]/span/a").InnerText.Trim();
            Image = ItemBase.List.FirstOrDefault(x => x.ItemName == ItemName)?.Image;
            Id = item.Attributes["id"].Value.Replace("mybuyorder_", string.Empty);
            string order_price = item.SelectSingleNode(".//div[2]").InnerText.Trim();
            Count = Convert.ToInt32(item.SelectSingleNode(".//div[3]").InnerText.Trim());
            OrderPrice = Edit.GetDouble(order_price[3..].Trim());

            await SetService();
        }
        private async Task SetService()
        {
            var Item = ItemBase.List.FirstOrDefault(x => x.ItemName == ItemName);
            if (Item == null)
                return;

            switch (ServiceId)
            {
                case 1:
                    {
                        await Item.UpdateSteamItem();
                        ServicePrice = Item.Steam.LowestSellOrder;
                        ServiceGive = Math.Round(ServicePrice * SteamAccount.Inventory.Commission, 2);
                        break;
                    }
                case 2:
                    {
                        await Item.UpdateCsmItem(false);
                        ServicePrice = Item.Csm.Price;
                        ServiceGive = Math.Round(ServicePrice * SteamAccount.Csm.Commission, 2);
                        break;
                    }
                case 3:
                    {
                        await ItemBase.UpdateLfm();
                        ServicePrice = Item.Lfm.Price;
                        ServiceGive = Math.Round(ServicePrice * SteamAccount.Lfm.Commission, 2);
                        break;
                    }
                case 4:
                    {
                        await Item.UpdateBuffItem();
                        ServicePrice = Item.Buff.BuyOrder;
                        ServiceGive = Math.Round(ServicePrice * SteamAccount.Buff.Commission, 2);
                        break;
                    }
                case 5:
                    {
                        await Item.UpdateBuffItem();
                        ServicePrice = Item.Buff.Price;
                        ServiceGive = Math.Round(ServicePrice * SteamAccount.Buff.Commission, 2);
                        break;
                    }
            }
            if (ServiceId != 0)
            {
                Precent = Edit.Precent(OrderPrice, ServiceGive);
                Difference = Edit.Difference(ServiceGive, OrderPrice);
            }
        }

        [ObservableProperty]
        private ActionStatus isCanceled = ActionStatus.None;
        [RelayCommand]
        private async void CancelOrder() => await Cancel();
        public async Task Cancel()
        {
            var res = await SteamRequest.Post.CancelBuyOrder(ItemName, Id);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
                IsCanceled = ActionStatus.OK;
        }
        public async Task Push(double availableAmount, double minPrecent)
        {
            var item = ItemBase.List.FirstOrDefault(x => x.ItemName == ItemName);
            if (item != null)
            {
                var id = SteamAccount.Currency.Id;
                await item.UpdateSteamItem(id);
                double highestBuyOrder = item.Steam.HighestBuyOrder;
                double buyorder_dol = id != 1 ? Currencies.ConverterToUsd(highestBuyOrder, id) : highestBuyOrder;
                double precent = Edit.Precent(buyorder_dol, ServiceGive);

                if (SteamAccount.Balance < highestBuyOrder || (precent < minPrecent && ServiceId != 0))
                {
                    await Cancel();
                    return;
                }
                else if (highestBuyOrder <= OrderPrice)
                    return;
                else if ((highestBuyOrder - OrderPrice) > availableAmount)
                {
                    Impossible = true;
                    return;
                }

                var res = await SteamRequest.Post.CancelBuyOrder(ItemName, Id);
                await Task.Delay(1500);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IsCanceled = ActionStatus.OK;
                    res = await SteamRequest.Post.CreateBuyOrder(ItemName, highestBuyOrder, id, Count);
                    await Task.Delay(1500);
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        IsCanceled = ActionStatus.None;
                        Pushed++;
                        OrderPrice = highestBuyOrder + 0.01d;
                        await SetService();
                    }
                }
            }
        }
    }
}
