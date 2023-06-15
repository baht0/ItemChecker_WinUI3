using ItemChecker.ViewModels.AccountViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ItemChecker.Views.Account
{
    public sealed partial class InfoPage : Page
    {
        readonly static InfoViewModel ViewModel = new();
        public InfoPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => ViewModel.ShowApiKeyCommand.Execute(null);
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ViewModel.CopyBtnCommand.Execute(btn?.Name);
        }
        private void ServiceToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ServiceInfoBar.IsOpen = true;
        }
    }
}
