using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarFragmentMetaDataReader
	{
		private static List<AvatarFragmentMetaData> _itemList;

		private static Dictionary<int, AvatarFragmentMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/AvatarFragmentData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, AvatarFragmentMetaData>();
			_itemList = new List<AvatarFragmentMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				AvatarFragmentMetaData avatarFragmentMetaData = new AvatarFragmentMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), float.Parse(array2[7]), float.Parse(array2[8]), float.Parse(array2[9]), float.Parse(array2[10]), array2[11].Trim(), int.Parse(array2[12]), array2[13].Trim(), array2[14].Trim(), int.Parse(array2[15]), array2[16].Trim(), array2[17].Trim(), float.Parse(array2[18]), int.Parse(array2[19]));
				if (!_itemDict.ContainsKey(avatarFragmentMetaData.ID))
				{
					_itemList.Add(avatarFragmentMetaData);
					_itemDict.Add(avatarFragmentMetaData.ID, avatarFragmentMetaData);
				}
			}
		}

		public static AvatarFragmentMetaData GetAvatarFragmentMetaDataByKey(int ID)
		{
			AvatarFragmentMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static AvatarFragmentMetaData TryGetAvatarFragmentMetaDataByKey(int ID)
		{
			AvatarFragmentMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<AvatarFragmentMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (AvatarFragmentMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
