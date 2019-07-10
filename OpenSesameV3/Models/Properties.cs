using System;
using SQLite;

namespace OpenSesameV3
{
	public class Properties
	{
		public Properties ()
		{
		}
		[PrimaryKey]
		public string KEY { get; set; }
		public string VALUE { get; set; }
	}
}
