using System.Collections.Generic;

namespace MoleMole
{
	public class MaterialAvatarExpBonusMetaDataReader
	{
		private static List<MaterialAvatarExpBonusMetaData> _itemList;

		private static Dictionary<int, MaterialAvatarExpBonusMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MaterialAvatarExpBonusData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MaterialAvatarExpBonusMetaData>();
			_itemList = new List<MaterialAvatarExpBonusMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MaterialAvatarExpBonusMetaData materialAvatarExpBonusMetaData = new MaterialAvatarExpBonusMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]), float.Parse(array2[3]));
				if (!_itemDict.ContainsKey(materialAvatarExpBonusMetaData.materialId))
				{
					_itemList.Add(materialAvatarExpBonusMetaData);
					_itemDict.Add(materialAvatarExpBonusMetaData.materialId, materialAvatarExpBonusMetaData);
				}
			}
		}

		public static MaterialAvatarExpBonusMetaData GetMaterialAvatarExpBonusMetaDataByKey(int materialId)
		{
			MaterialAvatarExpBonusMetaData value;
			_itemDict.TryGetValue(materialId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MaterialAvatarExpBonusMetaData TryGetMaterialAvatarExpBonusMetaDataByKey(int materialId)
		{
			MaterialAvatarExpBonusMetaData value;
			_itemDict.TryGetValue(materialId, out value);
			return value;
		}

		public static List<MaterialAvatarExpBonusMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MaterialAvatarExpBonusMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
