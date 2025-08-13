using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarDefencePunishMetaDataReader
	{
		private static List<AvatarDefencePunishMetaData> _itemList;

		private static Dictionary<int, AvatarDefencePunishMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/DefencePunishData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarDefencePunishMetaData>();
			_itemList = new List<AvatarDefencePunishMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarDefencePunishMetaData avatarDefencePunishMetaData = new AvatarDefencePunishMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				if (!_itemDict.ContainsKey(avatarDefencePunishMetaData.LevelDifference))
				{
					_itemList.Add(avatarDefencePunishMetaData);
					_itemDict.Add(avatarDefencePunishMetaData.LevelDifference, avatarDefencePunishMetaData);
				}
			}
		}

		public static AvatarDefencePunishMetaData GetAvatarDefencePunishMetaDataByKey(int LevelDifference)
		{
			AvatarDefencePunishMetaData value;
			_itemDict.TryGetValue(LevelDifference, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarDefencePunishMetaData TryGetAvatarDefencePunishMetaDataByKey(int LevelDifference)
		{
			AvatarDefencePunishMetaData value;
			_itemDict.TryGetValue(LevelDifference, out value);
			return value;
		}

		public static List<AvatarDefencePunishMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarDefencePunishMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
