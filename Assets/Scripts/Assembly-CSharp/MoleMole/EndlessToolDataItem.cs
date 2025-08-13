using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class EndlessToolDataItem : StorageDataItemBase
	{
		private EndlessToolMetaData _metaData;

		public EndlessItemType ToolType
		{
			get
			{
				return (EndlessItemType)_metaData.type;
			}
		}

		public bool ApplyToSelf
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Invalid comparison between Unknown and I4
				return (int)ToolType == 1 || (int)ToolType == 5;
			}
		}

		public string ParamString
		{
			get
			{
				if (string.IsNullOrEmpty(_metaData.paramStr))
				{
					return null;
				}
				return _metaData.paramStr;
			}
		}

		public string ReportTextMapId
		{
			get
			{
				return _metaData.report;
			}
		}

		public bool ShowIcon
		{
			get
			{
				return _metaData.showIcon != 0;
			}
		}

		public string EffectPrefatPath
		{
			get
			{
				return _metaData.effectPath;
			}
		}

		public EndlessToolDataItem(int metaID, int number = 1)
		{
			uid = 0;
			_metaData = EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaID);
			ID = metaID;
			rarity = _metaData.rarity;
			level = 1;
			exp = 0;
			base.number = number;
		}

		public override StorageDataItemBase Clone()
		{
			EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem(ID);
			endlessToolDataItem.level = level;
			endlessToolDataItem.exp = exp;
			endlessToolDataItem.number = number;
			return endlessToolDataItem;
		}

		public override int GetIdForKey()
		{
			return ID;
		}

		public override float GetPriceForSell()
		{
			return 0f;
		}

		public override string GetImagePath()
		{
			throw new NotImplementedException();
		}

		public override string GetIconPath()
		{
			return _metaData.iconPath;
		}

		public string GetSmallIconPath()
		{
			return _metaData.smallIconPath;
		}

		public override int GetCost()
		{
			return 0;
		}

		public override float GetHPAdd()
		{
			throw new NotImplementedException();
		}

		public override float GetSPAdd()
		{
			throw new NotImplementedException();
		}

		public override float GetAttackAdd()
		{
			throw new NotImplementedException();
		}

		public override float GetCriticalAdd()
		{
			throw new NotImplementedException();
		}

		public override float GetDefenceAdd()
		{
			throw new NotImplementedException();
		}

		public override int GetMaxLevel()
		{
			return level;
		}

		public override float GetGearExp()
		{
			return 0f;
		}

		public override int GetMaxRarity()
		{
			return _metaData.rarity;
		}

		public override int GetMaxExp()
		{
			throw new NotImplementedException();
		}

		public override int GetExpType()
		{
			throw new NotImplementedException();
		}

		public override string GetDisplayTitle()
		{
			return LocalizationGeneralLogic.GetText(_metaData.name);
		}

		public override string GetDescription()
		{
			return LocalizationGeneralLogic.GetText(_metaData.description);
		}

		public string GetBGDescription()
		{
			throw new NotImplementedException();
		}

		public override List<KeyValuePair<int, int>> GetEvoMaterial()
		{
			throw new NotImplementedException();
		}

		public override StorageDataItemBase GetEvoStorageItem()
		{
			return null;
		}

		public override int GetCoinNeedToUpLevel()
		{
			throw new NotImplementedException();
		}

		public override int GetCoinNeedToUpRarity()
		{
			throw new NotImplementedException();
		}

		public override int GetSubRarity()
		{
			return 0;
		}

		public override int GetMaxSubRarity()
		{
			throw new NotImplementedException();
		}

		public override void UpLevel()
		{
		}

		public override void UpRarity()
		{
		}

		public float GetAvatarExpProvideNum()
		{
			return 0f;
		}

		public override int GetBaseType()
		{
			throw new NotImplementedException();
		}

		public override string GetBaseTypeName()
		{
			throw new NotImplementedException();
		}

		public int GetTimeSpanInSeconds()
		{
			return _metaData.paramTime;
		}
	}
}
