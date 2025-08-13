using System;

namespace MoleMole
{
	public class CacheData<T>
	{
		private DateTime _lastUpdateTime;

		private T _value;

		public T Value
		{
			get
			{
				return (!CacheValid) ? default(T) : _value;
			}
			set
			{
				_lastUpdateTime = TimeUtil.Now;
				_value = value;
			}
		}

		public bool CacheValid
		{
			get
			{
				return _lastUpdateTime.ToString("MM/dd/yyyy HH") == TimeUtil.Now.ToString("MM/dd/yyyy HH");
			}
		}

		public CacheData()
		{
		}

		public CacheData(T value)
		{
			_value = value;
			_lastUpdateTime = TimeUtil.Now;
		}
	}
}
