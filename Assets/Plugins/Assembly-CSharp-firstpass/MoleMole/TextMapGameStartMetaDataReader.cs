using System.Collections.Generic;

namespace MoleMole
{
	public class TextMapGameStartMetaDataReader
	{
		private static List<TextMapMetaData> _itemList;

		private static Dictionary<string, TextMapMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("DataPersistent/_ExcelOutput/TextMapGameStart");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<string, TextMapMetaData>();
			_itemList = new List<TextMapMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
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
