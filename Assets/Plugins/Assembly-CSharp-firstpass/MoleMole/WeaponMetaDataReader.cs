using System.Collections.Generic;

namespace MoleMole
{
	public class WeaponMetaDataReader
	{
		private static List<WeaponMetaData> _itemList;

		private static Dictionary<int, WeaponMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/WeaponData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, WeaponMetaData>();
			_itemList = new List<WeaponMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				WeaponMetaData weaponMetaData = new WeaponMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), float.Parse(array2[10]), float.Parse(array2[11]), float.Parse(array2[12]), float.Parse(array2[13]), array2[14].Trim(), int.Parse(array2[15]), int.Parse(array2[16]), array2[17].Trim(), array2[18].Trim(), array2[19].Trim(), int.Parse(array2[20]), array2[21].Trim(), array2[22].Trim(), float.Parse(array2[23]), float.Parse(array2[24]), float.Parse(array2[25]), float.Parse(array2[26]), float.Parse(array2[27]), float.Parse(array2[28]), float.Parse(array2[29]), float.Parse(array2[30]), float.Parse(array2[31]), float.Parse(array2[32]), float.Parse(array2[33]), float.Parse(array2[34]), float.Parse(array2[35]), float.Parse(array2[36]), int.Parse(array2[37]), CommonUtils.GetStringListFromString(array2[38].Trim()), int.Parse(array2[39]), int.Parse(array2[40]), float.Parse(array2[41]), float.Parse(array2[42]), float.Parse(array2[43]), float.Parse(array2[44]), float.Parse(array2[45]), float.Parse(array2[46]), int.Parse(array2[47]), float.Parse(array2[48]), float.Parse(array2[49]), float.Parse(array2[50]), float.Parse(array2[51]), float.Parse(array2[52]), float.Parse(array2[53]), int.Parse(array2[54]), float.Parse(array2[55]), float.Parse(array2[56]), float.Parse(array2[57]), float.Parse(array2[58]), float.Parse(array2[59]), float.Parse(array2[60]));
				if (!_itemDict.ContainsKey(weaponMetaData.ID))
				{
					_itemList.Add(weaponMetaData);
					_itemDict.Add(weaponMetaData.ID, weaponMetaData);
				}
			}
		}

		public static WeaponMetaData GetWeaponMetaDataByKey(int ID)
		{
			WeaponMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static WeaponMetaData TryGetWeaponMetaDataByKey(int ID)
		{
			WeaponMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<WeaponMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (WeaponMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
