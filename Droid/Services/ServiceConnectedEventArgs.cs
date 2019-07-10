using System;
using Android.OS;

namespace OpenSesameV3.Droid.Services
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}