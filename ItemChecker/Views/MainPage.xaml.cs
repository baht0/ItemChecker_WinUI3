using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace ItemChecker.Views
{
    public sealed partial class MainPage : Page
    {
        public StackPanel AppTitleBar;
        public MainPage()
        {
            this.InitializeComponent();
        }
        public StackPanel CreateAppTitleBar(string title)
        {
            var appTitleBar = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(15, 0, 0, 0),
                Children =
                {
                    new ImageIcon()
                    {
                        Height = 20,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/icon.ico", UriKind.RelativeOrAbsolute))
                    },
                    new TextBlock
                    {
                        Text = title,
                        Margin = new Thickness(15, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                    }
                }
            };

            nvView.SetValue(Grid.RowProperty, 1);
            mainGrid.Children.Add(appTitleBar);
            return appTitleBar;
        }
        public void RemoveGridRow()
        {
            mainGrid.RowDefinitions.Remove(titleRow);
        }
    }
}
