using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;

namespace ItemChecker.ViewModels
{
    public partial class CalculatorViewModel : ObservableObject
    {
        public CalculatorViewModel() => Commission = SteamAccount.Inventory.Commission;

        public List<string> Services
        {
            get
            {
                var services = BaseConfig.ServicesShort;
                services.Add("Custom");
                return services;
            }
        }
        [ObservableProperty]
        int serviceId;

        [ObservableProperty]
        double commission;
        [ObservableProperty]
        double purchase;
        [ObservableProperty]
        double price;
        [ObservableProperty]
        double get;
        [ObservableProperty]
        double precent;
        [ObservableProperty]
        double difference;

        partial void OnServiceIdChanged(int value)
        {
            Commission = ServiceId switch
            {
                0 => SteamAccount.Inventory.Commission,
                1 => SteamAccount.Csm.Commission,
                2 => SteamAccount.Lfm.Commission,
                3 => SteamAccount.Buff.Commission,
                _ => 0,
            };
        }
        partial void OnCommissionChanged(double value)
        {
            if (value < 1)
                commission = value;
            else
                commission = (100 - value) / 100;
            Compare();
        }
        partial void OnPurchaseChanged(double value) => Compare();
        partial void OnPriceChanged(double value) => Compare();
        private void Compare()
        {
            Get = Math.Round(Price * Commission, 2);
            Difference = Edit.Difference(Get, Purchase);
            Precent = Edit.Precent(Purchase, Get);
        }

        //currency
        public List<DataCurrency> CurrencyList => Currencies.Allow;
        [ObservableProperty]
        int currency1;
        [ObservableProperty]
        int currency2;

        public double Value
        {
            get => value;
            set
            {
                SetProperty(ref value, value);

                double dol = Currency1 != 0 ? Currencies.ConverterToUsd(value, CurrencyList[Currency1].Id) : Value;
                Converted = Currency2 != 0 ? Currencies.ConverterFromUsd(dol, CurrencyList[Currency2].Id) : dol;
            }
        }
        double value;
        [ObservableProperty]
        double converted;

        [RelayCommand]
        private void Switch()
        {
            int c1 = Currency1;
            int c2 = Currency2;
            Currency1 = c2;
            Currency2 = c1;
            Value = 0;
            Converted = 0;
        }
    }
}
