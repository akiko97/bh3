using System.Collections.Generic;

namespace MoleMole
{
	public class WaitingEventHelper
	{
		private Dictionary<string, bool> _eventFinishMap;

		public WaitingEventHelper(string[] eventNameArray = null)
		{
			_eventFinishMap = new Dictionary<string, bool>();
			if (eventNameArray != null)
			{
				foreach (string key in eventNameArray)
				{
					_eventFinishMap.Add(key, false);
				}
			}
		}

		public void Add(string eventName)
		{
			_eventFinishMap.Add(eventName, false);
		}

		public void Remove(string eventName)
		{
			_eventFinishMap.Remove(eventName);
		}

		public void SetFinish(string eventName)
		{
			_eventFinishMap[eventName] = true;
		}

		public void SetFinishAll()
		{
			foreach (string key in _eventFinishMap.Keys)
			{
				_eventFinishMap[key] = true;
			}
		}

		public bool isFinish(string eventName)
		{
			return _eventFinishMap[eventName];
		}

		public bool isFinishAll()
		{
			foreach (string key in _eventFinishMap.Keys)
			{
				if (!_eventFinishMap[key])
				{
					return false;
				}
			}
			return true;
		}
	}
}
