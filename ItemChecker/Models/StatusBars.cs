using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ItemChecker.Models
{
    public partial class LoadingBar : ObservableObject
    {
        public bool IsBusy { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }

        public LoadingBar()
        {
            IsBusy = false;
            Title = string.Empty;
            Message = string.Empty;
        }
        public LoadingBar(string title = "", string message = "")
        {
            IsBusy = true;
            Title = title;
            Message = message;
        }
        public LoadingBar(bool isBusy = false, string title = "", string message = "")
        {
            IsBusy = isBusy;
            Title = title;
            Message = message;
        }
    }
    public class MessageEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int Icon { get; set; }

        public MessageEventArgs(string title, string message, int icon)
        {
            Title = title;
            Message = message;
            Icon = icon;
        }
    }
}
