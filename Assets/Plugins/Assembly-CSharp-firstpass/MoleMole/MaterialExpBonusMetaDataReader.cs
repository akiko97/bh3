using System.Collections.Generic;

namespace MoleMole
{
	public class MaterialExpBonusMetaDataReader
	{
		private static List<MaterialExpBonusMetaData> _itemList;

		private static Dictionary<int, MaterialExpBonusMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MaterialExpBonusData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MaterialExpBonusMetaData>();
			_itemList = new List<MaterialExpBonusMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MaterialExpBonusMetaData materialExpBonusMetaData = new MaterialExpBonusMetaData(int.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2]));
				if (!_itemDict.ContainsKey(materialExpBonusMetaData.materialId))
				{
					_itemList.Add(materialExpBonusMetaData);
					_itemDict.Add(materialExpBonusMetaData.materialId, materialExpBonusMetaData);
				}
			}
		}

		public static MaterialExpBonusMetaData GetMaterialExpBonusMetaDataByKey(int materialId)
		{
			MaterialExpBonusMetaData value;
			_itemDict.TryGetValue(materialId, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MaterialExpBonusMetaData TryGetMaterialExpBonusMetaDataByKey(int materialId)
		{
			MaterialExpBonusMetaData value;
			_itemDict.TryGetValue(materialId, out value);
			return value;
		}

		public static List<MaterialExpBonusMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MaterialExpBonusMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
