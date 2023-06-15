using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace ItemChecker.Models
{
    public class Database
    {
        public static List<TabViewItem> Items { get; set; } = new();
        public static TabViewItem SelectedItem { get; set; } = new();
    }
}
