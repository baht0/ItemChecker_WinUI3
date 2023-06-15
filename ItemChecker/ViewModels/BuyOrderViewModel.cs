using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.BuyOrders;
using ItemChecker.Models.StaticModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.ViewModels
{
    public partial class BuyOrderViewModel : BaseViewModel<BuyOrderData>
    {
        public BuyOrderViewModel()
        {
            Info = new()
            {
                { "Service:", "-" },
                { "Min. precent:", 0 },
                { "Timer (min.):", 0 },
                { "Max order amount:", "-" },
                { "Available amount:", "-" }
            };
        }
        private BuyOrderTool Tool { get; set; } = new();

        [ObservableProperty]
        OrdersParameters parameters = new();

        [ObservableProperty]
        LoadingBar loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;
        public string CurrencyTable => SteamAccount.Currency.Name;

        private void UpdateInfo()
        {
            var info = Parameters.GetInfo();
            info.Add("Max order amount:", Tool.MaxOrderAmount);
            info.Add("Available amount:", Tool.AvailableAmount);
            Info = info;
        }

        [RelayCommand]
        private async void Refresh()
        {
            try
            {
                LoadingBar = new(true, "Refresh", "Checking...");
                var orders = await Tool.GetOrders(Parameters);
                Items = new(orders);
            }
            finally
            {
                LoadingBar = new();
                UpdateInfo();
            }
        }
        [RelayCommand]
        private void CancelAll()
        {
            try
            {
                LoadingBar = new(true, "Cancel all", $"0/0");
                foreach (var order in Items)
                {
                    LoadingBar = new("Cancel all:", $"{Items.IndexOf(order) + 1}/{Items.Count} items.");
                    order.Cancel();
                }
                Items = new();
            }
            finally
            {
                LoadingBar = new();
                UpdateInfo();
            }
        }

        [RelayCommand]
        private void Start()
        {
            LoadingBar = new(true, "Push", "Starting...");
            UpdateInfo();
            CheckTimer();
        }
        [RelayCommand]
        private void Stop()
        {
            LoadingBar = new();
            CTSource.Cancel();
        }
        [RelayCommand]
        private void ResetTime() => Secounds = 3;
        private async void CheckTimer()
        {
            LoadingBar = new("Next check:", "00:00 min.");
            Secounds = Parameters.Time * 60;
            while (Secounds > 0 && LoadingBar.IsBusy)
            {
                Secounds--;
                var timeSpan = TimeSpan.FromSeconds(Secounds);
                LoadingBar = new("Next check:", timeSpan.ToString("mm':'ss") + " min.");
                if (!Items.Any())
                {
                    MessageBar = new MessageBar()
                    {
                        IsOpen = true,
                        Severity = InfoBarSeverity.Error,
                        Title = "Push",
                        Message = "No orders. Click \"Refresh\" to load orders.",
                    };
                    LoadingBar = new();
                    break;
                }
                await Task.Delay(1000);
            }
            if (Secounds <= 0 && LoadingBar.IsBusy)
                PushOrders();
        }
        private async void PushOrders()
        {
            try
            {
                LoadingBar = new("Pushing:", "0/0 items.");
                foreach (var order in Items)
                {
                    try
                    {
                        LoadingBar = new("Pushing:", $"{Items.IndexOf(order) + 1}/{Items.Count} items.");
                        await order.Push(Tool.AvailableAmount, Parameters.MinPrecent);
                    }
                    catch
                    {
                        continue;
                    }
                    if (CToken.IsCancellationRequested)
                        break;
                }
                LoadingBar = new("Push:", "Placement of orders from the list.");
                Tool.PlaceOrders();
            }
            catch (Exception exp)
            {
                LoadingBar = new();
                Debug.WriteLine(exp.Message);
            }
            finally
            {
                Items = new(Items.Where(o => !o.IsCanceled).OrderByDescending(o => o.Precent));
                UpdateInfo();
                CheckTimer();
            }
        }

        [RelayCommand]
        private void SortingAscending(string column)
        {
            List<BuyOrderData> sort = column switch
            {
                "ItemName" => Items.OrderBy(o => o.ItemName).ToList(),
                "OrderPrice" => Items.OrderBy(o => o.OrderPrice).ToList(),
                "ServicePrice" => Items.OrderBy(o => o.ServicePrice).ToList(),
                "ServiceGive" => Items.OrderBy(o => o.ServiceGive).ToList(),
                _ => Items.OrderBy(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void SortingDescending(string column)
        {
            List<BuyOrderData> sort = column switch
            {
                "ItemName" => Items.OrderByDescending(o => o.ItemName).ToList(),
                "OrderPrice" => Items.OrderByDescending(o => o.OrderPrice).ToList(),
                "ServicePrice" => Items.OrderByDescending(o => o.ServicePrice).ToList(),
                "ServiceGive" => Items.OrderByDescending(o => o.ServiceGive).ToList(),
                _ => Items.OrderByDescending(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
    }
}
