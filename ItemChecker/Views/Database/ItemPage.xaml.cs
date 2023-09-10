using CommunityToolkit.WinUI.UI.Controls;
using ItemChecker.Models;
using ItemChecker.ViewModels.DatabaseViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace ItemChecker.Views.Database
{
    public sealed partial class ItemPage : Page
    {
        private static ItemViewModel ViewModel;
        public ItemPage(string itemName)
        {
            this.InitializeComponent();
            ViewModel = new(itemName);
            DataContext = ViewModel;
            ViewModel.MessageEvent += MessageShow_Handler;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.MessageEvent -= MessageShow_Handler;
            base.OnNavigatedFrom(e);
        }

        private void MessageShow_Handler(object sender, EventArgs e)
        {
            var args = (MessageEventArgs)e;
            MessageBar.Severity = (InfoBarSeverity)args.Icon;
            MessageBar.Title = args.Title;
            MessageBar.Message = args.Message;
            MessageTip.IsOpen = true;
        }

        private void dataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e) => ViewModel.OpenInCommand.Execute(((DataGrid)sender).SelectedIndex);
        private void Image_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e) => FullSkinImage.IsOpen = true;
    }
}
