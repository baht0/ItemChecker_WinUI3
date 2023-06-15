using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.AccountModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Models.StaticModels.Accounts;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemChecker.ViewModels.AccountViewModels
{
    public partial class InventoryViewModel : BaseViewModel<DataInventory>
    {
        public string CurrencyTable => SteamAccount.Currency.Name;
        [ObservableProperty]
        SellParameters _parameters = new();
        [ObservableProperty]
        Records _records = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand), new string[] { nameof(AcceptTradesCommand), nameof(SellItemsCommand) })]
        LoadingBar _loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;

        public InventoryViewModel()
        {
            Info = new()
            {
                { "Inventory price currency:", SteamAccount.Currency.Name },
                { "Sum of items sell order:", 0 },
                { "Sum of items buy order:", 0 },
            };
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void Refresh()
        {
            string title = nameof(Refresh);
            try
            {
                LoadingBar = new(true, title, "Checking inventory...");
                Info = await SteamAccount.Inventory.Main();
                Items = new(SteamAccount.Inventory.Items);

                LoadingBar = new(true, title, "Calculating balance...");
                await Records.RefreshRecords();
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success,
                    Title = title,
                    Message = "Successful refreshed."
                };
            }
            catch
            {
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error,
                    Title = title,
                    Message = "An error has occurred."
                };
            }
            finally
            {
                LoadingBar = new();
            }
        }
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void AcceptTrades()
        {
            try
            {
                LoadingBar = new(true, "Accept trades", "Working...");
                var accepted = await SteamAccount.Inventory.AcceptTradeAsync();
                if (accepted > 0)
                {
                    await SteamAccount.Inventory.Main();
                    Items = new(SteamAccount.Inventory.Items);
                }
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success,
                    Title = "Accept trades",
                    Message = $"{accepted} accepted."
                };
            }
            catch
            {
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error,
                    Title = "Accept trades",
                    Message = "An error has occurred."
                };
            }
            finally
            {
                LoadingBar = new();
            }
        }
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void SellItems()
        {
            try
            {
                LoadingBar = new(true, "Sell items", "Working...");
                await SteamAccount.Inventory.SellItemAsync(Parameters);
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success,
                    Title = "Sell items",
                    Message = $"Sale listing created."
                };
            }
            catch
            {
                MessageBar = new()
                {
                    IsOpen = true,
                    Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error,
                    Title = "Sell items",
                    Message = "An error has occurred."
                };
            }
            finally
            {
                LoadingBar = new();
            }
        }

        [RelayCommand]
        private void OpenIn(int id) => Edit.OpenUrl("https://steamcommunity.com/my/inventory/#730_2_" + Items[id].Data.AssetId);
        [RelayCommand]
        private void SortingAscending(string column)
        {
            List<DataInventory> sort = column switch
            {
                "ItemName" => Items.OrderBy(o => o.ItemName).ToList(),
                "HighestBuyOrder" => Items.OrderBy(o => o.HighestBuyOrder).ToList(),
                "LowestSellOrder" => Items.OrderBy(o => o.LowestSellOrder).ToList(),
                _ => Items.OrderBy(o => o.IsTradable).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void SortingDescending(string column)
        {
            List<DataInventory> sort = column switch
            {
                "ItemName" => Items.OrderByDescending(o => o.ItemName).ToList(),
                "HighestBuyOrder" => Items.OrderByDescending(o => o.HighestBuyOrder).ToList(),
                "LowestSellOrder" => Items.OrderByDescending(o => o.LowestSellOrder).ToList(),
                _ => Items.OrderByDescending(o => o.IsTradable).ToList(),
            };
            Items = new(sort);
        }

        [RelayCommand]
        private void SwitchInterval(int index) => Records.SwitchInterval(index);
        [RelayCommand]
        private void BeginInterval(DateTime date) => Records.BeginInterval(date);
        [RelayCommand]
        private void EndInterval(DateTime date) => Records.EndInterval(date);
    }
}
