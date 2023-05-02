// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using ItemChecker.Views.Rare;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ItemChecker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RarePage : Page
    {
        public RarePage()
        {
            this.InitializeComponent();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            CheckShowDialogAsync();
        }
        public async void CheckShowDialogAsync()
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
        private void AdditionalBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterTeachingTip.IsOpen = true;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var win = (MainWindow)App.MainWindow;
            win.NavigateToItemBasePage();
        }
    }
}
