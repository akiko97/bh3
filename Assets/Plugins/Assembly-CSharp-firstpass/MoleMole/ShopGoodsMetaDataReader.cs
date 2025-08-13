using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class ShopGoodsMetaDataReader
	{
		private static List<ShopGoodsMetaData> _itemList;

		private static Dictionary<int, ShopGoodsMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/ShopGoodsData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ShopGoodsMetaData>();
			_itemList = new List<ShopGoodsMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ShopGoodsMetaData shopGoodsMetaData = new ShopGoodsMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), int.Parse(array2[15]), int.Parse(array2[16]), int.Parse(array2[17]), int.Parse(array2[18]), Convert.ToBoolean(int.Parse(array2[19])));
				if (!_itemDict.ContainsKey(shopGoodsMetaData.ID))
				{
					_itemList.Add(shopGoodsMetaData);
					_itemDict.Add(shopGoodsMetaData.ID, shopGoodsMetaData);
				}
			}
		}

		public static ShopGoodsMetaData GetShopGoodsMetaDataByKey(int ID)
		{
			ShopGoodsMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ShopGoodsMetaData TryGetShopGoodsMetaDataByKey(int ID)
		{
			ShopGoodsMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<ShopGoodsMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ShopGoodsMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
