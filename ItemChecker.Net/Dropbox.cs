using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ItemChecker.Net
{
    public class DropboxRequest : HttpRequest
    {
        public class Get
        {
            public static async Task<string> ReadAsync(string path)
            {
                var headers = new HttpClient().DefaultRequestHeaders;
                headers.Add("Dropbox-API-Arg", "{\"path\": \"/" + path + "\"}");
                headers.Add("Authorization", "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs");

                return await RequestGetAsync("https://content.dropboxapi.com/2/files/download", headers);
            }
        }
        public class Post
        {
            public static JObject ListFolder(string path)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("https://api.dropboxapi.com/2/files/list_folder");

                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers["Authorization"] = "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs";

                JObject json = new(
                            new JProperty("path", $"/{path}"),
                            new JProperty("recursive", false),
                            new JProperty("include_media_info", false),
                            new JProperty("include_deleted", false),
                            new JProperty("include_has_explicit_shared_members", false),
                            new JProperty("include_mounted_folders", true),
                            new JProperty("include_non_downloadable_files", true));
                string data = json.ToString(Formatting.None);

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data.ToLower());
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());

                return JObject.Parse(streamReader.ReadToEnd());
            }
            public static String Delete(string path)
            {
                JObject json = new(
                               new JProperty("path", $"/{path}"));
                string data = json.ToString(Formatting.None);
                var httpRequest = (HttpWebRequest)WebRequest.Create("https://api.dropboxapi.com/2/files/delete_v2");

                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            public static String Folder(string path)
            {
                JObject json = new(
                               new JProperty("path", $"/{path}"),
                               new JProperty("autorename", false));
                string data = json.ToString(Formatting.None);
                var httpRequest = (HttpWebRequest)WebRequest.Create("https://api.dropboxapi.com/2/files/create_folder_v2");

                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            public static String Upload(string path, string data)
            {
                JObject json = new(
                               new JProperty("path", $"/{path}"),
                               new JProperty("mode", "add"),
                               new JProperty("autorename", false),
                               new JProperty("mute", false),
                               new JProperty("strict_conflict", false));
                string args = json.ToString(Formatting.None);

                var httpRequest = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/upload");

                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/octet-stream";
                httpRequest.Headers["Dropbox-API-Arg"] = args;
                httpRequest.Headers["Authorization"] = "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            public static void UploadFile(string path, string filePath)
            {
                JObject json = new(
                               new JProperty("path", $"/{path}"),
                               new JProperty("mode", "add"),
                               new JProperty("autorename", false),
                               new JProperty("mute", false),
                               new JProperty("strict_conflict", false));
                string args = json.ToString(Formatting.None);

                using (WebClient client = new())
                {
                    client.Headers.Add("Content-Type", "application/octet-stream");
                    client.Headers.Add("Dropbox-API-Arg", args);
                    client.Headers.Add("Authorization", "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs");
                    using (Stream fileStream = File.OpenRead(filePath))
                    using (Stream requestStream = client.OpenWrite(new Uri("https://content.dropboxapi.com/2/files/upload"), "POST"))
                    {
                        fileStream.CopyTo(requestStream);
                    }
                }
            }
            public static void DownloadZip(string zipPath)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("https://content.dropboxapi.com/2/files/download_zip");

                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer a94CSH6hwyUAAAAAAAAAAf3zRyhyZknI9J8KM3VZihWEILAuv6Vr3ht_-4RQcJxs";
                httpRequest.Headers["Dropbox-API-Arg"] = "{\"path\": \"/ItemChecker\"}";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (Stream zip = File.OpenWrite(zipPath))
                    {
                        stream.CopyTo(zip);
                    }
                }
            }
        }
    }
}
