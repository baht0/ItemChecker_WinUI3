using ItemChecker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ItemChecker.ViewModels
{
    internal class StartUpViewModel : ObservableObject
    {
        public StartUpViewModel()
        {
            Task.Run(() =>
            {
                Thread.Sleep(2500);

                var win = (MainWindow)App.MainWindow;
                win.DispatcherQueue.TryEnqueue(() => { win.StartUpCompletion(); });
            });
        }
    }
}
