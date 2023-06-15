using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ItemChecker.Views.Rare
{
    public sealed partial class ParametersPage : Page
    {
        public ParametersPage(object arg)
        {
            this.InitializeComponent();
            DataContext = arg;
        }

        private void parameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (floatGroup != null && stickerGroup != null && phaseGroup != null)
            {
                floatGroup.Visibility = parameter.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
                stickerGroup.Visibility = parameter.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
                phaseGroup.Visibility = parameter.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
