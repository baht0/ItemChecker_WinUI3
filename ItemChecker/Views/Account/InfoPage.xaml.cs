using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace ItemChecker.Views.Account
{
    public class Test
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
    public sealed partial class InfoPage : Page
    {
        ObservableCollection<Test> itemsList = new();
        public InfoPage()
        {
            this.InitializeComponent();

            for (int i = 0; i< 40;i++)
            {
                var d = new Test()
                {
                    Date = DateTime.Now.AddDays(i),
                    Total = 100 * i,
                };
                itemsList.Add(d);
            }
        }
        private void IntervalBtn_Click(object sender, RoutedEventArgs e)
        {
            IntervalTeachingTip.IsOpen = true;
        }

        private void Interval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (BeginCalendar != null && EndCalendar != null)
                BeginCalendar.IsEnabled = EndCalendar.IsEnabled = cmb.SelectedIndex == 7;
        }
    }
}
