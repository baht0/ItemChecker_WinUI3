using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.Settings;
using ItemChecker.Models.StaticModels;
using ItemChecker.Properties;
using Microsoft.UI.Composition.SystemBackdrops;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItemChecker.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        int _themeId = AppProperties.Settings.ThemeId;
        [ObservableProperty]
        bool _isMicaTheme = AppProperties.Settings.IsMicaTheme;
        public bool IsMicaSupported => MicaController.IsSupported();
        [ObservableProperty]
        ObservableCollection<Note> _notes = new();
        [ObservableProperty]
        Note _selected = new();
        public string Version => AppConfig.CurrentVersion;

        partial void OnThemeIdChanged(int value) => AppProperties.Settings.ThemeId = value;
        partial void OnIsMicaThemeChanged(bool value) => AppProperties.Settings.IsMicaTheme = value;

        public SettingsViewModel() => ViewModelLoaded();
        private async void ViewModelLoaded()
        {
            var notes = await PatchNotes.GetPatchNotes();
            Notes = new(notes);
            Selected = notes.FirstOrDefault();
        }
    }
}
