using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoInviteRewardRow : MonoBehaviour
	{
		private InviteTab _inviteType;

		private InviteeFriendRewardData _inviteeRewardData;

		private bool _isAcceptInvitation;

		private InviteFriendRewardData _inviterRewardData;

		private List<RewardUIData> _rewardUIDataList;

		public void SetupView(bool isAcceptInvitation, InviteeFriendRewardData rewardData)
		{
			_inviteType = InviteTab.InviteeTab;
			_isAcceptInvitation = isAcceptInvitation;
			_inviteeRewardData = rewardData;
			_inviterRewardData = null;
			DoSetupView();
		}

		public void SetupView(InviteFriendRewardData rewardData)
		{
			_inviteType = InviteTab.InviterTab;
			_inviteeRewardData = null;
			_inviterRewardData = rewardData;
			DoSetupView();
		}

		private void DoSetupView()
		{
			Text component = base.transform.Find("Label/Text").GetComponent<Text>();
			if (_inviteType == InviteTab.InviteeTab)
			{
				component.gameObject.SetActive(_inviteeRewardData.levelSpecified);
				component.text = LocalizationGeneralLogic.GetText("InviteReward_Limit", _inviteeRewardData.level);
			}
			else
			{
				component.gameObject.SetActive(_inviterRewardData.levelSpecified);
				component.text = LocalizationGeneralLogic.GetText("InvitateeReward_Limit", _inviterRewardData.level);
			}
			if (_inviteType == InviteTab.InviterTab)
			{
				base.transform.Find("Label/Current").gameObject.SetActive(true);
				base.transform.Find("Label/DivisionSign").gameObject.SetActive(true);
				base.transform.Find("Label/Max").gameObject.SetActive(true);
				base.transform.Find("Label/Current").GetComponent<Text>().text = _inviterRewardData.cur_num.ToString();
				base.transform.Find("Label/Current").GetComponent<Text>().color = ((_inviterRewardData.cur_num < _inviterRewardData.max_num) ? MiscData.GetColor("WarningRed") : MiscData.GetColor("Blue"));
				base.transform.Find("Label/Max").GetComponent<Text>().text = _inviterRewardData.max_num.ToString();
			}
			else
			{
				base.transform.Find("Label/Current").gameObject.SetActive(false);
				base.transform.Find("Label/DivisionSign").gameObject.SetActive(false);
				base.transform.Find("Label/Max").gameObject.SetActive(false);
			}
			SetupRewardList();
			Transform transform = base.transform.Find("Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(i < _rewardUIDataList.Count);
				if (i < _rewardUIDataList.Count)
				{
					RewardUIData rewardUIData = _rewardUIDataList[i];
					Image component2 = child.Find("ItemIcon").GetComponent<Image>();
					Image component3 = child.Find("ItemIcon/Icon").GetComponent<Image>();
					component2.gameObject.SetActive(true);
					child.Find("SelectedMark").gameObject.SetActive(false);
					child.Find("ProtectedMark").gameObject.SetActive(false);
					child.Find("InteractiveMask").gameObject.SetActive(false);
					child.Find("NotEnough").gameObject.SetActive(false);
					child.Find("Star").gameObject.SetActive(false);
					child.Find("StigmataType").gameObject.SetActive(false);
					child.Find("UnidentifyText").gameObject.SetActive(false);
					child.Find("QuestionMark").gameObject.SetActive(false);
					child.Find("ItemIcon").GetComponent<Image>().color = Color.white;
					child.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
					if (rewardUIData.rewardType == ResourceType.Item)
					{
						StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardUIData.itemID, rewardUIData.level);
						dummyStorageDataItem.number = rewardUIData.value;
						MonoItemIconButton component4 = child.GetComponent<MonoItemIconButton>();
						component4.SetupView(dummyStorageDataItem);
						component4.SetClickCallback(OnItemClick);
					}
					else
					{
						component3.sprite = rewardUIData.GetIconSprite();
						child.Find("Text").GetComponent<Text>().text = "×" + rewardUIData.value;
					}
				}
			}
			if (_inviteType == InviteTab.InviteeTab)
			{
				base.transform.Find("AlreadyIssued").gameObject.SetActive(_isAcceptInvitation && Singleton<PlayerModule>.Instance.playerData.teamLevel >= _inviteeRewardData.level);
			}
			else
			{
				base.transform.Find("AlreadyIssued").gameObject.SetActive(_inviterRewardData.cur_num >= _inviterRewardData.max_num);
			}
		}

		private void SetupRewardList()
		{
			_rewardUIDataList = new List<RewardUIData>();
			if ((_inviteType == InviteTab.InviteeTab && _inviteeRewardData.reward_list.Count < 1) || (_inviteType == InviteTab.InviterTab && _inviterRewardData.reward_list.Count < 1))
			{
				return;
			}
			proto.RewardData val = ((_inviteType != InviteTab.InviteeTab) ? _inviterRewardData.reward_list[0] : _inviteeRewardData.reward_list[0]);
			if (val.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)val.exp);
				_rewardUIDataList.Add(playerExpData);
			}
			if (val.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)val.scoin);
				_rewardUIDataList.Add(scoinData);
			}
			if (val.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)val.hcoin);
				_rewardUIDataList.Add(hcoinData);
			}
			if (val.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)val.stamina);
				_rewardUIDataList.Add(staminaData);
			}
			if (val.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)val.skill_point);
				_rewardUIDataList.Add(skillPointData);
			}
			if (val.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)val.friends_point);
				_rewardUIDataList.Add(friendPointData);
			}
			foreach (RewardItemData item2 in val.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.id, (int)item2.level);
				_rewardUIDataList.Add(item);
			}
		}

		private void OnItemClick(StorageDataItemBase itemData, bool selected)
		{
			UIUtil.ShowItemDetail(itemData);
		}
	}
}
