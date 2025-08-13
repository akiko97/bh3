using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MissionRewardGotDialogContext : BaseDialogContext
	{
		public delegate void OnDialogDestroy(AvatarCardDataItem data);

		private OnDialogDestroy _onDestroy;

		private AvatarCardDataItem _avatarData;

		private List<proto.RewardData> _rewardDataList;

		private List<DropItem> _dropItemList;

		private SequenceAnimationManager _animationManager;

		private List<RewardUIData> _missionRewardList = new List<RewardUIData>();

		public MissionRewardGotDialogContext(List<proto.RewardData> rewardDataList, List<DropItem> dropList = null)
		{
			config = new ContextPattern
			{
				contextName = "MissionRewardGotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MissionRewardGotDialog"
			};
			_rewardDataList = rewardDataList;
			_dropItemList = dropList;
			_avatarData = null;
		}

		public void RegisterCallBack(OnDialogDestroy callback)
		{
			_onDestroy = callback;
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			ClearViews();
			InitRewardList();
			SetupContents();
			_animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>());
			_animationManager.StartPlay(0.5f, false);
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
		}

		private void OnBGBtnClick()
		{
			Destroy();
			if (_onDestroy != null)
			{
				_onDestroy(_avatarData);
			}
		}

		private void ClearViews()
		{
			base.view.transform.Find("Dialog/Content/RewardList/center").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/RewardList/left").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/RewardList/right").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/TextList/line1").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/TextList/line2").gameObject.SetActive(false);
		}

		private void SetupContents()
		{
			int count = _missionRewardList.Count;
			if (count == 0)
			{
				return;
			}
			if (count == 1)
			{
				RewardUIData data = _missionRewardList[0];
				Transform icon = base.view.transform.Find("Dialog/Content/RewardList/center");
				SetupIcon(icon, data);
				Transform line = base.view.transform.Find("Dialog/Content/TextList/line1");
				SetupLine(line, data);
			}
			else if (count >= 2)
			{
				RewardUIData data2 = _missionRewardList[0];
				Transform icon2 = base.view.transform.Find("Dialog/Content/RewardList/left");
				SetupIcon(icon2, data2);
				data2 = _missionRewardList[1];
				Transform icon3 = base.view.transform.Find("Dialog/Content/RewardList/right");
				SetupIcon(icon3, data2);
				data2 = _missionRewardList[0];
				Transform line2 = base.view.transform.Find("Dialog/Content/TextList/line1");
				SetupLine(line2, data2);
				data2 = _missionRewardList[1];
				Transform line3 = base.view.transform.Find("Dialog/Content/TextList/line2");
				SetupLine(line3, data2);
				if (count <= 2)
				{
				}
			}
		}

		private void SetupIcon(Transform icon, RewardUIData data)
		{
			icon.gameObject.SetActive(true);
			icon.Find("ItemIcon/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
			icon.Find("Text").GetComponent<Text>().text = string.Format("×{0}", data.value);
		}

		private void SetupLine(Transform line, RewardUIData data)
		{
			line.gameObject.SetActive(true);
			line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
			line.Find("Desc").GetComponent<Text>().text = GetDesc(data.valueLabelTextID, data.itemID);
			line.Find("Number").GetComponent<Text>().text = string.Format("×{0}", data.value);
		}

		private void InitRewardList()
		{
			proto.RewardData val = _rewardDataList[0];
			_missionRewardList.Clear();
			if (val.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)val.exp);
				_missionRewardList.Add(playerExpData);
			}
			if (val.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)val.scoin);
				_missionRewardList.Add(scoinData);
			}
			if (val.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)val.hcoin);
				_missionRewardList.Add(hcoinData);
			}
			if (val.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)val.stamina);
				_missionRewardList.Add(staminaData);
			}
			if (val.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)val.skill_point);
				_missionRewardList.Add(skillPointData);
			}
			if (val.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)val.friends_point);
				_missionRewardList.Add(friendPointData);
			}
			foreach (RewardItemData item3 in val.item_list)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, (int)item3.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item3.id, (int)item3.level);
				_missionRewardList.Add(item);
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item3.id, (int)item3.level);
				if (dummyStorageDataItem is AvatarCardDataItem)
				{
					_avatarData = dummyStorageDataItem as AvatarCardDataItem;
				}
			}
			if (_dropItemList == null)
			{
				return;
			}
			foreach (DropItem dropItem in _dropItemList)
			{
				RewardUIData item2 = new RewardUIData(ResourceType.Item, (int)dropItem.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)dropItem.item_id, (int)dropItem.level);
				_missionRewardList.Add(item2);
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
