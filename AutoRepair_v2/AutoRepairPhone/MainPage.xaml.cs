using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.Services.Maps;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace AutoRepairPhone
{

    

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            getloc();
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private async void ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private async void getloc()
        {
            MapService.ServiceToken = "WzC8t_4VSXw76YQjYvugLQ";


            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                global.SearchLatitude = geoposition.Coordinate.Latitude;
                global.SearchLongitude = geoposition.Coordinate.Longitude;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    ShowMessage("location  is disabled in phone settings.");
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void gotopage2(object sender, RoutedEventArgs e)
        {
            


        }

        private void getshops(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(page3));
        }

        private void RouteBtn_Click(object sender, RoutedEventArgs e)
        {
            string from = FromTbx.Text;
            string to = ToTbx.Text;
            

            if (!string.IsNullOrWhiteSpace(from))
            {
                if (!string.IsNullOrWhiteSpace(to))
                {
                    global.fromloc = from;
                    global.toloc = to;

                    Frame.Navigate(typeof(page2));

                }
                else
                {
                    ShowMessage("Invalid ‘To’ location.");
                }
            }
            else
            {
                ShowMessage("Invalid ‘From’ location.");
            }

        }
    }
}
