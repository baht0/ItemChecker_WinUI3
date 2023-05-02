using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Text;

namespace ItemChecker.Views.ItemBase
{
    public sealed partial class HomePage : Page
    {
        ObservableCollection<string> appBaseList = new();
        ObservableCollection<string> itemsList = new();
        public HomePage()
        {
            this.InitializeComponent();

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


        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }
        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

        private void addAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarToggleButton;
            listSuggest.Text = string.Empty;
            switch (btn.IsChecked)
            {
                case true:
                    listSuggest.PlaceholderText = "Add an item from the database to the list";
                    serviceComboBox.Visibility = Visibility.Visible;
                    listSuggest.ItemsSource = appBaseList;
                    listSuggest.TextChanged -= listSuggestSearch_TextChanged;
                    listSuggest.TextChanged += listSuggestAdd_TextChanged;
                    listSuggest.QuerySubmitted += listSuggest_QuerySubmitted;
                    listSuggest.SuggestionChosen += listSuggest_SuggestionChosen;
                    break;
                case false:
                    listSuggest.PlaceholderText = "Search for items in the list";
                    serviceComboBox.Visibility = Visibility.Collapsed;
                    listSuggest.ItemsSource = null;
                    listSuggest.TextChanged -= listSuggestAdd_TextChanged;
                    listSuggest.TextChanged += listSuggestSearch_TextChanged;
                    listSuggest.QuerySubmitted -= listSuggest_QuerySubmitted;
                    listSuggest.SuggestionChosen -= listSuggest_SuggestionChosen;
                    break;
            }
        }
        private void listSuggestSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                var results = itemsList.Where(i => i.ToLower().Contains(term)).ToList();
                itemsListView.ItemsSource = results;
            }
        }
        private void listSuggestAdd_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = sender.Text.ToLower();
                var results = appBaseList.Where(i => i.ToLower().Contains(term)).ToList();
                listSuggest.ItemsSource = results;
                listSuggest.IsSuggestionListOpen = true;
            }
        }
        private void listSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var term = args.QueryText.ToLower();
            var results = appBaseList.Where(i => i.ToLower().Contains(term)).ToList();
            listSuggest.ItemsSource = results;
            listSuggest.IsSuggestionListOpen = true;
        }
        private void listSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var itemName = args.SelectedItem as string;
            itemsList.Add(itemName);
            itemsListView.ItemsSource = itemsList;
        }
    }
}
