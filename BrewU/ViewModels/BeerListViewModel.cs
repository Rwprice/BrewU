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
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace BrewU.ViewModels
{
    public class BeerListViewModel : ViewModelBase
    {
        public BeerListViewModel()
        {
            BeerList = new ObservableCollection<Beer>();
            IsLoading = false;
        }

        public ObservableCollection<Beer> BeerList
        {
            get;
            private set;
        }

        public async void LoadPage(User user, BeerListRequest beerRequest)
        {
            IsLoading = true;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(ResourceLoader.GetString("BeerListsEndpoint")));
            request.Headers["Cookie"] = user.Cookie;
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(beerRequest.ToString());

            using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null))
            {
                await requestStream.WriteAsync(data, 0, data.Length);
            }

            request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
        }

        private async void ReadCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var beerRequest = new GetBeerRequest();
                    JObject o = JObject.Parse(reader.ReadLine());
                    JArray content = o.GetValue("content") as JArray;

                    foreach(var id in content)
                    {
                        beerRequest.BeerIDs.Add(id.ToString());
                    }

                    string beersJson = await Post(ResourceLoader.GetString("BeerEndpoint"), beerRequest.ToString(), App.User.Cookie);

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        o = JObject.Parse(beersJson);
                        var obj = o.GetValue("content") as JObject;
                        content = obj.GetValue("BrewLib.BeerModel") as JArray;

                        foreach (var beerToken in content)
                        {
                            var beer = new Beer();
                            beer.Guid = beerToken["guid"].ToString();
                            beer.Description = beerToken["description"].ToString();
                            beer.StyleID = beerToken["style_id"].ToString();
                            beer.Available = beerToken["avail"].ToString() != "0";
                            beer.Name = beerToken["name"].ToString();
                            beer.AlcoholByVolume = double.Parse(beerToken["abv"].ToString());
                            beer.BreweryName = beerToken["brewery_name"].ToString();
                            beer.InFridge = beerToken["in_fridge"].ToString() != "0";
                            beer.IsBeerOfTheMonth = beerToken["is_bom"].ToString() != "0";

                            DateTime time;
                            if (DateTime.TryParse(beerToken["added_on"].ToString(), out time))
                                beer.AddedOn = time;
                            if (DateTime.TryParse(beerToken["updated_on"].ToString(), out time))
                                beer.UpdatedOn = time;
                            if (DateTime.TryParse(beerToken["last_had_on"].ToString(), out time))
                                beer.LastHadOn = time;
                            if (DateTime.TryParse(beerToken["had_on"].ToString(), out time))
                                beer.HadOn = time;

                            beer.Image = new BitmapImage(new Uri(ResourceLoader.GetString(string.Format("ImageEndpoint",
                                beerToken["large_image"].ToString()))));

                            BeerList.Add(beer);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            IsLoading = false;
        }

        public void ClearList()
        {
            BeerList.Clear();
            IsLoading = false;
        }
    }
}
