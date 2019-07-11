package md5e72bad741f9573ef9b4c0c87f2638d2e;


public class ShakeDetector
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.hardware.SensorEventListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAccuracyChanged:(Landroid/hardware/Sensor;I)V:GetOnAccuracyChanged_Landroid_hardware_Sensor_IHandler:Android.Hardware.ISensorEventListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onSensorChanged:(Landroid/hardware/SensorEvent;)V:GetOnSensorChanged_Landroid_hardware_SensorEvent_Handler:Android.Hardware.ISensorEventListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("OpenSesameV3.Droid.Services.ShakeDetector, OpenSesameV3.Droid", ShakeDetector.class, __md_methods);
	}


	public ShakeDetector ()
	{
		super ();
		if (getClass () == ShakeDetector.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.ShakeDetector, OpenSesameV3.Droid", "", this, new java.lang.Object[] {  });
	}

	public ShakeDetector (float p0, float p1, int p2, boolean p3)
	{
		super ();
		if (getClass () == ShakeDetector.class)
			mono.android.TypeManager.Activate ("OpenSesameV3.Droid.Services.ShakeDetector, OpenSesameV3.Droid", "System.Single, mscorlib:System.Single, mscorlib:System.Int32, mscorlib:System.Boolean, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void onAccuracyChanged (android.hardware.Sensor p0, int p1)
	{
		n_onAccuracyChanged (p0, p1);
	}

	private native void n_onAccuracyChanged (android.hardware.Sensor p0, int p1);


	public void onSensorChanged (android.hardware.SensorEvent p0)
	{
		n_onSensorChanged (p0);
	}

	private native void n_onSensorChanged (android.hardware.SensorEvent p0);

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
