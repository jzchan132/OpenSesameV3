using System;
using Xamarin.Forms;
using System.IO;

namespace OpenSesameV3.iOS
{
	public class SQLite_Init
	{
		private const string sqliteFilename = "OpenSesame.db3";
		string documentsPath, path;

		public SQLite_Init()
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folderr
			string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
			path = Path.Combine(libraryPath, sqliteFilename);
		}

		public SQLite.SQLiteConnection getConnection()
		{
			if (!File.Exists(path))
				ResetFactorySettings();

			var conn = new SQLite.SQLiteConnection(path);
			return conn;
		}

		public void ResetFactorySettings()
		{
			System.Diagnostics.Debug.WriteLine("SQLite_iOS CurrentDirectory = " + System.Environment.CurrentDirectory);
			System.Diagnostics.Debug.WriteLine("SQLite_iOS path = " + path);
			{
				System.Diagnostics.Debug.WriteLine("SQLite_iOS  Copy database from Resource to local folder: " + path);
				var existingDb = Foundation.NSBundle.MainBundle.PathForResource("raw/OpenSesame", ".db3"); // RESOURCE NAME ###
				File.Copy(existingDb, path);
			}
		}

		/// <summary>
		/// helper method to get the database out of /raw/ and into the user filesystem
		/// </summary>
		void ReadWriteStream(Stream readStream, Stream writeStream)
		{
			int Length = 256;
			Byte[] buffer = new Byte[Length];
			int bytesRead = readStream.Read(buffer, 0, Length);
			// write the required bytes
			while (bytesRead > 0)
			{
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, Length);
			}
			readStream.Close();
			writeStream.Close();
		}

	}
}
