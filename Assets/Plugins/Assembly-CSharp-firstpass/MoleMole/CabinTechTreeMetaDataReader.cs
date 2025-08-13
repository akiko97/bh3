using System.Collections.Generic;

namespace MoleMole
{
	public class CabinTechTreeMetaDataReader
	{
		private static List<CabinTechTreeMetaData> _itemList;

		private static Dictionary<int, CabinTechTreeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinTechTreeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, CabinTechTreeMetaData>();
			_itemList = new List<CabinTechTreeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				CabinTechTreeMetaData cabinTechTreeMetaData = new CabinTechTreeMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), array2[13].Trim(), array2[14].Trim(), array2[15].Trim());
				if (!_itemDict.ContainsKey(cabinTechTreeMetaData.ID))
				{
					_itemList.Add(cabinTechTreeMetaData);
					_itemDict.Add(cabinTechTreeMetaData.ID, cabinTechTreeMetaData);
				}
			}
		}

		public static CabinTechTreeMetaData GetCabinTechTreeMetaDataByKey(int ID)
		{
			CabinTechTreeMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinTechTreeMetaData TryGetCabinTechTreeMetaDataByKey(int ID)
		{
			CabinTechTreeMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<CabinTechTreeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinTechTreeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
