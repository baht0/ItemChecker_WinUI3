using System;

namespace ItemChecker.Models.Base
{
    public class LfmItem
    {
        public DateTime Updated { get; set; } = DateTime.Now.AddHours(-1);
        public decimal Price { get; set; }
        public int Have { get; set; }
        public int Limit { get; set; }
        public int Reservable { get; set; }
        public int Tradable { get; set; }
        public decimal SteamPriceRate { get; set; }
        public bool IsHave { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.Unavailable;
    }
}
