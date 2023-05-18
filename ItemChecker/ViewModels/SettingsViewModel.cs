using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Composition.SystemBackdrops;

namespace ItemChecker.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        public bool IsMicaSupported
        {
            get => _isMicaSupported;
            set => SetProperty(ref _isMicaSupported, value);
        }
        bool _isMicaSupported = MicaController.IsSupported();
        public bool IsMicaTheme
        {
            get => _isMicaTheme;
            set => SetProperty(ref _isMicaTheme, value);
        }
        bool _isMicaTheme = true;
    }
}
