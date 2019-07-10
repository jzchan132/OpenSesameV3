using Foundation;
using UIKit;

namespace OpenSesameV3.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			global::Xamarin.Forms.Forms.Init();

			int ScreenHeight = (int)(UIScreen.MainScreen.Bounds.Size.Height * UIScreen.MainScreen.Scale);
			int ScreenWidth = (int)(UIScreen.MainScreen.Bounds.Size.Width * UIScreen.MainScreen.Scale);
			LoadApplication(new App(ScreenHeight, ScreenWidth));

			return base.FinishedLaunching(uiApplication, launchOptions);
		}
	}
}
