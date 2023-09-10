using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.DatabaseItem;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels.DatabaseViewModels
{
    public partial class ItemViewModel : BaseViewModel<DataService>
    {
        #region ObservableProperty
        [ObservableProperty]
        Item item;

        [ObservableProperty]
        DataService selected = new();

        [ObservableProperty]
        History history;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        LoadingBar loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;

        async partial void OnSelectedChanged(DataService value)
        {
            if (value != null && value != new DataService())
            {
                History = new(Item);
                await History.GetSaleHistory(value.ServiceId);
            }
        }
        #endregion

        public ItemViewModel(string itemName)
        {
            Item = ItemBase.List.FirstOrDefault(x => x.ItemName == itemName);
            Load();
        }
        private async void Load()
        {
            try
            {
                LoadingBar = new(true, "Data", "Loading...");
                await Item.UpdateSteamItem();
                await Item.UpdateCsmItem(true);
                await ItemBase.UpdateLfm();
                await Item.UpdateBuffItem();

                foreach (var serv in BaseConfig.Services)
                {
                    var service = new DataService();
                    await Task.Run(() => service.UpdateData(Item, serv));
                    Items.Add(service);
                }
                foreach (var serv in Items)
                    serv.Compare.DataServices = Items.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                LoadingBar = new();
            }
        }

        [RelayCommand]
        private void CopyName()
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Item.ItemName);
            Clipboard.SetContent(dataPackage);
            MessageShow("Item name", "Copied.", 1);
        }
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private void Refresh()
        {
            Items = new();
            Selected = new();
            Load();
        }
        [RelayCommand]
        private void OpenIn(int id)
        {
            string itemName = Item.ItemName.Replace("(Holo/Foil)", "(Holo-Foil)");
            string market_hash_name = Uri.EscapeDataString(itemName);
            switch (id)
            {
                case 0 or 1:
                    Edit.OpenUrl("https://steamcommunity.com/market/listings/730/" + market_hash_name);
                    break;
                case 2:
                    Edit.OpenCsm(itemName);
                    break;
                case 3:
                    Edit.OpenUrl("https://loot.farm/#skin=730_" + market_hash_name);
                    break;
                case 4:
                    Edit.OpenUrl("https://buff.163.com/goods/" + Item.Buff.Id + "#tab=buying");
                    break;
                case 5:
                    Edit.OpenUrl("https://buff.163.com/goods/" + Item.Buff.Id);
                    break;
            }
        }
    }
}