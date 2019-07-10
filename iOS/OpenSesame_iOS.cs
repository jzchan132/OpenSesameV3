using Xamarin.Forms;
using OpenSesameV3.iOS;
using OpenSesameV3.iOS.Services;

[assembly: Dependency (typeof (OpenSesame_iOS))]

namespace OpenSesameV3.iOS
{
	public class OpenSesame_iOS : IOpenSesame
	{
		public OpenSesameService oss;

		public OpenSesame_iOS ()
		{
			oss = new OpenSesameService();
		}

		#region IOpenSesame implementation
		public string GrantAccess ()
		{
			return oss.GrantAccess ();
		}

		public string CardOnly ()
		{
			return oss.CardOnly ();
		}

		public string Unlock ()
		{
			return oss.Unlock ();
		}

		public string Hold()
		{
			return oss.Hold();
		}

		public void LOGMSG (string format)
		{
			oss.LOGMSG (format);
		}

		public void LOGMSG (string format, params object[] args)
		{
			oss.LOGMSG (format, args);
		}

		public string Authenticate(string username, string password)
		{
			return oss.Authenticate(username, password);
		}

		public void SetLanguage(string p_language)
		{
			oss.SetLanguage(p_language);
		}

		public void LoadFactorySettings()
		{
			oss.LoadFactorySettings();
		}

		public void SetVolume(float p_volume)
		{
			oss.SetVolume(p_volume);
		}

		public void SetShakingForceThreshold(float p_gforce)
		{
			oss.SetShakingForceThreshold(p_gforce);
		}

		public void SetShakeCount(int p_shakecount)
		{
			oss.SetShakeCount(p_shakecount);
		}

		public void SetShakeEnabled(bool p_shakeEnabled)
		{
			oss.SetShakeEnabled(p_shakeEnabled);
		}

		public void SetScreenDimensions(int height, int width)
		{
			oss.SetScreenDimensions(height, width);
		}

		public DatabaseAccess getDatabase()
		{
			return oss.getDatabase ();
		}
		#endregion
	}
}
