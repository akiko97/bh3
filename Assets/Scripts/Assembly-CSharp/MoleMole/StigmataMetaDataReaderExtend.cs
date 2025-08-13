using System.Collections.Generic;

namespace MoleMole
{
	public class StigmataMetaDataReaderExtend
	{
		private static Dictionary<int, HashSet<int>> _evoMap;

		public static void LoadFromFileAndBuildMap()
		{
			StigmataMetaDataReader.LoadFromFile();
			List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
			_evoMap = new Dictionary<int, HashSet<int>>();
			foreach (StigmataMetaData item in itemList)
			{
				if (item.evoID > 0)
				{
					_evoMap[item.ID] = CalculateEvoList(item);
				}
			}
		}

		public static bool IsEvoRelation(int id1, int id2)
		{
			return CanEvo(id1, id2) || CanEvo(id2, id1);
		}

		public static bool CanEvo(int from, int to)
		{
			return _evoMap.ContainsKey(from) && _evoMap[from].Contains(to);
		}

		private static HashSet<int> CalculateEvoList(StigmataMetaData meta)
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int evoID = meta.evoID; evoID > 0; evoID = StigmataMetaDataReader.GetStigmataMetaDataByKey(evoID).evoID)
			{
				hashSet.Add(evoID);
			}
			return hashSet;
		}
	}
}
