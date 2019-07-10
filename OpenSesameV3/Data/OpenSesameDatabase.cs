using Xamarin.Forms;

namespace OpenSesameV3
{
	public class OpenSesameDatabase : DatabaseAccess
	{
		public OpenSesameDatabase ()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();
		}

		public void ResetFactorySettings()
		{
			DependencyService.Get<ISQLite>().ResetFactorySettings();
		}
	}
}

