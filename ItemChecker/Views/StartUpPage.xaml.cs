using ItemChecker.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views
{
    public sealed partial class StartUpPage : Page
    {
        public StartUpPage()
        {
            this.InitializeComponent();
            DataContext = new StartUpViewModel();
        }
    }
}
