using Xamarin.Forms;
using OpenSesameV3.Droid;

[assembly: Dependency(typeof(SQLite_Android))]

namespace OpenSesameV3.Droid
{
	public class SQLite_Android : ISQLite
	{
		SQLite_Init si = null ;
		public SQLite_Android ()
		{
			si = new SQLite_Init ();
		}

		#region ISQLite implementation
		public SQLite.SQLiteConnection GetConnection ()
		{
			return si.getConnection ();
		}
		public void ResetFactorySettings()
		{
			si.ResetFactorySettings();
		}
		#endregion
	}
}
