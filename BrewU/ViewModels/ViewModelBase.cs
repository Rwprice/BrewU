using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace BrewU.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ResourceLoader ResourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private bool _isLoading = false;

        public async Task<string> Post(string path, string postdata, string cookie = null)
        {
            var request = WebRequest.Create(new Uri(path)) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/json";

            if(cookie != null)
                request.Headers["Cookie"] = cookie;

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

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
