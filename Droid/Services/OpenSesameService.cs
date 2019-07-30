using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Android.App;
using Android.Content;
using Android.Net;
using Android.Media;
using Android.OS;
using Android.Util;

using Newtonsoft.Json.Linq;
using Android.Net.Wifi;

namespace OpenSesameV3.Droid.Services
{
	[Service]
	public class OpenSesameService : Service
	{
		MediaPlayer _player;
		readonly string LogTag = "OpenSesameService";
		IBinder binder;
		ShakeDetector sd;
		float m_volume = 1.0f;

		public OpenSesameService() 
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			DBGMSG("OnCreate called in the OpenSesame Service. Heap memory size=" + Java.Lang.Runtime.GetRuntime().MaxMemory());
		}
			
		public string OnShakeAction(bool vibrate)
		{
			LOGMSG("OnShakeAction... " + vibrate);

			if (CBCMGWiFiConnected()) 
			{
				return ShakeAccess (vibrate);
			} 
			else 
			{
				PlayWiFiDisconnected();
				return "E00";
			}
		}

		public void SetVolume(float p_volume)
		{
			m_volume = p_volume;
			PlaySound(Resource.Raw.beep1);
		}
		public void SetShakingForceThreshold(float p_gforce)
		{
			sd.SetShakingForceThreshold(p_gforce);
		}
		public void SetShakeCount(int p_shakecount)
		{
			sd.SetShakeCount(p_shakecount);
		}
		public void SetShakeEnabled(bool p_shakeEnabled)
		{
			sd.SetShakeEnabled(p_shakeEnabled);
		}

		// This gets called once, the first time any client bind to the Service
		// and returns an instance of the OpenSesameServiceBinder. All future clients will
		// reuse the same instance of the binder
		public override IBinder OnBind (Intent intent)
		{
			DBGMSG ("Client now bound to service");
			binder = new OpenSesameServiceBinder (this);
			return binder;
		}

		public override void OnDestroy ()
		{
			if (_broadcastReceiver == null)
			{
                LOGMSG("Network status monitoring not active.");
				// throw new InvalidOperationException("Network status monitoring not active.");
			}
			// Unregister the receiver so we no longer get updates.
			Application.Context.UnregisterReceiver(_broadcastReceiver);

			// Set the variable to nil, so that we know the receiver is no longer used.
			_broadcastReceiver.ConnectionStatusChanged -= OnNetworkStatusChanged;
			_broadcastReceiver = null;

			base.OnDestroy ();
			DBGMSG("Service has been terminated");
		}
			
		String m_username = null ;
		String m_sessionKey = null ;
		String m_language = null ;
		String m_role = null ;
		String IPRange = null;
		String UserPort = null ;
		String AdminPort = null ;
		String port = null ;
		int m_WiFiTimeout = 60;

		public DatabaseAccess getDatabase()
		{
			return Database;
		}
		DatabaseAccess Database = null;

		public event EventHandler NetworkStatusChanged;
		NetworkStatusBroadcastReceiver _broadcastReceiver;
		public enum NetworkState
		{
			Unknown,
			ConnectedWifi,
			ConnectedData,
			Disconnected
		}
		private NetworkState _state;

		public NetworkState WiFiStatus()
		{
			UpdateNetworkStatus();
			return _state;
		}

		public void UpdateNetworkStatus()
		{
			_state = NetworkState.Unknown;
			// Retrieve the connectivity manager service
			var connectivityManager = (ConnectivityManager) Application.Context.GetSystemService(Context.ConnectivityService);

			// Check if the network is connected or connecting. This means that it will be available, or become available in a few seconds.
			var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
			if ((activeNetworkInfo != null) && (activeNetworkInfo.IsConnectedOrConnecting))
			{
				// Now that we know it's connected, determine if we're on WiFi or something else.
				_state = activeNetworkInfo.Type == ConnectivityType.Wifi ? NetworkState.ConnectedWifi : NetworkState.ConnectedData;
			}
			else 
			{
				_state = NetworkState.Disconnected;
			}
		}

		public void StartUpdates () 
		{ 
			Database = new DatabaseAccess (new SQLite_Init ().getConnection ());

			m_username = Database.GetPropertyStrValue ("username");
			m_sessionKey = Database.GetPropertyStrValue ("sessionKey");
			m_language = Database.GetPropertyStrValue ("language");
			m_role = Database.GetPropertyStrValue ("role");
			LOGMSG ("username = " + m_language + " " + m_username+" "+ m_sessionKey+" "+m_role);

			IPRange = Database.GetPropertyStrValue ("IPRange");
			UserPort = Database.GetPropertyStrValue ("UserPort");
			AdminPort = Database.GetPropertyStrValue ("AdminPort");
			if (m_role.Equals ("admin"))
				port = AdminPort;
			else
				port = UserPort;
			m_volume = Database.GetPropertyIntValue("Volume") * 0.01f;
			m_WiFiTimeout = Database.GetPropertyIntValue("WiFiTimeout");

			LOGMSG("GForceThreshold=" + Database.GetPropertyIntValue("GForceThreshold"));
			sd = new ShakeDetector(Database.GetPropertyIntValue("GForceThreshold") * 0.1f, Database.GetPropertyIntValue("MinGForce") * 0.1f, Database.GetPropertyIntValue("ShakeCount"), Database.GetPropertyBoolValue("Shake"));
			sd.ShakeDetected += (object sender, EventArgs e) =>
			{
				OnShakeAction(true);
			};

            // Create the broadcast receiver and bind the event handler, so that the app gets updates of the network connectivity status
            if (_broadcastReceiver != null) LOGMSG("Network status monitoring already active.");
            _broadcastReceiver = new NetworkStatusBroadcastReceiver();
            _broadcastReceiver.ConnectionStatusChanged += OnNetworkStatusChanged;
            // Register the broadcast receiver
            Application.Context.RegisterReceiver(_broadcastReceiver, new IntentFilter(ConnectivityManager.ConnectivityAction));
        }

        void OnNetworkStatusChanged(object sender, EventArgs e)
		{
			var currentStatus = _state;

			UpdateNetworkStatus();

			if (currentStatus != _state && NetworkStatusChanged != null)
			{
				NetworkStatusChanged(this, EventArgs.Empty);
			}
		}

		bool WiFiDisconnectWarning = true;
		DateTime WiFiConnectDateTime = DateTime.Now;
		DateTime WiFiDisconnectDateTime = DateTime.Now;
		public async void EventHandlers()
		{
			PlaySound(Resource.Raw.beep1);
			if (sd == null) return;
			LOGMSG("Network EventHandlers sd enabled? " + sd.GetEnabled());

			if (CBCMGWiFiConnected())
			{
				WiFiConnectDateTime = DateTime.Now;
				WiFiDisconnectDateTime = DateTime.Now;
				if(!sd.GetEnabled())
				{
					LOGMSG("AcquireShakeDetector");
					sd.AcquireShakeDetector();
					Vibrate(1000);
					PlaySound(Resource.Raw.Logon);
				}
				WiFiDisconnectWarning = false;
			}
			else
			{
				PlayWiFiDisconnectedWarning();
				WiFiDisconnectDateTime = DateTime.Now;
				await WaitAndExecute((m_WiFiTimeout+1) * 1000, () => WiFiLogoff());
			}
		}

        private async Task WaitAndExecute(int milisec, Action actionToExecute)
		{
			await Task.Delay(milisec);
			actionToExecute();
		}

		private void WiFiLogoff()
		{
			LOGMSG("WiFiLogoff (" +  DateTime.Now + " - " + WiFiDisconnectDateTime + " " + (DateTime.Now - WiFiDisconnectDateTime).TotalSeconds + ") > " +m_WiFiTimeout + "? " + WiFiDisconnectWarning);
			if (((DateTime.Now - WiFiDisconnectDateTime).TotalSeconds > m_WiFiTimeout) && !WiFiDisconnectWarning && !CBCMGWiFiConnected())
			{
				LOGMSG("Play logoff and DisposeShakeDetector");
				sd.DisposeShakeDetector();
				Vibrate(300);
				PlaySound(Resource.Raw.Logoff);
				WiFiDisconnectWarning = true;
			}
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
			Log.Debug (LogTag, s);
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

		private void PlaySound(int restid)
		{
			PlaySound (restid, m_volume);
		}

		public void PlaySound(int restid, float volume)
		{
			if (_player != null) _player.Release ();
			_player = MediaPlayer.Create(this, restid);
			_player.SetVolume (volume, volume);
			_player.Start();
		}

		public bool CBCMGWiFiConnected()
		{
			bool wc = false;
			bool gc = false;
			String WiFiIP = null;

			if(WiFiStatus() == NetworkState.ConnectedWifi)
			{
				wc = true;
                PlayWiFiConnected();
                Java.Util.IEnumeration networkInterfaces = Java.Net.NetworkInterface.NetworkInterfaces;
				while (networkInterfaces.HasMoreElements)
				{
					Java.Net.NetworkInterface netInterface = (Java.Net.NetworkInterface)networkInterfaces.NextElement();
					Java.Util.IEnumeration a = netInterface.InetAddresses;
					while (a.HasMoreElements)
					{
						Java.Net.InetAddress b = (Java.Net.InetAddress)a.NextElement();
						String IP = b.GetAddress()[0] + "." + b.GetAddress()[1] + "." + b.GetAddress()[2] + "." + b.GetAddress()[3];
						if (IP.StartsWith(IPRange, StringComparison.Ordinal))
						{
							WiFiIP = IP;
							gc = true;
                            PlayCBCMGWiFiConnected();
                        }
					}
				}
			}
			LOGMSG("IP " + WiFiIP + " WiFi connected: " + wc + " CBCMG connected: " + gc + " " + sd.GetEnabled() + ":" + WiFiConnectDateTime + " " + WiFiDisconnectWarning + ":" + WiFiDisconnectDateTime + " " + sd.GetMovingDateTime());
			return gc;
		}

		private void ConnectToWiFi()
		{
			LOGMSG("ConnectToWiFi");
			var mawifi = (WifiManager)GetSystemService(WifiService);
			mawifi.SetWifiEnabled(true);

			var wfc = new WifiConfiguration();
			wfc.Ssid = "\"" + Database.GetPropertyStrValue("WiFiSSID") + "\"";
			wfc.PreSharedKey = "\"" + Database.GetPropertyStrValue("WiFiPassword") + "\"";
			wfc.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);

			var wifiManager = (WifiManager)GetSystemService(WifiService);
			int id = wifiManager.AddNetwork(wfc);
			wifiManager.EnableNetwork(id, true);
		}

		public string ShakeAccess(bool vibrate)
		{
			LOGMSG ("ShakeAccess");
			string ErrorCode = Request("00", "Grant Access", m_username, m_sessionKey, UserPort);
			if (ErrorCode.Equals("S01"))
			{
				PlaySound(Resource.Raw.HallelujahChorus);
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
				Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
                vibrator.Vibrate(VibrationEffect.CreateOneShot(duration, VibrationEffect.DefaultAmplitude));
                //vibrator.Vibrate(duration);
			}
		}

		private void PlayWiFiDisconnected()
		{
			Vibrate(300);
			PlaySound(Resource.Raw.WiFiDisconnected);
		}

		private void PlayWiFiDisconnectedWarning()
		{
			LOGMSG("PlayWiFiDisconnectedWarning");
			Vibrate(300);
			PlaySound(Resource.Raw.OnDestroy);
		}

		private void PlayWiFiConnected()
        {
			LOGMSG("PlayWiFiConnected");
			Vibrate(300);
			PlaySound(Resource.Raw.LOGMSG);
        }

        private void PlayCBCMGWiFiConnected()
        {
			LOGMSG("PlayCBCMGWiFiConnected");
            Vibrate(300);
            PlaySound(Resource.Raw.beep4);
        }

		private void DoorAction(string ErrorCode)
		{
			if (ErrorCode.Equals("S01"))
			{
				Vibrate(300);
				PlaySound(Resource.Raw.DoorAction);
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
