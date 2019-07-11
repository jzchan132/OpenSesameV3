package md5e72bad741f9573ef9b4c0c87f2638d2e;


public class OpenSesameServiceConnection
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.content.ServiceConnection
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onServiceConnected:(Landroid/content/ComponentName;Landroid/os/IBinder;)V:GetOnServiceConnected_Landroid_content_ComponentName_Landroid_os_IBinder_Handler:Android.Content.IServiceConnectionInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onServiceDisconnected:(Landroid/content/ComponentName;)V:GetOnServiceDisconnected_Landroid_content_ComponentName_Handler:Android.Content.IServiceConnectionInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("OpenSesameV3.Droid.Services.OpenSesameServiceConnection, OpenSesameV3.Droid", OpenSesameServiceConnection.class, __md_methods);
	}


	public OpenSesameServiceConnection ()
	{
		super ();
		if (getClass () == OpenSesameServiceConnection.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.OpenSesameServiceConnection, OpenSesameV3.Droid", "", this, new java.lang.Object[] {  });
	}

	public OpenSesameServiceConnection (md5e72bad741f9573ef9b4c0c87f2638d2e.OpenSesameServiceBinder p0)
	{
		super ();
		if (getClass () == OpenSesameServiceConnection.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.OpenSesameServiceConnection, OpenSesameV3.Droid", "OpenSesameV3.Droid.Services.OpenSesameServiceBinder, OpenSesameV3.Droid", this, new java.lang.Object[] { p0 });
	}


	public void onServiceConnected (android.content.ComponentName p0, android.os.IBinder p1)
	{
		n_onServiceConnected (p0, p1);
	}

	private native void n_onServiceConnected (android.content.ComponentName p0, android.os.IBinder p1);


	public void onServiceDisconnected (android.content.ComponentName p0)
	{
		n_onServiceDisconnected (p0);
	}

	private native void n_onServiceDisconnected (android.content.ComponentName p0);

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
