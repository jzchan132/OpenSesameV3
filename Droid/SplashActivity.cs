using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace OpenSesameV3.Droid
{
	[Activity(Label = "Open Sesame", Icon = "@drawable/icon", Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
			 MainLauncher = true, //Set it as boot activity
	         ScreenOrientation = ScreenOrientation.Portrait,
			 NoHistory = true)] //Doesn't place it in back stack
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.StartActivity(typeof(MainActivity));
		}
	}
}
