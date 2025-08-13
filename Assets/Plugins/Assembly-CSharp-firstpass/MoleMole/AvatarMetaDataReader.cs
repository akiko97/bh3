using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarMetaDataReader
	{
		private static List<AvatarMetaData> _itemList;

		private static Dictionary<int, AvatarMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarMetaData>();
			_itemList = new List<AvatarMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarMetaData avatarMetaData = new AvatarMetaData(int.Parse(array2[0]), int.Parse(array2[1]), array2[2].Trim(), array2[3].Trim(), array2[4].Trim(), array2[5].Trim(), CommonUtils.GetIntListFromString(array2[6].Trim()), int.Parse(array2[7]), CommonUtils.GetIntListFromString(array2[8].Trim()), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), int.Parse(array2[15]), float.Parse(array2[16]), float.Parse(array2[17]), float.Parse(array2[18]), int.Parse(array2[19]), float.Parse(array2[20]), float.Parse(array2[21]), float.Parse(array2[22]), int.Parse(array2[23]), float.Parse(array2[24]), float.Parse(array2[25]), float.Parse(array2[26]), int.Parse(array2[27]), float.Parse(array2[28]), int.Parse(array2[29]));
				if (!_itemDict.ContainsKey(avatarMetaData.avatarID))
				{
					_itemList.Add(avatarMetaData);
					_itemDict.Add(avatarMetaData.avatarID, avatarMetaData);
				}
			}
		}

		public static AvatarMetaData GetAvatarMetaDataByKey(int avatarID)
		{
			AvatarMetaData value;
			_itemDict.TryGetValue(avatarID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarMetaData TryGetAvatarMetaDataByKey(int avatarID)
		{
			AvatarMetaData value;
			_itemDict.TryGetValue(avatarID, out value);
			return value;
		}

		public static List<AvatarMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
