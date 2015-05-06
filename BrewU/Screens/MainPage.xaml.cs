using BrewU.Objects;
using BrewU.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BrewU.Screens
{
    public sealed partial class MainPage : Page
    {
        BeerListViewModel _beersOfTheMonthViewModel;
        BeerListViewModel _whatsNewViewModel;

        public MainPage()
        {
            _beersOfTheMonthViewModel = new BeerListViewModel();
            _whatsNewViewModel = new BeerListViewModel();
            
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Location.Text = "Crabapple";
            Welcome.Text = string.Format("Welcome, {0}", App.User.DisplayName);
            OTMHub.DataContext = _beersOfTheMonthViewModel.BeerList;
            WhatsNewHub.DataContext = _whatsNewViewModel.BeerList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ReloadPage();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        private void HideBeerButton_Toggled(object sender, RoutedEventArgs e)
        {
            App.HideMyBeers = !App.HideMyBeers;

            if(!App.HideMyBeers)
            {
                HideBeerButton.Label = "Show my Beers";
            }

            else
            {
                HideBeerButton.Label = "Hide my Beers";
            }

            ReloadPage();
        }

        private void ReloadPage()
        {
            _beersOfTheMonthViewModel.ClearList();
            _whatsNewViewModel.ClearList();

            var request = new BeerListRequest
            {
                Format = 0,
                HideMyBeers = App.HideMyBeers,
                GroupID = (int)GroupType.BeerOfTheMonth,
                IsFromGroup = true,
                UserID = App.User.UserID
            };

            _beersOfTheMonthViewModel.LoadPage(App.User, request);

            request.GroupID = (int)GroupType.WhatsNew;

            _whatsNewViewModel.LoadPage(App.User, request);
        }

        private void LocationButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
