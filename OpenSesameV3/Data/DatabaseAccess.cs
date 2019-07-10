using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OpenSesameV3
{
	public class DatabaseAccess
	{
		static object locker = new object ();
		public SQLiteConnection database;
		List<Message> MessageTable = null;

		public DatabaseAccess()
		{
		}

		public DatabaseAccess(SQLiteConnection db)
		{
			database = db;
		}

		public IEnumerable<Message> GetMessages ()
		{
			lock (locker) 
			{
				return (from i in database.Table<Message>() select i).ToList();
			}
		}

		public void GetMessagesByLanguage (string lang)
		{
			MessageTable = GetMessageByLang(lang).ToList<Message> ();
		}

		private IEnumerable<Message> GetMessageByLang (string lang)
		{
			lock (locker) 
			{
				return database.Query<Message>("SELECT * FROM [MESSAGE] WHERE [LANGUAGE] = '"+lang+"'");
			}
		}

		public Message GetMessage (int id) 
		{
			lock (locker) 
			{
				return database.Table<Message>().FirstOrDefault(x => x.MESSAGE_ID == id);
			}
		}

		public Message GetMessage(string lang, string key)
		{
			lock (locker)
			{
				return database.Table<Message>().FirstOrDefault(x => x.LANGUAGE == lang && x.KEY == key);
			}
		}

		public int SaveMessage (string LANG, string KEY, string VALUE)
		{
			Message item = GetMessage(LANG, KEY);
			item.VALUE = VALUE;
			return SaveMessage(item);
		}

		public int SaveMessage (Message item) 
		{
			lock (locker) 
			{
				if (item.MESSAGE_ID != 0) 
				{
					database.Update(item);
					return item.MESSAGE_ID;
				} 
				else 
				{
					return database.Insert(item);
				}
			}
		}

		public int DeleteMessage(int id)
		{
			lock (locker) 
			{
				return database.Delete<Message>(id);
			}
		}

		public string GetMessageStrValue (string key) 
		{
			foreach (var m in MessageTable) 
			{
				if (m.KEY.Equals (key)) 
				{
					return m.VALUE;
				}
			}
			return null;
		}

		public int GetMessageIntValue (string key) 
		{
			string p = GetMessageStrValue (key);
			return Convert.ToInt32 (p);
		}
			
		public IEnumerable<Properties> GetProperties ()
		{
			lock (locker) 
			{
				return (from i in database.Table<Properties>() select i).ToList();
			}
		}

		public Properties GetProperties (string key) 
		{
			lock (locker) 
			{
				return database.Table<Properties>().FirstOrDefault(x => x.KEY.Equals(key));
			}
		}

		public string SaveProperties(string KEY, string VALUE)
		{
			Properties item = new Properties();
			item.KEY = KEY;
			item.VALUE = VALUE;
			return SaveProperties(item);
		}

		public string SaveProperties (Properties item) 
		{
			lock (locker) 
			{
				if (item.KEY != null) 
				{
					database.Update(item);
					return item.KEY;
				} 
				else 
				{
					database.Insert(item);
					return item.KEY;
				}
			}
		}

		public int DeleteProperties(string key)
		{
			lock (locker) 
			{
				return database.Delete<Properties>(key);
			}
		}

		public string GetPropertyStrValue (string key) 
		{
			lock (locker) 
			{
				return database.Table<Properties>().FirstOrDefault(x => x.KEY.Equals(key)).VALUE;
			}
		}

		public int GetPropertyIntValue (string key) 
		{
			string p = GetPropertyStrValue (key);
			return Convert.ToInt32 (p);
		}
	
		public bool GetPropertyBoolValue(string key)
		{
			string p = GetPropertyStrValue(key);
			return Convert.ToBoolean(p);
		}

		public bool GetPropertyBoolValue(string key, string TrueValue)
		{
			string p = GetPropertyStrValue(key);
			return p.Equals(TrueValue);
		}

	}
}
