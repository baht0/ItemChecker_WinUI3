using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

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
    public partial class MessageBar : ObservableObject
    {
        [ObservableProperty]
        bool _isOpen;
        [ObservableProperty]
        InfoBarSeverity _severity = InfoBarSeverity.Informational;
        [ObservableProperty]
        string _title = string.Empty;
        [ObservableProperty]
        string _message = string.Empty;
    }
}
