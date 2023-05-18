using ItemChecker.ViewModels.AccountPage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace ItemChecker.Views.Account
{
    public sealed partial class InfoPage : Page
    {
        InfoViewModel ViewModel = new();
        public InfoPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ViewModel.ShowApiKey();
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            ViewModel.CopyBtn(btn.Name);
        }

        private void IntervalBtn_Click(object sender, RoutedEventArgs e)
        {
            IntervalTeachingTip.IsOpen = true;
        }
        private void Interval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (BeginCalendar != null && EndCalendar != null)
            {
                EndCalendar.MaxDate = DateTime.Now;
                BeginCalendar.Date = EndCalendar.Date = null;
                BeginCalendar.IsEnabled = EndCalendar.IsEnabled = (string)cmb.SelectedItem == "Custom";
            }
            ViewModel.SwitchInterval(cmb.SelectedIndex);
        }
        private void BeginCalendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = sender.Date.GetValueOrDefault().DateTime;
            ViewModel.BeginInterval(date);
        }
        private void EndCalendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            var date = sender.Date.GetValueOrDefault().DateTime;
            ViewModel.EndInterval(date);
        }
        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }
        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

    }
}
