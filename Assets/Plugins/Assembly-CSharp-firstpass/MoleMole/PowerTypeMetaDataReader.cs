using System.Collections.Generic;

namespace MoleMole
{
	public class PowerTypeMetaDataReader
	{
		private static List<PowerTypeMetaData> _itemList;

		private static Dictionary<int, PowerTypeMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/PowerTypeData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, PowerTypeMetaData>();
			_itemList = new List<PowerTypeMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				PowerTypeMetaData powerTypeMetaData = new PowerTypeMetaData(int.Parse(array2[0]), float.Parse(array2[1]));
				if (!_itemDict.ContainsKey(powerTypeMetaData.type))
				{
					_itemList.Add(powerTypeMetaData);
					_itemDict.Add(powerTypeMetaData.type, powerTypeMetaData);
				}
			}
		}

		public static PowerTypeMetaData GetPowerTypeMetaDataByKey(int type)
		{
			PowerTypeMetaData value;
			_itemDict.TryGetValue(type, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static PowerTypeMetaData TryGetPowerTypeMetaDataByKey(int type)
		{
			PowerTypeMetaData value;
			_itemDict.TryGetValue(type, out value);
			return value;
		}

		public static List<PowerTypeMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (PowerTypeMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
