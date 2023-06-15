using ItemChecker.Properties;
using ItemChecker.Support;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.StaticModels.Accounts
{
    public class BuffAccount : Buff163
    {
        public bool IsActive { get; private set; } = AppProperties.Services.BUFF;
        public double Balance { get; private set; }
        public double Commission => 0.975d;

        internal BuffAccount()
        {
            if (IsActive)
                SignInService();
        }
        private async void SignInService() => await Post.SignInAsync();
        internal async Task GetBalanceAsync()
        {
            if (IsActive)
            {
                var blnInCny = await Get.BalanceAsync();
                Balance = Currencies.ConverterToUsd(blnInCny, 23);
            }
        }
    }
}
