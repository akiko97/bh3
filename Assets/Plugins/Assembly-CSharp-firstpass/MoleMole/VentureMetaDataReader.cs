using System.Collections.Generic;

namespace MoleMole
{
	public class VentureMetaDataReader
	{
		private static List<VentureMetaData> _itemList;

		private static Dictionary<int, VentureMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CabinDispatchVentureData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, VentureMetaData>();
			_itemList = new List<VentureMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				VentureMetaData ventureMetaData = new VentureMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), array2[4].Trim(), array2[5].Trim(), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), int.Parse(array2[15]), int.Parse(array2[16]), int.Parse(array2[17]), int.Parse(array2[18]), int.Parse(array2[19]), int.Parse(array2[20]), CommonUtils.GetIntListFromString(array2[21].Trim()), CommonUtils.GetIntListFromString(array2[22].Trim()));
				if (!_itemDict.ContainsKey(ventureMetaData.ID))
				{
					_itemList.Add(ventureMetaData);
					_itemDict.Add(ventureMetaData.ID, ventureMetaData);
				}
			}
		}

		public static VentureMetaData GetVentureMetaDataByKey(int ID)
		{
			VentureMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static VentureMetaData TryGetVentureMetaDataByKey(int ID)
		{
			VentureMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<VentureMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (VentureMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
