using ItemChecker.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ItemChecker.Views
{
    public sealed partial class StartUpPage : Page
    {
        public StartUpPage()
        {
            this.InitializeComponent();
            DataContext = new StartUpViewModel();
        }

        private void PassBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SignIn();
        }
        private void SignInBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            SignIn();
        }
        private void SignIn()
        {
            var viewModel = (StartUpViewModel)DataContext;
            viewModel.SignInSubmit(PassBox.Password);
        }

        private void code2FA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (code2FA.Text.Length == 5)
            {
                var viewModel = (StartUpViewModel)DataContext;
                viewModel.SubmitCode(code2FA.Text);
            }
        }
    }
}
