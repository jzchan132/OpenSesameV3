using Xamarin.Forms;
using OpenSesameV3.iOS;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace OpenSesameV3.iOS
{
	public class SQLite_iOS : ISQLite
	{
		SQLite_Init si = null ;
		public SQLite_iOS ()
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