using CommunityToolkit.Mvvm.ComponentModel;
using ItemChecker.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemChecker.Models.Settings
{
    public class PatchNotes
    {
        public static async Task<List<Note>> GetPatchNotes()
        {
            var list = new List<Note>();
            JArray json = JArray.Parse(await DropboxRequest.Get.ReadAsync("Updates.json"));
            foreach (JObject update in json.Cast<JObject>())
            {
                var note = new Note()
                {
                    Version = (string)update["version"],
                    Text = (string)update["text"],
                    Date = (DateTime)update["date"],
                };
                list.Add(note);
            }
            list.Reverse();
            return list;
        }
    }
    public class Note
    {
        public string Version { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
