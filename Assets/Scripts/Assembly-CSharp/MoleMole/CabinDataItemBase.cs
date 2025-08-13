using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public abstract class CabinDataItemBase
	{
		public CabinType cabinType;

		public int level;

		public int extendGrade;

		public DateTime levelUpEndTime;

		public CabinTechTree _techTree;

		public CabinStatus status
		{
			get
			{
				if (level > 0)
				{
					return CabinStatus.UnLocked;
				}
				return CabinStatus.Locked;
			}
		}

		public string GetCabinName()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, 1);
			return LocalizationGeneralLogic.GetText(cabinLevelMetaData.cabinName);
		}

		public int GetCabinMaxLevel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected I4, but got Unknown
			CabinExtendGradeMetaData cabinExtendGradeMetaData = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey((int)cabinType, extendGrade);
			return cabinExtendGradeMetaData.cabinLevelMax;
		}

		public int GetCabinMaxLevelNextExntendGrade()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinExtendGradeMetaData cabinExtendGradeMetaData = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey((int)cabinType, extendGrade + 1);
			return cabinExtendGradeMetaData.cabinLevelMax;
		}

		public int GetCabinExntendScoinCost()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinExtendGradeMetaData cabinExtendGradeMetaData = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey((int)cabinType, extendGrade + 1);
			return cabinExtendGradeMetaData.scoinNeed;
		}

		public int GetCabinLevelUpTimeCost()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, level + 1);
			return cabinLevelMetaData.upLevelTimeNeed;
		}

		public int GetCabinLevelUpScoinCost()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, level + 1);
			return cabinLevelMetaData.scoinNeed;
		}

		public List<StorageDataItemBase> GetLevelUpItemNeed()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, level + 1);
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			foreach (CabinLevelMetaData.CabinUpLevelNeedItem item in cabinLevelMetaData.itemListNeed)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item.itemMetaID);
				if (dummyStorageDataItem != null)
				{
					dummyStorageDataItem.number = item.itemNum;
					list.Add(dummyStorageDataItem);
				}
			}
			return list;
		}

		public List<StorageDataItemBase> GetExtendItemNeed()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinExtendGradeMetaData cabinExtendGradeMetaData = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey((int)cabinType, extendGrade + 1);
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			foreach (CabinExtendGradeMetaData.CabinExtendNeedItem item in cabinExtendGradeMetaData.itemListNeed)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item.itemMetaID);
				dummyStorageDataItem.number = item.itemNum;
				list.Add(dummyStorageDataItem);
			}
			return list;
		}

		public bool CanExtendCabin()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinExtendGradeMetaData cabinExtendGradeMetaData = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey((int)cabinType, extendGrade + 1);
			if (cabinExtendGradeMetaData == null || status == CabinStatus.Locked)
			{
				return false;
			}
			return true;
		}

		public bool CanUpLevel()
		{
			return status == CabinStatus.UnLocked && level < GetCabinMaxLevel();
		}

		public int GetPlayerLevelNeedToUpLevel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, level + 1);
			return cabinLevelMetaData.playerLevelNeed;
		}

		public bool HasTechTree()
		{
			return _techTree != null;
		}

		public int GetUnlockPlayerLevel()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected I4, but got Unknown
			CabinLevelMetaData cabinLevelMetaData = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey((int)cabinType, 1);
			return cabinLevelMetaData.playerLevelNeed;
		}

		public int GetUsedPower()
		{
			if (_techTree == null)
			{
				return 0;
			}
			return _techTree.GetPowerUsed();
		}

		public bool IsUpLevel()
		{
			return levelUpEndTime > TimeUtil.Now;
		}

		public bool NeedToShowLevelUpComplete()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[cabinType] && !IsUpLevel();
		}

		public int GetResetScoin()
		{
			if (_techTree == null)
			{
				return 0;
			}
			return _techTree.GetResetScoin();
		}

		public virtual void SetupMateData()
		{
		}
	}
}
