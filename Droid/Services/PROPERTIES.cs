using System;

using Android.Content;

namespace OpenSesameV3.Droid.Services
{
	public class PROPERTIES
	{
		static readonly string ProfileName = "OpenSesameSerivce";

		public PROPERTIES ()
		{
		}

		public void SetString(string key, string value)
		{
			ISharedPreferences prefs = Android.App.Application.Context.GetSharedPreferences(ProfileName, FileCreationMode.Private);
			var editor = prefs.Edit();
			editor.PutString(key, value);
			editor.Commit();
		}

		public string GetString(string key, string defValue)
		{
			ISharedPreferences prefs = Android.App.Application.Context.GetSharedPreferences(ProfileName, FileCreationMode.Private);
			return prefs.GetString(key, defValue);
		}

	}
}
