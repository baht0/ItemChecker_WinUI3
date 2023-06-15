using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace ItemChecker.Views
{
    public sealed partial class NavigationPage : Page
    {
        public NavigationPage()
        {
            this.InitializeComponent();

            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(typeof(ParserPage));
        }

        private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                ContentFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null && (args.InvokedItemContainer.Tag != null))
            {
                var newPage = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                ContentFrame.Navigate(newPage, args.RecommendedNavigationTransitionInfo);
            }
        }
        private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            NavigationViewControl.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                NavigationViewControl.SelectedItem = (NavigationViewItem)NavigationViewControl.SettingsItem;
                NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
            }
            else if (ContentFrame.SourcePageType == typeof(AccountPage))
            {
                NavigationViewControl.SelectedItem = NavigationViewControl.FooterMenuItems.OfType<NavigationViewItem>().First();
                ContentFrame.Navigate(typeof(AccountPage));
                NavigationViewControl.Header = "Account";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
                NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
            }
        }

        private void calculatorNavViewItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            calculatorTeachingTip.Content = new CalculatorPage();
            calculatorTeachingTip.IsOpen = !calculatorTeachingTip.IsOpen;
        }
    }
}
