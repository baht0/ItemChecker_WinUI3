using ItemChecker.Properties;
using System;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.StaticModels.Accounts
{
    public class LfmAccount : LootFarm
    {
        public bool IsActive { get; private set; } = AppProperties.Services.LFM;
        public double Commission => 0.95d;
        public double Balance { get; private set; }
        public double SumOfItems { get; private set; }

        internal LfmAccount()
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
                var obj = await Get.InventoryItemsAsync();

                double sum = 0;
                if (obj != null)
                    foreach (var i in obj)
                        sum += Convert.ToDouble(i.Value["p"]) / 100;
                SumOfItems = sum;
            }
        }
    }
}
