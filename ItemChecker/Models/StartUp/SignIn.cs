using ItemChecker.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using static ItemChecker.Net.SteamRequest;
using static ItemChecker.Net.ServicesRequest;
using ItemChecker.Models;

namespace ItemChecker.Model.StartUp
{
    public partial class SignIn : ObservableObject
    {
        public string AccountName
        {
            get => _accountName;
            set => SetProperty(ref _accountName, value);
        }
        private string _accountName = string.Empty;

        public bool IsErrorShow
        {
            get => _isErrorShow;
            set => SetProperty(ref _isErrorShow, value);
        }
        private bool _isErrorShow;

        public bool IsSignInShow
        {
            get => _isSignInShow;
            set => SetProperty(ref _isSignInShow, value);
        }
        private bool _isSignInShow;
        public bool IsSubmitShow
        {
            get => _isSubmitShow;
            set => SetProperty(ref _isSubmitShow, value);
        }
        bool _isSubmitShow;
        public bool IsSubmitEnabled
        {
            get => _isSubmitEnabled;
            set => SetProperty(ref _isSubmitEnabled, value);
        }
        bool _isSubmitEnabled = true;

        public bool IsConfirmationShow
        {
            get => _isConfirmationShow;
            set => SetProperty(ref _isConfirmationShow, value);
        }
        bool _isConfirmationShow;
        public bool IsCodeEnabled
        {
            get => _isCodeEnabled;
            set => SetProperty(ref _isCodeEnabled, value);
        }
        bool _isCodeEnabled = true;
        public bool IsExpiredShow
        {
            get => _isExpiredShow;
            set => SetProperty(ref _isExpiredShow, value);
        }
        private bool _isExpiredShow;
        public string Remaining
        {
            get => _remaining;
            set => SetProperty(ref _remaining, value);
        }
        private string _remaining = "05 min. 00 sec.";
        public string ErrorMess
        {
            get => _errorMess;
            set => SetProperty(ref _errorMess, value);
        }
        private string _errorMess = "Invalid code.";

        bool IsSubmitted { get; set; }
        bool IsSetToken { get; set; }
        System.Timers.Timer Timer { get; set; } = new(1000);
        int TimerTick { get; set; } = 300;

        public async Task MainAsync()
        {
            var isAuthorized = await Session.IsAuthorizedAsync();
            if (!isAuthorized)
            {
                IsSubmitShow = true;
                IsSignInShow = true;

                await Task.Run(() =>
                {
                    while (!IsSubmitted || !IsSetToken)
                        Thread.Sleep(100);
                });

                IsSignInShow = false;
                IsConfirmationShow = false;
            }
        }

        public async void Submit(string pass)
        {
            try
            {
                IsSubmitEnabled = false;
                IsErrorShow = false;

                if (!string.IsNullOrEmpty(AccountName) && !string.IsNullOrEmpty(pass))
                {
                    var isAllow = await AllowUserAsync(AccountName);
                    if (isAllow)
                    {
                        IsSubmitted = await Session.SubmitSignIn(AccountName, pass);
                        if (IsSubmitted)
                        {
                            Timer.Elapsed += SessionTimerTick;
                            Timer.Enabled = true;

                            IsErrorShow = false;
                            IsSubmitShow = false;
                            IsConfirmationShow = true;
                        }
                        else
                            IsErrorShow = true;
                    }
                    else
                        IsErrorShow = true;
                }
                else
                    IsErrorShow = true;

                IsSubmitEnabled = true;
            }
            catch
            {

            }            
        }
        async Task<bool> AllowUserAsync(string login)
        {
            JArray users = JArray.Parse(await DropboxRequest.Get.ReadAsync("Users.json"));
            JObject user = (JObject)users.FirstOrDefault(x => x["Login"].ToString() == login);
            if (user != null)
            {
                int id = users.IndexOf(user);
                users[id]["LastLoggedIn"] = DateTime.Now;
                users[id]["Version"] = AppConfig.CurrentVersion;

                DropboxRequest.Post.Delete("Users.json");
                Thread.Sleep(200);
                DropboxRequest.Post.Upload("Users.json", users.ToString());
                return Convert.ToBoolean(user["Allowed"]);
            }
            return false;
        }
        async void SessionTimerTick(Object sender, ElapsedEventArgs e)
        {
            TimerTick--;
            var win = (MainWindow)App.MainWindow;
            win.DispatcherQueue.TryEnqueue(() =>
            {
                var time = TimeSpan.FromSeconds(TimerTick);
                Remaining = time.ToString("mm' min 'ss' sec.'");
            });
            if (TimerTick % 5 == 0)
            {
                IsSetToken = await Session.CheckAuthStatus();                
                if (IsSetToken)
                {
                    Timer.Enabled = false;
                    Timer.Elapsed -= SessionTimerTick;
                }
                else if(!IsCodeEnabled && !IsSetToken)
                {
                    win.DispatcherQueue.TryEnqueue(() =>
                    {
                        IsCodeEnabled = true;
                        IsErrorShow = true;
                    });
                }
            }
            else if (TimerTick <= 0)
            {
                win.DispatcherQueue.TryEnqueue(() =>
                {
                    IsExpiredShow = true;
                    IsCodeEnabled = false;
                    ErrorMess = "The sign in request has expired. Restart to login.";
                });
                Timer.Enabled = false;
                Timer.Elapsed -= SessionTimerTick;
            }
        }

        public async void SubmitCode(string code)
        {
            try
            {
                IsErrorShow = false;
                await Session.SubmitCode(code);
                IsCodeEnabled = false;
            }
            catch
            {

            }
        }
    }
}
