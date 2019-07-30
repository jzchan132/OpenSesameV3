package md5e72bad741f9573ef9b4c0c87f2638d2e;


public class NetworkStatusBroadcastReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("OpenSesameV3.Droid.Services.NetworkStatusBroadcastReceiver, OpenSesameV3.Droid", NetworkStatusBroadcastReceiver.class, __md_methods);
	}


	public NetworkStatusBroadcastReceiver ()
	{
		super ();
		if (getClass () == NetworkStatusBroadcastReceiver.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.NetworkStatusBroadcastReceiver, OpenSesameV3.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
