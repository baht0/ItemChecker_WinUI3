using ItemChecker.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views
{
    public sealed partial class StartUpPage : Page
    {
        readonly StartUpViewModel ViewModel = new();
        public StartUpPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void PassBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SignIn();
        }
        private void SignInBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => SignIn();
        private void SignIn() => ViewModel.SignInSubmitCommand.Execute(PassBox.Password);

        private void code2FA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (code2FA.Text.Length == 5)
            {
                ViewModel.SubmitCodeCommand.Execute(code2FA.Text);
            }
        }
    }
}
