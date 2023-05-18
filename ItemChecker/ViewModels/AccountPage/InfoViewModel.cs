using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ItemChecker.Models;
using ItemChecker.Models.AccountPage;
using ItemChecker.Support;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace ItemChecker.ViewModels.AccountPage
{
    public partial class InfoViewModel : ObservableObject
    {
        public InfoAccount Info
        {
            get => _info;
            set => SetProperty(ref _info, value);
        }
        InfoAccount _info = new();
        public RecordsAccount Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }
        RecordsAccount _records = new();

        [RelayCommand]
        private void ShowProfile() => Edit.OpenUrl("https://steamcommunity.com/profiles/" + Info.ID64);
        [RelayCommand]
        private void SignOut()
        {
            var config = new UserConfig();
            config.Delete();

            App.HandleClosedEvents = false;
            var mainWindow = (MainWindow)App.MainWindow;
            mainWindow?.Close();
        }
        [RelayCommand]
        public void ShowApiKey() => Info.ApiKey = Account.ApiKey;
        [RelayCommand]
        public void CopyBtn(string name)
        {
            var dataPackage = new DataPackage();
            switch (name)
            {
                case "apiKey":
                    dataPackage.SetText(Account.ApiKey);
                    break;
                case "id64":
                    dataPackage.SetText(Account.ID64);
                    break;
            }
            Clipboard.SetContent(dataPackage);
        }

        [RelayCommand]
        public async void Refresh() => await Records.RefreshRecords();
        [RelayCommand]
        public void SwitchInterval(int index) => Records.SwitchInterval(index);
        [RelayCommand]
        public void BeginInterval(DateTime date) => Records.BeginInterval(date);
        [RelayCommand]
        public void EndInterval(DateTime date) => Records.EndInterval(date);
    }
}
