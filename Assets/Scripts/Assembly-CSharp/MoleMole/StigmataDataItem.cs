using System.Collections.Generic;

namespace MoleMole
{
	public class StigmataDataItem : StorageDataItemBase
	{
		public class AffixSkillData
		{
			private StigmataAffixMetaData _metaData;

			public readonly EquipSkillDataItem skill;

			public readonly int affixID;

			public string NameMono
			{
				get
				{
					return LocalizationGeneralLogic.GetText(_metaData.nameMono);
				}
			}

			public string NameDual
			{
				get
				{
					return LocalizationGeneralLogic.GetText(_metaData.nameDual);
				}
			}

			public AffixSkillData(int affixID)
			{
				this.affixID = affixID;
				_metaData = StigmataAffixMetaDataReader.GetStigmataAffixMetaDataByKey(affixID);
				skill = new EquipSkillDataItem(_metaData.propID, _metaData.PropParam1, _metaData.PropParam2, _metaData.PropParam3, 0f, 0f, 0f);
			}

			public float GetAttrAdd(AvatarDataItem avatarData, int attrType)
			{
				if (_metaData.UINature != 0 && (avatarData == null || _metaData.UINature != avatarData.Attribute))
				{
					return 0f;
				}
				if (_metaData.UIClass != 0 && (avatarData == null || _metaData.UIClass != avatarData.ClassId))
				{
					return 0f;
				}
				if (_metaData.UIType != attrType)
				{
					return 0f;
				}
				return _metaData.UIValue;
			}
		}

		public const int UID_BEGIN_NUM = 20000;

		public const int MAX_UID_NUM = 40000;

		public float durability;

		private StigmataMetaData _metaData;

		private EquipSetDataItem _equipSetData;

		private List<AffixSkillData> _affixSkillList;

		public List<EquipSkillDataItem> skills { get; private set; }

		public AffixSkillData PreAffixSkill { get; private set; }

		public AffixSkillData SufAffixSkill { get; private set; }

		public bool IsAffixIdentify { get; private set; }

		public bool CanRefine
		{
			get
			{
				return _metaData.canRefine == 1;
			}
		}

		public StigmataDataItem(int uid, StigmataMetaData stigmataMetaData)
		{
			base.uid = uid;
			_metaData = stigmataMetaData;
			ID = _metaData.ID;
			rarity = _metaData.rarity;
			level = 1;
			durability = _metaData.durabilityMax;
			exp = 0;
			number = 1;
			if (_metaData != null)
			{
				if (_metaData.setID != 0)
				{
					_equipSetData = new EquipSetDataItem(_metaData.setID);
				}
				else
				{
					_equipSetData = null;
				}
				skills = GetSkills();
			}
		}

		public override StorageDataItemBase Clone()
		{
			StigmataDataItem stigmataDataItem = new StigmataDataItem(uid, _metaData);
			stigmataDataItem.level = level;
			stigmataDataItem.exp = exp;
			stigmataDataItem.number = number;
			int pre_affix_id = ((PreAffixSkill != null) ? PreAffixSkill.affixID : 0);
			int suf_affix_id = ((SufAffixSkill != null) ? SufAffixSkill.affixID : 0);
			stigmataDataItem.SetAffixSkill(IsAffixIdentify, pre_affix_id, suf_affix_id);
			return stigmataDataItem;
		}

		public override int GetIdForKey()
		{
			return uid;
		}

		public override float GetPriceForSell()
		{
			return _metaData.sellPriceBase + _metaData.sellPriceAdd * (float)(level - 1);
		}

		public override float GetHPAdd()
		{
			return _metaData.HPBase + _metaData.HPAdd * (float)(level - 1);
		}

		public override float GetSPAdd()
		{
			return _metaData.SPBase + _metaData.SPAdd * (float)(level - 1);
		}

		public override float GetAttackAdd()
		{
			return _metaData.attackBase + _metaData.attackAdd * (float)(level - 1);
		}

		public override float GetCriticalAdd()
		{
			return _metaData.criticalBase + _metaData.criticalAdd * (float)(level - 1);
		}

		public override float GetDefenceAdd()
		{
			return _metaData.defenceBase + _metaData.defenceAdd * (float)(level - 1);
		}

		public float GetHPAddWithAffix(AvatarDataItem avatarData)
		{
			float num = _metaData.HPBase + _metaData.HPAdd * (float)(level - 1);
			float num2 = 0f;
			if (_affixSkillList != null)
			{
				foreach (AffixSkillData affixSkill in _affixSkillList)
				{
					num2 += affixSkill.GetAttrAdd(avatarData, 1);
				}
			}
			return num + num2;
		}

		public float GetSPAddWithAffix(AvatarDataItem avatarData)
		{
			float num = _metaData.SPBase + _metaData.SPAdd * (float)(level - 1);
			float num2 = 0f;
			if (_affixSkillList != null)
			{
				foreach (AffixSkillData affixSkill in _affixSkillList)
				{
					num2 += affixSkill.GetAttrAdd(avatarData, 2);
				}
			}
			return num + num2;
		}

		public float GetAttackAddWithAffix(AvatarDataItem avatarData)
		{
			float num = _metaData.attackBase + _metaData.attackAdd * (float)(level - 1);
			float num2 = 0f;
			if (_affixSkillList != null)
			{
				foreach (AffixSkillData affixSkill in _affixSkillList)
				{
					num2 += affixSkill.GetAttrAdd(avatarData, 3);
				}
			}
			return num + num2;
		}

		public float GetCriticalAddWithAffix(AvatarDataItem avatarData)
		{
			float num = _metaData.criticalBase + _metaData.criticalAdd * (float)(level - 1);
			float num2 = 0f;
			if (_affixSkillList != null)
			{
				foreach (AffixSkillData affixSkill in _affixSkillList)
				{
					num2 += affixSkill.GetAttrAdd(avatarData, 5);
				}
			}
			return num + num2;
		}

		public float GetDefenceAddWithAffix(AvatarDataItem avatarData)
		{
			float num = _metaData.defenceBase + _metaData.defenceAdd * (float)(level - 1);
			float num2 = 0f;
			if (_affixSkillList != null)
			{
				foreach (AffixSkillData affixSkill in _affixSkillList)
				{
					num2 += affixSkill.GetAttrAdd(avatarData, 4);
				}
			}
			return num + num2;
		}

		public override string GetImagePath()
		{
			return _metaData.imagePath;
		}

		public override string GetIconPath()
		{
			return _metaData.iconPath;
		}

		public string GetSmallIconPath()
		{
			return _metaData.smallIcon;
		}

		public override int GetCost()
		{
			return _metaData.cost;
		}

		public override int GetMaxLevel()
		{
			return _metaData.maxLv;
		}

		public override float GetGearExp()
		{
			return _metaData.gearExpProvideBase + _metaData.gearExpPorvideAdd * (float)(level - 1);
		}

		public override int GetMaxRarity()
		{
			return _metaData.maxRarity;
		}

		public override int GetMaxExp()
		{
			return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(level).expList[_metaData.expType];
		}

		public override int GetExpType()
		{
			return _metaData.expType;
		}

		public override string GetDisplayTitle()
		{
			return LocalizationGeneralLogic.GetText(_metaData.displayTitle);
		}

		public override string GetDescription()
		{
			return LocalizationGeneralLogic.GetText(_metaData.displayDescription);
		}

		public override List<KeyValuePair<int, int>> GetEvoMaterial()
		{
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			foreach (string item in _metaData.evoMaterial)
			{
				char[] separator = new char[1] { ':' };
				string[] array = item.Split(separator);
				list.Add(new KeyValuePair<int, int>(int.Parse(array[0]), int.Parse(array[1])));
			}
			return list;
		}

		public override StorageDataItemBase GetEvoStorageItem()
		{
			if (_metaData.evoID == 0)
			{
				return null;
			}
			StigmataDataItem stigmataDataItem = new StigmataDataItem(0, StigmataMetaDataReader.GetStigmataMetaDataByKey(_metaData.evoID));
			stigmataDataItem.level = stigmataDataItem.GetMaxLevel();
			return stigmataDataItem;
		}

		public override int GetCoinNeedToUpLevel()
		{
			return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(level).stigmataUpgradeCost;
		}

		public override int GetCoinNeedToUpRarity()
		{
			return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(level).stigmataEvoCost;
		}

		public override int GetSubRarity()
		{
			return _metaData.subRarity;
		}

		public override int GetMaxSubRarity()
		{
			return _metaData.subMaxRarity;
		}

		public override int GetBaseType()
		{
			return _metaData.baseType;
		}

		public override string GetBaseTypeName()
		{
			return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.StigmataBaseTypeName[_metaData.baseType]);
		}

		public override void UpLevel()
		{
		}

		public override void UpRarity()
		{
		}

		public int GetEquipmentSetID()
		{
			return _metaData.setID;
		}

		public string GetEquipSetName()
		{
			if (_equipSetData != null)
			{
				return _equipSetData.EquipSetName;
			}
			return string.Empty;
		}

		private List<EquipSkillDataItem> GetSkills()
		{
			List<EquipSkillDataItem> list = new List<EquipSkillDataItem>();
			if (_metaData == null)
			{
				return list;
			}
			if (_metaData.prop1ID > 0)
			{
				list.Add(new EquipSkillDataItem(_metaData.prop1ID, _metaData.prop1Param1, _metaData.prop1Param2, _metaData.prop1Param3, _metaData.prop1Param1Add, _metaData.prop1Param2Add, _metaData.prop1Param3Add));
			}
			if (_metaData.prop2ID > 0)
			{
				list.Add(new EquipSkillDataItem(_metaData.prop2ID, _metaData.prop2Param1, _metaData.prop2Param2, _metaData.prop2Param3, _metaData.prop2Param1Add, _metaData.prop2Param2Add, _metaData.prop2Param3Add));
			}
			if (_metaData.prop3ID > 0)
			{
				list.Add(new EquipSkillDataItem(_metaData.prop3ID, _metaData.prop3Param1, _metaData.prop3Param2, _metaData.prop3Param3, _metaData.prop3Param1Add, _metaData.prop3Param2Add, _metaData.prop3Param3Add));
			}
			return list;
		}

		public SortedDictionary<int, EquipSkillDataItem> GetAllSetSkills()
		{
			if (_equipSetData != null)
			{
				return _equipSetData.EquipSkillDict;
			}
			return new SortedDictionary<int, EquipSkillDataItem>();
		}

		public string GetTattooPath()
		{
			return _metaData.tattooPath;
		}

		public float GetOffesetX()
		{
			return _metaData.offsetX;
		}

		public float GetOffesetY()
		{
			return _metaData.offsetY;
		}

		public float GetScale()
		{
			return _metaData.scale;
		}

		public void SetAffixSkill(bool is_affix_identify, int pre_affix_id, int suf_affix_id)
		{
			IsAffixIdentify = is_affix_identify;
			PreAffixSkill = ((pre_affix_id != 0) ? new AffixSkillData(pre_affix_id) : null);
			SufAffixSkill = ((suf_affix_id != 0) ? new AffixSkillData(suf_affix_id) : null);
			_affixSkillList = new List<AffixSkillData>();
			if (PreAffixSkill != null)
			{
				_affixSkillList.Add(PreAffixSkill);
			}
			if (SufAffixSkill != null)
			{
				_affixSkillList.Add(SufAffixSkill);
			}
		}

		public void SetDummyAffixSkill()
		{
			SetAffixSkill(true, 0, 0);
		}

		public List<AffixSkillData> GetAffixSkillList()
		{
			if (_affixSkillList == null)
			{
				return new List<AffixSkillData>();
			}
			return _affixSkillList;
		}

		public string GetAffixName()
		{
			string result = string.Empty;
			if (IsAffixIdentify)
			{
				if (PreAffixSkill != null && SufAffixSkill != null)
				{
					result = PreAffixSkill.NameDual + SufAffixSkill.NameDual;
				}
				else if (PreAffixSkill != null && SufAffixSkill == null)
				{
					result = PreAffixSkill.NameMono;
				}
				else if (PreAffixSkill == null && SufAffixSkill != null)
				{
					result = SufAffixSkill.NameMono;
				}
			}
			else
			{
				result = LocalizationGeneralLogic.GetText("Menu_NotIdentifyAffix");
			}
			return result;
		}

		public List<EquipSkillDataItem> GetSkillsWithAffix()
		{
			List<EquipSkillDataItem> list = new List<EquipSkillDataItem>();
			list.AddRange(skills);
			if (PreAffixSkill != null)
			{
				list.Add(PreAffixSkill.skill);
			}
			if (SufAffixSkill != null)
			{
				list.Add(SufAffixSkill.skill);
			}
			return list;
		}

		public float GetPowerUpConf()
		{
			return PowerTypeMetaDataReader.GetPowerTypeMetaDataByKey(_metaData.powerType).powerConf;
		}
	}
}
