using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSignInRewardItemIconButton : MonoBehaviour
	{
		public delegate void ClickCallBack(RewardData item);

		private ClickCallBack _clickCallBack;

		private RewardData _rewardData;

		private StorageDataItemBase _rewardItemData;

		private int _rewardItemNum;

		private bool _rewardAlreadyGot;

		private bool _isTodayCanGet;

		public void SetupView(RewardData rewardData, bool rewardAlreadyGot, bool todayCanGet)
		{
			_rewardData = rewardData;
			_rewardAlreadyGot = rewardAlreadyGot;
			_isTodayCanGet = todayCanGet;
			base.transform.Find("Star").gameObject.SetActive(false);
			base.transform.Find("StigmataType").gameObject.SetActive(false);
			base.transform.Find("FragmentIcon").gameObject.SetActive(false);
			base.transform.Find("BG/Unselected").gameObject.SetActive(true);
			base.transform.Find("BG/Selected").gameObject.SetActive(false);
			SetupRewardItemIcon();
			if (_isTodayCanGet)
			{
				base.transform.GetComponent<Animator>().SetTrigger("Play");
			}
		}

		public void OnClick()
		{
			if (_clickCallBack != null)
			{
				_clickCallBack(_rewardData);
			}
		}

		public void SetClickCallback(ClickCallBack callback)
		{
			_clickCallBack = callback;
		}

		public ResourceType GetRewardType()
		{
			if (_rewardData.RewardExp > 0)
			{
				_rewardItemNum = _rewardData.RewardExp;
				return ResourceType.PlayerExp;
			}
			if (_rewardData.RewardFriendPoint > 0)
			{
				_rewardItemNum = _rewardData.RewardFriendPoint;
				return ResourceType.FriendPoint;
			}
			if (_rewardData.RewardHCoin > 0)
			{
				_rewardItemNum = _rewardData.RewardHCoin;
				return ResourceType.Hcoin;
			}
			if (_rewardData.RewardSCoin > 0)
			{
				_rewardItemNum = _rewardData.RewardSCoin;
				return ResourceType.Scoin;
			}
			if (_rewardData.RewardStamina > 0)
			{
				_rewardItemNum = _rewardData.RewardStamina;
				return ResourceType.Stamina;
			}
			if (_rewardData.RewardSkillPoint > 0)
			{
				_rewardItemNum = _rewardData.RewardSkillPoint;
				return ResourceType.SkillPoint;
			}
			return ResourceType.Item;
		}

		private void SetupRewardItemData()
		{
			ResourceType rewardType = GetRewardType();
			if (rewardType == ResourceType.Item)
			{
				if (_rewardData.RewardItem1ID > 0)
				{
					_rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_rewardData.RewardItem1ID, _rewardData.RewardItem1Level);
					_rewardItemData.number = _rewardData.RewardItem1Num;
					_rewardItemNum = _rewardData.RewardItem1Num;
					return;
				}
				if (_rewardData.RewardItem2ID > 0)
				{
					_rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_rewardData.RewardItem2ID, _rewardData.RewardItem2Level);
					_rewardItemData.number = _rewardData.RewardItem2Num;
					_rewardItemNum = _rewardData.RewardItem2Num;
					return;
				}
				if (_rewardData.RewardItem3ID > 0)
				{
					_rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_rewardData.RewardItem3ID, _rewardData.RewardItem3Level);
					_rewardItemData.number = _rewardData.RewardItem3Num;
					_rewardItemNum = _rewardData.RewardItem3Num;
					return;
				}
				if (_rewardData.RewardItem4ID > 0)
				{
					_rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_rewardData.RewardItem4ID, _rewardData.RewardItem4Level);
					_rewardItemData.number = _rewardData.RewardItem4Num;
					_rewardItemNum = _rewardData.RewardItem4Num;
					return;
				}
				if (_rewardData.RewardItem5ID > 0)
				{
					_rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(_rewardData.RewardItem5ID, _rewardData.RewardItem5Level);
					_rewardItemData.number = _rewardData.RewardItem5Num;
					_rewardItemNum = _rewardData.RewardItem5Num;
					return;
				}
			}
			_rewardItemData = null;
		}

		private void SetupRewardItemIcon()
		{
			ResourceType rewardType = GetRewardType();
			SetupRewardItemData();
			Sprite resourceSprite = UIUtil.GetResourceSprite(rewardType, _rewardItemData);
			base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = resourceSprite;
			base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
			Text component = base.transform.Find("Text").GetComponent<Text>();
			if (rewardType == ResourceType.Item)
			{
				if (_rewardItemData is WeaponDataItem || _rewardItemData is StigmataDataItem)
				{
					component.text = "Lv." + _rewardItemData.level;
				}
				else
				{
					component.text = "x" + _rewardItemData.number;
				}
				base.transform.Find("FragmentIcon").gameObject.SetActive(_rewardItemData is AvatarFragmentDataItem);
				base.transform.Find("StigmataType").gameObject.SetActive(_rewardItemData is StigmataDataItem);
				if (_rewardItemData is StigmataDataItem)
				{
					base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[_rewardItemData.GetBaseType()]);
					if (_rewardAlreadyGot)
					{
						base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("SignInGotGrey");
					}
				}
				base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[_rewardItemData.rarity]);
			}
			else
			{
				component.text = "x" + _rewardItemNum;
				base.transform.Find("ItemIcon").GetComponent<Image>().sprite = ((rewardType != ResourceType.Hcoin) ? Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComBlue") : Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComPurple"));
			}
			base.transform.Find("Received").gameObject.SetActive(_rewardAlreadyGot);
			base.transform.GetComponent<Button>().interactable = !_rewardAlreadyGot;
		}
	}
}
