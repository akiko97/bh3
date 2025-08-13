using System.Collections.Generic;

namespace MoleMole
{
	public class TextMapMetaDataReader
	{
		private static List<TextMapMetaData> _itemList;

		private static Dictionary<string, TextMapMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Tutorial");
			array = text.Split("\n"[0]);
			for (int j = 1; j < array.Length; j++)
			{
				if (array[j].Length >= 1)
				{
					list.Add(array[j]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_StageFailTips");
			array = text.Split("\n"[0]);
			for (int k = 1; k < array.Length; k++)
			{
				if (array[k].Length >= 1)
				{
					list.Add(array[k]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_MissionName");
			array = text.Split("\n"[0]);
			for (int l = 1; l < array.Length; l++)
			{
				if (array[l].Length >= 1)
				{
					list.Add(array[l]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_MissionDisplay");
			array = text.Split("\n"[0]);
			for (int m = 1; m < array.Length; m++)
			{
				if (array[m].Length >= 1)
				{
					list.Add(array[m]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Endless");
			array = text.Split("\n"[0]);
			for (int n = 1; n < array.Length; n++)
			{
				if (array[n].Length >= 1)
				{
					list.Add(array[n]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Affix");
			array = text.Split("\n"[0]);
			for (int num = 1; num < array.Length; num++)
			{
				if (array[num].Length >= 1)
				{
					list.Add(array[num]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_DispatchVenture");
			array = text.Split("\n"[0]);
			for (int num2 = 1; num2 < array.Length; num2++)
			{
				if (array[num2].Length >= 1)
				{
					list.Add(array[num2]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_TechTree");
			array = text.Split("\n"[0]);
			for (int num3 = 1; num3 < array.Length; num3++)
			{
				if (array[num3].Length >= 1)
				{
					list.Add(array[num3]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Cabin");
			array = text.Split("\n"[0]);
			for (int num4 = 1; num4 < array.Length; num4++)
			{
				if (array[num4].Length >= 1)
				{
					list.Add(array[num4]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Activity");
			array = text.Split("\n"[0]);
			for (int num5 = 1; num5 < array.Length; num5++)
			{
				if (array[num5].Length >= 1)
				{
					list.Add(array[num5]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_TouchBuff");
			array = text.Split("\n"[0]);
			for (int num6 = 1; num6 < array.Length; num6++)
			{
				if (array[num6].Length >= 1)
				{
					list.Add(array[num6]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Avatar");
			array = text.Split("\n"[0]);
			for (int num7 = 1; num7 < array.Length; num7++)
			{
				if (array[num7].Length >= 1)
				{
					list.Add(array[num7]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Material");
			array = text.Split("\n"[0]);
			for (int num8 = 1; num8 < array.Length; num8++)
			{
				if (array[num8].Length >= 1)
				{
					list.Add(array[num8]);
				}
			}
			text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/TextMap_Plot");
			array = text.Split("\n"[0]);
			for (int num9 = 1; num9 < array.Length; num9++)
			{
				if (array[num9].Length >= 1)
				{
					list.Add(array[num9]);
				}
			}
			int num10 = list.Count - 1;
			_itemDict = new Dictionary<string, TextMapMetaData>();
			_itemList = new List<TextMapMetaData>(num10);
			for (int num11 = 1; num11 <= num10; num11++)
			{
				string[] array2 = list[num11].Split("\t"[0]);
				TextMapMetaData textMapMetaData = new TextMapMetaData(array2[0].Trim(), array2[1].Trim());
				if (!_itemDict.ContainsKey(textMapMetaData.ID))
				{
					_itemList.Add(textMapMetaData);
					_itemDict.Add(textMapMetaData.ID, textMapMetaData);
				}
			}
		}

		public static TextMapMetaData GetTextMapMetaDataByKey(string ID)
		{
			TextMapMetaData value;
			_itemDict.TryGetValue(ID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static TextMapMetaData TryGetTextMapMetaDataByKey(string ID)
		{
			TextMapMetaData value;
			_itemDict.TryGetValue(ID, out value);
			return value;
		}

		public static List<TextMapMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (TextMapMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}
	}
}
