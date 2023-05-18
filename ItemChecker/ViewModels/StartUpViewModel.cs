using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Model.StartUp;
using ItemChecker.Models;
using ItemChecker.Models.StartUp;
using ItemChecker.Support;
using System;
using System.Globalization;
using System.Timers;

namespace ItemChecker.ViewModels
{
    internal partial class StartUpViewModel : ObservableObject
    {
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        bool _isLoading = true;
        public SignIn SignIn
        {
            get => _signIn;
            set => SetProperty(ref _signIn, value);
        }
        SignIn _signIn = new();
        public SelectCurrency Currency
        {
            get => _currency;
            set => SetProperty(ref _currency, value);
        }
        SelectCurrency _currency = new();

        public StartUpViewModel() => StartUpAsync();
        private async void StartUpAsync()
        {
            Timer timer = new(10);
            try
            {
                timer.Elapsed += IsLoadingTimerTick;
                timer.Enabled = true;

                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-Us");
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-Us");

                await AppConfig.AppCheckAsync();
                if (AppConfig.IsUpdate)
                {

                }

                await SignIn.MainAsync();
                await Account.MainAsync();

                SignIn.AccountName = Account.AccountName;
                await Currency.MainAsync();

                await ItemBase.GetItemsBaseAsync();

                var mainWindow = (MainWindow)App.MainWindow;
                mainWindow?.StartUpCompletion();
            }
            catch (Exception exp)
            {
                //BaseModel.ErrorLog(exp, true);
                App.HandleClosedEvents = false;
                var mainWindow = (MainWindow)App.MainWindow;
                mainWindow?.Close();
            }
            finally
            {
                timer.Elapsed -= IsLoadingTimerTick;
                timer.Enabled = false;
            }
        }
        void IsLoadingTimerTick(Object sender, ElapsedEventArgs e)
        {
            var mainWindow = (MainWindow)App.MainWindow;
            mainWindow?.DispatcherQueue?.TryEnqueue(() => IsLoading = !SignIn.IsSignInShow && !Currency.IsCurrencyShow);        
        }

        [RelayCommand]
        public void SignInSubmit(string pass) => SignIn.Submit(pass);
        [RelayCommand]
        public void SubmitCode(string code) => SignIn.SubmitCode(code);
        [RelayCommand]
        public void SubmitCurrency(DataCurrency currency) => Currency.SubmitCurrency(currency);
    }
}
