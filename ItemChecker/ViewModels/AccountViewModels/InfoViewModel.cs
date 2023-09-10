using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models.StaticModels;
using ItemChecker.Properties;
using ItemChecker.Support;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels.AccountViewModels
{
    public partial class InfoViewModel : BaseViewModel<object>
    {
        [ObservableProperty]
        string avatarUrl = SteamAccount.AvatarUrl;
        [ObservableProperty]
        string accountName = SteamAccount.AccountName;
        [ObservableProperty]
        string userName = SteamAccount.UserName;
        [ObservableProperty]
        string apiKey = new('●', SteamAccount.ApiKey.Length);
        [ObservableProperty]
        string id64 = SteamAccount.ID64;
        [ObservableProperty]
        ObservableCollection<DataCurrency> currencies = new(Support.Currencies.All);
        [ObservableProperty]
        DataCurrency currency = SteamAccount.Currency;

        [ObservableProperty]
        bool isActiveBuff = SteamAccount.Buff.IsActive;
        public string ActiveBuff => isActiveBuff ? "Active" : "Inactive";

        partial void OnCurrencyChanged(DataCurrency value)
        {
            AppProperties.Account.SteamCurrencyId = value.Id;
            MessageShow("Currency", "The change will take effect after the application is restarted.", 2);
        }
        partial void OnIsActiveBuffChanged(bool value)
        {
            AppProperties.Account.BUFF = value;
            MessageShow("Active services", "The change will take effect after the application is restarted.", 2);
        }

        [RelayCommand]
        private void ShowProfile() => Edit.OpenUrl("https://steamcommunity.com/profiles/" + Id64);
        [RelayCommand]
        private void SignOut()
        {
            File.Delete(AppConfig.DocumentPath + "user\\cookies.json");
            AppProperties.Account.SteamCurrencyId = 0;
            App.HandleClosedEvents = false;
            var mainWindow = (MainWindow)App.MainWindow;
            mainWindow?.Close();
        }
        [RelayCommand]
        private void ShowApiKey()
        {
            if (ApiKey.Contains('●'))
                ApiKey = SteamAccount.ApiKey;
            else
                ApiKey = new('●', SteamAccount.ApiKey.Length);
        }
        [RelayCommand]
        private void CopyBtn(string name)
        {
            var dataPackage = new DataPackage();
            switch (name)
            {
                case "apiKey":
                    dataPackage.SetText(SteamAccount.ApiKey);
                    MessageShow("Api Key", "Copied.", 1);
                    break;
                case "id64":
                    dataPackage.SetText(SteamAccount.ID64);
                    MessageShow("ID64", "Copied.", 1);
                    break;
            }
            Clipboard.SetContent(dataPackage);
        }
    }
}
