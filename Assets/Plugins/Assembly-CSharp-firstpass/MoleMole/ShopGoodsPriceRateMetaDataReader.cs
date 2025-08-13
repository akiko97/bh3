using System.Collections.Generic;

namespace MoleMole
{
	public class ShopGoodsPriceRateMetaDataReader
	{
		private static List<ShopGoodsPriceRateMetaData> _itemList;

		private static Dictionary<int, ShopGoodsPriceRateMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/ShopGoodsPriceRate");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, ShopGoodsPriceRateMetaData>();
			_itemList = new List<ShopGoodsPriceRateMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				ShopGoodsPriceRateMetaData shopGoodsPriceRateMetaData = new ShopGoodsPriceRateMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]));
				if (!_itemDict.ContainsKey(shopGoodsPriceRateMetaData.BuyTimes))
				{
					_itemList.Add(shopGoodsPriceRateMetaData);
					_itemDict.Add(shopGoodsPriceRateMetaData.BuyTimes, shopGoodsPriceRateMetaData);
				}
			}
		}

		public static ShopGoodsPriceRateMetaData GetShopGoodsPriceRateMetaDataByKey(int BuyTimes)
		{
			ShopGoodsPriceRateMetaData value;
			_itemDict.TryGetValue(BuyTimes, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static ShopGoodsPriceRateMetaData TryGetShopGoodsPriceRateMetaDataByKey(int BuyTimes)
		{
			ShopGoodsPriceRateMetaData value;
			_itemDict.TryGetValue(BuyTimes, out value);
			return value;
		}

		public static List<ShopGoodsPriceRateMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (ShopGoodsPriceRateMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
