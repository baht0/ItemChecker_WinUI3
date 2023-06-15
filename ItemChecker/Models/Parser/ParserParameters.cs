using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Support;
using System;
using System.Collections.Generic;

namespace ItemChecker.Models.Parser
{
    public partial class ParserParameters : ObservableObject
    {
        public List<string> Services => BaseConfig.Services;
        [ObservableProperty]
        private int serviceOneId;
        public int ServiceTwoId { get; set; } = 0;
        private string ServiceOneName => Services[ServiceOneId];
        private string ServiceTwoName => Services[ServiceTwoId];
        public double MinPrice { get; set; } = 0;
        [ObservableProperty]
        private double maxPrice;
        [ObservableProperty]
        private bool all;
        [ObservableProperty]
        private bool notWeapon;
        [ObservableProperty]
        private bool normal;
        [ObservableProperty]
        private bool souvenir;
        [ObservableProperty]
        private bool unique;
        [ObservableProperty]
        private bool stattrak;
        [ObservableProperty]
        private bool uniqueStattrak;

        partial void OnServiceOneIdChanged(int value)
        {
            All = false;
            switch (value)
            {
                case 0 or 1:
                    MaxPrice = Currencies.ConverterToUsd(SteamAccount.Balance, SteamAccount.Currency.Id);
                    break;
                case 2:
                    MaxPrice = SteamAccount.Csm.Balance;
                    break;
                case 3:
                    MaxPrice = SteamAccount.Lfm.Balance;
                    break;
                case 4 or 5:
                    MaxPrice = SteamAccount.Buff.Balance;
                    All = true;
                    NotWeapon = false;
                    Normal = false;
                    Souvenir = false;
                    Stattrak = false;
                    Unique = false;
                    UniqueStattrak = false;
                    break;
            }
        }

        public ParserParameters Clone() => this.MemberwiseClone() as ParserParameters;
        public Dictionary<string, object> GetInfo() => new()
            {
                { "From:", ServiceOneName },
                { "To:", ServiceTwoName },
                { "Date time:", DateTime.Now },
                { "Min. price:", MinPrice },
                { "Max. price:", MaxPrice },
            };
    }
}
