using System.Collections.Generic;

namespace MoleMole
{
	public class NPCLevelMetaDataReader
	{
		private static List<NPCLevelMetaData> _itemList;

		private static Dictionary<int, NPCLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/NPCLevelLogic");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, NPCLevelMetaData>();
			_itemList = new List<NPCLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				NPCLevelMetaData nPCLevelMetaData = new NPCLevelMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]), float.Parse(array2[3]), float.Parse(array2[4]));
				if (!_itemDict.ContainsKey(nPCLevelMetaData.HardLevel))
				{
					_itemList.Add(nPCLevelMetaData);
					_itemDict.Add(nPCLevelMetaData.HardLevel, nPCLevelMetaData);
				}
			}
		}

		public static NPCLevelMetaData GetNPCLevelMetaDataByKey(int HardLevel)
		{
			NPCLevelMetaData value;
			_itemDict.TryGetValue(HardLevel, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static NPCLevelMetaData TryGetNPCLevelMetaDataByKey(int HardLevel)
		{
			NPCLevelMetaData value;
			_itemDict.TryGetValue(HardLevel, out value);
			return value;
		}

		public static List<NPCLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (NPCLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
