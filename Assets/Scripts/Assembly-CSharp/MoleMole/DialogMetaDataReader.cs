using System.Collections.Generic;

namespace MoleMole
{
	public class DialogMetaDataReader
	{
		private static List<DialogMetaData> _itemList;

		private static Dictionary<int, DialogMetaData> _itemDict;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			string text = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/DialogData");
			string[] array = text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_itemDict = new Dictionary<int, DialogMetaData>();
			_itemList = new List<DialogMetaData>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				List<DialogMetaData.PlotChatNode> list2 = new List<DialogMetaData.PlotChatNode>();
				foreach (string item in CommonUtils.GetStringListFromString(array2[4]))
				{
					list2.Add(new DialogMetaData.PlotChatNode(item));
				}
				DialogMetaData dialogMetaData = new DialogMetaData(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), array2[3].Trim(), list2, array2[5].Trim());
				if (!_itemDict.ContainsKey(dialogMetaData.dialogID))
				{
					_itemList.Add(dialogMetaData);
					_itemDict.Add(dialogMetaData.dialogID, dialogMetaData);
				}
			}
		}

		public static DialogMetaData GetDialogMetaDataByKey(int dialogID)
		{
			DialogMetaData value;
			_itemDict.TryGetValue(dialogID, out value);
			if (value == null)
			{
			}
			return value;
		}

		public static DialogMetaData TryGetDialogMetaDataByKey(int dialogID)
		{
			DialogMetaData value;
			_itemDict.TryGetValue(dialogID, out value);
			return value;
		}

		public static List<DialogMetaData> GetItemList()
		{
			return _itemList;
		}

		public static int CalculateContentHash()
		{
			int lastHash = 0;
			foreach (DialogMetaData item in _itemList)
			{
				HashUtils.TryHashObject(item, ref lastHash);
			}
			return lastHash;
		}

		public static DialogDataItem GetFirstLeftDialogDataItem(PlotDataItem plotDataItem)
		{
			if (_itemDict.ContainsKey(plotDataItem.startDialogID) && _itemDict.ContainsKey(plotDataItem.endDialogID))
			{
				for (int i = plotDataItem.startDialogID; i < plotDataItem.endDialogID; i++)
				{
					if (_itemDict.ContainsKey(i) && _itemDict[i].screenSide == 0)
					{
						DialogMetaData dialogMetaData = _itemDict[i];
						return new DialogDataItem(dialogMetaData);
					}
				}
			}
			return null;
		}

		public static DialogDataItem GetFirstRightDialogDataItem(PlotDataItem plotDataItem)
		{
			if (_itemDict.ContainsKey(plotDataItem.startDialogID) && _itemDict.ContainsKey(plotDataItem.endDialogID))
			{
				for (int i = plotDataItem.startDialogID; i < plotDataItem.endDialogID; i++)
				{
					if (_itemDict.ContainsKey(i) && _itemDict[i].screenSide == 1)
					{
						DialogMetaData dialogMetaData = _itemDict[i];
						return new DialogDataItem(dialogMetaData);
					}
				}
			}
			return null;
		}
	}
}
