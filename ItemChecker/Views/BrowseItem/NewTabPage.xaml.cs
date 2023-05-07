using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace ItemChecker.Views.BrowseItem
{
    public sealed partial class NewTabPage : Page
    {
        BrowseItemPage BrowsePage;
        bool IsSuggestionChosen = false;
        ObservableCollection<string> appBaseList = new();
        public NewTabPage(object page)
        {
            this.InitializeComponent();

            BrowsePage = page as BrowseItemPage;

            appBaseList.Add($"AK-47 | Neon Revolution (Field-Tested)");
            appBaseList.Add($"AK-47 | Slate (Field-Tested)");
            appBaseList.Add($"M4A1-S | Leaded Glass (Field-Tested)");
            appBaseList.Add($"★ StatTrak™ Bowie Knife | Case Hardened (Battle-Scarred)");
            appBaseList.Add($"AWP | Atheris (Field-Tested)");
            appBaseList.Add($"Souvenir Dual Berettas | Drift Wood (Field-Tested)");
            appBaseList.Add($"Antwerp 2022 Challengers Autograph Capsule");
            appBaseList.Add($"Sticker | TYLOO (Holo) | 2020 RMR");
            appBaseList.Add($"★ Driver Gloves | Black Tie (Field-Tested)");
            appBaseList.Add($"M4A4 | Tooth Fairy (Field-Tested)");
            appBaseList.Add($"Shattered Web Case");
            appBaseList.Add($"M4A1-S | Decimator (Field-Tested)");
        }

        private void searchSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent() && !IsSuggestionChosen)
            {
                var term = sender.Text.ToLower();
                var results = appBaseList.Where(i => i.ToLower().Contains(term)).ToList();
                searchSuggest.ItemsSource = results;
                searchSuggest.IsSuggestionListOpen = true;
            }
        }
        private void searchSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!IsSuggestionChosen)
            {
                var term = args.QueryText.ToLower();
                var results = appBaseList.Where(i => i.ToLower().Contains(term)).ToList();
                searchSuggest.ItemsSource = results;
                searchSuggest.IsSuggestionListOpen = true;
            }
        }
        private void searchSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            IsSuggestionChosen = true;
            BrowsePage.OpenItem(args.SelectedItem as string);
        }
    }
}
