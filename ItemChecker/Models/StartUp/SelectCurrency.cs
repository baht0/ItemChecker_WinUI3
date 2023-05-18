using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Support;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ItemChecker.Models.StartUp
{
    internal partial class SelectCurrency : ObservableObject
    {
        public bool IsCurrencyShow
        {
            get => _isCurrencyShow;
            set => SetProperty(ref _isCurrencyShow, value);
        }
        bool _isCurrencyShow;
        public ObservableCollection<DataCurrency> CurrencyList
        {
            get => _currencyList;
            set => SetProperty(ref _currencyList, value);
        }
        ObservableCollection<DataCurrency> _currencyList = new();
        public DataCurrency SelectedCurrency
        {
            get => _selectedCurrency;
            set => SetProperty(ref _selectedCurrency, value);
        }
        DataCurrency _selectedCurrency = new();

        UserConfig Config { get; set; } = new();
        public async Task MainAsync()
        {
            await Currencies.GetSteamCurrenciesAsync();

            if (Config.SteamCurrencyId == 0)
            {
                CurrencyList = new(Currencies.All);
                SelectedCurrency = Currencies.All.FirstOrDefault();
                IsCurrencyShow = true;

                await Task.Run(() =>
                {
                    while (IsCurrencyShow)
                        Thread.Sleep(100);
                });
            }
            else
                _ = Account.SetCurrencyAsync(Config.SteamCurrencyId);
        }
        public void SubmitCurrency(DataCurrency currency)
        {
            Config.SteamCurrencyId = currency.Id;
            Config.SaveAsync();

            _ = Account.SetCurrencyAsync(Config.SteamCurrencyId);
            IsCurrencyShow = false;
        }
    }
}
