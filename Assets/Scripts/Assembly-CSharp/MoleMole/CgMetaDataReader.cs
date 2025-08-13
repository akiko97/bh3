using System.Collections.Generic;

namespace MoleMole
{
	public class CgMetaDataReader
	{
		private static List<CgMetaData> _itemList;

		private static Dictionary<int, CgMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CgData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CgMetaData>();
			_itemList = new List<CgMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CgMetaData cgMetaData = new CgMetaData(int.Parse(array2[0]), int.Parse(array2[1]), array2[2].Trim(), array2[3].Trim());
				if (!_itemDict.ContainsKey(cgMetaData.CgID))
				{
					_itemList.Add(cgMetaData);
					_itemDict.Add(cgMetaData.CgID, cgMetaData);
				}
			}
		}

		public static CgMetaData GetCgMetaDataByKey(int CgID)
		{
			CgMetaData value;
			_itemDict.TryGetValue(CgID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CgMetaData TryGetCgMetaDataByKey(int CgID)
		{
			CgMetaData value;
			_itemDict.TryGetValue(CgID, out value);
			return value;
		}

		public static List<CgMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CgMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
