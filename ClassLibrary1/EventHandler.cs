using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EventHandler
{
	public delegate void NotificationHandler(String n, Args args);
	public class EventHandlerException : Exception {
		public String error;
        public EventHandlerException(String s)
        {
			error = s;
		}
	}
	public class EventHandler
	{

		#region LOGIN ---------------------------------------
        public String LOGIN_SUCCESS = "LOGIN_SUCCESS";
		#endregion

        private class Handlers : List<NotificationHandler> {};
		private class Registry : Dictionary<String, Handlers> {};
		private Registry reg = new Registry();

		#region SINGLETON -----------------------
        private static EventHandler _instance = null;
        public static EventHandler getInstance()
        {
			if (_instance == null)
                _instance = new EventHandler();
			return _instance;
		}
		#endregion

        public string ConnectCamera = "Test Event";

        // Alignment 
        public EventHandler()
        {
		}
		public void Register(String n, NotificationHandler handler) {
			Handlers hs;
			if (reg.ContainsKey(n)) {
				hs = reg[n];
			} else {
				hs = new Handlers();
				reg[n] = hs;
			}
			hs.Add(handler);
		}
		public void UnRegister(String n, NotificationHandler handler) {
			Handlers hs;
			if (reg.ContainsKey(n)) {
				hs = reg[n];
				if (hs.Contains(handler))
					hs.Remove(handler);
			}
		}

        public bool isHandlerPresent(string n)
        {
            if (!reg.ContainsKey(n))
            {
                return false;
            }
            return true;
        }

		public void Notify(String n) {
			Notify(n, new Args());
		}
		public void Notify(String n, Args args) {
			if ( ! reg.ContainsKey(n)) {
				//return;
                EventHandlerException e = new EventHandlerException("Can't find handlers for " + n);
				throw e;
			}
			Handlers handlers = reg[n];
			if (args == null) args = new Args();
			foreach (NotificationHandler h in handlers) {
				h(n, args);
			}
		}
	}

    public class Args : Dictionary<String, Object>
    {
        public Args()
        {
        }
        public Args(String key, object value)
        {
            Add(key, value);
        }
        public Args(String key1, object value1, String key2, object value2)
        {
            Add(key1, value1);
            Add(key2, value2);
        }
        public String Trim(String field)
        {
            return ToString(field).Trim();
        }
        public String ToString(String field)
        {
            if (this[field] != null)
            {
                return this[field].ToString();
            }
            else
            {
                return "";
            }
        }
        public void AddLike(String key, String value)
        {
            Add(key, "%" + value + "%");
        }
    }
}
