using ItemChecker.Properties;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.StaticModels.Accounts
{
    public class BuffAccount : Buff163
    {
        public bool IsActive { get; private set; } = AppProperties.Account.BUFF;
        public double Balance { get; private set; }
        public double Commission => 0.975d;

        internal BuffAccount()
        {
            if (IsActive)
                SignInService();
        }
        private async void SignInService() => await Post.SignInAsync();
    }
}
