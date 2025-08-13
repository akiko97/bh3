using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarStarMetaDataReader
	{
		private static List<AvatarStarMetaData> _itemList;

		private static Dictionary<KeyValuePair<int, int>, AvatarStarMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarStarData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<KeyValuePair<int, int>, AvatarStarMetaData>();
			_itemList = new List<AvatarStarMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarStarMetaData avatarStarMetaData = new AvatarStarMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), array2[4].Trim(), array2[5].Trim(), array2[6].Trim(), float.Parse(array2[7]), float.Parse(array2[8]), float.Parse(array2[9]), float.Parse(array2[10]), float.Parse(array2[11]), float.Parse(array2[12]), float.Parse(array2[13]), float.Parse(array2[14]), float.Parse(array2[15]), float.Parse(array2[16]));
				if (!_itemDict.ContainsKey(new KeyValuePair<int, int>(avatarStarMetaData.avatarID, avatarStarMetaData.star)))
				{
					_itemList.Add(avatarStarMetaData);
					_itemDict.Add(new KeyValuePair<int, int>(avatarStarMetaData.avatarID, avatarStarMetaData.star), avatarStarMetaData);
				}
			}
		}

		public static AvatarStarMetaData GetAvatarStarMetaDataByKey(int avatarID, int star)
		{
			AvatarStarMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(avatarID, star), out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarStarMetaData TryGetAvatarStarMetaDataByKey(int avatarID, int star)
		{
			AvatarStarMetaData value;
			_itemDict.TryGetValue(new KeyValuePair<int, int>(avatarID, star), out value);
			return value;
		}

		public static List<AvatarStarMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarStarMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
