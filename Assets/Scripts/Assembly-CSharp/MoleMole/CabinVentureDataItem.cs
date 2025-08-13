using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CabinVentureDataItem : CabinDataItemBase
	{
		private static CabinVentureDataItem _instance;

		private CabinVentureLevelDataMetaData _levelMetaData;

		private CabinVentureDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)5;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinVentureDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinVentureDataItem();
			}
			return _instance;
		}

		public int GetMaxVentureNum()
		{
			int num = _levelMetaData.maxVentureNumBase;
			List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
			foreach (CabinTechTreeNode item in activeNodeList)
			{
				CabinTechTreeMetaData metaData = item._metaData;
				if (metaData.AbilityType == 2)
				{
					num += metaData.Argument1;
				}
			}
			return num;
		}

		public int GetMaxVentureNumInProgress()
		{
			int num = _levelMetaData.maxVentureInProgressNumBase;
			List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
			foreach (CabinTechTreeNode item in activeNodeList)
			{
				CabinTechTreeMetaData metaData = item._metaData;
				if (metaData.AbilityType == 3)
				{
					num += metaData.Argument1;
				}
			}
			return num;
		}

		public bool GetRefreshCost(int times)
		{
			CabinVentureRefreshDataMetaData cabinVentureRefreshDataMetaData = CabinVentureRefreshMetaDataReader.TryGetCabinVentureRefreshDataMetaDataByKey(times);
			if (cabinVentureRefreshDataMetaData == null)
			{
				return false;
			}
			if (_levelMetaData.refreshType > cabinVentureRefreshDataMetaData.needHcoinList.Count)
			{
				return false;
			}
			return true;
		}

		public override void SetupMateData()
		{
			_levelMetaData = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(level);
		}
	}
}
