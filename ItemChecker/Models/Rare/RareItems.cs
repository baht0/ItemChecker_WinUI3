using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ItemChecker.Models.Rare
{
    public partial class RareItems : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Item> addBase = new(ItemBase.List);
        [ObservableProperty]
        ObservableCollection<DataRareItem> items = new();
        public List<string> Services => new(BaseConfig.RareParameters);
        [ObservableProperty]
        int selectedService;

        partial void OnSelectedServiceChanged(int value)
        {
            var items = value switch
            {
                0 => ItemBase.List.Where(x => x.Type == Type.Weapon || x.Type == Type.Knife || x.Type == Type.Gloves).ToList(),
                1 => ItemBase.List.Where(x => x.Type == Type.Weapon).ToList(),
                2 => ItemBase.List.Where(x => x.ItemName.Contains("Doppler") && (x.Type == Type.Weapon || x.Type == Type.Knife)).ToList(),
                _ => new List<Item>(),
            };
            AddBase = new(items);
        }

        public RareItems() => ReadFile();
        private async void ReadFile()
        {
            string path = AppConfig.DocumentPath + "list.json";

            var json = new JObject(
                    new JProperty(nameof(Rare), new JArray()));
            if (File.Exists(path))
                json = JObject.Parse(await File.ReadAllTextAsync(path));

            Items = json[nameof(Rare)].ToObject<ObservableCollection<DataRareItem>>();
        }

        public async void Add(string itemName)
        {
            var item = new DataRareItem(itemName, Services[SelectedService]);
            if (!Items.Any(x => x.ItemName == item.ItemName && x.Service == item.Service))
            {
                Items.Add(item);
                await SaveAsync();
            }
        }
        public async void Delete(DataRareItem item)
        {
            Items.Remove(item);
            await SaveAsync();
        }
        public async void Clear()
        {
            Items.Clear();
            await SaveAsync();
        }
        private async Task SaveAsync()
        {
            string path = AppConfig.DocumentPath + "list.json";
            var json = new JObject(
                    new JProperty(nameof(Rare), JArray.FromObject(Items)));
            await File.WriteAllTextAsync(path, json.ToString());
        }
    }
    public partial class DataRareItem
    {
        public string ItemName { get; set; } = "Unknown";
        public string Service { get; set; } = "-";

        [JsonIgnore]
        public ICommand Command { get; set; }

        public DataRareItem(string itemName, string service)
        {
            this.ItemName = itemName;
            this.Service = service;
        }
    }
}
