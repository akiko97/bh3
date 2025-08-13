using System.Collections.Generic;

namespace MoleMole
{
	public class EndlessToolMetaDataReader
	{
		private static List<EndlessToolMetaData> _itemList;

		private static Dictionary<int, EndlessToolMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/EndlessItem");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, EndlessToolMetaData>();
			_itemList = new List<EndlessToolMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				EndlessToolMetaData endlessToolMetaData = new EndlessToolMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), array2[8].Trim(), int.Parse(array2[9]), array2[10].Trim(), array2[11].Trim(), array2[12].Trim(), array2[13].Trim(), array2[14].Trim());
				if (!_itemDict.ContainsKey(endlessToolMetaData.ID))
				{
					_itemList.Add(endlessToolMetaData);
					_itemDict.Add(endlessToolMetaData.ID, endlessToolMetaData);
				}
			}
		}

		public static EndlessToolMetaData GetEndlessToolMetaDataByKey(int ID)
		{
			EndlessToolMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static EndlessToolMetaData TryGetEndlessToolMetaDataByKey(int ID)
		{
			EndlessToolMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<EndlessToolMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (EndlessToolMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
