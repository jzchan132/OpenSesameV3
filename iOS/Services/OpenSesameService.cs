using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using AudioToolbox;
using Foundation;
using AVFoundation;
using UIKit;
using System.Drawing;
using CoreGraphics;
using WatchKit;
using SystemConfiguration;
using CoreFoundation;

namespace OpenSesameV3.iOS.Services
{
	public class OpenSesameService
	{
		readonly string LogTag = "OpenSesameService";
		float m_volume = 1.0f;

		public OpenSesameService()
		{
			StartUpdates();
		}

		public void SetVolume(float p_volume)
		{
			m_volume = p_volume;
			PlaySound("beep1.mp3");
		}
		public void SetShakingForceThreshold(float p_gforce)
		{
		}
		public void SetShakeCount(int p_shakecount)
		{
		}
		public void SetShakeEnabled(bool p_shakeEnabled)
		{
		}


		String m_username = null ;
		String m_sessionKey = null ;
		String m_language = null ;
		String m_role = null ;
		//String IPRange = null;
		String UserPort = null ;
		String AdminPort = null ;
		String port = null ;
		//int m_WiFiTimeout = 60;

		public DatabaseAccess getDatabase()
		{
			return Database;
		}
		DatabaseAccess Database = null;


		public void StartUpdates () 
		{
			PlaySound("beep1.mp3");
			Database = new DatabaseAccess (new SQLite_Init ().getConnection ());

			m_username = Database.GetPropertyStrValue ("username");
			m_sessionKey = Database.GetPropertyStrValue ("sessionKey");
			m_language = Database.GetPropertyStrValue ("language");
			m_role = Database.GetPropertyStrValue ("role");
			LOGMSG ("username = " + m_language + " " + m_username+" "+ m_sessionKey+" "+m_role);

			//IPRange = Database.GetPropertyStrValue ("IPRange");
			UserPort = Database.GetPropertyStrValue ("UserPort");
			AdminPort = Database.GetPropertyStrValue ("AdminPort");
			if (m_role.Equals ("admin"))
				port = AdminPort;
			else
				port = UserPort;
			m_volume = Database.GetPropertyIntValue("Volume") * 0.01f;
			//m_WiFiTimeout = Database.GetPropertyIntValue("WiFiTimeout");

			LOGMSG("GForceThreshold=" + Database.GetPropertyIntValue("GForceThreshold"));
        }

		//bool WiFiDisconnectWarning = true;
		//DateTime WiFiConnectDateTime = DateTime.Now;
		//DateTime WiFiDisconnectDateTime = DateTime.Now;

		public enum NetworkStatus
		{
			NotReachable,
			ReachableViaCarrierDataNetwork,
			ReachableViaWiFiNetwork
		}
		NetworkReachability adHocWiFiNetworkReachability;
		public event EventHandler ReachabilityChanged;

		void OnChange(NetworkReachabilityFlags flags)
		{
			var h = ReachabilityChanged;
			if (h != null)
				h(null, EventArgs.Empty);
		}
		public bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
		{
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
				|| (flags & NetworkReachabilityFlags.IsWWAN) != 0;

			return isReachable && noConnectionRequired;
		}
		public bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
		{
			if (adHocWiFiNetworkReachability == null)
			{
				adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte[] { 192, 168, 131, 0 }));
				adHocWiFiNetworkReachability.SetNotification(OnChange);
				adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}

			return adHocWiFiNetworkReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
		}
		public NetworkStatus LocalWifiConnectionStatus()
		{
			NetworkReachabilityFlags flags;
			if (IsAdHocWiFiNetworkAvailable(out flags))
				if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
					return NetworkStatus.ReachableViaWiFiNetwork;

			return NetworkStatus.NotReachable;
		}

		private async Task WaitAndExecute(int milisec, Action actionToExecute)
		{
			await Task.Delay(milisec);
			actionToExecute();
		}

		public String NOW()
		{
			return DateTime.Now.ToString("HH:mm:ss");
		}

		public String DBGMSG (string format)
		{
			return DBGMSG ("{0}", format);
		}
		public String DBGMSG (string format, params object[] args)
		{
			String s = NOW() + " " + String.Format (format, args);
			System.Diagnostics.Debug.WriteLine(LogTag + " " + s);
			return s;
		}

		public void LOGMSG (string format)
		{
			LOGMSG ("{0}", format);
		}

		public void LOGMSG (string format, params object[] args)
		{
			DBGMSG (format, args);
		}

		private void PlaySound(string filename)
		{
			NSUrl url;
			url = NSUrl.FromFilename("raw/" + filename);

			AVAudioPlayer soundEffect;
			NSError err;
			soundEffect = new AVAudioPlayer(url, "wav", out err);
			soundEffect.Volume = m_volume;
			soundEffect.FinishedPlaying += delegate
			{
				soundEffect = null;
			};
			soundEffect.NumberOfLoops = 0;
			soundEffect.Play();
		}

		public string ShakeAccess(bool vibrate)
		{
			LOGMSG ("ShakeAccess");
			string ErrorCode = Request("00", "Grant Access", m_username, m_sessionKey, UserPort);
			if (ErrorCode.Equals("S01"))
			{
				if (vibrate)
				{
					Vibrate(900);
					System.Threading.Thread.Sleep(1000);
					Vibrate(300);
					System.Threading.Thread.Sleep(500);
					Vibrate(300);
					System.Threading.Thread.Sleep(500);
					Vibrate(300);
				}
			}
			else
			{
				PlayWiFiDisconnected();
			}

			return ErrorCode;
		}

		public string GrantAccess()
		{
			LOGMSG ("GrantAccess");
			string ErrorCode = Request ("00", "Grant Access", m_username, m_sessionKey, port);
			DoorAction (ErrorCode);
			return ErrorCode;
		}

		public string CardOnly()
		{
			LOGMSG ("CardOnly");
			string ErrorCode = Request ("00", "Card Only", m_username, m_sessionKey, port);
			DoorAction (ErrorCode);
			return ErrorCode;
		}

		public string Unlock()
		{
			LOGMSG ("Unlock");
			string ErrorCode = Request ("00", "Unlock", m_username, m_sessionKey, port);
			DoorAction (ErrorCode);
			return ErrorCode;
		}

		public string Hold()
		{
			LOGMSG ("Hold");
			string ErrorCode = Request ("09", "Unlock", m_username, m_sessionKey, port);
			DoorAction (ErrorCode);
			return ErrorCode;
		}

		private void Vibrate(int duration)
		{
			if (Database.GetPropertyBoolValue("Vibrate"))
			{
				SystemSound.Vibrate.PlayAlertSound();
			}
		}

		private void PlayWiFiDisconnected()
		{
			Vibrate(300);
			PlaySound("WiFiDisconnected.mp3");
		}

		private void PlayWiFiDisconnectedWarning()
		{
			LOGMSG("PlayWiFiDisconnectedWarning");
			Vibrate(300);
			PlaySound("OnDestroy.mp3");
		}

		private void PlayWiFiConnected()
        {
			LOGMSG("PlayWiFiConnected");
			Vibrate(300);
			PlaySound("beep4.mp3");
		}

        private void PlayCBCMGWiFiConnected()
        {
			LOGMSG("PlayCBCMGWiFiConnected");
            Vibrate(300);
			PlaySound("beep4.mp3");
		}

		private void DoorAction(string ErrorCode)
		{
			if (ErrorCode.Equals("S01"))
			{
				Vibrate(300);
				PlaySound("DoorAction.mp3");
			}
			else
			{
				PlayWiFiDisconnected();
			}
		}

		public string Request(String ID, String action, String username,  String sessionKey, String port)
		{
			String ErrorCode = "E00";
			if (Database.GetPropertyBoolValue("ProductionMode", "Production"))
			{
				string url = Database.GetPropertyStrValue("URL") + ":" + port + "/" + Database.GetPropertyStrValue("PAGE") + "?ID=" + ID + "&request=" + action + "&name=" + username + "&sessionKey=" + sessionKey;
				string responseString = RESTWebService(url);
				if (responseString.Equals("")) ErrorCode = "S01";
			}
			else
				ErrorCode = "S01";
			LOGMSG("Request ErrorCode=" + ErrorCode);
			return ErrorCode;
		}

		private string RESTWebService(string url)
		{
			LOGMSG("url=" + url);
			String responseString = null;
			try
			{
				System.Uri uri = new System.Uri(url);
				HttpWebRequest request = new HttpWebRequest(uri);
				request.Method = "GET";

				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					responseString = sr.ReadToEnd();
				}
				response.Close();
				LOGMSG("RESTWebService responseString=[" + responseString + "]");
			}
			catch (Exception ex)
			{
				responseString = ex.Message;
				LOGMSG("RESTWebService Excetion responseString=" + responseString);
			}
			return responseString;
		}

		int ScreenHeight = 0, ScreenWidth = 0;
		public void SetScreenDimensions(int height, int width)
		{
			ScreenHeight = height;
			ScreenWidth = width;
			LOGMSG("Screen Size=" + ScreenHeight + " " + ScreenWidth);
		}

		public string Authenticate(String p_username, String p_password)
		{
			string StatusCode = "E04";
			try
			{
				string url = Database.GetPropertyStrValue("CBCMURL") + "?action=00&username=" + p_username + "&password=" + WebUtility.UrlEncode(p_password) + "&version=3.0.0&screenheight=" + ScreenHeight + "&screenwidth=" + ScreenWidth;
				string responseString = RESTWebService(url);
				var json = JObject.Parse(responseString);
				string j_STATUS = json["STATUS"].ToString();
				LOGMSG("status=" + j_STATUS);
				if (j_STATUS.Equals("S01"))
				{
					m_sessionKey = json["SESSION_KEY"].ToString();
					m_username = p_username;
					string j_ROLE = json["ROLE"].ToString();

					if (j_ROLE.Equals("admin") || j_ROLE.Equals("user"))
					{
						m_role = j_ROLE;
						LOGMSG("result=" + j_STATUS + " " + m_sessionKey + " " + m_role);
						Database.SaveProperties("sessionKey", m_sessionKey);
						Database.SaveProperties("username", p_username);
						Database.SaveProperties("role", m_role);
						StatusCode = "S02";  // Successful
					}
				}
				else
				{
					StatusCode = j_STATUS;
				}
			}
			catch (Exception ex)
			{
				LOGMSG("Authenticate error: " + ex.Message);
				StatusCode = "E00";
			}
			LOGMSG("StatusCode = " + StatusCode);
			return StatusCode;
		}

		public void LoadFactorySettings()
		{
			LoadFactoryProperties();
			LoadFactoryMessages("ENG");
			LoadFactoryMessages("ZHT");
			LoadFactoryMessages("ZHS");
		}

		public void LoadFactoryProperties()
		{
			try
			{
				string url = Database.GetPropertyStrValue("URL") + ":" + AdminPort + "/" + Database.GetPropertyStrValue("PAGE") + "?ID=16&language=SYS3";
				string responseString = RESTWebService(url);
				var json = JObject.Parse(responseString);
				foreach (JProperty property in json.Properties())
				{
					LOGMSG("Properties " + property.Name + " - " + property.Value);
					Database.SaveProperties(property.Name, property.Value.ToString());
				}
			}
			catch (Exception ex)
			{
				LOGMSG("Load error: " + ex.Message);
			}
		}

		public void LoadFactoryMessages(string lang)
		{
			try
			{
				string url = Database.GetPropertyStrValue("URL") + ":" + AdminPort + "/" + Database.GetPropertyStrValue("PAGE") + "?ID=16&language=" + lang + "3";
				string responseString = RESTWebService(url);
				var json = JObject.Parse(responseString);
				foreach (JProperty property in json.Properties())
				{
					LOGMSG("Message " + property.Name + " - " + property.Value);
					Database.SaveMessage(lang, property.Name, property.Value.ToString());
				}
			}
			catch (Exception ex)
			{
				LOGMSG("Load error: " + ex.Message);
			}
		}

		public void SetLanguage(string p_language)
		{
			LOGMSG("SetLanguage " + p_language);
			m_language = p_language;
			Database.SaveProperties("language", p_language);
		}
	}
}
