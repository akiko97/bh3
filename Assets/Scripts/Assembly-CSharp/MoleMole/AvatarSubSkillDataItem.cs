using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class AvatarSubSkillDataItem
	{
		public int level;

		private int _avatarID;

		public int subSkillID;

		private SubSkillStatus _status;

		public AvatarSubSkillMetaData _metaData;

		private Dictionary<int, NeedItemData> _lvUpNeedItemDict;

		private Dictionary<int, NeedItemData> _unlockNeedItemDict;

		private AvatarDataItem _avatar
		{
			get
			{
				return Singleton<AvatarModule>.Instance.GetAvatarByID(_avatarID);
			}
		}

		public SubSkillStatus Status
		{
			get
			{
				if (!UnLocked)
				{
					if (_avatar != null && _avatar.level >= UnlockLv && _avatar.star >= UnlockStar)
					{
						_status = SubSkillStatus.CanUnlock;
					}
					else
					{
						_status = SubSkillStatus.Locked;
					}
				}
				else if (level < MaxLv && _avatar != null && _avatar.level >= LvUpNeedAvatarLevel && _avatar.star >= GetUpLevelStarNeed())
				{
					_status = SubSkillStatus.CanUpLevel;
				}
				else
				{
					_status = SubSkillStatus.CannotUpLevel;
				}
				return _status;
			}
		}

		public bool UnLocked
		{
			get
			{
				return level > 0;
			}
		}

		public int UnlockLv
		{
			get
			{
				return Mathf.FloorToInt(_metaData.unlockLv + _metaData.unlockLvAdd * level);
			}
		}

		public int UnlockStar
		{
			get
			{
				return _metaData.unlockStar;
			}
		}

		public int UnlockPoint
		{
			get
			{
				return _metaData.unlockPoint;
			}
		}

		public int UnlockSCoin
		{
			get
			{
				return _metaData.unlockScoin;
			}
		}

		public int LvUpPoint
		{
			get
			{
				return _metaData.preLvPoint;
			}
		}

		public int MaxLv
		{
			get
			{
				return _metaData.maxLv;
			}
		}

		public string Name
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.name);
			}
		}

		public string Info
		{
			get
			{
				return LocalizationGeneralLogic.GetTextWithParamArray(_metaData.info, MiscData.GetColor("Blue"), SkillParamArray);
			}
		}

		public string NextLevelInfo
		{
			get
			{
				return LocalizationGeneralLogic.GetTextWithParamArray(_metaData.info, MiscData.GetColor("Blue"), NextLevelSkillParamArray);
			}
		}

		public bool CanTry
		{
			get
			{
				return _metaData.canTry == 1;
			}
		}

		public string IconPath
		{
			get
			{
				return _metaData.iconPath;
			}
		}

		public float SkillParam_1
		{
			get
			{
				return (level != 0) ? (_metaData.paramBase_1 + _metaData.paramAdd_1 * (float)(level - 1)) : _metaData.paramBase_1;
			}
		}

		public float SkillParam_2
		{
			get
			{
				return (level != 0) ? (_metaData.paramBase_2 + _metaData.paramAdd_2 * (float)(level - 1)) : _metaData.paramBase_2;
			}
		}

		public float SkillParam_3
		{
			get
			{
				return (level != 0) ? (_metaData.paramBase_3 + _metaData.paramAdd_3 * (float)(level - 1)) : _metaData.paramBase_3;
			}
		}

		public int ShowOrder
		{
			get
			{
				return _metaData.showOrder;
			}
		}

		public float[] SkillParamArray
		{
			get
			{
				return new float[3] { SkillParam_1, SkillParam_2, SkillParam_3 };
			}
		}

		public float[] NextLevelSkillParamArray
		{
			get
			{
				return new float[3]
				{
					_metaData.paramBase_1 + _metaData.paramAdd_1 * (float)level,
					_metaData.paramBase_2 + _metaData.paramAdd_2 * (float)level,
					_metaData.paramBase_3 + _metaData.paramAdd_3 * (float)level
				};
			}
		}

		public int LvUpSCoin
		{
			get
			{
				if (level == 0)
				{
					return 0;
				}
				return AvatarSubSkillLevelMetaDataReader.GetAvatarSubSkillLevelMetaDataByKey(LvUpNeedAvatarLevel).needScoinList[_metaData.lvUpScoinType];
			}
		}

		public int LvUpNeedAvatarLevel
		{
			get
			{
				return Mathf.FloorToInt(_metaData.unlockLv + level * _metaData.unlockLvAdd);
			}
		}

		public List<NeedItemData> LvUpNeedItemList
		{
			get
			{
				List<NeedItemData> list = new List<NeedItemData>();
				_lvUpNeedItemDict = new Dictionary<int, NeedItemData>();
				if (level == 0)
				{
					return list;
				}
				List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list2 = AvatarSubSkillLevelMetaDataReader.GetAvatarSubSkillLevelMetaDataByKey(LvUpNeedAvatarLevel).needItemList[_metaData.lvUpItemType];
				if (list2 != null)
				{
					foreach (AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem item in list2)
					{
						if (item.itemMetaID > 0)
						{
							NeedItemData needItemData = new NeedItemData(item.itemMetaID, item.itemNum);
							list.Add(needItemData);
							_lvUpNeedItemDict.Add(item.itemMetaID, needItemData);
						}
					}
				}
				return list;
			}
		}

		public List<NeedItemData> UnlockNeedItemList
		{
			get
			{
				List<NeedItemData> list = new List<NeedItemData>();
				_unlockNeedItemDict = new Dictionary<int, NeedItemData>();
				foreach (AvatarSubSkillMetaData.SkillUpLevelNeedItem unlockItem in _metaData.unlockItemList)
				{
					if (unlockItem.itemMetaID > 0)
					{
						NeedItemData needItemData = new NeedItemData(unlockItem.itemMetaID, unlockItem.itemNum);
						list.Add(needItemData);
						_unlockNeedItemDict.Add(unlockItem.itemMetaID, needItemData);
					}
				}
				return list;
			}
		}

		public AvatarSubSkillDataItem(int avatarSubSkillID, int avatarID)
		{
			subSkillID = avatarSubSkillID;
			_avatarID = avatarID;
			level = 0;
			_metaData = AvatarSubSkillMetaDataReader.GetAvatarSubSkillMetaDataByKey(avatarSubSkillID);
		}

		public bool ShouldShowHintPoint()
		{
			return Status == SubSkillStatus.CanUnlock || Status == SubSkillStatus.CanUpLevel;
		}

		public bool CanUnlock(AvatarDataItem avatar)
		{
			if (!UnLocked)
			{
				PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
				return avatar.level >= UnlockLv && avatar.star >= UnlockStar && playerData.skillPoint >= UnlockPoint && playerData.scoin >= UnlockSCoin && UnlockHasEnoughItems();
			}
			return false;
		}

		public bool CanLvUp(AvatarDataItem avatar)
		{
			if (UnLocked)
			{
				PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
				return avatar.level >= LvUpNeedAvatarLevel && level < MaxLv && playerData.skillPoint >= LvUpPoint && playerData.scoin >= LvUpSCoin && LvUpHasEnoughItems() && avatar.star >= GetUpLevelStarNeed();
			}
			return false;
		}

		public bool LvUpHasEnoughItems()
		{
			bool flag = true;
			foreach (NeedItemData lvUpNeedItem in LvUpNeedItemList)
			{
				flag = flag && Singleton<StorageModule>.Instance.HasEnoughItem(lvUpNeedItem.itemMetaID, lvUpNeedItem.itemNum);
			}
			return flag;
		}

		public bool UnlockHasEnoughItems()
		{
			bool flag = true;
			foreach (NeedItemData unlockNeedItem in UnlockNeedItemList)
			{
				flag = flag && Singleton<StorageModule>.Instance.HasEnoughItem(unlockNeedItem.itemMetaID, unlockNeedItem.itemNum);
			}
			return flag;
		}

		public int GetUpLevelStarNeed()
		{
			int result = 0;
			foreach (AvatarSubSkillMetaData.UpLevelStarNeed upLevelStarNeed in _metaData.upLevelStarNeedList)
			{
				if (upLevelStarNeed.level == level + 1)
				{
					result = upLevelStarNeed.starNeed;
				}
			}
			return result;
		}

		public NeedItemData GetLvUpNeedItemDataByID(int metaID)
		{
			NeedItemData value;
			_lvUpNeedItemDict.TryGetValue(metaID, out value);
			return value;
		}

		public NeedItemData GetUnlockNeedItemDataByID(int metaID)
		{
			NeedItemData value;
			_unlockNeedItemDict.TryGetValue(metaID, out value);
			return value;
		}

		public float GetParaValue(int index)
		{
			switch (index)
			{
			case 1:
				return SkillParam_1;
			case 2:
				return SkillParam_2;
			case 3:
				return SkillParam_3;
			default:
				return 0f;
			}
		}
	}
}
