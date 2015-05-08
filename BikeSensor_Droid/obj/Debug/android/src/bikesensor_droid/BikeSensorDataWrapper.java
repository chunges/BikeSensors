package bikesensor_droid;


public class BikeSensorDataWrapper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("BikeSensor_Droid.BikeSensorDataWrapper, BikeSensor_Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BikeSensorDataWrapper.class, __md_methods);
	}


	public BikeSensorDataWrapper () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BikeSensorDataWrapper.class)
			mono.android.TypeManager.Activate ("BikeSensor_Droid.BikeSensorDataWrapper, BikeSensor_Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
