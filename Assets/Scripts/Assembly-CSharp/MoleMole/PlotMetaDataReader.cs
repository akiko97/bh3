using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class PlotMetaDataReader
	{
		private static List<PlotMetaData> _itemList;

		private static Dictionary<int, PlotMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/PlotData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, PlotMetaData>();
			_itemList = new List<PlotMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				PlotMetaData plotMetaData = new PlotMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]));
				_itemList.Add(plotMetaData);
				_itemDict.Add(plotMetaData.plotID, plotMetaData);
			}
		}

		public static PlotMetaData GetPlotMetaDataByKey(int plotID)
		{
			PlotMetaData value;
			_itemDict.TryGetValue(plotID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static PlotMetaData TryGetPlotMetaDataByKey(int plotID)
		{
			PlotMetaData value;
			_itemDict.TryGetValue(plotID, out value);
			return value;
		}

		public static List<PlotMetaData> GetItemList()
		{
			return _itemList;
		}
	}
}
