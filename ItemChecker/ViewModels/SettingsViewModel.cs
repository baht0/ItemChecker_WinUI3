using ItemChecker.Core;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChecker.ViewModels
{
    internal class SettingsViewModel : ObservableObject
    {
        public bool MicaTheme
        {
            get
            { 
                return _isMicaSupported;
            }
            set
            {
                _micaTheme = value;
                OnPropertyChanged();
            }
        }
        bool _micaTheme = true;
        public bool IsMicaSupported
        {
            get
            {
                return _isMicaSupported;
            }
            set
            {
                _isMicaSupported = value;
                OnPropertyChanged();
            }
        }
        bool _isMicaSupported = MicaController.IsSupported();
    }
}
