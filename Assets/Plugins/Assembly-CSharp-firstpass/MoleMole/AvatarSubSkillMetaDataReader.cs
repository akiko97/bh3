using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarSubSkillMetaDataReader
	{
		private static List<AvatarSubSkillMetaData> _itemList;

		private static Dictionary<int, AvatarSubSkillMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarSubSkillData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarSubSkillMetaData>();
			_itemList = new List<AvatarSubSkillMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<AvatarSubSkillMetaData.SkillUpLevelNeedItem> list2 = new List<AvatarSubSkillMetaData.SkillUpLevelNeedItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[12]))
				{
					list2.Add(new AvatarSubSkillMetaData.SkillUpLevelNeedItem(item));
				}
				List<AvatarSubSkillMetaData.UpLevelStarNeed> list3 = new List<AvatarSubSkillMetaData.UpLevelStarNeed>();
				foreach (string item2 in CommonUtils.GetStringListFromString(array2[25]))
				{
					list3.Add(new AvatarSubSkillMetaData.UpLevelStarNeed(item2));
				}
				AvatarSubSkillMetaData avatarSubSkillMetaData = new AvatarSubSkillMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), int.Parse(array2[4]), array2[5].Trim(), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), list2, int.Parse(array2[13]), int.Parse(array2[14]), int.Parse(array2[15]), float.Parse(array2[16]), float.Parse(array2[17]), float.Parse(array2[18]), float.Parse(array2[19]), float.Parse(array2[20]), float.Parse(array2[21]), int.Parse(array2[22]), int.Parse(array2[23]), int.Parse(array2[24]), list3);
				if (!_itemDict.ContainsKey(avatarSubSkillMetaData.avatarSubSkillId))
				{
					_itemList.Add(avatarSubSkillMetaData);
					_itemDict.Add(avatarSubSkillMetaData.avatarSubSkillId, avatarSubSkillMetaData);
				}
			}
		}

		public static AvatarSubSkillMetaData GetAvatarSubSkillMetaDataByKey(int avatarSubSkillId)
		{
			AvatarSubSkillMetaData value;
			_itemDict.TryGetValue(avatarSubSkillId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarSubSkillMetaData TryGetAvatarSubSkillMetaDataByKey(int avatarSubSkillId)
		{
			AvatarSubSkillMetaData value;
			_itemDict.TryGetValue(avatarSubSkillId, out value);
			return value;
		}

		public static List<AvatarSubSkillMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarSubSkillMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
