using ItemChecker.Views.BuyOrders;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ItemChecker.Views
{
    public sealed partial class BuyOrdersPage : Page
    {
        public BuyOrdersPage()
        {
            this.InitializeComponent();
        }

        private void Push_Click(object sender, RoutedEventArgs e)
        {
            PushShowDialogAsync();
        }
        public async void PushShowDialogAsync()
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Parameters",
                PrimaryButtonText = "Start",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = new ParametersPage(),
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                //
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //var win = (MainWindow)App.MainWindow;
            //win.NavigateToItemBasePage();
        }
    }
}
