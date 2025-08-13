using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class CacheDataUtil
	{
		private class DataWrapper
		{
			public object _data;

			public Action _req;

			public ushort _rspID;

			public NotifyTypes _notify;
		}

		private Dictionary<ECacheData, DataWrapper> _cacheDataUtilDic = new Dictionary<ECacheData, DataWrapper>();

		public void CreateCacheUtil(ECacheData type, object data, Action req, ushort rspID)
		{
			DataWrapper dataWrapper = new DataWrapper();
			dataWrapper._data = data;
			dataWrapper._req = req;
			dataWrapper._rspID = rspID;
			dataWrapper._notify = NotifyTypes.None;
			_cacheDataUtilDic.Add(type, dataWrapper);
		}

		public void CheckCacheValidAndGo<T>(ECacheData type, NotifyTypes notify) where T : class
		{
			foreach (KeyValuePair<ECacheData, DataWrapper> item in _cacheDataUtilDic)
			{
				if (item.Key == type)
				{
					CacheData<T> cacheData = item.Value._data as CacheData<T>;
					if (cacheData.CacheValid)
					{
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(notify));
						break;
					}
					item.Value._notify = notify;
					item.Value._req();
					break;
				}
			}
		}

		public void OnGetRsp(ushort cmdID)
		{
			foreach (KeyValuePair<ECacheData, DataWrapper> item in _cacheDataUtilDic)
			{
				if (item.Value._rspID == cmdID)
				{
					if (item.Value._notify != NotifyTypes.None)
					{
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(item.Value._notify));
						item.Value._notify = NotifyTypes.None;
					}
					break;
				}
			}
		}
	}
}
