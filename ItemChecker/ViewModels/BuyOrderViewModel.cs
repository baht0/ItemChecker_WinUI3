using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.BuyOrders;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels
{
    public partial class BuyOrderViewModel : BaseViewModel<BuyOrderData>
    {
        public BuyOrderViewModel()
        {
            Info = new()
            {
                { "Service:", "-" },
                { "Min. precent (%):", 0 },
                { "Timer (min.):", 0 },
                { $"Max order amount ({SteamAccount.Currency.Symbol}):", "-" },
                { $"Available amount ({SteamAccount.Currency.Symbol}):", "-" }
            };
        }
        private BuyOrderTool Tool { get; set; } = new();

        [ObservableProperty]
        OrdersParameters parameters = new();

        [ObservableProperty]
        string queueService;
        [ObservableProperty]
        ObservableCollection<string> queue = new();

        [ObservableProperty]
        LoadingBar loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;
        public string CurrencyTable => SteamAccount.Currency.Name;

        private void UpdateInfo()
        {
            var info = Parameters.GetInfo();
            info.Add($"Max order amount ({SteamAccount.Currency.Symbol}):", Tool.MaxOrderAmount);
            info.Add($"Available amount ({SteamAccount.Currency.Symbol}):", Tool.AvailableAmount);
            Info = info;
        }

        [RelayCommand]
        private async void Refresh()
        {
            try
            {
                LoadingBar = new(true, "Refresh:", "Checking...");
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
        private async void CancelAll()
        {
            try
            {
                LoadingBar = new(true, "Cancel all:", "0/0");
                foreach (var order in Items)
                {
                    LoadingBar = new("Cancel all:", $"{Items.IndexOf(order) + 1}/{Items.Count} items.");
                    await order.Cancel();
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
        private async void Start()
        {
            LoadingBar = new(true, "Push:", "Starting...");
            var orders = await Tool.GetOrders(Parameters);
            Items = new(orders);
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
            try
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
                        MessageShow("Push", "No orders.", 3);
                        LoadingBar = new();
                        break;
                    }
                    await Task.Delay(1000);
                }
                if (Secounds <= 0 && LoadingBar.IsBusy)
                    PushOrders();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
                if (Parameters.ServiceId == BaseConfig.Services.IndexOf(QueueService))
                {
                    LoadingBar = new("Push:", "Placement of orders from the list.");
                }
            }
            catch (Exception exp)
            {
                LoadingBar = new();
                Debug.WriteLine(exp.Message);
            }
            finally
            {
                Items = new(Items.Where(o => o.IsCanceled != BaseConfig.ActionStatus.OK).OrderByDescending(o => o.Precent));
                UpdateInfo();
                CheckTimer();
            }
        }

        [RelayCommand]
        private void OpenIn(object[] obj)
        {
            string itemName = ((BuyOrderData)obj[1]).ItemName;
            switch (obj[0])
            {
                case 0:
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(itemName);
                    Clipboard.SetContent(dataPackage);
                    break;
                case 2:
                    OpenIn(itemName, 0);
                    break;
                case 3:
                    OpenIn(itemName, Parameters.ServiceId);
                    break;
            }
        }
        private void OpenIn(string itemName, int id)
        {
            var item = ItemBase.List.FirstOrDefault(x => x.ItemName == itemName);
            string market_hash_name = Uri.EscapeDataString(item.ItemName);
            switch (id)
            {
                case 0 or 1:
                    Edit.OpenUrl("https://steamcommunity.com/market/listings/730/" + market_hash_name);
                    break;
                case 2:
                    Edit.OpenCsm(item.ItemName);
                    break;
                case 3:
                    Edit.OpenUrl("https://loot.farm/#skin=730_" + market_hash_name);
                    break;
                case 4:
                    Edit.OpenUrl("https://buff.163.com/goods/" + item.Buff.Id + "#tab=buying");
                    break;
                case 5:
                    Edit.OpenUrl("https://buff.163.com/goods/" + item.Buff.Id);
                    break;
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

        [RelayCommand]
        private async void OpenQueue()
        {
            await BuyOrderTool.ImportQueue();
            Queue = new(BuyOrderTool.Queue);
            QueueService = BuyOrderTool.QueueService;
        }
        [RelayCommand]
        private void ClearList()
        {
            Queue.Clear(); BuyOrderTool.Queue.Clear();
            QueueService = BuyOrderTool.QueueService = "Unknown";
            var path = AppConfig.DocumentPath + "queue.json";
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
