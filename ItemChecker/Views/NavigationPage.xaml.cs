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
            ContentFrame.Navigate(typeof(ParserPage));
        }

        private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                if (!ContentFrame.CurrentSourcePageType.Name.Contains("Settings"))
                    ContentFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null && args.InvokedItemContainer.Tag != null)
            {
                if (ContentFrame.CurrentSourcePageType.FullName != args.InvokedItemContainer.Tag.ToString())
                {
                    var newPage = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                    ContentFrame.Navigate(newPage, null, args.RecommendedNavigationTransitionInfo);
                }
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
            else if (ContentFrame.SourcePageType != null)
            {
                var navigationViewItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>()
                    .FirstOrDefault(x => x.Tag != null && x.Tag.ToString() == ContentFrame.SourcePageType.FullName);

                NavigationViewControl.SelectedItem = navigationViewItem ?? NavigationViewControl.FooterMenuItems.OfType<NavigationViewItem>()
                    .FirstOrDefault(x => x.Tag.ToString() == ContentFrame.SourcePageType.FullName);

                NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
            }
        }

        private void calculatorNavViewItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            calculatorTeachingTip.Content = new CalculatorPage();
            calculatorTeachingTip.IsOpen = !calculatorTeachingTip.IsOpen;
        }
        public void DatabasePageInvoke(object item) => ContentFrame.Navigate(typeof(DatabasePage), item);
    }
}
