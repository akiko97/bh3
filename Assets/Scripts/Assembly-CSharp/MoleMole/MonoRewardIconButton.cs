using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoRewardIconButton : MonoBehaviour
	{
		public enum RewardType
		{
			HCoin = 0,
			SCoin = 1,
			Stamina = 2,
			SkillPoint = 3,
			Item = 4
		}

		private RewardType _rewardType;

		protected int _num;

		private StorageDataItemBase _itemData;

		public void SetupView(RewardType rewardType, int num, StorageDataItemBase itemData)
		{
			_rewardType = rewardType;
			_num = num;
			_itemData = itemData;
			string iconPath = GetIconPath();
			GameObject gameObject = Miscs.LoadResource<GameObject>(iconPath);
			base.transform.Find("Icon").GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			string text = GetName();
			if (num > 1)
			{
				text = text + " x" + num;
			}
			base.transform.Find("Desc").GetComponent<Text>().text = text;
		}

		public void OnButtonClick()
		{
			if (_itemData != null)
			{
				UIUtil.ShowItemDetail(_itemData);
			}
		}

		private string GetIconPath()
		{
			string text = "SpriteOutput/SpecialIcons/";
			switch (_rewardType)
			{
			case RewardType.HCoin:
				return text + "HCoinIcon";
			case RewardType.SCoin:
				return text + "SCoinIcon";
			case RewardType.Stamina:
				return text + "StaminaIcon";
			case RewardType.SkillPoint:
				return text + "SkillPointIcon";
			case RewardType.Item:
				return _itemData.GetIconPath();
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		private string GetName()
		{
			switch (_rewardType)
			{
			case RewardType.HCoin:
				return LocalizationGeneralLogic.GetText("Menu_Hcoin");
			case RewardType.SCoin:
				return LocalizationGeneralLogic.GetText("Menu_Scoin");
			case RewardType.Stamina:
				return LocalizationGeneralLogic.GetText("Menu_Stamina");
			case RewardType.SkillPoint:
				return LocalizationGeneralLogic.GetText("Menu_SkillPtNum");
			case RewardType.Item:
				return _itemData.GetDisplayTitle();
			default:
				throw new Exception("Invalid Type or State!");
			}
		}
	}
}
