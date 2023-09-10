using Microsoft.UI.Composition.SystemBackdrops;
using Windows.Storage;

namespace ItemChecker.Properties
{
    internal class AppProperties
    {
        public static AccountProperties Account { get; set; } = new();
        public static SettingsProperties Settings { get; set; } = new();
        public static RareProperties Rare { get; set; } = new();

        public AppProperties()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();
        }
    }

    public class AccountProperties
    {
        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        readonly ApplicationDataCompositeValue Composite = new();

        public int SteamCurrencyId
        {
            get
            {
                if (Composite[nameof(SteamCurrencyId)] != null) //load saved
                    steamCurrencyId = (int)Composite[nameof(SteamCurrencyId)];
                return steamCurrencyId;
            }
            set
            {
                steamCurrencyId = value;
                Composite[nameof(SteamCurrencyId)] = value;
                LocalSettings.Values[nameof(AccountProperties)] = Composite;
            }
        }
        int steamCurrencyId;

        public bool BUFF
        {
            get
            {
                if (Composite[nameof(BUFF)] != null) //load saved
                    buff = (bool)Composite[nameof(BUFF)];
                return buff;
            }
            set
            {
                buff = value;
                Composite[nameof(BUFF)] = value;
                LocalSettings.Values[nameof(AccountProperties)] = Composite;
            }
        }
        bool buff = true;

        public AccountProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(AccountProperties)];
            Composite ??= new();
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
                    maxFloatFN = (double)Composite[nameof(MaxFloatFN)];
                return maxFloatFN;
            }
            set
            {
                maxFloatFN = value;
                Composite[nameof(MaxFloatFN)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double maxFloatFN = 0.001d;
        public double MaxFloatMW
        {
            get
            {
                if (Composite[nameof(MaxFloatMW)] != null) //load saved
                    maxFloatMW = (double)Composite[nameof(MaxFloatMW)];
                return maxFloatMW;
            }
            set
            {
                maxFloatMW = value;
                Composite[nameof(MaxFloatMW)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double maxFloatMW = 0.080d;
        public double MaxFloatFT
        {
            get
            {
                if (Composite[nameof(MaxFloatFT)] != null) //load saved
                    maxFloatFT = (double)Composite[nameof(MaxFloatFT)];
                return maxFloatFT;
            }
            set
            {
                maxFloatFT = value;
                Composite[nameof(MaxFloatFT)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double maxFloatFT = 0.175d;
        public double MaxFloatWW
        {
            get
            {
                if (Composite[nameof(MaxFloatWW)] != null) //load saved
                    maxFloatWW = (double)Composite[nameof(MaxFloatWW)];
                return maxFloatWW;
            }
            set
            {
                maxFloatWW = value;
                Composite[nameof(MaxFloatWW)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double maxFloatWW = 0.400d;
        public double MaxFloatBS
        {
            get
            {
                if (Composite[nameof(MaxFloatBS)] != null) //load saved
                    maxFloatBS = (double)Composite[nameof(MaxFloatBS)];
                return maxFloatBS;
            }
            set
            {
                maxFloatBS = value;
                Composite[nameof(MaxFloatBS)] = value;
                LocalSettings.Values[nameof(RareProperties)] = Composite;
            }
        }
        double maxFloatBS = 0.500d;

        public RareProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(RareProperties)];
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
                    themeId = (int)Composite[nameof(ThemeId)];
                return themeId;
            }
            set
            {
                themeId = value;
                Composite[nameof(ThemeId)] = value;
                LocalSettings.Values[nameof(SettingsProperties)] = Composite;
            }
        }
        int themeId;
        public bool IsMicaTheme
        {
            get
            {
                if (Composite[nameof(IsMicaTheme)] != null) //load saved
                    isMicaTheme = (bool)Composite[nameof(IsMicaTheme)];
                return isMicaTheme;
            }
            set
            {
                isMicaTheme = value;
                Composite[nameof(IsMicaTheme)] = value;
                LocalSettings.Values[nameof(SettingsProperties)] = Composite;
            }
        }
        bool isMicaTheme = MicaController.IsSupported();

        public SettingsProperties()
        {
            Composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(SettingsProperties)];
            Composite ??= new();
        }
    }
}
