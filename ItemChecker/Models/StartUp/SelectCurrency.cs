using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Properties;
using ItemChecker.Support;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ItemChecker.Models.StartUp
{
    internal partial class SelectCurrency : ObservableObject
    {
        [ObservableProperty]
        bool _isCurrencyShow;
        [ObservableProperty]
        ObservableCollection<DataCurrency> _currencyList = new();
        [ObservableProperty]
        DataCurrency _selectedCurrency = new();

        public async Task MainAsync()
        {
            await Currencies.GetSteamCurrenciesAsync();

            if (AppProperties.User.SteamCurrencyId == 0)
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
                _ = SteamAccount.SetCurrencyAsync(AppProperties.User.SteamCurrencyId);
        }
        public void SubmitCurrency(DataCurrency currency)
        {
            AppProperties.User.SteamCurrencyId = currency.Id;
            _ = SteamAccount.SetCurrencyAsync(AppProperties.User.SteamCurrencyId);
            IsCurrencyShow = false;
        }
    }
}
