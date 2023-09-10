using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ItemChecker.Models.Parser
{
    public partial class ParserParameters : ObservableObject
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        [JsonIgnore]
        public List<string> Services => BaseConfig.Services;
        [ObservableProperty]
        private int serviceOneId;
        public int ServiceTwoId { get; set; } = 1;
        public string ServiceOneName => Services[ServiceOneId];
        public string ServiceTwoName => Services[ServiceTwoId];
        public double MinPrice { get; set; } = 0;
        [ObservableProperty]
        private double maxPrice;
        [ObservableProperty]
        private bool all;
        [ObservableProperty]
        private bool notWeapon;
        [ObservableProperty]
        private bool normal;
        [ObservableProperty]
        private bool souvenir;
        [ObservableProperty]
        private bool unique;
        [ObservableProperty]
        private bool stattrak;
        [ObservableProperty]
        private bool uniqueStattrak;

        partial void OnServiceOneIdChanged(int value)
        {
            All = false;
            switch (value)
            {
                case 4 or 5:
                    All = true;
                    NotWeapon = false;
                    Normal = false;
                    Souvenir = false;
                    Stattrak = false;
                    Unique = false;
                    UniqueStattrak = false;
                    break;
            }
        }

        public ParserParameters Clone() => this.MemberwiseClone() as ParserParameters;
        public Dictionary<string, object> GetInfo() => new()
            {
                { "From:", ServiceOneName },
                { "To:", ServiceTwoName },
                { "Date time:", DateTime.ToString("dd MMM yy HH:mm") },
                { "Min. price ($):", MinPrice },
                { "Max. price ($):", MaxPrice },
            };

        public async Task<List<ParserList>> CreateList(CancellationToken CToken)
        {
            try
            {
                switch (ServiceOneId)
                {
                    case 0 or 1:
                        await ItemBase.UpdateSteam();
                        break;
                    case 2:
                        await ItemBase.UpdateCsm(MinPrice, MaxPrice);
                        break;
                    case 3:
                        await ItemBase.UpdateLfm();
                        break;
                    case 4 or 5:
                        await ItemBase.UpdateBuff(this, 1);
                        break;
                }
                switch (ServiceTwoId)
                {
                    case 3:
                        await ItemBase.UpdateLfm();
                        return Apply();
                    case 4 or 5:
                        var list = Apply();
                        if (ItemBase.List.Count / 80 < list.Count)
                            await ItemBase.UpdateBuff(this, 2);
                        return list;
                    default:
                        return Apply();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return new();
            }
        }
        private List<ParserList> Apply()
        {
            List<ParserList> list = new();
            foreach (var item in ItemBase.List)
            {
                string itemName = item.ItemName;
                //standart
                if (itemName.Contains("Doppler") || item.Steam.Id == 0)
                    continue;
                //have
                if ((ServiceOneId == 1 && !item.Steam.IsHave) || (ServiceOneId == 2 && !item.Csm.Inventory.Any()) || (ServiceOneId == 3 && !item.Lfm.IsHave) || (ServiceOneId == 4 && !item.Buff.IsHave) || (ServiceOneId == 5 && !item.Buff.IsHave))
                    continue;
                //Unavailable
                if (ServiceTwoId == 2 && (item.Csm.Status == ItemStatus.Unavailable || item.Csm.Status == ItemStatus.Overstock))
                    continue;
                //Overstock
                if (ServiceTwoId == 3 && (item.Lfm.Status == ItemStatus.Unavailable || item.Lfm.Status == ItemStatus.Overstock))
                    continue;
                //Price
                if (MinPrice != 0)
                {
                    if (ServiceOneId < 2 && (item.Steam.AvgPrice == 0 || item.Steam.AvgPrice < MinPrice))
                        continue;
                    else if (ServiceOneId == 2 && ItemBase.List.FirstOrDefault(x => x.ItemName == itemName)?.Csm.Inventory.Select(x => x.Price).DefaultIfEmpty().Min() < MinPrice)
                        continue;
                    else if (ServiceOneId == 3 && item.Lfm.Price < MinPrice)
                        continue;
                    else if (ServiceOneId == 4 && item.Buff.BuyOrder < MinPrice)
                        continue;
                    else if (ServiceOneId == 5 && item.Buff.Price < MinPrice)
                        continue;
                }
                if (MaxPrice != 0)
                {
                    if (ServiceOneId < 2 && (item.Steam.AvgPrice == 0 || item.Steam.AvgPrice > MaxPrice))
                        continue;
                    else if (ServiceOneId == 2 && ItemBase.List.FirstOrDefault(x => x.ItemName == itemName)?.Csm.Inventory.Select(x => x.Price).DefaultIfEmpty().Min() > MaxPrice)
                        continue;
                    else if (ServiceOneId == 3 && item.Lfm.Price > MaxPrice)
                        continue;
                    else if (ServiceOneId == 4 && item.Buff.BuyOrder > MaxPrice)
                        continue;
                    else if (ServiceOneId == 5 && item.Buff.Price > MaxPrice)
                        continue;
                }
                //add
                var addItem = new ParserList(itemName);
                if (NotWeapon && item.Type != StaticModels.Type.Weapon && item.Type != StaticModels.Type.Knife && item.Type != StaticModels.Type.Gloves)
                    list.Add(addItem);
                else if (Normal && item.Type == StaticModels.Type.Weapon && !itemName.Contains("Souvenir") && !itemName.Contains("StatTrak™"))
                    list.Add(addItem);
                else if (Souvenir && item.Type == StaticModels.Type.Weapon && itemName.Contains("Souvenir"))
                    list.Add(addItem);
                else if (Stattrak && item.Type == StaticModels.Type.Weapon && itemName.Contains("StatTrak™"))
                    list.Add(addItem);
                else if (Unique && (item.Type == StaticModels.Type.Knife | item.Type == StaticModels.Type.Gloves))
                    list.Add(addItem);
                else if (UniqueStattrak && (item.Type == StaticModels.Type.Knife | item.Type == StaticModels.Type.Gloves) && itemName.Contains("StatTrak™"))
                    list.Add(addItem);
                else if (!Normal && !Souvenir && !Stattrak && !Unique && !UniqueStattrak)
                    list.Add(addItem);
            }
            return list;
        }
    }
}
