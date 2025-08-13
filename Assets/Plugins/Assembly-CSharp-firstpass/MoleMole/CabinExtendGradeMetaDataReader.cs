using System.Collections.Generic;

namespace MoleMole
{
	public class CabinExtendGradeMetaDataReader
	{
		private static List<CabinExtendGradeMetaData> _itemList;

		private static Dictionary<KeyValuePair<int, int>, CabinExtendGradeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinExtendGradeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<int, int>, CabinExtendGradeMetaData>();
			_itemList = new List<CabinExtendGradeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<CabinExtendGradeMetaData.CabinExtendNeedItem> list2 = new List<CabinExtendGradeMetaData.CabinExtendNeedItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[4]))
				{
					list2.Add(new CabinExtendGradeMetaData.CabinExtendNeedItem(item));
				}
				CabinExtendGradeMetaData cabinExtendGradeMetaData = new CabinExtendGradeMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), list2, array2[5].Trim());
				if (!_itemDict.ContainsKey(new KeyValuePair<int, int>(cabinExtendGradeMetaData.cabinType, cabinExtendGradeMetaData.extendGrade)))
				{
					_itemList.Add(cabinExtendGradeMetaData);
					_itemDict.Add(new KeyValuePair<int, int>(cabinExtendGradeMetaData.cabinType, cabinExtendGradeMetaData.extendGrade), cabinExtendGradeMetaData);
				}
			}
		}

		public static CabinExtendGradeMetaData GetCabinExtendGradeMetaDataByKey(int cabinType, int extendGrade)
		{
			CabinExtendGradeMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(cabinType, extendGrade), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static CabinExtendGradeMetaData TryGetCabinExtendGradeMetaDataByKey(int cabinType, int extendGrade)
		{
			CabinExtendGradeMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(cabinType, extendGrade), out value);
			return value;
		}

		public static List<CabinExtendGradeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (CabinExtendGradeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
