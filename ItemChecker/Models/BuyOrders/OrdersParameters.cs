using ItemChecker.Models.StaticModels;
using System.Collections.Generic;

namespace ItemChecker.Models.BuyOrders
{
    public class OrdersParameters
    {
        public List<string> Services => BaseConfig.Services;
        public int ServiceId { get; set; }
        public double MinPrecent { get; set; } = 20;
        public int Time { get; set; } = 10;

        public OrdersParameters Clone() => this.MemberwiseClone() as OrdersParameters;
        public Dictionary<string, object> GetInfo() => new()
            {
                { "Service:", Services[ServiceId] },
                { "Min. precent:", MinPrecent },
                { "Timer (min.):", Time }
            };
    }
}
