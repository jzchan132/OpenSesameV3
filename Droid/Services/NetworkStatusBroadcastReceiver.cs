using System;
using Android.Content;
using Android.Util;

using OpenSesameV3.Droid;

namespace OpenSesameV3.Droid.Services
{
	[BroadcastReceiver]
	public class NetworkStatusBroadcastReceiver : BroadcastReceiver
	{
        protected virtual void OnConnectionStatusChanged(EventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        public event EventHandler ConnectionStatusChanged;

		public override void OnReceive (Context context, Intent intent)
		{
			MainActivity.Current.OpenSesameService.EventHandlers ();
		}
	}
}
