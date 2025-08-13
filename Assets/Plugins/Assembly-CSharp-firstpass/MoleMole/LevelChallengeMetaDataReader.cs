using System.Collections.Generic;

namespace MoleMole
{
	public class LevelChallengeMetaDataReader
	{
		private static List<LevelChallengeMetaData> _itemList;

		private static Dictionary<int, LevelChallengeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StageChallengeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, LevelChallengeMetaData>();
			_itemList = new List<LevelChallengeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				LevelChallengeMetaData levelChallengeMetaData = new LevelChallengeMetaData(int.Parse(array2[0]), int.Parse(array2[1]), CommonUtils.GetIntListFromString(array2[2].Trim()), array2[3].Trim());
				if (!_itemDict.ContainsKey(levelChallengeMetaData.challengeId))
				{
					_itemList.Add(levelChallengeMetaData);
					_itemDict.Add(levelChallengeMetaData.challengeId, levelChallengeMetaData);
				}
			}
		}

		public static LevelChallengeMetaData GetLevelChallengeMetaDataByKey(int challengeId)
		{
			LevelChallengeMetaData value;
			_itemDict.TryGetValue(challengeId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static LevelChallengeMetaData TryGetLevelChallengeMetaDataByKey(int challengeId)
		{
			LevelChallengeMetaData value;
			_itemDict.TryGetValue(challengeId, out value);
			return value;
		}

		public static List<LevelChallengeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (LevelChallengeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
