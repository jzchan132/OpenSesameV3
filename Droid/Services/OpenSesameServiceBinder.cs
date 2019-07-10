using System;
using Android.OS;

namespace OpenSesameV3.Droid.Services
{
	public class OpenSesameServiceBinder : Binder
	{
		public OpenSesameService Service
		{
			get { return this.service; }
		} 
		protected OpenSesameService service;

		public bool IsBound { get; set; }
			
		public OpenSesameServiceBinder (OpenSesameService service)
		{
			this.service = service;
		}
	}
}

