using System;

namespace OpenSesameV3
{
	public interface IOpenSesame
	{
	    string GrantAccess();
		string CardOnly();
		string Unlock();
		string Hold();
		void LOGMSG (string format);
		void LOGMSG (string format, params object[] args);
		string Authenticate(string username, string password);
		void SetLanguage(string p_language);
		void LoadFactorySettings();
		void SetVolume(float p_volume);
		void SetShakingForceThreshold(float p_gforce);
		void SetShakeCount(int p_shakecount);
		void SetShakeEnabled(bool p_shakeEnabled);
		void SetScreenDimensions(int height, int width);
		DatabaseAccess getDatabase() ;
	}
}
