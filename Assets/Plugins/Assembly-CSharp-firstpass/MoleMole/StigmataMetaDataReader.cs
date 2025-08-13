using System.Collections.Generic;

namespace MoleMole
{
	public class StigmataMetaDataReader
	{
		private static List<StigmataMetaData> _itemList;

		private static Dictionary<int, StigmataMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/StigmataData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, StigmataMetaData>();
			_itemList = new List<StigmataMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				StigmataMetaData stigmataMetaData = new StigmataMetaData(int.Parse(array2[0]), array2[1].Trim(), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), float.Parse(array2[10]), float.Parse(array2[11]), float.Parse(array2[12]), float.Parse(array2[13]), array2[14].Trim(), int.Parse(array2[15]), array2[16].Trim(), array2[17].Trim(), int.Parse(array2[18]), array2[19].Trim(), array2[20].Trim(), float.Parse(array2[21]), float.Parse(array2[22]), float.Parse(array2[23]), float.Parse(array2[24]), float.Parse(array2[25]), float.Parse(array2[26]), float.Parse(array2[27]), float.Parse(array2[28]), float.Parse(array2[29]), float.Parse(array2[30]), float.Parse(array2[31]), float.Parse(array2[32]), float.Parse(array2[33]), float.Parse(array2[34]), CommonUtils.GetStringListFromString(array2[35].Trim()), int.Parse(array2[36]), int.Parse(array2[37]), float.Parse(array2[38]), float.Parse(array2[39]), float.Parse(array2[40]), float.Parse(array2[41]), float.Parse(array2[42]), float.Parse(array2[43]), int.Parse(array2[44]), float.Parse(array2[45]), float.Parse(array2[46]), float.Parse(array2[47]), float.Parse(array2[48]), float.Parse(array2[49]), float.Parse(array2[50]), int.Parse(array2[51]), float.Parse(array2[52]), float.Parse(array2[53]), float.Parse(array2[54]), float.Parse(array2[55]), float.Parse(array2[56]), float.Parse(array2[57]), int.Parse(array2[58]), array2[59].Trim(), array2[60].Trim(), float.Parse(array2[61]), float.Parse(array2[62]), float.Parse(array2[63]), int.Parse(array2[64]), int.Parse(array2[65]), int.Parse(array2[66]), int.Parse(array2[67]), int.Parse(array2[68]));
				if (!_itemDict.ContainsKey(stigmataMetaData.ID))
				{
					_itemList.Add(stigmataMetaData);
					_itemDict.Add(stigmataMetaData.ID, stigmataMetaData);
				}
			}
		}

		public static StigmataMetaData GetStigmataMetaDataByKey(int ID)
		{
			StigmataMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static StigmataMetaData TryGetStigmataMetaDataByKey(int ID)
		{
			StigmataMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<StigmataMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (StigmataMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
