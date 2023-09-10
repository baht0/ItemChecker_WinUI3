using ItemChecker.Models.StaticModels;
using System.Collections.Generic;
using ItemChecker.Models;
using ItemChecker.Models.Rare;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Support;

namespace ItemChecker.ViewModels
{
    public partial class RareViewModel : BaseViewModel<DataRare>
    {
        private RareTool Tool { get; set; } = new();
        public RareViewModel()
        {
            Info = new()
            {
                { "Parameter:", "-" },
                { "Compare price:", "-" },
                { "Min. precent:", 0 },
                { "Timer (min.):", 0 }
            };
        }

        public string CurrencyTable => SteamAccount.Currency.Name;
        [ObservableProperty]
        RareParameters parameters = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FilterCommand))]
        LoadingBar loadingBar = new();
        public bool IsNotBusy() => !LoadingBar.IsBusy;

        [RelayCommand]
        private void Start()
        {
            LoadingBar = new(true, "Check", "Starting...");
            Info = Parameters.GetInfo();
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
                if (!RareItems.Items.Any(x => x.Service == Parameters.ParameterName))
                {
                    MessageShow(Parameters.ParameterName, "The list does not contain items of this type.", 3);
                    LoadingBar = new();
                    break;
                }
                await Task.Delay(1000);
            }
            if (Secounds <= 0 && LoadingBar.IsBusy)
                CheckList();
        }
        private async void CheckList()
        {
            try
            {
                LoadingBar = new("Checking:", "0/0 items.");
                var list = RareItems.Items.Where(x => x.Service == Parameters.ParameterName).ToList();
                foreach (var item in list)
                {
                    try
                    {
                        LoadingBar = new("Checking:", $"{list.IndexOf(item) + 1}/{list.Count} items.");
                        var checkedItem = await Tool.CheckItemAsync(item.ItemName, Parameters.Clone());
                        InitialItems.AddRange(checkedItem);
                    }
                    catch
                    {
                        continue;
                    }
                    if (CToken.IsCancellationRequested)
                        break;
                }
                Items = new(InitialItems.OrderByDescending(o => o.Precent));
                CheckTimer();
            }
            catch (Exception exp)
            {
                LoadingBar = new();
                Debug.WriteLine(exp.Message);
            }
        }
        [RelayCommand]
        private void OpenIn(int id)
        {
            string market_hash_name = Uri.EscapeDataString(Items[id].ItemName);
            Edit.OpenUrl("https://steamcommunity.com/market/listings/730/" + market_hash_name);
        }
        [RelayCommand]
        private void SortingAscending(string column)
        {
            List<DataRare> sort = column switch
            {
                "ItemName" => Items.OrderBy(o => o.ItemName).ToList(),
                "Float" => Items.OrderBy(o => o.FloatValue).ToList(),
                "Stickers" => Items.OrderBy(o => o.Stickers.Count).ToList(),
                "Phase" => Items.OrderBy(o => o.Phase).ToList(),
                "Price" => Items.OrderBy(o => o.Price).ToList(),
                _ => Items.OrderBy(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void SortingDescending(string column)
        {
            List<DataRare> sort = column switch
            {
                "ItemName" => Items.OrderByDescending(o => o.ItemName).ToList(),
                "Float" => Items.OrderByDescending(o => o.FloatValue).ToList(),
                "Stickers" => Items.OrderByDescending(o => o.Stickers.Count).ToList(),
                "Phase" => Items.OrderByDescending(o => o.Phase).ToList(),
                "Price" => Items.OrderByDescending(o => o.Price).ToList(),
                _ => Items.OrderByDescending(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void Filter(KeyValuePair<string, bool> pair)
        {
            var filter = pair.Key switch
            {
                "normal" => Items.Where(x => !x.ItemName.Contains("Souvenir") && !x.ItemName.Contains("StatTrak™") && !x.ItemName.Contains("★")).ToList(),
                "souvenir" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "stattrak" => Items.Where(x => x.ItemName.Contains("StatTrak™")).ToList(),
                "unique" => Items.Where(x => x.ItemName.Contains("★")).ToList(),
                "unique_stattrak" => Items.Where(x => x.ItemName.Contains("★ StatTrak™")).ToList(),

                "not_painted" => Items.Where(x => !x.ItemName.Contains("Battle-Scarred") && !x.ItemName.Contains("Well-Worn") && !x.ItemName.Contains("Field-Tested") && !x.ItemName.Contains("Minimal Wear") && !x.ItemName.Contains("Factory New")).ToList(),
                "factory_new" => Items.Where(x => x.ItemName.Contains("Factory New")).ToList(),
                "minimal_wear" => Items.Where(x => x.ItemName.Contains("Minimal Wear")).ToList(),
                "field_tested" => Items.Where(x => x.ItemName.Contains("Field-Tested")).ToList(),
                "well_worn" => Items.Where(x => x.ItemName.Contains("Well-Worn")).ToList(),
                "battle_scarred" => Items.Where(x => x.ItemName.Contains("Battle-Scarred")).ToList(),

                "normal_sticker" => Items.Where(x => x.ItemName == "").ToList(),
                "holo_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "glitter_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "foil_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "lenticular_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "gold_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),
                "contraband_sticker" => Items.Where(x => x.ItemName.Contains("Souvenir")).ToList(),

                _ => InitialItems,
            };
            Items = new(filter);
        }

        #region Items
        [ObservableProperty]
        RareItems rareItems = new();
        [RelayCommand]
        private void AddToList(object obj)
        {
            string itemName = ((Item)obj).ItemName;
            RareItems.Add(itemName);
            MessageShow("Items", "Successfully added.", 1);
        }
        [RelayCommand]
        private void DeleteItem(object obj)
        {
            var item = obj as DataRareItem;
            RareItems.Delete(item);
        }
        [RelayCommand]
        private void ClearList() => RareItems.Clear();
        #endregion
    }
}
