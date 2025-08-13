using System.Collections.Generic;

namespace MoleMole
{
	public class WeaponDataItem : StorageDataItemBase
	{
		public const int UID_BEGIN_NUM = 0;

		public const int MAX_UID_NUM = 20000;

		public float durability;

		private WeaponMetaData _metaData;

		public List<EquipSkillDataItem> skills { get; private set; }

		public WeaponDataItem(int uid, WeaponMetaData weaponMetaData)
		{
			base.uid = uid;
			_metaData = weaponMetaData;
			ID = _metaData.ID;
			rarity = _metaData.rarity;
			level = 1;
			durability = _metaData.durabilityMax;
			exp = 0;
			number = 1;
			if (_metaData != null)
			{
				skills = GetSkills();
			}
		}

		public override StorageDataItemBase Clone()
		{
			WeaponDataItem weaponDataItem = new WeaponDataItem(uid, _metaData);
			weaponDataItem.level = level;
			weaponDataItem.exp = exp;
			weaponDataItem.number = number;
			return weaponDataItem;
		}

		public override int GetIdForKey()
		{
			return uid;
		}

		public override int GetBaseType()
		{
			return _metaData.baseType;
		}

		public override string GetBaseTypeName()
		{
			return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.WeaponBaseTypeName[_metaData.baseType]);
		}

		public override float GetPriceForSell()
		{
			return _metaData.sellPriceBase + _metaData.sellPriceAdd * (float)(level - 1);
		}

		public override string GetImagePath()
		{
			return _metaData.imagePath;
		}

		public override string GetIconPath()
		{
			return _metaData.iconPath;
		}

		public override int GetCost()
		{
			return _metaData.cost;
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

		public override float GetGearExp()
		{
			return _metaData.gearExpProvideBase + _metaData.gearExpPorvideAdd * (float)(level - 1);
		}

		public override int GetMaxLevel()
		{
			return _metaData.maxLv;
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
			WeaponDataItem weaponDataItem = new WeaponDataItem(0, WeaponMetaDataReader.GetWeaponMetaDataByKey(_metaData.evoID));
			weaponDataItem.level = weaponDataItem.GetMaxLevel();
			return weaponDataItem;
		}

		public override int GetCoinNeedToUpLevel()
		{
			return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(level).weaponUpgradeCost;
		}

		public override int GetCoinNeedToUpRarity()
		{
			return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(level).weaponEvoCost;
		}

		public override int GetSubRarity()
		{
			return _metaData.subRarity;
		}

		public override int GetMaxSubRarity()
		{
			return _metaData.subMaxRarity;
		}

		public override void UpLevel()
		{
		}

		public override void UpRarity()
		{
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

		public string GetPrefabPath()
		{
			return _metaData.bodyMod;
		}

		public int GetEvoID()
		{
			return _metaData.evoID;
		}

		public float GetPowerUpConf()
		{
			return PowerTypeMetaDataReader.GetPowerTypeMetaDataByKey(_metaData.powerType).powerConf;
		}
	}
}
