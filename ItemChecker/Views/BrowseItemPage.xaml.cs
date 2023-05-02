// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ItemChecker.Views.BrowseItem;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ItemChecker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowseItemPage : Page
    {
        public BrowseItemPage()
        {
            this.InitializeComponent();
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            var tab = CreateNewTab();
            sender.TabItems.Add(tab);
            sender.SelectedItem = tab;
        }
        private TabViewItem CreateNewTab()
        {
            TabViewItem newItem = new()
            {
                Header = "New Tab",
                IconSource = new SymbolIconSource() { Symbol = Symbol.Document }
            };
            Frame frame = new();
            frame.Navigate(typeof(NewTabPage));
            newItem.Content = frame;

            return newItem;
        }
        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }
    }
}
