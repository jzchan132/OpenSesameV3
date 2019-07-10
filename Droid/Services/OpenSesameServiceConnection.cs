using System;

using Android.Content;
using Android.OS;
using Android.Util;

namespace OpenSesameV3.Droid.Services
{
	public class OpenSesameServiceConnection : Java.Lang.Object, IServiceConnection
	{
		public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate {};

		public OpenSesameServiceBinder Binder
		{
			get { return this.binder; }
			set { this.binder = value; }
		}
		protected OpenSesameServiceBinder binder;

		public OpenSesameServiceConnection (OpenSesameServiceBinder binder)
		{
			if (binder != null) {
				this.binder = binder;
			}
		}

		// This gets called when a client tries to bind to the Service with an Intent and an 
		// instance of the ServiceConnection. The system will locate a binder associated with the 
		// running Service 
		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			OpenSesameServiceBinder serviceBinder = service as OpenSesameServiceBinder;
			if (serviceBinder != null) 
			{
				this.binder = serviceBinder;
				this.binder.IsBound = true;
				Log.Debug ( "ServiceConnection", "OnServiceConnected Called" );
				this.ServiceConnected(this, new ServiceConnectedEventArgs () { Binder = service } );
				serviceBinder.Service.StartUpdates();
			}
		}
			
		// This will be called when the Service unbinds, or when the app crashes
		public void OnServiceDisconnected (ComponentName name)
		{
			this.binder.IsBound = false;
			Log.Debug ( "ServiceConnection", "Service unbound" );
		}
	}
}

