using System;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

using OpenSesameV3.Droid.Services;

namespace OpenSesameV3.Droid
{
	[Activity(Label = "Open Sesame", Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public event EventHandler<ServiceConnectedEventArgs> OpenSesameServiceConnected = delegate { };
		protected readonly string logTag = "MainActivity";
		protected OpenSesameServiceConnection ServiceConnection;

		public static MainActivity Current
		{
			get { return current; }
		}
		private static MainActivity current;

		public static App OpenSesameApp
		{
			get { return app; }
		}
		private static App app;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			current = this;

			var metrics = Resources.DisplayMetrics;
			int ScreenWidth = metrics.WidthPixels;
			int ScreenHeight = metrics.HeightPixels;

			app = new App(ScreenHeight, ScreenWidth);
			LoadApplication(app);
			StartOpenSesameService();

			BroadcastReceiver receiver = new BootCompletedReceiver();
			RegisterReceiver(receiver, new IntentFilter(Intent.ActionBootCompleted));
		}

		public void StartOpenSesameService()
		{
			new Task(() =>
		    {
				// start our main service
				Log.Debug(logTag, "Calling StartService");
			    Android.App.Application.Context.StartService(new Intent(Android.App.Application.Context, typeof(OpenSesameService)));

				// create a new service connection so we can get a binder to the service
				this.ServiceConnection = new OpenSesameServiceConnection(null);

				// this event will fire when the Service connectin in the OnServiceConnected call 
				this.ServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
			    {
				    Log.Debug(logTag, "Service Connected");
					// we will use this event to notify MainActivity when to start updating the UI
					this.OpenSesameServiceConnected(this, e);
			    };

				// bind our service (Android goes and finds the running service by type, and puts a reference
				// on the binder to that service)
				// The Intent tells the OS where to find our Service (the Context) and the Type of Service
				// we're looking for (OpenSesameService)
				Intent OpenSesameServiceIntent = new Intent(Android.App.Application.Context, typeof(OpenSesameService));
			    Log.Debug(logTag, "Calling service binding");

				// Finally, we can bind to the Service using our Intent and the ServiceConnection we
				// created in a previous step.
				Android.App.Application.Context.BindService(OpenSesameServiceIntent, ServiceConnection, Bind.AutoCreate);
		    }).Start();
			Log.Debug(logTag, "OpenSesameService started.");
		}

		public OpenSesameService OpenSesameService
		{
			get
			{
				if (this.ServiceConnection.Binder == null)
					throw new Exception("Service not bound yet");
				// note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
				return this.ServiceConnection.Binder.Service;
			}
		}

	}
}

