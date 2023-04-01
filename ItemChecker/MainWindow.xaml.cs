using ItemChecker.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using Windows.ApplicationModel;

namespace ItemChecker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            if (App.IsMicaSupported)
                MainGrid.Background = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(TitleBar);
            }
            else
            {
                TitleBar.Visibility = Visibility.Collapsed;
                MainGrid.RowDefinitions.Remove(TitleRow);
            }

            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(typeof(SteamPage));
        }

        public string GetAppTitleFromSystem()
        {
            return Package.Current.DisplayName;
        }

        public async void Exit_ShowDialog()
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = MainGrid.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Do you really want to quit?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                App.HandleClosedEvents = false;
                App.MainWindow.Close();
            }
        }
        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Exit_ShowDialog();
        }

        private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null && (args.InvokedItemContainer.Tag != null))
            {
                Type newPage = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                ContentFrame.Navigate(
                       newPage,
                       null,
                       args.RecommendedNavigationTransitionInfo
                       );
            }
        }

        private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            NavigationViewControl.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
                NavigationViewControl.SelectedItem = (NavigationViewItem)NavigationViewControl.SettingsItem;
            else if (ContentFrame.SourcePageType != null)
            {
                NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
            }

            NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
        }
    }
}
