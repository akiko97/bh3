using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarAttackPunishMetaDataReader
	{
		private static List<AvatarAttackPunishMetaData> _itemList;

		private static Dictionary<int, AvatarAttackPunishMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AttackPunishData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarAttackPunishMetaData>();
			_itemList = new List<AvatarAttackPunishMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarAttackPunishMetaData avatarAttackPunishMetaData = new AvatarAttackPunishMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				if (!_itemDict.ContainsKey(avatarAttackPunishMetaData.LevelDifference))
				{
					_itemList.Add(avatarAttackPunishMetaData);
					_itemDict.Add(avatarAttackPunishMetaData.LevelDifference, avatarAttackPunishMetaData);
				}
			}
		}

		public static AvatarAttackPunishMetaData GetAvatarAttackPunishMetaDataByKey(int LevelDifference)
		{
			AvatarAttackPunishMetaData value;
			_itemDict.TryGetValue(LevelDifference, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarAttackPunishMetaData TryGetAvatarAttackPunishMetaDataByKey(int LevelDifference)
		{
			AvatarAttackPunishMetaData value;
			_itemDict.TryGetValue(LevelDifference, out value);
			return value;
		}

		public static List<AvatarAttackPunishMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarAttackPunishMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
