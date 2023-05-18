using ItemChecker.Support;
using System.Threading.Tasks;
using Windows.Storage;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.Accounts
{
    public class BuffAccount : Buff163
    {
        public bool IsActive { get; set; } = true;
        public decimal Balance { get; set; }

        internal BuffAccount()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var composite = (ApplicationDataCompositeValue)localSettings.Values["ActiveServices"];
            if (composite != null && composite["BUFF"] != null)
                IsActive = (bool)composite["BUFF"];

            _ = IsActive ? SignInService() : null;
        }
        private async Task SignInService() => await Post.SignInAsync();
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
