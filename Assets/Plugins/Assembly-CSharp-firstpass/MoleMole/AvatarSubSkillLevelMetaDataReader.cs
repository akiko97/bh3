using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarSubSkillLevelMetaDataReader
	{
		private static List<AvatarSubSkillLevelMetaData> _itemList;

		private static Dictionary<int, AvatarSubSkillLevelMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarSubSkillLevelData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarSubSkillLevelMetaData>();
			_itemList = new List<AvatarSubSkillLevelMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list2 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[26]))
				{
					list2.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list3 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item2 in CommonUtils.GetStringListFromString(array2[27]))
				{
					list3.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item2));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list4 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item3 in CommonUtils.GetStringListFromString(array2[28]))
				{
					list4.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item3));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list5 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item4 in CommonUtils.GetStringListFromString(array2[29]))
				{
					list5.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item4));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list6 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item5 in CommonUtils.GetStringListFromString(array2[30]))
				{
					list6.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item5));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list7 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item6 in CommonUtils.GetStringListFromString(array2[31]))
				{
					list7.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item6));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list8 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item7 in CommonUtils.GetStringListFromString(array2[32]))
				{
					list8.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item7));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list9 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item8 in CommonUtils.GetStringListFromString(array2[33]))
				{
					list9.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item8));
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list10 = new List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>();
				foreach (string item9 in CommonUtils.GetStringListFromString(array2[34]))
				{
					list10.Add(new AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem(item9));
				}
				AvatarSubSkillLevelMetaData avatarSubSkillLevelMetaData = new AvatarSubSkillLevelMetaData(int.Parse(array2[0]), new List<int>
				{
					0,
					int.Parse(array2[1]),
					int.Parse(array2[2]),
					int.Parse(array2[3]),
					int.Parse(array2[4]),
					int.Parse(array2[5]),
					int.Parse(array2[6]),
					int.Parse(array2[7]),
					int.Parse(array2[8]),
					int.Parse(array2[9]),
					int.Parse(array2[10]),
					int.Parse(array2[11]),
					int.Parse(array2[12]),
					int.Parse(array2[13]),
					int.Parse(array2[14]),
					int.Parse(array2[15]),
					int.Parse(array2[16]),
					int.Parse(array2[17]),
					int.Parse(array2[18]),
					int.Parse(array2[19]),
					int.Parse(array2[20]),
					int.Parse(array2[21]),
					int.Parse(array2[22]),
					int.Parse(array2[23]),
					int.Parse(array2[24]),
					int.Parse(array2[25])
				}, new List<List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem>> { null, list2, list3, list4, list5, list6, list7, list8, list9, list10 });
				if (!_itemDict.ContainsKey(avatarSubSkillLevelMetaData.unlockLv))
				{
					_itemList.Add(avatarSubSkillLevelMetaData);
					_itemDict.Add(avatarSubSkillLevelMetaData.unlockLv, avatarSubSkillLevelMetaData);
				}
			}
		}

		public static AvatarSubSkillLevelMetaData GetAvatarSubSkillLevelMetaDataByKey(int unlockLv)
		{
			AvatarSubSkillLevelMetaData value;
			_itemDict.TryGetValue(unlockLv, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarSubSkillLevelMetaData TryGetAvatarSubSkillLevelMetaDataByKey(int unlockLv)
		{
			AvatarSubSkillLevelMetaData value;
			_itemDict.TryGetValue(unlockLv, out value);
			return value;
		}

		public static List<AvatarSubSkillLevelMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarSubSkillLevelMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
