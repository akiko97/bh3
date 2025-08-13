using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class AchieveRewardGotContext : BaseDialogContext
	{
		public delegate void OnDialogDestroy();

		private OnDialogDestroy _onDestroy;

		private List<RewardUIData> _achieveRewardList = new List<RewardUIData>();

		private proto.RewardData _rewardData;

		private SequenceAnimationManager _animationManager;

		public AchieveRewardGotContext(List<proto.RewardData> dataList)
		{
			config = new ContextPattern
			{
				contextName = "AchieveRewardGotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AchieveRewardGotDialog"
			};
			_rewardData = dataList[0];
			InitRewardList();
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			SetupScrollView();
			SetupDetail();
			_animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>());
			_animationManager.StartPlay(0.5f, false);
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
		}

		private void SetupScrollView()
		{
			MonoGridScroller component = base.view.transform.Find("Dialog/Content/Rewards/ScrollView").GetComponent<MonoGridScroller>();
			component.Init(delegate(Transform trans, int index)
			{
				RewardUIData data = _achieveRewardList[index];
				SetupIconView(trans, data);
			}, _achieveRewardList.Count);
		}

		private void SetupDetail()
		{
			for (int i = 0; i < 3; i++)
			{
				SetupDetailItem(base.view.transform.Find("Dialog/Content/RewardDetail/Lines/" + (i + 1)).gameObject, (i >= _achieveRewardList.Count) ? null : _achieveRewardList[i]);
			}
		}

		private void SetupDetailItem(GameObject item, RewardUIData data)
		{
			if (data == null)
			{
				item.SetActive(false);
				return;
			}
			item.SetActive(true);
			Text component = item.transform.Find("Item").GetComponent<Text>();
			Text component2 = item.transform.Find("Number").GetComponent<Text>();
			if (data.rewardType == ResourceType.PlayerExp)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Level");
			}
			else if (data.rewardType == ResourceType.Scoin)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Scoin");
			}
			else if (data.rewardType == ResourceType.Hcoin)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Hcoin");
			}
			else if (data.rewardType == ResourceType.FriendPoint)
			{
				component.text = LocalizationGeneralLogic.GetText("10119");
			}
			else if (data.rewardType == ResourceType.Item)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID);
				component.text = dummyStorageDataItem.GetDisplayTitle();
			}
			component2.text = data.value.ToString();
		}

		private void OnBGBtnClick()
		{
			Destroy();
			if (_onDestroy != null)
			{
				_onDestroy();
			}
		}

		private void SetupIconView(Transform trans, RewardUIData data)
		{
			if (data.rewardType == ResourceType.PlayerExp || data.rewardType == ResourceType.FriendPoint || data.rewardType == ResourceType.Hcoin || data.rewardType == ResourceType.Scoin || data.rewardType == ResourceType.SkillPoint || data.rewardType == ResourceType.Stamina)
			{
				trans.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(data.iconPath);
				trans.Find("ItemIcon/Icon").GetComponent<Image>().SetNativeSize();
				trans.Find("QuestionMark").gameObject.SetActive(false);
				trans.Find("Star").gameObject.SetActive(false);
				trans.Find("Text").GetComponent<Text>().text = string.Format("×{0}", data.value.ToString());
				trans.Find("ItemIcon").GetComponent<Image>().color = Color.white;
			}
			else
			{
				MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
				if (component != null)
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID);
					dummyStorageDataItem.number = data.value;
					component.SetupView(dummyStorageDataItem);
				}
			}
		}

		private void InitRewardList()
		{
			_achieveRewardList.Clear();
			if (_rewardData.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)_rewardData.exp);
				_achieveRewardList.Add(playerExpData);
			}
			if (_rewardData.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)_rewardData.scoin);
				_achieveRewardList.Add(scoinData);
			}
			if (_rewardData.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)_rewardData.hcoin);
				_achieveRewardList.Add(hcoinData);
			}
			if (_rewardData.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)_rewardData.stamina);
				_achieveRewardList.Add(staminaData);
			}
			if (_rewardData.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)_rewardData.skill_point);
				_achieveRewardList.Add(skillPointData);
			}
			if (_rewardData.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)_rewardData.friends_point);
				_achieveRewardList.Add(friendPointData);
			}
			foreach (RewardItemData item2 in _rewardData.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.id, (int)item2.level);
				_achieveRewardList.Add(item);
			}
		}

		private string GetDesc(string textID, int id)
		{
			if (textID == RewardUIData.ITEM_ICON_TEXT_ID)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id);
				return dummyStorageDataItem.GetDisplayTitle();
			}
			return LocalizationGeneralLogic.GetText(textID);
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.UnlockAvatar)
			{
				int avatarID = (int)ntf.body;
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(avatarID));
			}
			return false;
		}
	}
}
