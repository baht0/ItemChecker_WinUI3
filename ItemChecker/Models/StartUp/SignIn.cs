using ItemChecker.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using static ItemChecker.Net.SteamRequest;
using ItemChecker.Models.StaticModels;

namespace ItemChecker.Model.StartUp
{
    public partial class SignIn : ObservableObject
    {
        [ObservableProperty]
        private string _accountName = string.Empty;

        [ObservableProperty]
        private bool _isErrorShow;

        [ObservableProperty]
        private bool _isSignInShow;
        [ObservableProperty]
        bool _isSubmitShow;
        [ObservableProperty]
        bool _isSubmitEnabled = true;

        [ObservableProperty]
        bool _isConfirmationShow;
        [ObservableProperty]
        bool _isCodeEnabled = true;
        [ObservableProperty]
        private bool _isExpiredShow;
        [ObservableProperty]
        private string _remaining = "05 min. 00 sec.";
        [ObservableProperty]
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
