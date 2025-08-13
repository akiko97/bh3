using System.Collections.Generic;

namespace MoleMole
{
	public class ActMetaDataReader
	{
		private static List<ActMetaData> _itemList;

		private static Dictionary<int, ActMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/ActData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ActMetaData>();
			_itemList = new List<ActMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ActMetaData actMetaData = new ActMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), array2[3].Trim(), int.Parse(array2[4]), array2[5].Trim(), array2[6].Trim(), array2[7].Trim());
				if (!_itemDict.ContainsKey(actMetaData.actId))
				{
					_itemList.Add(actMetaData);
					_itemDict.Add(actMetaData.actId, actMetaData);
				}
			}
		}

		public static ActMetaData GetActMetaDataByKey(int actId)
		{
			ActMetaData value;
			_itemDict.TryGetValue(actId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ActMetaData TryGetActMetaDataByKey(int actId)
		{
			ActMetaData value;
			_itemDict.TryGetValue(actId, out value);
			return value;
		}

		public static List<ActMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ActMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
