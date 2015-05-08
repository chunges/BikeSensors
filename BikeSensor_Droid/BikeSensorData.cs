using System;
using Newtonsoft.Json;

namespace BikeSensor_Droid
{
    public class BikeSensorData
    {
      
        public string id { get; set; }

        [JsonProperty(PropertyName = "imeiId")]
        public string imeiId { get; set; }     

        [JsonProperty(PropertyName = "location")]
        public string location { get; set; }      
        
        [JsonProperty(PropertyName = "latitude")]
        public string latitude { get; set; }

        [JsonProperty(PropertyName = "longtitude")]
        public string longtitude { get; set; } 
    }

    public class BikeSensorDataWrapper: Java.Lang.Object
    {
        public BikeSensorDataWrapper(BikeSensorData item)
        {
            BikeSensorData = item;
        }

        public BikeSensorData BikeSensorData { get; private set; }
    }
}