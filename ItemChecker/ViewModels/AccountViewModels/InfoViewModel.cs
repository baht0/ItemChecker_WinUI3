using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models.StaticModels;
using ItemChecker.Properties;
using ItemChecker.Support;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels.AccountViewModels
{
    public partial class InfoViewModel : ObservableObject
    {
        [ObservableProperty]
        string _avatarUrl = SteamAccount.AvatarUrl;
        [ObservableProperty]
        string _accountName = SteamAccount.AccountName;
        [ObservableProperty]
        string _userName = SteamAccount.UserName;
        [ObservableProperty]
        string _apiKey = new('●', SteamAccount.ApiKey.Length);
        [ObservableProperty]
        string _id64 = SteamAccount.ID64;
        [ObservableProperty]
        string _currencyName = SteamAccount.Currency.Name;

        [ObservableProperty]
        bool _isActiveCsm = SteamAccount.Csm.IsActive;
        public string ActiveCsm => _isActiveCsm ? "Active" : "Inactive";
        [ObservableProperty]
        bool _isActiveLfm = SteamAccount.Lfm.IsActive;
        public string ActiveLfm => _isActiveLfm ? "Active" : "Inactive";
        [ObservableProperty]
        bool _isActiveBuff = SteamAccount.Buff.IsActive;
        public string ActiveBuff => _isActiveBuff ? "Active" : "Inactive";

        partial void OnIsActiveCsmChanged(bool value) => AppProperties.Services.CSM = value;
        partial void OnIsActiveLfmChanged(bool value) => AppProperties.Services.LFM = value;
        partial void OnIsActiveBuffChanged(bool value) => AppProperties.Services.BUFF = value;

        [RelayCommand]
        private void ShowProfile() => Edit.OpenUrl("https://steamcommunity.com/profiles/" + Id64);
        [RelayCommand]
        private void SignOut()
        {
            AppProperties.User.SteamCurrencyId = 0;
            App.HandleClosedEvents = false;
            var mainWindow = (MainWindow)App.MainWindow;
            mainWindow?.Close();
        }
        [RelayCommand]
        private void ShowApiKey() => ApiKey = SteamAccount.ApiKey;
        [RelayCommand]
        private void CopyBtn(string name)
        {
            var dataPackage = new DataPackage();
            switch (name)
            {
                case "apiKey":
                    dataPackage.SetText(SteamAccount.ApiKey);
                    break;
                case "id64":
                    dataPackage.SetText(SteamAccount.ID64);
                    break;
            }
            Clipboard.SetContent(dataPackage);
        }
    }
}
