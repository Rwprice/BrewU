using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace BrewU.Objects
{
    public class User
    {
        public string DisplayName { get; set; }
        public string UserID { get; set; }
        public string Cookie { get; set; }

        private static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public static async Task<User> GetCookie(string userName, string password)
        {
            var result = await Post(resourceLoader.GetString("AuthorizationEndpoint"),
                "{\"username\":\"" + userName + "\",\"password\":\"" + password + "\"}");

            JObject o = JObject.Parse(result);
            JArray auth = o.GetValue("auth") as JArray;

            if (auth != null)
            {
                var user = new User();
                JArray content = o.GetValue("content") as JArray;

                user.UserID = content[0]["guid"].ToString();
                user.DisplayName = content[0]["first_name"].ToString() + " " + content[0]["last_name"].ToString();

                int index = int.Parse(auth[0].ToString());
                string id = auth[1].ToString();
                string guid = auth[2].ToString();
                string timeStamp = auth[3].ToString();

                timeStamp = timeStamp.Replace(",", "%2C");
                timeStamp = timeStamp.Replace(" ", "%20");
                timeStamp = timeStamp.Replace(":", "%3A");

                user.Cookie = "auth=";
                user.Cookie += index + "%7C";
                user.Cookie += id + "%7C";
                user.Cookie += guid + "%7C";
                user.Cookie += timeStamp;

                return user;
            }

            else
            {
                return null;
            }
        }

        private static async Task<string> Post(string path, string postdata)
        {
            var request = WebRequest.Create(new Uri(path)) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/json";

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
