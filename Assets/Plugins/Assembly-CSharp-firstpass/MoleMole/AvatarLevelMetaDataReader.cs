using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarLevelMetaDataReader
	{
		private static List<AvatarLevelMetaData> _itemList;

		private static Dictionary<int, AvatarLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarLevelData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarLevelMetaData>();
			_itemList = new List<AvatarLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarLevelMetaData avatarLevelMetaData = new AvatarLevelMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), float.Parse(array2[3]));
				if (!_itemDict.ContainsKey(avatarLevelMetaData.level))
				{
					_itemList.Add(avatarLevelMetaData);
					_itemDict.Add(avatarLevelMetaData.level, avatarLevelMetaData);
				}
			}
		}

		public static AvatarLevelMetaData GetAvatarLevelMetaDataByKey(int level)
		{
			AvatarLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarLevelMetaData TryGetAvatarLevelMetaDataByKey(int level)
		{
			AvatarLevelMetaData value;
			_itemDict.TryGetValue(level, out value);
			return value;
		}

		public static List<AvatarLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
