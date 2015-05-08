using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks; 
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Telephony;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.IO;
using System.Json;

namespace BikeSensor_Droid
{
    [Activity(Label = "BikeSensor_Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity,ILocationListener
    {
        Location currentLocation;
        LocationManager locationManager;

        TextView locationText;
        TextView latitudeText;
        TextView longtitudeText;
        TextView addressText;
        TextView imeiText;
        Button _btnUpdateLoc; 
        string locationProvider;

        private MobileServiceClient client;     
        private IMobileServiceTable<BikeSensorData> BikeSensorTable;
        private BikeSensorDataAdapter adapter; 

        const string applicationURL = @"https://hovikmastertesting.azure-mobile.net/";
        const string applicationKey = @"XwuqHwYFLWvlFusSBpoxCnZmIyLsGa15";
        const string localDbFilename = "localstore.db";     

        public String getUniqueID()
        {
            String myAndroidDeviceId = "";
            TelephonyManager mTelephony = (TelephonyManager)GetSystemService(Context.TelephonyService);
            myAndroidDeviceId = mTelephony.DeviceId;

            return myAndroidDeviceId;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);                          

            addressText = FindViewById<TextView>(Resource.Id.address_text);
            locationText = FindViewById<TextView>(Resource.Id.location_text);
            imeiText = FindViewById<TextView>(Resource.Id.imei_text);
            latitudeText = FindViewById<TextView>(Resource.Id.latitude_text);
            longtitudeText = FindViewById<TextView>(Resource.Id.location_text);             

            //Display IMEI as well                               
            imeiText.Text = getUniqueID();

            //CurrentPlatform.Init();
            //client = new MobileServiceClient(applicationURL, applicationKey);
            //await InitLocalStoreAsync();
            //BikeSensorTable = client.GetSyncTable<BikeSensorData>(); 

            _btnUpdateLoc = FindViewById<Button>(Resource.Id.btnUpdateLoc);
            _btnUpdateLoc.Click += btnUpdateLocation;

            CurrentPlatform.Init();
            client = new MobileServiceClient(applicationURL, applicationKey);
            BikeSensorTable = client.GetTable<BikeSensorData>();
            adapter = new BikeSensorDataAdapter(this, Resource.Layout.Main); 

            //Initialising the LocationManager to provide access to the system location services.
            //The LocationManager class will listen for GPS updates from the device and notify the application by way of events.
            locationManager = (LocationManager)GetSystemService(LocationService);

            //Define a Criteria for the best location provider
            Criteria criteriaForLocationService = new Criteria
            {
                //A constant indicating an approximate accuracy
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
                locationProvider = acceptableLocationProviders.First();
            else
                locationProvider = String.Empty;
        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }       

        public void btnUpdateLocation(object sender, EventArgs eventArgs)
        {
            try
            {                
                //imeiText = FindViewById<TextView>(Resource.Id.imei_text);
                //BikeSensorData bikesensordata = new BikeSensorData { location = locationText.Text.ToString()
                                                                     //,latitude = Convert.ToDecimal(latitudeText.Text)};
                //MobileService.GetTable<BikeSensorData>().InsertAsync(bikesensordata);
                //await BikeSensorTable.InsertAsync(bikesensordata);                               
                InsertBikeSensorData();                
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        [Java.Interop.Export()]
        private async void InsertBikeSensorData()
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView

            // Create a new item
            var item = new BikeSensorData
            {
                imeiId = imeiText.Text,
                location = locationText.Text,
                latitude = latitudeText.Text,
                longtitude = longtitudeText.Text
            };

            try
            {
                await BikeSensorTable.InsertAsync(item);
                var progressBar = (ProgressBar)FindViewById(Resource.Id.progressBar1);
                progressBar.Visibility = ViewStates.Visible; 
                adapter.Add(item);
                progressBar.Visibility = ViewStates.Gone;
                
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }       

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;

                if (currentLocation == null)
                    locationText.Text = "Unable to determine your location.";
                else
                {
                    locationText.Text = String.Format("{0},{1}", currentLocation.Latitude, currentLocation.Longitude);
                    longtitudeText.Text = Convert.ToString(currentLocation.Longitude);
                    latitudeText.Text = Convert.ToString(currentLocation.Latitude); 

                    Geocoder geocoder = new Geocoder(this);

                    //The Geocoder class retrieves a list of address from Google over the internet
                    IList<Address> addressList = geocoder.GetFromLocation(currentLocation.Latitude, currentLocation.Longitude, 10);

                    Address address = addressList.FirstOrDefault();

                    if (address != null)
                    {
                        StringBuilder deviceAddress = new StringBuilder();

                        for (int i = 0; i < address.MaxAddressLineIndex; i++)
                            deviceAddress.Append(address.GetAddressLine(i))
                                .AppendLine(",");

                        addressText.Text = deviceAddress.ToString();
                    }
                    else
                        addressText.Text = "Unable to determine the address.";
                }
            }
            catch
            {
                addressText.Text = "Unable to determine the address.";
            }
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            
        }

        void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }

        class ProgressHandler : DelegatingHandler
        {
            int busyCount = 0;

            public event Action<bool> BusyStateChange;

            #region implemented abstract members of HttpMessageHandler

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                //assumes always executes on UI thread
                if (busyCount++ == 0 && BusyStateChange != null)
                    BusyStateChange(true);

                var response = await base.SendAsync(request, cancellationToken);

                // assumes always executes on UI thread
                if (--busyCount == 0 && BusyStateChange != null)
                    BusyStateChange(false);

                return response;
            }

            #endregion

        }

    }


    //public class DeviceLocation
    //{
    //    public string IMEIId { get; set; }
    //    public string xy { get; set; }
    //}    
}

