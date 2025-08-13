using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class AvatarDataItem
	{
		public static readonly EquipmentSlot[] EQUIP_SLOTS = (EquipmentSlot[])(object)new EquipmentSlot[4]
		{
			(EquipmentSlot)1,
			(EquipmentSlot)2,
			(EquipmentSlot)3,
			(EquipmentSlot)4
		};

		public int avatarID;

		public int star;

		public int level;

		public int exp;

		public int fragment;

		public Dictionary<EquipmentSlot, StorageDataItemBase> equipsMap;

		private AvatarMetaData _metaData;

		private AvatarStarMetaData _starMetaData;

		private AvatarLevelMetaData _levelMetaData;

		private ClassMetaData _classMetaData;

		public bool Initialized;

		public bool UnLocked;

		private int _unlockNeedFragment;

		public List<AvatarSkillDataItem> skillDataList;

		private Dictionary<int, AvatarSkillDataItem> _skillDataMap;

		private int _leaderSkillId;

		public bool CanStarUp
		{
			get
			{
				return fragment >= MaxFragment && MaxFragment > 0;
			}
		}

		public bool CanUnlock
		{
			get
			{
				return !UnLocked && fragment > _unlockNeedFragment;
			}
		}

		public int ClassId
		{
			get
			{
				return _metaData.classID;
			}
		}

		public string ClassFirstName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_classMetaData.firstName);
			}
		}

		public string ClassLastName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_classMetaData.lastName);
			}
		}

		public string ClassEnFirstName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_classMetaData.enFirstName);
			}
		}

		public string ClassEnLastName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_classMetaData.enLastName);
			}
		}

		public string FullName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.fullName);
			}
		}

		public string ShortName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.shortName);
			}
		}

		public string AvatarRegistryKey
		{
			get
			{
				return _metaData.avatarRegistryKey;
			}
		}

		public List<int> WeaponBaseTypeList
		{
			get
			{
				return _metaData.weaponBaseTypeList;
			}
		}

		public int Attribute
		{
			get
			{
				return _metaData.attribute;
			}
		}

		public string AttributeName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarAttributeName[Attribute]);
			}
		}

		public string AttributeIconPath
		{
			get
			{
				return MiscData.Config.PrefabPath.AvatarAttrIcon[Attribute];
			}
		}

		public int InitialWeapon
		{
			get
			{
				return _metaData.initialWeapon;
			}
		}

		public int LevelTutorialID
		{
			get
			{
				return _metaData.levelTutorialID;
			}
		}

		public string Desc
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.desc);
			}
		}

		public string IconPath
		{
			get
			{
				return _starMetaData.iconPath;
			}
		}

		public string IconPathInLevel
		{
			get
			{
				return _starMetaData.iconPathInLevel;
			}
		}

		public string FigurePath
		{
			get
			{
				return _starMetaData.figurePath;
			}
		}

		public string RankName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarRankName[star]);
			}
		}

		public int MaxFragment
		{
			get
			{
				return (!UnLocked) ? _unlockNeedFragment : _starMetaData.upgradeFragment;
			}
		}

		public int MaxCost
		{
			get
			{
				int num = 0;
				Dictionary<int, int> costAddByAvatarStar = Singleton<PlayerModule>.Instance.playerData.costAddByAvatarStar;
				if (costAddByAvatarStar.ContainsKey(star))
				{
					num = costAddByAvatarStar[star];
				}
				return _levelMetaData.cost + num;
			}
		}

		public int MaxExp
		{
			get
			{
				return _levelMetaData.exp;
			}
		}

		public float FinalHP
		{
			get
			{
				return GetBaseHp() + GetEquipHPAdd();
			}
		}

		public float FinalSP
		{
			get
			{
				return GetBaseSP() + GetEquipSPAdd();
			}
		}

		public float FinalAttack
		{
			get
			{
				return GetBaseAttack() + GetEquipAttackAdd();
			}
		}

		public float FinalCritical
		{
			get
			{
				return GetBaseCritical() + GetEquipCriticalAdd();
			}
		}

		public float FinalDefense
		{
			get
			{
				return GetBaseDefense() + GetEquipDefenseAdd();
			}
		}

		public float FinalHPUI
		{
			get
			{
				return GetBaseHp() + GetEquipHPAddWithAffix();
			}
		}

		public float FinalSPUI
		{
			get
			{
				return GetBaseSP() + GetEquipSPAddWithAffix();
			}
		}

		public float FinalAttackUI
		{
			get
			{
				return GetBaseAttack() + GetEquipAttackAddWithAffix();
			}
		}

		public float FinalCriticalUI
		{
			get
			{
				return GetBaseCritical() + GetEquipCriticalAddWithAffix();
			}
		}

		public float FinalDefenseUI
		{
			get
			{
				return GetBaseDefense() + GetEquipDefenseAddWithAffix();
			}
		}

		public float CombatNum
		{
			get
			{
				PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
				float num = 1f + (float)(star - 1) * (float)playerData.avatarCombatBaseStarRate / 10000f;
				float num2 = 1f + (float)(level - 1) * (float)playerData.avatarCombatBaseLevelRate / 10000f;
				float num3 = 1f + (float)(_metaData.unlockStar - 1) * (float)playerData.avatarCombatBaseUnlockStarRate / 10000f;
				float num4 = (float)playerData.avatarCombatBaseWeight * num * num2 * num3;
				int num5 = 0;
				int num6 = 0;
				foreach (AvatarSkillDataItem skillData in skillDataList)
				{
					foreach (AvatarSubSkillDataItem avatarSubSkill in skillData.avatarSubSkillList)
					{
						num6 += avatarSubSkill.MaxLv;
						num5 += avatarSubSkill.level;
					}
				}
				float num7 = 0f;
				if (num6 > 0)
				{
					num7 = (float)num5 / (float)num6 * (float)playerData.avatarCombatSkillWeight;
				}
				float num8 = 0f;
				CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
				if (avatarEnhanceCabinByClass != null && avatarEnhanceCabinByClass.status == CabinStatus.UnLocked)
				{
					num8 = avatarEnhanceCabinByClass.GetTotalEnhancePoint() * playerData.avatarCombatIslandWeight;
				}
				float num9 = 0f;
				WeaponDataItem weaponDataItem = equipsMap[(EquipmentSlot)1] as WeaponDataItem;
				if (weaponDataItem != null)
				{
					num = 1f + (float)(weaponDataItem.rarity - 1) * (float)playerData.avatarCombatWeaponRarityRate / 10000f + (float)weaponDataItem.GetSubRarity() * (float)playerData.avatarCombatWeaponSubRarityRate / 10000f;
					num2 = 1f + (float)(weaponDataItem.level - 1) * (float)playerData.avatarCombatWeaponLevelRate / 10000f;
					num9 = (float)playerData.avatarCombatWeaponWeight * num * num2 * weaponDataItem.GetPowerUpConf();
				}
				float num10 = (float)playerData.avatarCombatStigmataRarityRate / 10000f;
				float num11 = (float)playerData.avatarCombatStigmataSubRarityRate / 10000f;
				float num12 = (float)playerData.avatarCombatStigmataLevelRate / 10000f;
				float num13 = 0f;
				StigmataDataItem stigmataDataItem = equipsMap[(EquipmentSlot)2] as StigmataDataItem;
				if (stigmataDataItem != null)
				{
					num = 1f + (float)(stigmataDataItem.rarity - 1) * num10 + (float)stigmataDataItem.GetSubRarity() * num11;
					num2 = 1f + (float)(stigmataDataItem.level - 1) * num12;
					num13 = (float)playerData.avatarCombatStigmataWeight * num * num2 * stigmataDataItem.GetPowerUpConf();
				}
				float num14 = 0f;
				StigmataDataItem stigmataDataItem2 = equipsMap[(EquipmentSlot)3] as StigmataDataItem;
				if (stigmataDataItem2 != null)
				{
					num = 1f + (float)(stigmataDataItem2.rarity - 1) * num10 + (float)stigmataDataItem2.GetSubRarity() * num11;
					num2 = 1f + (float)(stigmataDataItem2.level - 1) * num12;
					num14 = (float)playerData.avatarCombatStigmataWeight * num * num2 * stigmataDataItem2.GetPowerUpConf();
				}
				float num15 = 0f;
				StigmataDataItem stigmataDataItem3 = equipsMap[(EquipmentSlot)4] as StigmataDataItem;
				if (stigmataDataItem3 != null)
				{
					num = 1f + (float)(stigmataDataItem3.rarity - 1) * num10 + (float)stigmataDataItem3.GetSubRarity() * num11;
					num2 = 1f + (float)(stigmataDataItem3.level - 1) * num12;
					num15 = (float)playerData.avatarCombatStigmataWeight * num * num2 * stigmataDataItem3.GetPowerUpConf();
				}
				EquipSetDataItem ownEquipSetData = GetOwnEquipSetData();
				float num16 = 0f;
				if (ownEquipSetData != null)
				{
					num16 = (num13 + num14 + num15) * (float)(GetOwnEquipSetData().ownNum - 1) * (float)playerData.avatarCombatStigmataSuitNumRate / 10000f;
				}
				return num4 + num7 + num9 + num13 + num14 + num15 + num16 + num8;
			}
		}

		public string AvatarTachie
		{
			get
			{
				return _starMetaData.figurePath;
			}
		}

		public AvatarDataItem(int avatarID, int level = 1, int star = 0)
		{
			AvatarMetaData avatarMetaDataByKey = AvatarMetaDataReader.GetAvatarMetaDataByKey(avatarID);
			ClassMetaData classMetaDataByKey = ClassMetaDataReader.GetClassMetaDataByKey(avatarMetaDataByKey.classID);
			Init(avatarID, avatarMetaDataByKey, classMetaDataByKey, null, null, level, star);
		}

		public AvatarDataItem(int avatarID, AvatarMetaData metaData, ClassMetaData classMetaData, AvatarStarMetaData starMetaData, AvatarLevelMetaData levelMetaData, int level, int star)
		{
			Init(avatarID, metaData, classMetaData, starMetaData, levelMetaData, level, star);
		}

		private float GetBaseHp()
		{
			float num = _starMetaData.hpBase + (float)(level - 1) * _starMetaData.hpAdd;
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
			if (avatarEnhanceCabinByClass != null)
			{
				num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)1);
			}
			return num;
		}

		private float GetBaseSP()
		{
			float num = _starMetaData.spBase + (float)(level - 1) * _starMetaData.spAdd;
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
			if (avatarEnhanceCabinByClass != null)
			{
				num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)2);
			}
			return num;
		}

		private float GetBaseAttack()
		{
			float num = _starMetaData.atkBase + (float)(level - 1) * _starMetaData.atkAdd;
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
			if (avatarEnhanceCabinByClass != null)
			{
				num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)3);
			}
			return num;
		}

		private float GetBaseDefense()
		{
			float num = _starMetaData.dfsBase + (float)(level - 1) * _starMetaData.dfsAdd;
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
			if (avatarEnhanceCabinByClass != null)
			{
				num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)4);
			}
			return num;
		}

		private float GetBaseCritical()
		{
			float num = _starMetaData.crtBase + (float)(level - 1) * _starMetaData.crtAdd;
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(ClassId);
			if (avatarEnhanceCabinByClass != null)
			{
				num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)5);
			}
			return num;
		}

		private void Init(int avatarID, AvatarMetaData metaData, ClassMetaData classMetaData, AvatarStarMetaData starMetaData, AvatarLevelMetaData levelMetaData, int level, int star)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			this.avatarID = avatarID;
			equipsMap = new Dictionary<EquipmentSlot, StorageDataItemBase>();
			EquipmentSlot[] eQUIP_SLOTS = EQUIP_SLOTS;
			foreach (EquipmentSlot key in eQUIP_SLOTS)
			{
				equipsMap.Add(key, null);
			}
			_metaData = metaData;
			_classMetaData = classMetaData;
			_starMetaData = starMetaData;
			_levelMetaData = levelMetaData;
			Initialized = false;
			UnLocked = false;
			SetupDefaultSkillList();
			this.star = ((star != 0) ? star : _metaData.unlockStar);
			OnStarUpdate(this.star, this.star);
			this.level = level;
			OnLevelUpdate(this.level, this.level);
			_unlockNeedFragment = CalculateUnlockNeedFragment();
		}

		private void SetupDefaultSkillList()
		{
			skillDataList = new List<AvatarSkillDataItem>();
			_skillDataMap = new Dictionary<int, AvatarSkillDataItem>();
			foreach (int skill in _metaData.skillList)
			{
				AvatarSkillDataItem avatarSkillDataItem = new AvatarSkillDataItem(skill, avatarID);
				skillDataList.Add(avatarSkillDataItem);
				_skillDataMap.Add(skill, avatarSkillDataItem);
				if (avatarSkillDataItem.IsLeaderSkill)
				{
					_leaderSkillId = avatarSkillDataItem.skillID;
				}
			}
		}

		public AvatarSkillDataItem GetLeaderSkill()
		{
			return _skillDataMap[_leaderSkillId];
		}

		public AvatarSkillDataItem GetUltraSkill()
		{
			return _skillDataMap[_metaData.ultraSkillID];
		}

		public void OnStarUpdate(int preValue, int newValue)
		{
			if (_starMetaData == null || preValue != newValue)
			{
				_starMetaData = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarID, newValue);
				UpdateSkillInfo();
			}
		}

		public void OnLevelUpdate(int preValue, int newValue)
		{
			if (_levelMetaData == null || preValue != newValue)
			{
				_levelMetaData = AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(newValue);
				UpdateSkillInfo();
			}
		}

		public void UpdateSkillInfo()
		{
			foreach (AvatarSkillDataItem skillData in skillDataList)
			{
				skillData.UnLocked = level >= skillData.UnLockLv && star >= skillData.UnLockStar;
			}
		}

		public float GetEquipHPAdd()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetHPAdd();
				}
			}
			return num;
		}

		public float GetEquipSPAdd()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetSPAdd();
				}
			}
			return num;
		}

		public float GetEquipAttackAdd()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetAttackAdd();
				}
			}
			return num;
		}

		public float GetEquipCriticalAdd()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetCriticalAdd();
				}
			}
			return num;
		}

		public float GetEquipDefenseAdd()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetDefenceAdd();
				}
			}
			return num;
		}

		public float GetEquipHPAddWithAffix()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					if (value is WeaponDataItem)
					{
						num += value.GetHPAdd();
					}
					else if (value is StigmataDataItem)
					{
						num += (value as StigmataDataItem).GetHPAddWithAffix(this);
					}
				}
			}
			return num;
		}

		public float GetEquipSPAddWithAffix()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					if (value is WeaponDataItem)
					{
						num += value.GetSPAdd();
					}
					else if (value is StigmataDataItem)
					{
						num += (value as StigmataDataItem).GetSPAddWithAffix(this);
					}
				}
			}
			return num;
		}

		public float GetEquipAttackAddWithAffix()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value is WeaponDataItem)
				{
					num += value.GetAttackAdd();
				}
				else if (value is StigmataDataItem)
				{
					num += (value as StigmataDataItem).GetAttackAddWithAffix(this);
				}
			}
			return num;
		}

		public float GetEquipCriticalAddWithAffix()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value is WeaponDataItem)
				{
					num += value.GetCriticalAdd();
				}
				else if (value is StigmataDataItem)
				{
					num += (value as StigmataDataItem).GetCriticalAddWithAffix(this);
				}
			}
			return num;
		}

		public float GetEquipDefenseAddWithAffix()
		{
			float num = 0f;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					if (value is WeaponDataItem)
					{
						num += value.GetDefenceAdd();
					}
					else if (value is StigmataDataItem)
					{
						num += (value as StigmataDataItem).GetDefenceAddWithAffix(this);
					}
				}
			}
			return num;
		}

		public float GetAvatarCombatUsingNewEquip(StorageDataItemBase equipData)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			EquipmentSlot slot;
			if (UIUtil.GetEquipmentSlot(equipData, out slot))
			{
				AvatarDataItem avatarDataItem = Clone();
				foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> item in equipsMap)
				{
					if (item.Key == slot)
					{
						avatarDataItem.equipsMap[item.Key] = equipData;
					}
				}
				return avatarDataItem.CombatNum;
			}
			return CombatNum;
		}

		public int GetCurrentCost()
		{
			int num = 0;
			foreach (StorageDataItemBase value in equipsMap.Values)
			{
				if (value != null)
				{
					num += value.GetCost();
				}
			}
			return num;
		}

		private int CalculateUnlockNeedFragment()
		{
			int num = 0;
			for (int i = 0; i < _metaData.unlockStar; i++)
			{
				num += AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarID, i).upgradeFragment;
			}
			return num;
		}

		public AvatarSkillDataItem GetAvatarSkillBySkillID(int skillID)
		{
			return _skillDataMap[skillID];
		}

		public AvatarDataItem Clone()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			AvatarDataItem avatarDataItem = new AvatarDataItem(avatarID, level, star);
			avatarDataItem.UnLocked = UnLocked;
			avatarDataItem.Initialized = Initialized;
			avatarDataItem.exp = exp;
			avatarDataItem.fragment = fragment;
			EquipmentSlot[] eQUIP_SLOTS = EQUIP_SLOTS;
			foreach (EquipmentSlot key in eQUIP_SLOTS)
			{
				StorageDataItemBase value = ((equipsMap[key] != null) ? equipsMap[key].Clone() : null);
				avatarDataItem.equipsMap[key] = value;
			}
			foreach (AvatarSkillDataItem skillData in skillDataList)
			{
				AvatarSkillDataItem avatarSkillBySkillID = avatarDataItem.GetAvatarSkillBySkillID(skillData.skillID);
				avatarSkillBySkillID.UnLocked = skillData.UnLocked;
				foreach (AvatarSubSkillDataItem avatarSubSkill in skillData.avatarSubSkillList)
				{
					avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId(avatarSubSkill.subSkillID).level = avatarSubSkill.level;
				}
			}
			return avatarDataItem;
		}

		public WeaponDataItem GetWeapon()
		{
			return equipsMap[(EquipmentSlot)1] as WeaponDataItem;
		}

		public List<StigmataDataItem> GetStigmataList()
		{
			List<StigmataDataItem> list = new List<StigmataDataItem>();
			list.Add(equipsMap[(EquipmentSlot)2] as StigmataDataItem);
			list.Add(equipsMap[(EquipmentSlot)3] as StigmataDataItem);
			list.Add(equipsMap[(EquipmentSlot)4] as StigmataDataItem);
			return list;
		}

		public Dictionary<EquipmentSlot, StigmataDataItem> GetStigmataDict()
		{
			Dictionary<EquipmentSlot, StigmataDataItem> dictionary = new Dictionary<EquipmentSlot, StigmataDataItem>();
			dictionary[(EquipmentSlot)2] = equipsMap[(EquipmentSlot)2] as StigmataDataItem;
			dictionary[(EquipmentSlot)3] = equipsMap[(EquipmentSlot)3] as StigmataDataItem;
			dictionary[(EquipmentSlot)4] = equipsMap[(EquipmentSlot)4] as StigmataDataItem;
			return dictionary;
		}

		public StigmataDataItem GetStigmata(EquipmentSlot slot)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			StorageDataItemBase storageDataItemBase = equipsMap[slot];
			return (storageDataItemBase != null) ? (storageDataItemBase as StigmataDataItem) : null;
		}

		public EquipmentSlot SearchEquipSlot(StorageDataItemBase item)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> item2 in equipsMap)
			{
				if (item2.Value != null && item2.Value.uid == item.uid)
				{
					return item2.Key;
				}
			}
			return (EquipmentSlot)0;
		}

		public AvatarFragmentDataItem GetAvatarFragmentDataItem()
		{
			int avatarFragmentID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarID).avatarFragmentID;
			AvatarFragmentMetaData avatarFragmentMetaDataByKey = AvatarFragmentMetaDataReader.GetAvatarFragmentMetaDataByKey(avatarFragmentID);
			AvatarFragmentDataItem avatarFragmentDataItem = new AvatarFragmentDataItem(avatarFragmentMetaDataByKey);
			avatarFragmentDataItem.number = fragment;
			return avatarFragmentDataItem;
		}

		public EquipSetDataItem GetOwnEquipSetData()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (StigmataDataItem stigmata in GetStigmataList())
			{
				if (stigmata != null && stigmata.GetEquipmentSetID() != 0)
				{
					if (dictionary.ContainsKey(stigmata.GetEquipmentSetID()))
					{
						Dictionary<int, int> dictionary3;
						Dictionary<int, int> dictionary2 = (dictionary3 = dictionary);
						int equipmentSetID;
						int key = (equipmentSetID = stigmata.GetEquipmentSetID());
						equipmentSetID = dictionary3[equipmentSetID];
						dictionary2[key] = equipmentSetID + 1;
					}
					else
					{
						dictionary[stigmata.GetEquipmentSetID()] = 1;
					}
				}
			}
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				EquipSetDataItem equipSetDataItem = new EquipSetDataItem(item.Key, item.Value);
				Dictionary<int, EquipSkillDataItem> ownSetSkills = equipSetDataItem.GetOwnSetSkills();
				if (ownSetSkills.Count > 0)
				{
					return equipSetDataItem;
				}
			}
			return null;
		}

		public int GetSkillPointAddNum()
		{
			int num = 0;
			foreach (AvatarSkillDataItem skillData in skillDataList)
			{
				num += skillData.GetLevelSum();
			}
			return num;
		}

		public int GetCost(int theStar)
		{
			int num = 0;
			Dictionary<int, int> costAddByAvatarStar = Singleton<PlayerModule>.Instance.playerData.costAddByAvatarStar;
			if (costAddByAvatarStar.ContainsKey(theStar))
			{
				num = costAddByAvatarStar[theStar];
			}
			return _levelMetaData.cost + num;
		}

		public bool IsEasterner()
		{
			foreach (int easternerClassID in MiscData.Config.EasternerClassIDList)
			{
				if (easternerClassID == ClassId)
				{
					return true;
				}
			}
			return false;
		}

		public Sprite GetBGSprite()
		{
			switch ((EntityNature)Attribute)
			{
			case EntityNature.Mechanic:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrJiXie");
			case EntityNature.Biology:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrShengWu");
			case EntityNature.Psycho:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrYiNeng");
			default:
				return null;
			}
		}
	}
}
