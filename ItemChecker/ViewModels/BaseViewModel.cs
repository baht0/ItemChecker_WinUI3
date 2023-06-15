using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace ItemChecker.ViewModels
{
    public partial class BaseViewModel<T> : ObservableObject
    {
        public List<T> InitialItems { get; set; } = new();

        [ObservableProperty]
        ObservableCollection<T> _items = new();

        [ObservableProperty]
        MessageBar _messageBar = new();
        [ObservableProperty]
        Dictionary<string, object> _info = new();

        public int Secounds { get; set; }

        public CancellationTokenSource CTSource { get; set; } = new();
        public CancellationToken CToken { get; set; }
    }
}
