using System.Collections.Generic;

namespace MoleMole
{
	public class EndlessGroupMetaDataReader
	{
		private static List<EndlessGroupMetaData> _itemList;

		private static Dictionary<int, EndlessGroupMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EndlessGroup");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EndlessGroupMetaData>();
			_itemList = new List<EndlessGroupMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EndlessGroupMetaData endlessGroupMetaData = new EndlessGroupMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), float.Parse(array2[12]), float.Parse(array2[13]));
				if (!_itemDict.ContainsKey(endlessGroupMetaData.groupLevel))
				{
					_itemList.Add(endlessGroupMetaData);
					_itemDict.Add(endlessGroupMetaData.groupLevel, endlessGroupMetaData);
				}
			}
		}

		public static EndlessGroupMetaData GetEndlessGroupMetaDataByKey(int groupLevel)
		{
			EndlessGroupMetaData value;
			_itemDict.TryGetValue(groupLevel, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EndlessGroupMetaData TryGetEndlessGroupMetaDataByKey(int groupLevel)
		{
			EndlessGroupMetaData value;
			_itemDict.TryGetValue(groupLevel, out value);
			return value;
		}

		public static List<EndlessGroupMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EndlessGroupMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
