using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarCardDataItem : StorageDataItemBase
	{
		private AvatarCardMetaData _metaData;

		private bool isSplite;

		private int spliteFragmentNum;

		public AvatarCardDataItem(AvatarCardMetaData avatarCardMetaData)
		{
			uid = 0;
			_metaData = avatarCardMetaData;
			ID = _metaData.ID;
			rarity = _metaData.rarity;
			level = 1;
			exp = 0;
			number = 1;
			isSplite = false;
			spliteFragmentNum = 0;
		}

		public override StorageDataItemBase Clone()
		{
			throw new NotImplementedException();
		}

		public override int GetIdForKey()
		{
			return ID;
		}

		public bool IsSplite()
		{
			return isSplite;
		}

		public int GetSpliteFragmentNum()
		{
			return spliteFragmentNum;
		}

		public void SpliteToFragment(int num)
		{
			isSplite = true;
			spliteFragmentNum = num;
		}

		public override float GetPriceForSell()
		{
			return _metaData.sellPriceBase + _metaData.sellPriceAdd * (float)level;
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
			throw new NotImplementedException();
		}

		public override int GetExpType()
		{
			throw new NotImplementedException();
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
			return _metaData.characterExpProvide;
		}

		public override int GetBaseType()
		{
			throw new NotImplementedException();
		}

		public override string GetBaseTypeName()
		{
			throw new NotImplementedException();
		}
	}
}
