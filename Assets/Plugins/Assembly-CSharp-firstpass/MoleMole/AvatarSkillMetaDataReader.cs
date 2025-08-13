using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarSkillMetaDataReader
	{
		private static List<AvatarSkillMetaData> _itemList;

		private static Dictionary<int, AvatarSkillMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarSkillData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarSkillMetaData>();
			_itemList = new List<AvatarSkillMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarSkillMetaData avatarSkillMetaData = new AvatarSkillMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), array2[8].Trim(), array2[9].Trim(), array2[10].Trim(), float.Parse(array2[11]), char.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), float.Parse(array2[15]), char.Parse(array2[16]), int.Parse(array2[17]), int.Parse(array2[18]), float.Parse(array2[19]), char.Parse(array2[20]), int.Parse(array2[21]), int.Parse(array2[22]), array2[23].Trim());
				if (!_itemDict.ContainsKey(avatarSkillMetaData.skillId))
				{
					_itemList.Add(avatarSkillMetaData);
					_itemDict.Add(avatarSkillMetaData.skillId, avatarSkillMetaData);
				}
			}
		}

		public static AvatarSkillMetaData GetAvatarSkillMetaDataByKey(int skillId)
		{
			AvatarSkillMetaData value;
			_itemDict.TryGetValue(skillId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarSkillMetaData TryGetAvatarSkillMetaDataByKey(int skillId)
		{
			AvatarSkillMetaData value;
			_itemDict.TryGetValue(skillId, out value);
			return value;
		}

		public static List<AvatarSkillMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarSkillMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
