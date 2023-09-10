using ItemChecker.Models.Parser;
using ItemChecker.Models.StaticModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ItemChecker.Models
{
    public class ParserTool
    {
        private ParserParameters Parameters { get; set; }
        public ParserTool(ParserParameters parameters) => Parameters = parameters;

        public async Task ExportAsync(List<ParserData> items)
        {
            var itemsArray = JArray.Parse(JsonConvert.SerializeObject(items, Formatting.Indented));

            JObject json = new(
                new JProperty("Size", items.Count),
                new JProperty("Parameters", JObject.FromObject(Parameters)),
                new JProperty("Items", itemsArray));

            string path = AppConfig.DocumentPath + "parser\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            await File.WriteAllTextAsync(path + $"export_{Parameters.DateTime:ddMMyyyyHHmmss}.json", json.ToString());
        }
    }
}
