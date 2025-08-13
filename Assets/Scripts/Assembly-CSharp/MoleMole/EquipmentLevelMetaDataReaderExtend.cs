using System.Collections.Generic;

namespace MoleMole
{
	public class EquipmentLevelMetaDataReaderExtend
	{
		private static Dictionary<int, List<int>> _accumulateExpDict;

		public static void LoadFromFileAndBuildMap()
		{
			EquipmentLevelMetaDataReader.LoadFromFile();
			_accumulateExpDict = new Dictionary<int, List<int>>();
			List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
			int num = 7;
			foreach (EquipmentLevelMetaData item in itemList)
			{
				if (!_accumulateExpDict.ContainsKey(item.level))
				{
					_accumulateExpDict.Add(item.level, new List<int>());
				}
				if (item.level == 1)
				{
					for (int i = 0; i <= num; i++)
					{
						_accumulateExpDict[item.level].Add(0);
					}
				}
				else
				{
					for (int j = 0; j <= num; j++)
					{
						_accumulateExpDict[item.level].Add(_accumulateExpDict[item.level - 1][j] + EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(item.level - 1).expList[j]);
					}
				}
			}
		}

		public static int GetAccumulateExp(int level, int type)
		{
			return _accumulateExpDict[level][type];
		}

		private static void TestAll()
		{
			for (int i = 1; i < 10; i++)
			{
				Test(i, 1);
			}
			for (int j = 1; j < 10; j++)
			{
				Test(j, 2);
			}
		}

		private static void Test(int level, int type)
		{
		}
	}
}
