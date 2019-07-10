using System;
using Android.Content;
using Android.Util;

using OpenSesameV3.Droid;

namespace OpenSesameV3.Droid.Services
{
	[BroadcastReceiver]
	public class NetworkStatusBroadcastReceiver : BroadcastReceiver
	{
		public event EventHandler ConnectionStatusChanged;

		public override void OnReceive (Context context, Intent intent)
		{
			MainActivity.Current.OpenSesameService.EventHandlers ();
		}
	}
}
