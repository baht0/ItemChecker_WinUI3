using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace ItemChecker.ViewModels
{
    public partial class BaseViewModel<T> : ObservableObject
    {
        public List<T> InitialItems { get; set; } = new();

        [ObservableProperty]
        ObservableCollection<T> items = new();

        [ObservableProperty]
        Dictionary<string, object> info = new();

        public int Secounds { get; set; }

        public CancellationTokenSource CTSource { get; set; } = new();
        public CancellationToken CToken { get; set; }

        public event EventHandler MessageEvent;
        public void MessageShow(string title, string message, int icon = 0)
        {
            var args = new MessageEventArgs(title, message, icon);
            MessageEvent?.Invoke(this, args);
        }
    }
}
