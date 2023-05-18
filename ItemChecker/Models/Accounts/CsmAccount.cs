using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.Accounts
{
    public class CsmAccount : CsMoney
    {
        public bool IsActive { get; set; } = true;
        public decimal Balance { get; set; }
        public decimal SumOfItems { get; set; }

        internal CsmAccount()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            var composite = (ApplicationDataCompositeValue)localSettings.Values["ActiveServices"];
            if (composite != null && composite["CSM"] != null)
                IsActive = (bool)composite["CSM"];

            //_ = IsActive ? SignInService() : null;
        }
        private async Task SignInService() => await Post.SignInAsync();
        internal async Task GetBalanceAsync() => Balance = IsActive ? await Get.BalanceAsync() : 0;
        internal async Task GetSumOfItemsAsync()
        {
            if (IsActive)
            {
                var array = await Get.InventoryItemsAsync();

                decimal sum = 0;
                foreach (JObject item in array)
                {
                    if (item.ContainsKey("isVirtual"))
                        sum += Convert.ToDecimal(item["price"]);
                }
                SumOfItems = sum;
            }
        }
    }
}
