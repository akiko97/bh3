using System.Collections.Generic;

namespace MoleMole
{
	public class UnlockUIDataReader
	{
		private static List<UnlockUIData> _itemList;

		private static Dictionary<int, UnlockUIData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/UnlockUIData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, UnlockUIData>();
			_itemList = new List<UnlockUIData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				UnlockUIData unlockUIData = new UnlockUIData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]));
				if (!_itemDict.ContainsKey(unlockUIData.id))
				{
					_itemList.Add(unlockUIData);
					_itemDict.Add(unlockUIData.id, unlockUIData);
				}
			}
		}

		public static UnlockUIData GetUnlockUIDataByKey(int id)
		{
			UnlockUIData value;
			_itemDict.TryGetValue(id, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static UnlockUIData TryGetUnlockUIDataByKey(int id)
		{
			UnlockUIData value;
			_itemDict.TryGetValue(id, out value);
			return value;
		}

		public static List<UnlockUIData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (UnlockUIData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
