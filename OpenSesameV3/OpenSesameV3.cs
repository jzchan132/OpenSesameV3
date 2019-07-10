using Xamarin.Forms;

namespace OpenSesameV3
{
	public class App : Application
	{
		static OpenSesameDatabase database;
		GridPage mp ;

        public App(int height, int width)
		{
			mp = new GridPage(Database, height, width);
			MainPage = mp;
		}

		public static OpenSesameDatabase Database 
		{
			get 
			{ 
				if (database == null) 
				{
					database = new OpenSesameDatabase ();
				}
				return database; 
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
			Database.GetMessagesByLanguage (Database.GetPropertyStrValue ("language"));
			mp.CreatePage();
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps (Home button was pressed)
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

