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

        public static async Task UserLogin(string userName, string password)
        {
            var response = await post("https://www.tmbrew.com/m/");

            string postData = null;
            response = await post("https://www.tmbrew.com/m/login", postData);
        }

        private static async Task<string> post(string path)
        {
            return await post(path, null);
        }

        private static async Task<string> post(string path, string postdata)
        {
            var request = WebRequest.Create(new Uri(path)) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (_cookieHeader != null)
                request.Headers["Cookie"] = _cookieHeader;

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
                _cookieHeader = response.Headers["Set-Cookie"];
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
