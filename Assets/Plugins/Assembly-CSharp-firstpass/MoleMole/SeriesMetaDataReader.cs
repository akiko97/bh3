using System.Collections.Generic;

namespace MoleMole
{
	public class SeriesMetaDataReader
	{
		private static List<SeriesMetaData> _itemList;

		private static Dictionary<int, SeriesMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/SeriesData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, SeriesMetaData>();
			_itemList = new List<SeriesMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				SeriesMetaData seriesMetaData = new SeriesMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), float.Parse(array2[3]), float.Parse(array2[4]), float.Parse(array2[5]), float.Parse(array2[6]));
				if (!_itemDict.ContainsKey(seriesMetaData.id))
				{
					_itemList.Add(seriesMetaData);
					_itemDict.Add(seriesMetaData.id, seriesMetaData);
				}
			}
		}

		public static SeriesMetaData GetSeriesMetaDataByKey(int id)
		{
			SeriesMetaData value;
			_itemDict.TryGetValue(id, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static SeriesMetaData TryGetSeriesMetaDataByKey(int id)
		{
			SeriesMetaData value;
			_itemDict.TryGetValue(id, out value);
			return value;
		}

		public static List<SeriesMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (SeriesMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
