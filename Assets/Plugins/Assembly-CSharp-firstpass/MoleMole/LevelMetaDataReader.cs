using System.Collections.Generic;

namespace MoleMole
{
	public class LevelMetaDataReader
	{
		private static List<LevelMetaData> _itemList;

		private static Dictionary<int, LevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StageData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, LevelMetaData>();
			_itemList = new List<LevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<LevelMetaData.LevelChallengeMetaNode> list2 = new List<LevelMetaData.LevelChallengeMetaNode>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[34]))
				{
					list2.Add(new LevelMetaData.LevelChallengeMetaNode(item));
				}
				LevelMetaData levelMetaData = new LevelMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), float.Parse(array2[15]), int.Parse(array2[16]), float.Parse(array2[17]), int.Parse(array2[18]), int.Parse(array2[19]), int.Parse(array2[20]), CommonUtils.GetIntListFromString(array2[21].Trim()), array2[22].Trim(), array2[23].Trim(), array2[24].Trim(), int.Parse(array2[25]), int.Parse(array2[26]), int.Parse(array2[27]), CommonUtils.GetIntListFromString(array2[28].Trim()), array2[29].Trim(), array2[30].Trim(), array2[31].Trim(), array2[32].Trim(), array2[33].Trim(), list2, int.Parse(array2[35]), int.Parse(array2[36]), int.Parse(array2[37]), int.Parse(array2[38]), int.Parse(array2[39]), int.Parse(array2[40]), CommonUtils.GetStringListFromString(array2[41].Trim()), int.Parse(array2[42]));
				if (!_itemDict.ContainsKey(levelMetaData.levelId))
				{
					_itemList.Add(levelMetaData);
					_itemDict.Add(levelMetaData.levelId, levelMetaData);
				}
			}
		}

		public static LevelMetaData GetLevelMetaDataByKey(int levelId)
		{
			LevelMetaData value;
			_itemDict.TryGetValue(levelId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static LevelMetaData TryGetLevelMetaDataByKey(int levelId)
		{
			LevelMetaData value;
			_itemDict.TryGetValue(levelId, out value);
			return value;
		}

		public static List<LevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (LevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
