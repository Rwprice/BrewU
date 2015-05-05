using BrewU.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BrewU.Screens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { }

        private async void LoginButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Set the background color of the status bar, and DON'T FORGET to set the opacity!
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundOpacity = .5;
 
            // Set the text on the ProgressIndicator, and show it.
            statusBar.ProgressIndicator.Text = "Logging in...";
            await statusBar.ProgressIndicator.ShowAsync();

            Password.IsEnabled = false;
            Username.IsEnabled = false;
            LoginButton.IsEnabled = false;

            var user = await User.GetCookie(Username.Text, Password.Password);

            if (user != null)
            {
                App.User = user;
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }

            else
            {
                MessageDialog msgbox = new MessageDialog("Incorrect username or password.");
                await msgbox.ShowAsync();
            }

            Password.IsEnabled = true;
            Username.IsEnabled = true;
            LoginButton.IsEnabled = true;
            await statusBar.ProgressIndicator.HideAsync();
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as PasswordBox).Password = "";
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "";
        }
    }
}
