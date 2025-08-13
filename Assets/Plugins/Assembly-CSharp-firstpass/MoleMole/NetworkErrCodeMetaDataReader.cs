using System.Collections.Generic;

namespace MoleMole
{
	public class NetworkErrCodeMetaDataReader
	{
		private static List<NetworkErrCodeMetaData> _itemList;

		private static Dictionary<KeyValuePair<string, string>, NetworkErrCodeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/NetworkErrCodeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<string, string>, NetworkErrCodeMetaData>();
			_itemList = new List<NetworkErrCodeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				NetworkErrCodeMetaData networkErrCodeMetaData = new NetworkErrCodeMetaData(array2[0].Trim(), array2[1].Trim(), array2[2].Trim());
				if (!_itemDict.ContainsKey(new KeyValuePair<string, string>(networkErrCodeMetaData.errType, networkErrCodeMetaData.retCode)))
				{
					_itemList.Add(networkErrCodeMetaData);
					_itemDict.Add(new KeyValuePair<string, string>(networkErrCodeMetaData.errType, networkErrCodeMetaData.retCode), networkErrCodeMetaData);
				}
			}
		}

		public static NetworkErrCodeMetaData GetNetworkErrCodeMetaDataByKey(string errType, string retCode)
		{
			NetworkErrCodeMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<string, string>(errType, retCode), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static NetworkErrCodeMetaData TryGetNetworkErrCodeMetaDataByKey(string errType, string retCode)
		{
			NetworkErrCodeMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<string, string>(errType, retCode), out value);
			return value;
		}

		public static List<NetworkErrCodeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (NetworkErrCodeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
