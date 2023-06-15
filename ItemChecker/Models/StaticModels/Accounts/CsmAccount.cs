using ItemChecker.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.StaticModels.Accounts
{
    public class CsmAccount : CsMoney
    {
        public bool IsActive { get; private set; } = AppProperties.Services.CSM;
        public double Commission => 0.93d;
        public double Balance { get; private set; }
        public double SumOfItems { get; private set; }

        internal CsmAccount()
        {
            if (IsActive)
                SignInService();
        }
        private async void SignInService() => await Post.SignInAsync();
        internal async Task GetBalanceAsync() => Balance = IsActive ? await Get.BalanceAsync() : 0;
        internal async Task GetSumOfItemsAsync()
        {
            if (IsActive)
            {
                var array = await Get.InventoryItemsAsync();

                double sum = 0;
                foreach (JObject item in array)
                {
                    if (item.ContainsKey("isVirtual"))
                        sum += Convert.ToDouble(item["price"]);
                }
                SumOfItems = sum;
            }
        }
    }
}
