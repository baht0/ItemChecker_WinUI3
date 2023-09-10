using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.Parser;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels
{
    public partial class ParserViewModel : BaseViewModel<ParserData>
    {
        private ParserTool Tool { get; set; }
        private List<ParserList> CreatedList { get; set; }

        [ObservableProperty]
        private Import importTool = new();

        [ObservableProperty]
        private bool isCanContinue;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddQueueCommand))]
        private ParserParameters parameters = new();
        [ObservableProperty]
        private LoadingBar loadingBar = new();
        private bool IsNotBusy() => !LoadingBar.IsBusy;
        private bool IsEnableQueue() => IsNotBusy() && Parameters.ServiceOneId < 2 && Items.Any();

        public ParserViewModel()
        {
            Info = new()
            {
                { "From:", "-" },
                { "To:", "-" },
                { "Date time:", "-" },
                { "Min. price:", 0 },
                { "Max. price:", 0 },
            };
        }

        [RelayCommand]
        private async void Start()
        {
            LoadingBar = new(true, "Parser", "Preparation...");
            CTSource = new();
            CToken = CTSource.Token;
            IsCanContinue = false;
            CreatedList = await Parameters.CreateList(CToken);

            var info = Parameters.GetInfo();
            Tool = new(Parameters);
            info.Add("Found:", CreatedList.Count);
            Info = info;
            if (CreatedList.Any())
            {
                Items.Clear();
                await CheckList();
            }
            else
            {
                MessageShow("Parser", "Nothing found. Adjust the parameters.", 2);
                LoadingBar = new();
            }
            LoadingBar = new();
        }
        [RelayCommand]
        private void Stop()
        {
            IsCanContinue = true;
            CTSource.Cancel();
            LoadingBar = new("Parser", "Stopping...");
        }
        [RelayCommand]
        private async void Continue()
        {
            CTSource = new();
            CToken = CTSource.Token;
            IsCanContinue = false;
            LoadingBar = new(true, "Parser", "Continue...");
            if (CreatedList.Any(x => !x.IsChecked))
                await CheckList();
            LoadingBar = new();
        }
        private async Task CheckList()
        {
            RemainingTime();
            foreach (var item in CreatedList.Where(x => !x.IsChecked))
            {
                if (CToken.IsCancellationRequested)
                    break;
                try
                {
                    var data = new ParserData(item.ItemName);
                    await data.CheckItem(Parameters.ServiceOneId, Parameters.ServiceTwoId);
                    Items.Add(data);
                    item.IsChecked = true;
                }
                catch (Exception exp)
                {
                    if (exp.Message.Contains("429"))
                        MessageShow("Parser", "HTTP 429 Too Many Requests.", 3);
                    CTSource.Cancel();
                    IsCanContinue = true;
                }
            }
            LoadingBar = new("Completion", "Almost ready...");
            Items = new(Items.OrderByDescending(x => x.Precent));
            await Tool.ExportAsync(Items.ToList());
        }
        private async void RemainingTime()
        {
            var startTime = DateTime.Now;
            while (CreatedList.Where(x => !x.IsChecked).Count() > Items.Count && !CToken.IsCancellationRequested)
            {
                string remaining = await Task.Run(() => Edit.CalcTimeLeft(startTime, CreatedList.Where(x => !x.IsChecked).Count(), Items.Count));
                LoadingBar = new("Remaining:", remaining);
                await Task.Delay(1000);
            }
        }

        [RelayCommand(CanExecute = nameof(IsEnableQueue))]
        private async void AddQueue()
        {
            BuyOrderTool.Queue = Items.Where(x => x.IsChecked).OrderByDescending(x => x.Precent).Select(x => x.ItemName).ToList();
            BuyOrderTool.QueueService = Parameters.ServiceTwoName;
            await BuyOrderTool.ExportQueue();
            MessageShow("Add queue", "Successfully added.", 1);
        }

        [RelayCommand]
        private void OpenIn(object[] obj)
        {
            string itemName = ((ParserData)obj[1]).ItemName;
            switch (obj[0])
            {
                case 1:
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(itemName);
                    Clipboard.SetContent(dataPackage);
                    break;
                case 2:
                    OpenIn(itemName, Parameters.ServiceOneId);
                    break;
                case 3:
                    OpenIn(itemName, Parameters.ServiceTwoId);
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
            List<ParserData> sort = column switch
            {
                "ItemName" => Items.OrderBy(o => o.ItemName).ToList(),
                "Purchase" => Items.OrderBy(o => o.Purchase).ToList(),
                "Price" => Items.OrderBy(o => o.Price).ToList(),
                "Get" => Items.OrderBy(o => o.Get).ToList(),
                "Difference" => Items.OrderBy(o => o.Difference).ToList(),
                _ => Items.OrderBy(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }
        [RelayCommand]
        private void SortingDescending(string column)
        {
            List<ParserData> sort = column switch
            {
                "ItemName" => Items.OrderByDescending(o => o.ItemName).ToList(),
                "Purchase" => Items.OrderByDescending(o => o.Purchase).ToList(),
                "Price" => Items.OrderByDescending(o => o.Price).ToList(),
                "Get" => Items.OrderByDescending(o => o.Get).ToList(),
                "Difference" => Items.OrderByDescending(o => o.Difference).ToList(),
                _ => Items.OrderByDescending(o => o.Precent).ToList(),
            };
            Items = new(sort);
        }

        [RelayCommand]
        private void OpenImport() => ImportTool.GetItems();
        [RelayCommand]
        private async void ImportData()
        {
            Parameters = ImportTool.Selected.Clone();
            Tool = new(Parameters);
            Info = ImportTool.Selected.GetInfo();
            var items = await ImportTool.LoadData();
            Items = new(items);

            MessageShow("Import", "Successfully.", 1);
        }
        [RelayCommand]
        private void ClearList()
        {
            ImportTool.Items.Clear(); ImportTool.Selected = new();
            var path = AppConfig.DocumentPath + "parser";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
