using System;

using Android.Content ;
using Android.Hardware;
using Android.OS ;
using Android.Util;
/* ---------------------------------------------------------------------------
    Sample to use ShakeDetector
    
  			sd = new ShakeDetector ();
			sd.ShakeDetected += (object sender, EventArgs e) => 
			{
				OnShakeAction(true);
			};
			 
   --------------------------------------------------------------------------- */

namespace OpenSesameV3.Droid.Services
{
	public class ShakeDetector: Java.Lang.Object, ISensorEventListener
	{
		readonly string LogTag = "ShakeDetector";
		bool hasUpdated = false;
		DateTime lastUpdate, firstUpdate;
        DateTime MovingDateTime = DateTime.Now;
        int currentCount = 0 ;
		float last_x = 0.0f;
		float last_y = 0.0f;
		float last_z = 0.0f;

		bool m_enabled = false;
		bool m_shakeEnabled = false;
		const int ShakeDetectionTimeLapse = 100;
		const double ShakeSpeedThreshold = 3000;
		const int ShakeDuration = 2500;
		private int ShakeCounter = 3;
		private float SHAKE_THRESHOLD_GRAVITY = 2.7F;
		private float SHAKE_NOMOVEMENT_GRAVITY = 1.3F;

		PowerManager.WakeLock wl ;
		SensorManager sensorManager;
		Sensor sensor;

		public event EventHandler<EventArgs> ShakeDetected = delegate { };

		public ShakeDetector (float GForce, float MinGForce, int ShakeCount, bool p_shakeEnabled)
		{
			SetShakingForceThreshold(GForce);
			SetShakeNoMovement(MinGForce);
			SetShakeCount(ShakeCount);
			SetShakeEnabled(p_shakeEnabled);
		
			PowerManager pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
			wl = pm.NewWakeLock(WakeLockFlags.Partial, "OpenSesame");
			sensorManager = Android.App.Application.Context.GetSystemService(Context.SensorService) as Android.Hardware.SensorManager;
			sensor = sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Accelerometer);
		}

		public void AcquireShakeDetector()
		{
			if (!m_enabled)
			{
				wl.Acquire();

				// Register this as a listener with the underlying service.
				sensorManager.RegisterListener(this, sensor, Android.Hardware.SensorDelay.Game);
				m_enabled = true;
			}
		}

		public void DisposeShakeDetector()
		{
			if (m_enabled)
			{
				sensorManager.UnregisterListener(this);
				wl.Release();
				m_enabled = false;
			}
		}

		public bool GetEnabled()
		{
			return m_enabled;
		}

		public void SetShakingForceThreshold(float GForce)
		{
			SHAKE_THRESHOLD_GRAVITY = GForce;
		}

		public void SetShakeNoMovement(float MinGForce)
		{
			SHAKE_NOMOVEMENT_GRAVITY = MinGForce;
		}

		public void SetShakeEnabled(bool p_shakeEnable)
		{
			m_shakeEnabled = p_shakeEnable;
		}

		public void SetShakeCount(int p_shakeCount)
		{
			ShakeCounter = p_shakeCount;
		}

		public DateTime GetMovingDateTime()
		{
			return MovingDateTime;
		}

		#region Android.Hardware.ISensorEventListener implementation
		public void OnAccuracyChanged (Android.Hardware.Sensor sensor, Android.Hardware.SensorStatus accuracy)
		{
		}

		public void OnSensorChanged (Android.Hardware.SensorEvent e)
		{
			if (!m_shakeEnabled) return;
			if (e.Sensor.Type == Android.Hardware.SensorType.Accelerometer)
			{
				float x = e.Values[0];
				float y = e.Values[1];
				float z = e.Values[2];
				float gX = x / SensorManager.GravityEarth;
				float gY = y / SensorManager.GravityEarth;
				float gZ = z / SensorManager.GravityEarth;
				// gForce will be close to 1 when there is no movement.
				double gForce = Math.Sqrt(gX * gX + gY * gY + gZ * gZ);

				if (gForce > SHAKE_NOMOVEMENT_GRAVITY)
				{
					// Log.Debug(LogTag, "*** (" + x + "," + y + "," + z + ") (" + gX + "," + gY + "," + gZ + ") " + gForce);
					MovingDateTime = DateTime.Now;
				}

				if (gForce <= SHAKE_THRESHOLD_GRAVITY)
					return;
				// Log.Debug(LogTag, "(" + x + "," + y + "," + z + ") (" + gX + "," + gY + "," + gZ + ") " + gForce);


				DateTime curTime = System.DateTime.Now;
				if (hasUpdated == false)
				{
					hasUpdated = true;
					lastUpdate = curTime;
					firstUpdate = curTime;
					last_x = x;
					last_y = y;
					last_z = z;
				}
				else
				{
					float dTime = (float)(curTime - firstUpdate).TotalMilliseconds;
					if (Convert.ToInt32 (dTime) > ShakeDuration) currentCount = 0;

					float diffTime = (float)(curTime - lastUpdate).TotalMilliseconds;
					if (Convert.ToInt32(diffTime) > ShakeDetectionTimeLapse) 
					{
						lastUpdate = curTime;
						float dX = Math.Abs(x - last_x);
						float dY = Math.Abs(y - last_y);
						float dZ = Math.Abs(z - last_z);
						float total = dX + dY + dZ;
						float speed = total / diffTime * 10000;

						if (speed > ShakeSpeedThreshold) 
						{
							Log.Debug (LogTag, "" + currentCount + " speed=" + speed + "/" + ShakeSpeedThreshold + " (" + dX + "," + dY + "," + dZ + ") " + firstUpdate.ToString ("HH:mm:ss.fff") + "/" + curTime.ToString ("HH:mm:ss.fff"));
							// shake deteced
							if (currentCount == 0) 
							{
								firstUpdate = curTime;
							} 
							else 
							{
								if (dTime < ShakeDuration) 
								{
									if (currentCount >= ShakeCounter) 
									{
										this.ShakeDetected (this, new EventArgs ());
										currentCount = 0;
										hasUpdated = false;
									}
								}
							}
							currentCount++;
						} 

						last_x = x;
						last_y = y;
						last_z = z;
					}
				}
			}
		}
		#endregion
	}
}

