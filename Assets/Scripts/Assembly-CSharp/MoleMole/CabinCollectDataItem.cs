using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CabinCollectDataItem : CabinDataItemBase
	{
		private static CabinCollectDataItem _instance;

		private CabinCollectLevelDataMetaData _metaData;

		public int currentScoinAmount;

		public bool canUpdateScoinLate;

		public DateTime nextScoinUpdateTime;

		public List<DropItem> dropItems;

		public float speed
		{
			get
			{
				float num = _metaData.scoinGrowthBase;
				List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
				foreach (CabinTechTreeNode item in activeNodeList)
				{
					CabinTechTreeMetaData metaData = item._metaData;
					if (metaData.AbilityType == 7)
					{
						num += (float)metaData.Argument1;
					}
				}
				return num;
			}
		}

		public float topLimit
		{
			get
			{
				float num = _metaData.scoinStorageBase;
				List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
				foreach (CabinTechTreeNode item in activeNodeList)
				{
					CabinTechTreeMetaData metaData = item._metaData;
					if (metaData.AbilityType == 8)
					{
						num += (float)metaData.Argument1;
					}
				}
				return num;
			}
		}

		public float crtRatio
		{
			get
			{
				float num = (float)_metaData.extraScoinRatioBase / 100f;
				List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
				foreach (CabinTechTreeNode item in activeNodeList)
				{
					CabinTechTreeMetaData metaData = item._metaData;
					if (metaData.AbilityType == 9)
					{
						num += (float)metaData.Argument1 / 100f;
					}
				}
				return num;
			}
		}

		public float crtExtraRatio
		{
			get
			{
				float num = _metaData.extraScoinRatioAddBase / 100f;
				List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
				foreach (CabinTechTreeNode item in activeNodeList)
				{
					CabinTechTreeMetaData metaData = item._metaData;
					if (metaData.AbilityType == 10)
					{
						num += (float)metaData.Argument1 / 100f;
					}
				}
				return num;
			}
		}

		private CabinCollectDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)3;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinCollectDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinCollectDataItem();
			}
			return _instance;
		}

		public bool CanFetchScoin()
		{
			if (currentScoinAmount <= 0 && canUpdateScoinLate && TimeUtil.Now >= nextScoinUpdateTime)
			{
				Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
				return false;
			}
			return currentScoinAmount > 0;
		}

		public bool HasScoin()
		{
			return currentScoinAmount > 0;
		}

		public bool TimeToFetch()
		{
			return currentScoinAmount <= 0 && canUpdateScoinLate && TimeUtil.Now >= nextScoinUpdateTime;
		}

		public override void SetupMateData()
		{
			_metaData = CabinCollectLevelMetaDataReader.TryGetCabinCollectLevelDataMetaDataByKey(level);
		}
	}
}
