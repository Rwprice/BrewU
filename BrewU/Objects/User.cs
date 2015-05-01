using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrewU.Objects
{
    public class User
    {
        public string DisplayName { get; set; }
        public string UserID { get; set; }

        private static string _cookieHeader;

        public static async Task<string> GetCookie(string userName, string password)
        {
            var result = await Post("https://www.brewniversity.com/server/auth_internal",
                "{\"username\":\"" + userName + "\",\"password\":\"" + password + "\"}");

            JObject o = JObject.Parse(result);
            JArray auth = o.GetValue("auth") as JArray;

            int index = int.Parse(auth[0].ToString());
            string id = auth[1].ToString();
            string guid = auth[2].ToString();
            string timeStamp = auth[3].ToString();

            timeStamp = timeStamp.Replace(",", "%2C");
            timeStamp = timeStamp.Replace(" ", "%20");
            timeStamp = timeStamp.Replace(":", "%3A");

            string cookie = "auth=";
            cookie += index + "%7C";
            cookie += id + "%7C";
            cookie += guid + "%7C";
            cookie += timeStamp;

            return cookie;
        }

        public static async Task<string> Post(string path, string postdata, string cookieHeader = null)
        {
            var request = WebRequest.Create(new Uri(path)) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/json";

            if (cookieHeader != null)
            {
                request.Headers["Cookie"] = cookieHeader;
            }

            if (postdata != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(postdata);

                using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null))
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }

            return await httpRequest(request);
        }

        private static async Task<string> httpRequest(HttpWebRequest request)
        {
            string received;

            using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(responseStream))
                    {
                        received = await sr.ReadToEndAsync();
                    }
                }
            }

            return received;
        }
    }
}
