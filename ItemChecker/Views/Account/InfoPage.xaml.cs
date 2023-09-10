using ItemChecker.Models;
using ItemChecker.ViewModels.AccountViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace ItemChecker.Views.Account
{
    public sealed partial class InfoPage : Page
    {
        readonly static InfoViewModel ViewModel = new();
        public InfoPage()
        {
            this.InitializeComponent();
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

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => ViewModel.ShowApiKeyCommand.Execute(null);
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ViewModel.CopyBtnCommand.Execute(btn?.Name);
        }
    }
}
