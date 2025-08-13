using System.Collections.Generic;

namespace MoleMole
{
	public class EndlessDropMetaDataReader
	{
		private static List<EndlessDropMetaData> _itemList;

		private static Dictionary<KeyValuePair<int, int>, EndlessDropMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EndlessDrop");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<int, int>, EndlessDropMetaData>();
			_itemList = new List<EndlessDropMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EndlessDropMetaData endlessDropMetaData = new EndlessDropMetaData(int.Parse(array2[0]), int.Parse(array2[1]), CommonUtils.GetIntListFromString(array2[2].Trim()), CommonUtils.GetIntListFromString(array2[3].Trim()), CommonUtils.GetIntListFromString(array2[4].Trim()), CommonUtils.GetIntListFromString(array2[5].Trim()));
				if (!_itemDict.ContainsKey(new KeyValuePair<int, int>(endlessDropMetaData.group, endlessDropMetaData.level)))
				{
					_itemList.Add(endlessDropMetaData);
					_itemDict.Add(new KeyValuePair<int, int>(endlessDropMetaData.group, endlessDropMetaData.level), endlessDropMetaData);
				}
			}
		}

		public static EndlessDropMetaData GetEndlessDropMetaDataByKey(int group, int level)
		{
			EndlessDropMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(group, level), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EndlessDropMetaData TryGetEndlessDropMetaDataByKey(int group, int level)
		{
			EndlessDropMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(group, level), out value);
			return value;
		}

		public static List<EndlessDropMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EndlessDropMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
