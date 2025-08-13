using System.Collections.Generic;

namespace MoleMole
{
	public class WeaponMetaDataReaderExtend
	{
		private static Dictionary<int, int> _evoPreDict;

		private static List<int> _path;

		public static void LoadFromFileAndBuildMap()
		{
			WeaponMetaDataReader.LoadFromFile();
			_evoPreDict = new Dictionary<int, int>();
			_path = new List<int>();
			List<WeaponMetaData> itemList = WeaponMetaDataReader.GetItemList();
			foreach (WeaponMetaData item in itemList)
			{
				if (item.evoID > 0)
				{
					_evoPreDict[item.evoID] = item.ID;
				}
			}
		}

		public static List<int> GetEvoPath(int id)
		{
			_path.Clear();
			_path.Add(id);
			int num = id;
			while (_evoPreDict.ContainsKey(num))
			{
				num = _evoPreDict[num];
				_path.Insert(0, num);
			}
			return _path;
		}
	}
}
