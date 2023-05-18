using System;
using System.Linq;
using Windows.Storage;

namespace ItemChecker.Properties
{
    internal class Default
    {
        public static ActiveServices ActiveServices { get; set; }
        public Default()
        {
        }
    }
    public class ActiveServices
    {
        public bool CSM { get; set; } = true;
        public bool LFM { get; set; } = true;
        public bool BUFF { get; set; } = true;

        public ActiveServices()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var composite = new ApplicationDataCompositeValue
            {
                [nameof(CSM)] = CSM,
                [nameof(LFM)] = LFM,
                [nameof(BUFF)] = BUFF,
            };
            if (localSettings.Values.)
                localSettings.Values[nameof(ActiveServices)] = composite;
        }
    }
}
