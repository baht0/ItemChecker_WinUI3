using System;
using ItemChecker.Models.StaticModels;

namespace ItemChecker.Models.StaticModels.Base
{
    public class LfmItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public double Price { get; set; }
        public int Have { get; set; }
        public int Limit { get; set; }
        public int Reservable { get; set; }
        public int Tradable { get; set; }
        public double SteamPriceRate { get; set; }
        public bool IsHave { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.Unavailable;
    }
}
