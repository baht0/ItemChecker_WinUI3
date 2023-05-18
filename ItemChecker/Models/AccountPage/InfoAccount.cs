using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Windows.Storage;

namespace ItemChecker.Models.AccountPage
{
    public class InfoAccount : ObservableObject
    {
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }
        string _avatarUrl = Account.AvatarUrl;
        public string AccountName
        {
            get => _accountName;
            set => SetProperty(ref _accountName, value);
        }
        string _accountName = Account.AccountName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }
        string _userName = Account.UserName;
        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }
        string _apiKey = new('●', Account.ApiKey.Length);
        public string ID64
        {
            get => _id64;
            set => SetProperty(ref _id64, value);
        }
        string _id64 = Account.ID64;
        public string CurrencyName
        {
            get => _currencyName;
            set => SetProperty(ref _currencyName, value);
        }
        string _currencyName = Account.Currency.Name;

        public bool IsActiveCsm
        {
            get => _isActiveCsm;
            set
            {
                SaveActiveServices(value, "CSM");
                SetProperty(ref _isActiveCsm, value);
            }
        }
        bool _isActiveCsm = Account.Csm.IsActive;
        public bool IsActiveLfm
        {
            get => _isActiveLfm;
            set
            {
                SetProperty(ref _isActiveLfm, value);
                SaveActiveServices(value, "LFM");
            }
        }
        bool _isActiveLfm = Account.Lfm.IsActive;
        public bool IsActiveBuff
        {
            get => _isActiveBuff;
            set
            {
                SetProperty(ref _isActiveBuff, value);
                SaveActiveServices(value, "BUFF");
            }
        }
        bool _isActiveBuff = Account.Buff.IsActive;

        private void SaveActiveServices(bool value, string name)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var composite = new ApplicationDataCompositeValue
            {
                [name] = value
            };
            localSettings.Values["ActiveServices"] = composite;
        }
    }
}
