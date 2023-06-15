using System.Linq;
using Microsoft.UI.Xaml.Controls;
using ItemChecker.ViewModels.DatabaseViewModels;
using ItemChecker.Models.StaticModels;

namespace ItemChecker.Views.Database
{
    public sealed partial class NewTabPage : Page
    {
        readonly static NewTabViewModel ViewModel = new();
        readonly DatabasePage ParentPage;
        bool IsSuggestionChosen = false;
        public NewTabPage(object page)
        {
            this.InitializeComponent();
            ParentPage = page as DatabasePage;
            DataContext = ViewModel;
        }

        private void searchSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent() && !IsSuggestionChosen)
            {
                var term = sender.Text.ToLower();
                var results = ViewModel.Base.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                searchSuggest.ItemsSource = results;
                searchSuggest.IsSuggestionListOpen = true;
            }
        }
        private void searchSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!IsSuggestionChosen)
            {
                var term = args.QueryText.ToLower();
                var results = ViewModel.Base.Where(i => i.ItemName.ToLower().Contains(term)).ToList();
                searchSuggest.ItemsSource = results;
                searchSuggest.IsSuggestionListOpen = true;
            }
        }
        private void searchSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            IsSuggestionChosen = true;
            var item = args.SelectedItem as Item;
            ParentPage.OpenItem(item?.ItemName);
        }
    }
}
