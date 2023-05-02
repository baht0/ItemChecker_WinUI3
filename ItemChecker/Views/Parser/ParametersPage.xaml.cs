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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ItemChecker.Views.Parser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ParametersPage : Page
    {
        public ParametersPage()
        {
            this.InitializeComponent();
            CreateButtons(0);
        }

        private void Service1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = service1.SelectedIndex;
            CreateButtons(id);
        }
        void CreateButtons(int id)
        {
            configGrid ??= new Grid();
            configGrid.Children.Clear();
            string[] binding = new string[] { "Normal", "Souvenir", "KnifeGlove", string.Empty, "Stattrak", "KnifeGloveStattrak" };
            string[] name = new string[] { "Normal", "Souvenir", "★", string.Empty, "StatTrak™", "★ StatTrak™" };
            name[3] = binding[3] = id == 4 || id == 5 ? "All" : "NotWeapon";
            for (int i = 0; i < name.Length; i++)
            {
                dynamic button = id == 4 || id == 5 ? new RadioButton() : new CheckBox();
                button.Content = name[i];
                //button.ToolTipService.ToolTip = name[i] == "NotWeapon" ? "Stickers, graffiti, agents, music kit, etc." : null;

                configGrid.Children.Add(button);

                switch (i)
                {
                    case 0:
                        Grid.SetRow(button, 0);
                        Grid.SetColumn(button, 0);
                        break;
                    case 1:
                        Grid.SetRow(button, 1);
                        Grid.SetColumn(button, 0);
                        break;
                    case 2:
                        Grid.SetRow(button, 2);
                        Grid.SetColumn(button, 0);
                        break;
                    case 3:
                        Grid.SetRow(button, 0);
                        Grid.SetColumn(button, 1);
                        break;
                    case 4:
                        Grid.SetRow(button, 1);
                        Grid.SetColumn(button, 1);
                        break;
                    case 5:
                        Grid.SetRow(button, 2);
                        Grid.SetColumn(button, 1);
                        break;
                }
            }
        }
    }
}
