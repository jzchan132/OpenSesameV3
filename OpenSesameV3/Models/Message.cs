using System;
using SQLite;

namespace OpenSesameV3
{
	public class Message
	{
		public Message ()
		{
		}
		[PrimaryKey, AutoIncrement]
		public int MESSAGE_ID { get; set; }
		public string LANGUAGE { get; set; }
		public string KEY { get; set; }
		public string VALUE { get; set; }
	}
}

