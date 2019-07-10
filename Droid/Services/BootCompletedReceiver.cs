using Android.App;
using Android.Content;
using Android.Widget;

namespace OpenSesameV3.Droid.Services
{
	[BroadcastReceiver(Enabled = true)]
    [IntentFilter (new[] {Intent.ActionBootCompleted})]
    class BootCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive (Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted) 
            {
				Toast.MakeText (context, "Starting Open Sesame service...", ToastLength.Long).Show ();
				context.ApplicationContext.StartService(new Intent(context, typeof(OpenSesameService)));
				Toast.MakeText (context, "Open Sesame started.", ToastLength.Long).Show ();
            }
		}
    }
}
