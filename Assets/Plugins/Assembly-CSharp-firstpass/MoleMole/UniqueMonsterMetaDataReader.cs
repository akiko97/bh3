using System.Collections.Generic;

namespace MoleMole
{
	public class UniqueMonsterMetaDataReader
	{
		private static List<UniqueMonsterMetaData> _itemList;

		private static Dictionary<uint, UniqueMonsterMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/UniqueMonsterData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<uint, UniqueMonsterMetaData>();
			_itemList = new List<UniqueMonsterMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				UniqueMonsterMetaData uniqueMonsterMetaData = new UniqueMonsterMetaData(uint.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), float.Parse(array2[4]), float.Parse(array2[5]), float.Parse(array2[6]), float.Parse(array2[7]), CommonUtils.GetFloatListFromString(array2[8].Trim()), array2[9].Trim(), array2[10].Trim(), CommonUtils.GetStringListFromString(array2[11].Trim()), CommonUtils.GetFloatListFromString(array2[12].Trim()), array2[13].Trim(), int.Parse(array2[14]), CommonUtils.GetFloatListFromString(array2[15].Trim()));
				if (!_itemDict.ContainsKey(uniqueMonsterMetaData.ID))
				{
					_itemList.Add(uniqueMonsterMetaData);
					_itemDict.Add(uniqueMonsterMetaData.ID, uniqueMonsterMetaData);
				}
			}
		}

		public static UniqueMonsterMetaData GetUniqueMonsterMetaDataByKey(uint ID)
		{
			UniqueMonsterMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static UniqueMonsterMetaData TryGetUniqueMonsterMetaDataByKey(uint ID)
		{
			UniqueMonsterMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<UniqueMonsterMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (UniqueMonsterMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
