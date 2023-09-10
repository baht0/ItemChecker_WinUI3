using ItemChecker.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using static ItemChecker.Net.SteamRequest;
using ItemChecker.Models.StaticModels;

namespace ItemChecker.Model.StartUp
{
    public partial class SignIn : ObservableObject
    {
        [ObservableProperty]
        private string accountName = string.Empty;

        [ObservableProperty]
        private string qrUrl = string.Empty;

        [ObservableProperty]
        private bool isErrorShow;

        [ObservableProperty]
        private bool isSignInShow;
        [ObservableProperty]
        private bool isSubmitShow;
        [ObservableProperty]
        private bool isSubmitEnabled = true;

        [ObservableProperty]
        private bool isConfirmationShow;
        [ObservableProperty]
        private bool isCodeEnabled = true;
        [ObservableProperty]
        private bool isExpiredShow;
        [ObservableProperty]
        private string remaining = "05 min. 00 sec.";
        [ObservableProperty]
        private string errorMess = "Invalid code.";

        bool IsSubmitted { get; set; }
        bool IsSetToken => Session.IsSetToken;
        int TimerSubmit { get; set; } = 300;

        public async Task MainAsync()
        {
            var isAuthorized = await Session.IsAuthorizedAsync();
            if (!isAuthorized)
            {
                QrUrl = await Session.BeginAuthSessionViaQR();
                ResetQr();
                Session.CheckAuthStatus();

                IsSubmitShow = true;
                IsSignInShow = true;

                while (!IsSetToken)
                {
                    await Task.Delay(100);
                }

                IsSignInShow = false;
                IsConfirmationShow = false;
            }
        }

        //Login with QR
        private async void ResetQr()
        {
            int timer = 20;
            while (!IsSetToken)
            {
                if (timer <= 0)
                {
                    QrUrl = await Session.BeginAuthSessionViaQR();
                    timer = 20;
                }
                timer--;
                await Task.Delay(1000);
            }
        }

        //Login with pass
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
                            SubmitRemaining();

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
        private async void SubmitRemaining()
        {
            while (TimerSubmit > 0)
            {
                TimerSubmit--;
                Remaining = TimeSpan.FromSeconds(TimerSubmit).ToString("mm' min. 'ss' sec.'");
                if (IsSetToken)
                    break;
                else if (!IsCodeEnabled && !IsSetToken)
                {
                    IsCodeEnabled = true;
                    IsErrorShow = true;
                }
                await Task.Delay(1000);
            }
            if (TimerSubmit <= 0)
            {
                IsExpiredShow = true;
                IsCodeEnabled = false;
                ErrorMess = "The sign in request has expired. Restart to login.";
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
    }
}
