using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoWelfareItem : MonoBehaviour
	{
		private const string expPrefabPath = "SpriteOutput/RewardGotIcons/Exp";

		private const string scoinPrefabPath = "SpriteOutput/RewardGotIcons/SCoin";

		private const string hcoinPrefabPath = "SpriteOutput/RewardGotIcons/HCoin";

		private const string friendPointPrefabPath = "SpriteOutput/RewardGotIcons/FriendPoint";

		private const string skillPointPrefabPath = "SpriteOutput/RewardGotIcons/SkillPoint";

		private const string staminaPrefabPath = "SpriteOutput/RewardGotIcons/Stamina";

		private WelfareDataItem _welfareDataItem;

		private Action _onGetBtnClick;

		public void SetupView(WelfareDataItem welfareDataItem, Action onGetBtnClick = null)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Invalid comparison between Unknown and I4
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Invalid comparison between Unknown and I4
			_welfareDataItem = welfareDataItem;
			_onGetBtnClick = onGetBtnClick;
			if ((int)welfareDataItem.rewardStatus == 2)
			{
				base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Get/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDesc", welfareDataItem.payHCoin);
				base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo1/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
				base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(false);
			}
			else if ((int)welfareDataItem.rewardStatus == 1)
			{
				base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Unget/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDesc", welfareDataItem.payHCoin);
				base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo2/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
				base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(false);
				int totalPayHCoin = Singleton<ShopWelfareModule>.Instance.totalPayHCoin;
				base.transform.Find("InnerPanel/ProgressPanel/HCoin/HCoin/Num").GetComponent<Text>().text = (welfareDataItem.payHCoin - totalPayHCoin).ToString();
				base.transform.Find("InnerPanel/ProgressPanel/ProgressBar").GetComponent<MonoMaskSlider>().UpdateValue(totalPayHCoin, welfareDataItem.payHCoin, 0f);
			}
			if ((int)welfareDataItem.rewardStatus == 3)
			{
				base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/BG/Over/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDescHasGot", welfareDataItem.payHCoin);
				base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/Reward/RewardNo3/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
				base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(true);
				base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(false);
				base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(false);
			}
			SetupRewardList();
		}

		public void OnClickRequestVipReward()
		{
			if (CanGetVipReward())
			{
				if (_onGetBtnClick != null)
				{
					_onGetBtnClick();
				}
				Singleton<NetworkManager>.Instance.RequestGetVipReward(_welfareDataItem.vipLevel);
			}
		}

		public WelfareDataItem GetWelfareDataItem()
		{
			return _welfareDataItem;
		}

		private void SetupRewardList()
		{
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Invalid comparison between Unknown and I4
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Invalid comparison between Unknown and I4
			RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(_welfareDataItem.rewardID);
			Transform transform = base.transform.Find("InnerPanel/RewardList/right");
			Transform transform2 = base.transform.Find("InnerPanel/RewardList/center");
			Transform transform3 = base.transform.Find("InnerPanel/RewardList/left");
			transform.gameObject.SetActive(false);
			transform2.gameObject.SetActive(false);
			transform3.gameObject.SetActive(false);
			int num = 0;
			List<Tuple<string, int>> list = new List<Tuple<string, int>>();
			if (rewardDataByKey.RewardExp > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/Exp", rewardDataByKey.RewardExp));
			}
			if (rewardDataByKey.RewardSCoin > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/SCoin", rewardDataByKey.RewardSCoin));
			}
			if (rewardDataByKey.RewardHCoin > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/HCoin", rewardDataByKey.RewardHCoin));
			}
			if (rewardDataByKey.RewardStamina > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/Stamina", rewardDataByKey.RewardStamina));
			}
			if (rewardDataByKey.RewardSkillPoint > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/SkillPoint", rewardDataByKey.RewardSkillPoint));
			}
			if (rewardDataByKey.RewardFriendPoint > 0)
			{
				list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/FriendPoint", rewardDataByKey.RewardFriendPoint));
			}
			foreach (Tuple<string, int> item in list)
			{
				num++;
				Transform rewardTrans = GetRewardTrans(num, transform3, transform2, transform);
				if (rewardDataByKey != null)
				{
					rewardTrans.gameObject.SetActive(true);
					HideRewardTransSomePart(rewardTrans);
					rewardTrans.GetComponent<MonoLevelDropIconButton>().Clear();
					rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(item.Item1);
					rewardTrans.Find("BG/Desc").GetComponent<Text>().text = "×" + item.Item2;
					if ((int)_welfareDataItem.rewardStatus == 3)
					{
						SetRewardItemGrey(rewardTrans);
					}
					else
					{
						SetItemDefaultMaterialAndColor(rewardTrans);
					}
				}
			}
			List<Tuple<int, int, int>> list2 = new List<Tuple<int, int, int>>();
			if (rewardDataByKey.RewardItem1ID > 0)
			{
				list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level, rewardDataByKey.RewardItem1Num));
			}
			if (rewardDataByKey.RewardItem2ID > 0)
			{
				list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level, rewardDataByKey.RewardItem2Num));
			}
			if (rewardDataByKey.RewardItem3ID > 0)
			{
				list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level, rewardDataByKey.RewardItem3Num));
			}
			if (rewardDataByKey.RewardItem4ID > 0)
			{
				list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level, rewardDataByKey.RewardItem4Num));
			}
			if (rewardDataByKey.RewardItem5ID > 0)
			{
				list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level, rewardDataByKey.RewardItem5Num));
			}
			foreach (Tuple<int, int, int> item2 in list2)
			{
				num++;
				Transform rewardTrans2 = GetRewardTrans(num, transform3, transform2, transform);
				if (rewardDataByKey != null)
				{
					rewardTrans2.gameObject.SetActive(true);
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item2.Item1, item2.Item2);
					dummyStorageDataItem.number = item2.Item3;
					rewardTrans2.GetComponent<MonoLevelDropIconButton>().SetupView(isGrey: (int)_welfareDataItem.rewardStatus == 3, itemData: dummyStorageDataItem, callBack: OnItemBtnClick, showDesc: true);
				}
			}
		}

		private void HideRewardTransSomePart(Transform rewardTrans)
		{
			rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
			rewardTrans.Find("NewMark").gameObject.SetActive(false);
			rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
			rewardTrans.Find("Star").gameObject.SetActive(false);
			rewardTrans.Find("StigmataType").gameObject.SetActive(false);
			rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
		}

		private void SetRewardItemGrey(Transform rewardTrans)
		{
			rewardTrans.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			rewardTrans.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
			Image component = rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
			if (component.material != component.defaultMaterial)
			{
				component.color = MiscData.GetColor("DropItemIconFullGrey");
			}
			else
			{
				component.color = MiscData.GetColor("DropItemIconGrey");
			}
		}

		private void SetItemDefaultMaterialAndColor(Transform rewardTrans)
		{
			rewardTrans.Find("BG/Unselected").GetComponent<Image>().material = null;
			rewardTrans.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
			rewardTrans.Find("BG/Image").GetComponent<Image>().material = null;
			rewardTrans.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
			rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().material = null;
			rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconDefaultColor");
			rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().material = null;
			rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
		}

		private Transform GetRewardTrans(int typeCount, params Transform[] rewardTrans)
		{
			switch (typeCount)
			{
			case 1:
				return rewardTrans[0];
			case 2:
				return rewardTrans[1];
			case 3:
				return rewardTrans[2];
			default:
				return null;
			}
		}

		private void OnItemBtnClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}

		private bool CanGetVipReward()
		{
			return true;
		}
	}
}
