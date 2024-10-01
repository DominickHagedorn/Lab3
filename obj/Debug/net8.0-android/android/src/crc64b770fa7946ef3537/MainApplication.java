package crc64b770fa7946ef3537;


public class MainApplication
	extends crc6488302ad6e9e4df1a.MauiApplication
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
	}

	public MainApplication ()
	{
		mono.MonoPackageManager.setContext (this);
	}

	private java.util.ArrayList refList;
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
