using SQLite;

namespace OpenSesameV3
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
		void ResetFactorySettings();
	}
}
