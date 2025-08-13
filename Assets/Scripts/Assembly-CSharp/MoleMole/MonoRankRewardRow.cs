using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoRankRewardRow : MonoBehaviour
	{
		private const string REWARD_ITEM_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/RewardItem";

		private List<RewardUIData> _rankRewardDataList = new List<RewardUIData>();

		private int _rewardID;

		public void SetupView(int rewardID)
		{
			_rewardID = rewardID;
			InitRewardList();
			Transform transform = base.transform.Find("RewardList/RewardContainer");
			if (_rankRewardDataList.Count <= 0)
			{
				transform.gameObject.SetActive(false);
				base.transform.Find("RewardList/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_EndlessNoReward");
				return;
			}
			transform.gameObject.SetActive(true);
			base.transform.Find("RewardList/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_Reward");
			transform.DestroyChildren();
			foreach (RewardUIData rankRewardData in _rankRewardDataList)
			{
				Transform transform2 = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/RewardItem")).transform;
				transform2.SetParent(transform, false);
				transform2.GetComponent<MonoRewardItem>().SetupView(rankRewardData);
			}
		}

		private void InitRewardList()
		{
			_rankRewardDataList.Clear();
			RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(_rewardID);
			if (rewardDataByKey != null)
			{
				if (rewardDataByKey.RewardExp > 0)
				{
					RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
					playerExpData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(playerExpData);
				}
				if (rewardDataByKey.RewardSCoin > 0)
				{
					RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
					scoinData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(scoinData);
				}
				if (rewardDataByKey.RewardHCoin > 0)
				{
					RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
					hcoinData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(hcoinData);
				}
				if (rewardDataByKey.RewardStamina > 0)
				{
					RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
					staminaData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(staminaData);
				}
				if (rewardDataByKey.RewardSkillPoint > 0)
				{
					RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
					skillPointData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(skillPointData);
				}
				if (rewardDataByKey.RewardFriendPoint > 0)
				{
					RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
					friendPointData.itemID = rewardDataByKey.RewardID;
					_rankRewardDataList.Add(friendPointData);
				}
				if (rewardDataByKey.RewardItem1ID > 0)
				{
					RewardUIData item = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
					_rankRewardDataList.Add(item);
				}
				if (rewardDataByKey.RewardItem2ID > 0)
				{
					RewardUIData item2 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
					_rankRewardDataList.Add(item2);
				}
				if (rewardDataByKey.RewardItem3ID > 0)
				{
					RewardUIData item3 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
					_rankRewardDataList.Add(item3);
				}
				if (rewardDataByKey.RewardItem4ID > 0)
				{
					RewardUIData item4 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
					_rankRewardDataList.Add(item4);
				}
				if (rewardDataByKey.RewardItem5ID > 0)
				{
					RewardUIData item5 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
					_rankRewardDataList.Add(item5);
				}
			}
		}
	}
}
