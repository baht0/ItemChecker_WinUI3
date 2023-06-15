using Microsoft.UI.Composition.SystemBackdrops;
using Windows.Storage;

namespace ItemChecker.Properties
{
    internal class AppProperties
    {
        public static UserProperties User { get; set; } = new();
        public static SettingsProperties Settings { get; set; } = new();
        public static ServicesProperties Services { get; set; } = new();
        public static RareProperties Rare { get; set; } = new();

        public AppProperties()
        {
            //ApplicationData.Current.LocalSettings.Values.Clear();
        }
    }

    public class RareProperties
    {
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly ApplicationDataCompositeValue Composite = new();

        public double MaxFloatFN
        {
            get
            {
                if (Composite[nameof(MaxFloatFN)] != null) //load saved
                    _maxFloatFN = (double)Composite[nameof(MaxFloatFN)];
                return _maxFloatFN;
            }
            set
            {
                _maxFloatFN = value;
                Composite[nameof(MaxFloatFN)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double _maxFloatFN = 0.001d;
        public double MaxFloatMW
        {
            get
            {
                if (Composite[nameof(MaxFloatMW)] != null) //load saved
                    _maxFloatMW = (double)Composite[nameof(MaxFloatMW)];
                return _maxFloatMW;
            }
            set
            {
                _maxFloatMW = value;
                Composite[nameof(MaxFloatMW)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double _maxFloatMW = 0.080d;
        public double MaxFloatFT
        {
            get
            {
                if (Composite[nameof(MaxFloatFT)] != null) //load saved
                    _maxFloatFT = (double)Composite[nameof(MaxFloatFT)];
                return _maxFloatFT;
            }
            set
            {
                _maxFloatFT = value;
                Composite[nameof(MaxFloatFT)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double _maxFloatFT = 0.175d;
        public double MaxFloatWW
        {
            get
            {
                if (Composite[nameof(MaxFloatWW)] != null) //load saved
                    _maxFloatWW = (double)Composite[nameof(MaxFloatWW)];
                return _maxFloatWW;
            }
            set
            {
                _maxFloatWW = value;
                Composite[nameof(MaxFloatWW)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double _maxFloatWW = 0.400d;
        public double MaxFloatBS
        {
            get
            {
                if (Composite[nameof(MaxFloatBS)] != null) //load saved
                    _maxFloatBS = (double)Composite[nameof(MaxFloatBS)];
                return _maxFloatBS;
            }
            set
            {
                _maxFloatBS = value;
                Composite[nameof(MaxFloatBS)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double _maxFloatBS = 0.500d;

        public RareProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(RareProperties)];
            Composite ??= new();
        }
    }
    public class ServicesProperties
    {
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly ApplicationDataCompositeValue Composite = new();

        public bool CSM
        {
            get
            {
                if (Composite[nameof(CSM)] != null) //load saved
                    _csm = (bool)Composite[nameof(CSM)];
                return _csm;
            }
            set
            {
                _csm = value;
                Composite[nameof(CSM)] = value;
                LocalSettings.Values[nameof(ServicesProperties)] = Composite;
            }
        }
        bool _csm = true;
        public bool LFM
        {
            get
            {
                if (Composite[nameof(LFM)] != null) //load saved
                    _lfm = (bool)Composite[nameof(LFM)];
                return _lfm;
            }
            set
            {
                _lfm = value;
                Composite[nameof(LFM)] = value;
                LocalSettings.Values[nameof(ServicesProperties)] = Composite;
            }
        }
        bool _lfm = true;
        public bool BUFF
        {
            get
            {
                if (Composite[nameof(BUFF)] != null) //load saved
                    _buff = (bool)Composite[nameof(BUFF)];
                return _buff;
            }
            set
            {
                _buff = value;
                Composite[nameof(BUFF)] = value;
                LocalSettings.Values[nameof(ServicesProperties)] = Composite;
            }
        }
        bool _buff = true;

        public ServicesProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(ServicesProperties)];
            Composite ??= new();
        }
    }
    public class SettingsProperties
    {
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly ApplicationDataCompositeValue Composite = new();

        public int ThemeId
        {
            get
            {
                if (Composite[nameof(ThemeId)] != null) //load saved
                    _themeId = (int)Composite[nameof(ThemeId)];
                return _themeId;
            }
            set
            {
                _themeId = value;
                Composite[nameof(ThemeId)] = value;
                LocalSettings.Values[nameof(SettingsProperties)] = Composite;
            }
        }
        int _themeId;
        public bool IsMicaTheme
        {
            get
            {
                if (Composite[nameof(IsMicaTheme)] != null) //load saved
                    _isMicaTheme = (bool)Composite[nameof(IsMicaTheme)];
                return _isMicaTheme;
            }
            set
            {
                _isMicaTheme = value;
                Composite[nameof(IsMicaTheme)] = value;
                LocalSettings.Values[nameof(SettingsProperties)] = Composite;
            }
        }
        bool _isMicaTheme = MicaController.IsSupported();

        public SettingsProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(SettingsProperties)];
            Composite ??= new();
        }
    }
    public class UserProperties
    {
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly ApplicationDataCompositeValue Composite = new();

        public int SteamCurrencyId
        {
            get
            {
                if (Composite[nameof(SteamCurrencyId)] != null) //load saved
                    _steamCurrencyId = (int)Composite[nameof(SteamCurrencyId)];
                return _steamCurrencyId;
            }
            set
            {
                _steamCurrencyId = value;
                Composite[nameof(SteamCurrencyId)] = value;
                LocalSettings.Values[nameof(UserProperties)] = Composite;
            }
        }
        int _steamCurrencyId;

        public UserProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(UserProperties)];
            Composite ??= new();
        }
    }
}
