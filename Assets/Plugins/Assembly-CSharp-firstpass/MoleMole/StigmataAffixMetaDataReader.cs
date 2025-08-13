using System.Collections.Generic;

namespace MoleMole
{
	public class StigmataAffixMetaDataReader
	{
		private static List<StigmataAffixMetaData> _itemList;

		private static Dictionary<int, StigmataAffixMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StigmataAffix");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, StigmataAffixMetaData>();
			_itemList = new List<StigmataAffixMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				StigmataAffixMetaData stigmataAffixMetaData = new StigmataAffixMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), int.Parse(array2[4]), int.Parse(array2[5]), float.Parse(array2[6]), float.Parse(array2[7]), float.Parse(array2[8]), int.Parse(array2[9]), float.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]));
				if (!_itemDict.ContainsKey(stigmataAffixMetaData.affixID))
				{
					_itemList.Add(stigmataAffixMetaData);
					_itemDict.Add(stigmataAffixMetaData.affixID, stigmataAffixMetaData);
				}
			}
		}

		public static StigmataAffixMetaData GetStigmataAffixMetaDataByKey(int affixID)
		{
			StigmataAffixMetaData value;
			_itemDict.TryGetValue(affixID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static StigmataAffixMetaData TryGetStigmataAffixMetaDataByKey(int affixID)
		{
			StigmataAffixMetaData value;
			_itemDict.TryGetValue(affixID, out value);
			return value;
		}

		public static List<StigmataAffixMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (StigmataAffixMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
