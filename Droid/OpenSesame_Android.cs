using System;
using OpenSesameV3;
using Xamarin.Forms;
using OpenSesameV3.Droid;
using OpenSesameV3.Droid.Services ;
using System.IO;
using Android.Util;

[assembly: Dependency (typeof (OpenSesame_Android))]

namespace OpenSesameV3.Droid
{
	public class OpenSesame_Android : IOpenSesame
	{

		OpenSesameService OpenSesameService ;
		public OpenSesame_Android ()
		{
			OpenSesameService = MainActivity.Current.OpenSesameService;
		}

		#region IOpenSesame implementation
		public string GrantAccess ()
		{
			return OpenSesameService.GrantAccess ();
		}

		public string CardOnly ()
		{
			return OpenSesameService.CardOnly ();
		}

		public string Unlock ()
		{
			return OpenSesameService.Unlock ();
		}

		public string Hold()
		{
			return OpenSesameService.Hold();
		}

		public void LOGMSG (string format)
		{
			OpenSesameService.LOGMSG (format);
		}

		public void LOGMSG (string format, params object[] args)
		{
			OpenSesameService.LOGMSG (format, args);
		}

		public string Authenticate(string username, string password)
		{
			return OpenSesameService.Authenticate(username, password);
		}

		public void SetLanguage(string p_language)
		{
			OpenSesameService.SetLanguage(p_language);
		}

		public void LoadFactorySettings()
		{
			OpenSesameService.LoadFactorySettings();
		}

		public void SetVolume(float p_volume)
		{
			OpenSesameService.SetVolume(p_volume);
		}

		public void SetShakingForceThreshold(float p_gforce)
		{
			OpenSesameService.SetShakingForceThreshold(p_gforce);
		}

		public void SetShakeCount(int p_shakecount)
		{
			OpenSesameService.SetShakeCount(p_shakecount);
		}

		public void SetShakeEnabled(bool p_shakeEnabled)
		{
			OpenSesameService.SetShakeEnabled(p_shakeEnabled);
		}

        public void SetScreenDimensions(int height, int width)
        {
            OpenSesameService.SetScreenDimensions(height, width);
        }

        public DatabaseAccess getDatabase()
		{
			return OpenSesameService.getDatabase ();
		}
		#endregion
	}
}
