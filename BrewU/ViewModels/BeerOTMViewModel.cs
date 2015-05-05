using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Resources;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using BrewU;
using BrewU.Objects;
using Windows.ApplicationModel.Resources;

namespace BrewU.ViewModels
{
    public class BeerOTMViewModel : ViewModelBase
    {
        public BeerOTMViewModel()
        {
            BeerList = new ObservableCollection<Beer>();
            IsLoading = false;
        }

        public ObservableCollection<Beer> BeerList
        {
            get;
            private set;
        }

        public void LoadPage(User user)
        {
            IsLoading = true;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(ResourceLoader.GetString("BeerListsEndpoint")));
            request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
        }

        private void ReadCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    JToken o = JObject.Parse(reader.ReadLine());
                    JArray featured = JArray.Parse(o.SelectToken("featured").ToString());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ClearList()
        {
            BeerList.Clear();
            IsLoading = false;
        }
    }
}
