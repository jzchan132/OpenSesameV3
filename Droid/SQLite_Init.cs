using System;
using Xamarin.Forms;
using System.IO;
using Android.Util;

namespace OpenSesameV3.Droid
{
	public class SQLite_Init
	{
		private const string sqliteFilename = "OpenSesame.db3";
		string documentsPath, path;

		public SQLite_Init()
		{
			documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			path = Path.Combine(documentsPath, sqliteFilename);
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
			Log.Debug("SQLite_Android", "CurrentDirectory = " + System.Environment.CurrentDirectory);
			Log.Debug("SQLite_Android", "path = " + path);
			{
				Log.Debug("SQLite_Android", "Copy database from Resource to local folder: " + path);
				var s = Android.App.Application.Context.Resources.OpenRawResource(Resource.Raw.OpenSesame);  // RESOURCE NAME ###
				FileStream writeStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
				ReadWriteStream(s, writeStream);
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
