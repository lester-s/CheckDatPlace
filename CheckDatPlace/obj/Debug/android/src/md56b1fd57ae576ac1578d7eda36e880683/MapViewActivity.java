package md56b1fd57ae576ac1578d7eda36e880683;


public class MapViewActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("CheckDatPlace.MapViewActivity, CheckDatPlace, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MapViewActivity.class, __md_methods);
	}


	public MapViewActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MapViewActivity.class)
			mono.android.TypeManager.Activate ("CheckDatPlace.MapViewActivity, CheckDatPlace, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
