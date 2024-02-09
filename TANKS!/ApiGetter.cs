using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TANKS_
{
    public static class ApiGetter
    {
        public static HttpClient client = new HttpClient();

        public static async void GetString(string url, Action<string> GetResponseCallback)
        {
            HttpResponseMessage httpResponse = await client.GetAsync(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                string response = await httpResponse.Content.ReadAsStringAsync();
                GetResponseCallback(response);
            }
        }



        public static async void Post(string url, string data, Action OnError)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(data));
            }
            catch
            {
                OnError();
            }
        }

        const string GenericDataUploadUrl = "https://highscores.neonrogue.net/uploadmap/";
        const string GenericDataDownloadUrl = "https://highscores.neonrogue.net/map/";
        const string GenericDataDeleteUrl = @"https://highscores.neonrogue.net/delmap/";

        public static void PostGenericString(string data, string tag, int attempts = 3)
        {
            if (attempts <= 0)
                return;

            Post($"{GenericDataUploadUrl}{tag}", data, () => PostGenericString(data, tag, attempts - 1));
        }

        public static void GetGenericString(Action<string> GetResponseCallback, string tag)
        {
            GetString($"{GenericDataDownloadUrl}{tag}", GetResponseCallback);
        }

        public static void DeleteMap(string map, int attempts = 3)
        {
            if (attempts <= 0)
                return;

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(map + HighScoreGetter.Salt));

            Post($"{GenericDataDeleteUrl}{map}", HighScoreGetter.ByteArrayToString(bytes).ToLower(), () => DeleteMap(map, attempts - 1));
        }
    }

    internal static class HighScoreGetter
    {
        public const string Salt = "massimo";
        public static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");
    }
}
