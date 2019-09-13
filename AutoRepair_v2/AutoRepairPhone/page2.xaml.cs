using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Maps;
//using BingMapsRESTService.Common.JSON;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml;
using Windows.Data.Xml.Dom;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using System.Runtime;
using System.Threading;
using System;
using System.Text;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using System.Windows;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Diagnostics;







// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace AutoRepairPhone
{
   

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class page2 : Page
    {


        double SearchLatitude = 12.9754;
        double SearchLongitude =79.1605 ;
        private shopsdata.restObj obj;
        double tolat;
        double tolong;
        double fromlat;
        double fromlong;
        string fromlocation =global.fromloc;
        string tolocation = global.toloc;
        int i = 1;
        

         

        public page2()
        {
            //getloc();
            this.InitializeComponent();
            MapService.ServiceToken = "WzC8t_4VSXw76YQjYvugLQ";
            MyMap.MapServiceToken = "WzC8t_4VSXw76YQjYvugLQ";
            
            
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            routemap();
            MyMap.Center =
                new Geopoint(new BasicGeoposition()
                {
                    Latitude = 12.9754,
                    Longitude = 79.1605
                });
                MyMap.ZoomLevel = 8;
                MyMap.LandmarksVisible = true;
         }

        private void geocode()
        {
            MapService.ServiceToken = "WzC8t_4VSXw76YQjYvugLQ";
            if(SearchLatitude != 0)
            {
                Geopoint mypoint = new Geopoint(new BasicGeoposition() { Latitude = SearchLatitude, Longitude = SearchLongitude });
                GetfromLocation(global.fromloc,mypoint);
                Gettolocation(global.toloc,mypoint);

                routemap();
            }
            else
            {
                ShowMessage("getting your Location wait..!");
            }
        }
        
        private async void ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public static async Task<MapLocation> GetPositionFromAddressAsync(string address)
        {
            // get current location to use as a search start point
            var locator = new Geolocator();
            var pos = await locator.GetGeopositionAsync();

            // convert current location to a GeoPoint
            var basicGeoposition = new BasicGeoposition();
            basicGeoposition.Latitude = pos.Coordinate.Point.Position.Latitude; ;
            basicGeoposition.Longitude = pos.Coordinate.Point.Position.Longitude;
            var point = new Geopoint(basicGeoposition);

            // using the address passed in as a parameter, search for MapLocations that match it
            var mapLocationFinderResult = await MapLocationFinder.FindLocationsAsync(address, point, 2);

            if (mapLocationFinderResult.Status == MapLocationFinderStatus.Success)
            {
                return mapLocationFinderResult.Locations[0];
            }

            return default(MapLocation);
        }
        
        private async void getloc()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 100;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                SearchLatitude = geoposition.Coordinate.Latitude;
                SearchLongitude = geoposition.Coordinate.Longitude;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    ShowMessage("location  is disabled in phone settings.");
                }
                else
                {
                    ShowMessage("Error Getting your Location");
                }
            }
           
        }

        public async void showpos()
        {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = new Geopoint(new BasicGeoposition() { Latitude = 12.9754, Longitude = 79.1605 });
            //MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = "Space Needle";
            MyMap.MapElements.Add(MapIcon1);
        }
   
        

        public async void GetfromLocation(string address, Geopoint queryHintPoint)
        {
            var result = await MapLocationFinder.FindLocationsAsync(address, queryHintPoint);
            // Get the coordinates
            if (result.Status == MapLocationFinderStatus.Success)
            {
                double fromlat = result.Locations[0].Point.Position.Latitude;
                double fromlong = result.Locations[0].Point.Position.Longitude;
            }
        }

        public async void Gettolocation(string address, Geopoint queryHintPoint)
        {
            var result = await MapLocationFinder.FindLocationsAsync(address, queryHintPoint);
            // Get the coordinates
            if (result.Status == MapLocationFinderStatus.Success)
            {
                double tolat = result.Locations[0].Point.Position.Latitude;
                double tolong = result.Locations[0].Point.Position.Longitude;
            }
        }

        public async void routeoriginal()
        {
            // Start at Microsoft in Redmond, Washington.
            BasicGeoposition startLocation = new BasicGeoposition();
            startLocation.Latitude = fromlat;
            startLocation.Longitude = fromlong;
            Geopoint startPoint = new Geopoint(startLocation);


            //GetLocation("chennai", startPoint);
            // End at the city of Seattle, Washington.
            BasicGeoposition endLocation = new BasicGeoposition();
            endLocation.Latitude = tolat;
            endLocation.Longitude = tolong;
            Geopoint endPoint = new Geopoint(endLocation);

            // Get the route between the points.
            MapRouteFinderResult routeResult =
                await MapRouteFinder.GetDrivingRouteAsync(
                startPoint,
                endPoint,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Yellow;
                viewOfRoute.OutlineColor = Colors.Black;

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                MyMap.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await MyMap.TrySetViewBoundsAsync(
                    routeResult.Route.BoundingBox,
                    null,
                    Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
        }

        public async void routemap()
        {
            string BingMapsKey = "KKjD0q35Vx6H5bEKB0Fi~zxFLw_EI-jdOpu0Q_NPeKw~AucWpk22LpUKQQtI3AFroBXwMy7c_Ow80Pqm_6m-59x0UDUiPirbvyRRcB8p7wiz";
            string DataSourceID = "38eef3deca3a48a88d1a88d64a328169";
            string bingMapsKey = BingMapsKey;

            if (SearchLatitude != 0)
            {
                
                
                
                string routeurl = string.Format("https://spatial.virtualearth.net/REST/v1/data/38eef3deca3a48a88d1a88d64a328169/stores2/RepairShop" +
                    "?spatialFilter=nearRoute('{0}','{1}')&$format=json&$select=*,__Distance&key={2}", fromlocation, tolocation, bingMapsKey);
                
                try
                {
                    
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(routeurl);
                    
                    string responsetext = await response.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<shopsdata.restObj>(responsetext);
                    

                    // Serialize our Product class into a string
                    // Changed to serialze the List
                    string jsonContents = JsonConvert.SerializeObject(obj);

                    // Get the app data folder and create or replace the file we are storing the JSON in.
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile textFile = await localFolder.CreateFileAsync("a.txt", CreationCollisionOption.ReplaceExisting);

                    // Open the file...
                    using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        // write the JSON string!
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(jsonContents);
                            await textWriter.StoreAsync();
                        }
                    }

                    {
                        //LocationCollection locations = new LocationCollection();
                        try
                        {
                            
                            foreach (AutoRepairPhone.shopsdata.Result l in obj.d.results)
                            {
                                
                                //Get the location of each result
                                Geopoint location = new Geopoint(new BasicGeoposition() { Latitude = l.Latitude, Longitude = l.Longitude });

                                //Create a pushpin each location

                                var jayway = new Geopoint(new BasicGeoposition() { Latitude = l.Latitude, Longitude = l.Longitude });
                                var youPin = CreatePin();
                                MyMap.Children.Add(youPin);
                                MapControl.SetLocation(youPin, jayway);
                                MapControl.SetNormalizedAnchorPoint(youPin, new Point(0.0, 1.0));
                                //string title = this.resourceLoader.GetString("Voisietequi");
                                MyMap.TrySetViewAsync(jayway, 15, 0, 0, MapAnimationKind.Bow);

                            }
                            //Pass the results to the item source of the GeocodeResult ListBox
                            MyMap.ZoomLevel = 14;
                            GeocodeResults.ItemsSource = obj.d.results;
                        }
                        catch
                        {
                            ShowMessage("no data available");
                        }
                    }

                    i = 1;


                }
                catch (Exception)
                {
                    ShowMessage("connect to internet and try again");

                }
            }

            else
            {
                ShowMessage("Getting Gps");
            }

        }

        private DependencyObject CreatePin()
        {  
            
            //Creating a Grid element.
            var myGrid = new Grid();
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.RowDefinitions.Add(new RowDefinition());
            myGrid.Background = new SolidColorBrush(Colors.Transparent);

            //Creating a Ellipse
            var myRectangle = new Ellipse{ Fill = new SolidColorBrush(Colors.DarkOrange), Height = 20, Width = 20 };
            myRectangle.SetValue(Grid.RowProperty, 0);
            myRectangle.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            myGrid.Children.Add(myRectangle);

            var mytext = new TextBlock
            {
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center
            };

            mytext.Text = i.ToString();
            mytext.FontSize = 12;
            i++;

            myGrid.Children.Add(mytext);
            return myGrid;
        }

        public async void storejson()
        {
            
                string BingMapsKey = "KKjD0q35Vx6H5bEKB0Fi~zxFLw_EI-jdOpu0Q_NPeKw~AucWpk22LpUKQQtI3AFroBXwMy7c_Ow80Pqm_6m-59x0UDUiPirbvyRRcB8p7wiz";
                string DataSourceID = "38eef3deca3a48a88d1a88d64a328169";
                string dataSourceName = "stores2";
                string dataEntityName = "RepairShop";
                string accessId = DataSourceID;
                string bingMapsKey = BingMapsKey;
                double Radius = 100; // km
                if (SearchLatitude != 0)
                {
                    //testname.Text = SearchLatitude.ToString();
                    string shopsurl = string.Format("http://spatial.virtualearth.net/REST/v1/data/{0}/{1}/{2}" +
                                                                 "?spatialFilter=nearby({3},{4},{5})&$format=json&$select=*,__Distance&key={6}", accessId, dataSourceName,
                                                                                     dataEntityName, SearchLatitude, SearchLongitude, Radius, bingMapsKey);

                    try
                    {
                        HttpClient client = new HttpClient();
                        HttpResponseMessage response = await client.GetAsync(shopsurl);
                        response.EnsureSuccessStatusCode();
                        string responsetext = await response.Content.ReadAsStringAsync();
                        obj = JsonConvert.DeserializeObject<shopsdata.restObj>(responsetext);

                    }
                    catch (Exception)
                    {
                        ShowMessage("connect to internet and try again");

                    }

                    // Serialize our Product class into a string
                    // Changed to serialze the List
                    string jsonContents = JsonConvert.SerializeObject(obj);

                    // Get the app data folder and create or replace the file we are storing the JSON in.
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile textFile = await localFolder.CreateFileAsync("a.txt", CreationCollisionOption.ReplaceExisting);

                    // Open the file...
                    using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        // write the JSON string!
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(jsonContents);
                            await textWriter.StoreAsync();
                        }
                    }
                }
                else
                {
                    ShowMessage("Try After Sometime, Getting Your present Location");
                }
            


        }

        public async void Job()
        {

            //storejson();

            //Deserialize json from file "a.txt"
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                // Getting JSON from file if it exists, or file not found exception if it does not
                StorageFile textFile = await localFolder.GetFileAsync("a.txt");
                using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                {

                    // Read text stream 
                    using (DataReader textReader = new DataReader(textStream))
                    {
                        //get size
                        uint textLength = (uint)textStream.Size;
                        await textReader.LoadAsync(textLength);
                        // read it
                        string jsonContents = textReader.ReadString(textLength);
                        // deserialize back to our products!
                        //I only had to change this following line in this function
                        obj = JsonConvert.DeserializeObject<shopsdata.restObj>(jsonContents);
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = "Exception: " + ex.Message;
                ShowMessage(mes);
            }

            //DeSerialization Complete
            //Sending data to List Box
            try
            {
                
                Debug.WriteLine("job done");
           

                foreach (AutoRepairPhone.shopsdata.Result l in obj.d.results)
                {

                    //Get the location of each result
                    Geopoint location = new Geopoint(new BasicGeoposition() { Latitude = l.Latitude, Longitude = l.Longitude });

                    //Create a pushpin each location

                    var jayway = new Geopoint(new BasicGeoposition() { Latitude = l.Latitude, Longitude = l.Longitude });
                    var youPin = CreatePin();
                    MyMap.Children.Add(youPin);
                    MapControl.SetLocation(youPin, jayway);
                    MapControl.SetNormalizedAnchorPoint(youPin, new Point(0.0, 1.0));
                     //string title = this.resourceLoader.GetString("Voisietequi");
                    MyMap.TrySetViewAsync(jayway, 15, 0, 0, MapAnimationKind.Bow);	

                }               
                //Pass the results to the item source of the GeocodeResult ListBox
                MyMap.ZoomLevel = 14;
                GeocodeResults.ItemsSource = obj.d.results;
            }
            catch (Exception)
            {
                ShowMessage("No Stored Data Available Plz start internet");
            }


            i = 1;
        }

        private void GeocodeResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var listBox = sender as ListBox;

            if (listBox.SelectedItems.Count > 0)
            {
                //Get the Selected Item
                var item = listBox.Items[listBox.SelectedIndex]
                       as shopsdata.Result;

                string result = item.Name + ",/r/n" + item.AddressLine + "," + item.StoreType;

                var point = new Geopoint(new BasicGeoposition() { Latitude = item.Latitude, Longitude = item.Longitude });
                MyMap.TrySetViewAsync(point, 12, 0, 0, MapAnimationKind.Bow);
                ShowMessage(result);

                
            }

        }

        private void clickback(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void forwardclick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(page3));
        }

        private void showpin(object sender, RoutedEventArgs e)
        {

            storejson();
            Job();

        }

        private void routeshops(object sender, RoutedEventArgs e)
        {
            routemap();
        }
                
     }
}