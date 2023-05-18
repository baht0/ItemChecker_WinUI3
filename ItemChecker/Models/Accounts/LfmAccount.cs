using ItemChecker.Properties;
using System;
using System.Threading.Tasks;
using static ItemChecker.Net.ServicesRequest;

namespace ItemChecker.Models.Accounts
{
    public class LfmAccount : LootFarm
    {
        public bool IsActive { get; set; } = Default.ActiveServices.LFM;
        public decimal Balance { get; set; }
        public decimal SumOfItems { get; set; }

        internal LfmAccount()
        {
            _ = IsActive ? SignInService() : null;
        }
        private async Task SignInService() => await Post.SignInAsync();
        internal async Task GetBalanceAsync() => Balance = IsActive ? await Get.BalanceAsync() : 0;
        internal async Task GetSumOfItemsAsync()
        {
            if (IsActive)
            {
                var obj = await Get.InventoryItemsAsync();

                decimal sum = 0;
                if (obj != null)
                    foreach (var i in obj)
                        sum += Convert.ToDecimal(i.Value["p"]) / 100;
                SumOfItems = sum;
            }
        }
    }
}
