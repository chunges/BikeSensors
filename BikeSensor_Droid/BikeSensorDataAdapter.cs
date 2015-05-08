using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BikeSensor_Droid; 

namespace BikeSensor_Droid
{
    public class BikeSensorDataAdapter :BaseAdapter<BikeSensorData>
    {
        private Activity activity;
		private int layoutResourceId;
        private List<BikeSensorData> items = new List<BikeSensorData>();

        // Constructor
		public BikeSensorDataAdapter(Activity activity, int layoutResourceId)
		{
			this.activity = activity;
			this.layoutResourceId = layoutResourceId;
		}

        public void Add(BikeSensorData item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        #region implemented abstract members of BaseAdapter
        
        public override int Count
        {
            get { return items.Count; }
        }

        public override BikeSensorData this[int position]
        {
            get { return items[position]; }
        }

        #endregion

        public override long GetItemId(int position)
        {
            return position;  
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return convertView; 
        }
    }
}