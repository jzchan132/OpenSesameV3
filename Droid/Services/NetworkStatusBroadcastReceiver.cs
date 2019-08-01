using System;
using Android.Content;
using Android.Util;

using OpenSesameV3.Droid;

namespace OpenSesameV3.Droid.Services
{
	[BroadcastReceiver]
	public class NetworkStatusBroadcastReceiver : BroadcastReceiver
	{
        #pragma warning disable 67
        public event EventHandler ConnectionStatusChanged;
        #pragma warning restore 67

        public override void OnReceive (Context context, Intent intent)
		{
			MainActivity.Current.OpenSesameService.EventHandlers ();
		}
	}
}
