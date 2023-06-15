using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using ItemChecker.Properties;
using System.Collections.Generic;

namespace ItemChecker.Models.Rare
{
    public partial class RareParameters : ObservableObject
    {
        public List<string> Parameters => BaseConfig.RareParameters;
        public int ParameterId { get; set; } = 0;
        public string ParameterName => Parameters[ParameterId];
        public List<string> ComparePrices => new() { "Lowest", "Median" };
        public int CompareId { get; set; } = 0;
        public string CompareName => ComparePrices[CompareId];
        public int Time { get; set; } = 10;
        public int MinPrecent { get; set; } = 7;

        //float
        [ObservableProperty]
        double _factoryNew = AppProperties.Rare.MaxFloatFN;
        [ObservableProperty]
        double _minimalWear = AppProperties.Rare.MaxFloatMW;
        [ObservableProperty]
        double _fieldTested = AppProperties.Rare.MaxFloatFT;
        [ObservableProperty]
        double _wellWorn = AppProperties.Rare.MaxFloatWW;
        [ObservableProperty]
        double _battleScarred = AppProperties.Rare.MaxFloatBS;

        partial void OnFactoryNewChanged(double value) => AppProperties.Rare.MaxFloatFN = value;
        partial void OnMinimalWearChanged(double value) => AppProperties.Rare.MaxFloatMW = value;
        partial void OnFieldTestedChanged(double value) => AppProperties.Rare.MaxFloatFT = value;
        partial void OnWellWornChanged(double value) => AppProperties.Rare.MaxFloatWW = value;
        partial void OnBattleScarredChanged(double value) => AppProperties.Rare.MaxFloatBS = value;

        //stickers
        public int MinSticker { get; set; } = 1;
        public string NameContains { get; set; } = string.Empty;
        public bool Normal { get; set; }
        public bool Holo { get; set; }
        public bool Glitter { get; set; }
        public bool Foil { get; set; }
        public bool Gold { get; set; }
        public bool Lenticular { get; set; }
        public bool Contraband { get; set; }

        //dopplers
        public bool Phase1 { get; set; }
        public bool Phase2 { get; set; }
        public bool Phase3 { get; set; }
        public bool Phase4 { get; set; }
        public bool Ruby { get; set; }
        public bool Sapphire { get; set; }
        public bool BlackPearl { get; set; }
        public bool Emerald { get; set; }

        public RareParameters Clone() => this.MemberwiseClone() as RareParameters;
        public Dictionary<string, object> GetInfo() => new()
            {
                { "Parameter:", ParameterName },
                { "Compare price:", CompareName },
                { "Min. precent:", MinPrecent },
                { "Timer (min.):", Time }
            };
    }
}
