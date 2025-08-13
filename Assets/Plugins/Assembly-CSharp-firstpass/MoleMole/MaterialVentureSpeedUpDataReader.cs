using System.Collections.Generic;

namespace MoleMole
{
	public class MaterialVentureSpeedUpDataReader
	{
		private static List<MaterialVentureSpeedUpData> _itemList;

		private static Dictionary<int, MaterialVentureSpeedUpData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/MaterialVentureSpeedUpData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, MaterialVentureSpeedUpData>();
			_itemList = new List<MaterialVentureSpeedUpData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				MaterialVentureSpeedUpData materialVentureSpeedUpData = new MaterialVentureSpeedUpData(int.Parse(array2[0]), int.Parse(array2[1]));
				if (!_itemDict.ContainsKey(materialVentureSpeedUpData.MaterialID))
				{
					_itemList.Add(materialVentureSpeedUpData);
					_itemDict.Add(materialVentureSpeedUpData.MaterialID, materialVentureSpeedUpData);
				}
			}
		}

		public static MaterialVentureSpeedUpData GetMaterialVentureSpeedUpDataByKey(int MaterialID)
		{
			MaterialVentureSpeedUpData value;
			_itemDict.TryGetValue(MaterialID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static MaterialVentureSpeedUpData TryGetMaterialVentureSpeedUpDataByKey(int MaterialID)
		{
			MaterialVentureSpeedUpData value;
			_itemDict.TryGetValue(MaterialID, out value);
			return value;
		}

		public static List<MaterialVentureSpeedUpData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (MaterialVentureSpeedUpData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
