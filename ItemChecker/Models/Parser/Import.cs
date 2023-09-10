using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Models.StaticModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.Parser
{
    public partial class Import : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ImportFile> items = new();
        [ObservableProperty]
        private ImportFile selected;
        [ObservableProperty]
        private bool isBusy;

        public async void GetItems()
        {
            IsBusy = true;
            Items.Clear();

            string folder = AppConfig.DocumentPath + "parser";
            if (Directory.Exists(folder))
            {
                foreach (var path in Directory.GetFiles(folder))
                {
                    var text = await File.ReadAllTextAsync(path);
                    var json = JObject.Parse(text);
                    var file = json["Parameters"].ToObject<ImportFile>();
                    file.Size = (int)json["Size"];
                    file.Path = path;
                    Items.Add(file);
                }
                Items = new(Items.OrderByDescending(d => d.DateTime));
            }
            Selected = Items.FirstOrDefault();
            IsBusy = false;
        }
        public async Task<List<ParserData>> LoadData()
        {
            string filePath = Selected.Path;
            if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
                return new();

            var text = await File.ReadAllTextAsync(filePath);
            var json = JObject.Parse(text);

            return JsonConvert.DeserializeObject<List<ParserData>>(json["Items"].ToString());
        }
    }
    public class ImportFile : ParserParameters
    {
        public string Path { get; set; }
        public int Size { get; set; }
    }
}
