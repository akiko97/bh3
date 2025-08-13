using System.Collections.Generic;

namespace MoleMole
{
	public class ClassMetaDataReader
	{
		private static List<ClassMetaData> _itemList;

		private static Dictionary<int, ClassMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/ClassData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ClassMetaData>();
			_itemList = new List<ClassMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ClassMetaData classMetaData = new ClassMetaData(int.Parse(array2[0]), array2[1].Trim(), array2[2].Trim(), array2[3].Trim(), array2[4].Trim());
				if (!_itemDict.ContainsKey(classMetaData.classID))
				{
					_itemList.Add(classMetaData);
					_itemDict.Add(classMetaData.classID, classMetaData);
				}
			}
		}

		public static ClassMetaData GetClassMetaDataByKey(int classID)
		{
			ClassMetaData value;
			_itemDict.TryGetValue(classID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ClassMetaData TryGetClassMetaDataByKey(int classID)
		{
			ClassMetaData value;
			_itemDict.TryGetValue(classID, out value);
			return value;
		}

		public static List<ClassMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ClassMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
