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
using Windows.UI.Popups;
using System.Net.Http;
using Windows.Data.Xml.Dom;
using Newtonsoft.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Services.Maps;
using System.Threading;
using System;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace AutoRepairPhone
{
    // <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class page3 : Page
    {
        private shopsdata.restObj obj;
        double SearchLatitude  = 12.9754;//global.SearchLatitude; //
        double SearchLongitude = 79.1605;//global.SearchLongitude;//
        public page3()
        {
            
            this.InitializeComponent();
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MapService.ServiceToken = "WzC8t_4VSXw76YQjYvugLQ";
            test.Text = SearchLatitude.ToString();
            //getloc();
            test.Text = SearchLatitude.ToString();   

        }

        

        private async Task getloc()
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

                    SearchLatitude = geoposition.Coordinate.Latitude;
                    SearchLongitude = geoposition.Coordinate.Longitude;
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult == 0x80004004)
                    {
                        // the application does not have the right capability or the location master switch is off
                        ShowMessage( "location  is disabled in phone settings.");
                    }
                    //else
                    {
                        // something else happened acquring the location
                    }
                }
                test.Text = SearchLatitude.ToString();

                return ;
            
        }

        private void gethelp(object sender, RoutedEventArgs e)
        {

            Job();

        }

        private async void shopsset(object sender, RoutedEventArgs e)
        {
           // getloc();

            Debug.WriteLine("Shopsset Start");
            string BingMapsKey = "KKjD0q35Vx6H5bEKB0Fi~zxFLw_EI-jdOpu0Q_NPeKw~AucWpk22LpUKQQtI3AFroBXwMy7c_Ow80Pqm_6m-59x0UDUiPirbvyRRcB8p7wiz";
            string DataSourceID = "38eef3deca3a48a88d1a88d64a328169";
            string dataSourceName = "stores2";
            string dataEntityName = "RepairShop";
            string accessId = DataSourceID;
            string bingMapsKey = BingMapsKey;
            double Radius = 100; // km

        
            if (SearchLatitude != 0)
            {

                test2.Text = SearchLatitude.ToString();
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

                Debug.WriteLine("Shopsset stop");
            }
            else
            {
                ShowMessage("Try After Sometime, Getting Your present Location");
            }
        }
        private async void ShowMessage(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public async void Job()
        {
           

            Debug.WriteLine("job Start");
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
                GeocodeResults.ItemsSource = obj.d.results;
                Debug.WriteLine("job done");
            }
            catch(Exception)
            {
                ShowMessage("No Stored Data Available Plz start internet");
            }
        }

        private void GeocodeResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var listBox = sender as ListBox;

            if (listBox.SelectedItems.Count > 0)
            {
                //Get the Selected Item
                var item = listBox.Items[listBox.SelectedIndex]
                       as shopsdata.Result;

                string result = item.Name + ",\n" + item.AddressLine + ",\n" + item.StoreType + ",\n" + item.__Distance;

                ShowMessage(result);


            }

        }



    }

}
