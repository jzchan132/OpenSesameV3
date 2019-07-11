package md5e72bad741f9573ef9b4c0c87f2638d2e;


public class OpenSesameServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("OpenSesameV3.Droid.Services.OpenSesameServiceBinder, OpenSesameV3.Droid", OpenSesameServiceBinder.class, __md_methods);
	}


	public OpenSesameServiceBinder ()
	{
		super ();
		if (getClass () == OpenSesameServiceBinder.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.OpenSesameServiceBinder, OpenSesameV3.Droid", "", this, new java.lang.Object[] {  });
	}

	public OpenSesameServiceBinder (md5e72bad741f9573ef9b4c0c87f2638d2e.OpenSesameService p0)
	{
		super ();
		if (getClass () == OpenSesameServiceBinder.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.OpenSesameServiceBinder, OpenSesameV3.Droid", "OpenSesameV3.Droid.Services.OpenSesameService, OpenSesameV3.Droid", this, new java.lang.Object[] { p0 });
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
