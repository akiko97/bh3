using System.Collections.Generic;

namespace MoleMole
{
	public class MonsterUIMetaDataReaderExtend
	{
		private static Dictionary<string, MonsterUIMetaData> _itemDict;

		public static void LoadFromFileAndBuildMap()
		{
			MonsterUIMetaDataReader.LoadFromFile();
			List<MonsterUIMetaData> itemList = MonsterUIMetaDataReader.GetItemList();
			_itemDict = new Dictionary<string, MonsterUIMetaData>();
			foreach (MonsterUIMetaData item in itemList)
			{
				_itemDict.Add(item.name, item);
			}
		}

		public static MonsterUIMetaData GetMonsterUIMetaDataByName(string name)
		{
			if (!_itemDict.ContainsKey(name))
			{
				return null;
			}
			return _itemDict[name];
		}
	}
}
