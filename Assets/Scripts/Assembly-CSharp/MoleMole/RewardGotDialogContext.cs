using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class RewardGotDialogContext : BaseDialogContext
	{
		public delegate void OnDialogDestroy();

		private const string DROP_ITEM_PREFAB_PATH = "UI/Menus/Widget/Map/DropItemButton";

		private OnDialogDestroy _onDestroy;

		private proto.RewardData _rewardData;

		private SequenceAnimationManager _animationManager;

		private string _titleTextID;

		private string _completeIconPrefabPath;

		private List<DropItem> _dropItemList;

		private List<RewardUIData> _nonItemRewardList;

		private Dictionary<int, StorageDataItemBase> _rewardItemDict;

		private int _playerLevelBefore;

		public RewardGotDialogContext(proto.RewardData rewardData, int playerLevelBefore, List<DropItem> dropList = null, string titleTextID = "", string completeIconPrefabPath = "")
		{
			config = new ContextPattern
			{
				contextName = "RewardGotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/RewardGotDialog"
			};
			_rewardData = rewardData;
			_dropItemList = dropList;
			_titleTextID = titleTextID;
			_completeIconPrefabPath = completeIconPrefabPath;
			_playerLevelBefore = playerLevelBefore;
		}

		public void RegisterCallBack(OnDialogDestroy callback)
		{
			_onDestroy = callback;
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			ClearViews();
			SetupTitle();
			SetupRewardList();
			SetupCompleteIcon();
			_animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>());
			_animationManager.StartPlay(0.5f, false);
			Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Button").GetComponent<Button>(), OnBGBtnClick);
		}

		private void OnBGBtnClick()
		{
			Destroy();
			if (_onDestroy != null)
			{
				_onDestroy();
			}
			if (_playerLevelBefore < Singleton<PlayerModule>.Instance.playerData.teamLevel)
			{
				PlayerLevelUpDialogContext playerLevelUpDialogContext = new PlayerLevelUpDialogContext();
				playerLevelUpDialogContext.SetLevelBeforeNoScoreManager(_playerLevelBefore);
				Singleton<MainUIManager>.Instance.ShowDialog(playerLevelUpDialogContext);
			}
		}

		private void ClearViews()
		{
			base.view.transform.Find("Dialog/Content/TextList/line1").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/TextList/line2").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/TextList/line3").gameObject.SetActive(false);
		}

		private void SetupIcon(Transform icon, RewardUIData data)
		{
			icon.gameObject.SetActive(true);
			icon.Find("ItemIcon/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
			icon.Find("Text").GetComponent<Text>().text = string.Format("×{0}", data.value);
		}

		private void SetupLine(Transform line, RewardUIData data, StorageDataItemBase item = null)
		{
			line.gameObject.SetActive(true);
			line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
			line.Find("Desc").GetComponent<Text>().text = GetDesc(data.valueLabelTextID, data.itemID);
			line.Find("Number").GetComponent<Text>().text = string.Format("×{0}", data.value);
		}

		private void SetupRewardList()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/TextList/line1");
			Transform transform2 = base.view.transform.Find("Dialog/Content/TextList/line2");
			Transform transform3 = base.view.transform.Find("Dialog/Content/TextList/line3");
			transform.gameObject.SetActive(false);
			transform2.gameObject.SetActive(false);
			transform3.gameObject.SetActive(false);
			int num = 0;
			_nonItemRewardList = new List<RewardUIData>();
			if (_rewardData.exp != 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int)_rewardData.exp);
				_nonItemRewardList.Add(playerExpData);
			}
			if (_rewardData.scoin != 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData((int)_rewardData.scoin);
				_nonItemRewardList.Add(scoinData);
			}
			if (_rewardData.hcoin != 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData((int)_rewardData.hcoin);
				_nonItemRewardList.Add(hcoinData);
			}
			if (_rewardData.stamina != 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData((int)_rewardData.stamina);
				_nonItemRewardList.Add(staminaData);
			}
			if (_rewardData.skill_point != 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData((int)_rewardData.skill_point);
				_nonItemRewardList.Add(skillPointData);
			}
			if (_rewardData.friends_point != 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData((int)_rewardData.friends_point);
				_nonItemRewardList.Add(friendPointData);
			}
			foreach (RewardUIData nonItemReward in _nonItemRewardList)
			{
				num++;
				Transform textLine = GetTextLine(num, transform, transform2, transform3);
				if (!(textLine == null))
				{
					SetupLine(textLine, nonItemReward);
				}
			}
			_rewardItemDict = new Dictionary<int, StorageDataItemBase>();
			foreach (RewardItemData item in _rewardData.item_list)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.id, (int)item.level);
				dummyStorageDataItem.number = (int)item.num;
				if (_rewardItemDict.ContainsKey(dummyStorageDataItem.ID))
				{
					_rewardItemDict[dummyStorageDataItem.ID].number += dummyStorageDataItem.number;
				}
				else
				{
					_rewardItemDict[dummyStorageDataItem.ID] = dummyStorageDataItem;
				}
			}
			if (_dropItemList != null)
			{
				foreach (DropItem dropItem in _dropItemList)
				{
					StorageDataItemBase dummyStorageDataItem2 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)dropItem.item_id, (int)dropItem.level);
					dummyStorageDataItem2.number = (int)dropItem.num;
					if (_rewardItemDict.ContainsKey(dummyStorageDataItem2.ID))
					{
						_rewardItemDict[dummyStorageDataItem2.ID].number += dummyStorageDataItem2.number;
					}
					else
					{
						_rewardItemDict[dummyStorageDataItem2.ID] = dummyStorageDataItem2;
					}
				}
			}
			Transform transform4 = base.view.transform.Find("Dialog/Content/RewardList/Content");
			transform4.DestroyChildren();
			foreach (RewardUIData nonItemReward2 in _nonItemRewardList)
			{
				Transform transform5 = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton")).transform;
				transform5.SetParent(transform4, false);
				HideRewardTransSomePart(transform5);
				transform5.GetComponent<MonoLevelDropIconButton>().Clear();
				transform5.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = nonItemReward2.GetIconSprite();
				transform5.Find("BG/Desc").GetComponent<Text>().text = "×" + nonItemReward2.value;
				transform5.GetComponent<CanvasGroup>().alpha = 1f;
			}
			foreach (StorageDataItemBase value in _rewardItemDict.Values)
			{
				Transform transform6 = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton")).transform;
				transform6.SetParent(transform4, false);
				transform6.GetComponent<MonoLevelDropIconButton>().SetupView(value, OnDropItemButtonClick, true);
				transform6.GetComponent<CanvasGroup>().alpha = 1f;
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

		private void HideRewardTransSomePart(Transform rewardTrans)
		{
			rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
			rewardTrans.Find("NewMark").gameObject.SetActive(false);
			rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
			rewardTrans.Find("Star").gameObject.SetActive(false);
			rewardTrans.Find("StigmataType").gameObject.SetActive(false);
			rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
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

		private Transform GetTextLine(int typeCount, params Transform[] lineTrans)
		{
			switch (typeCount)
			{
			case 1:
				return lineTrans[0];
			case 2:
				return lineTrans[1];
			case 3:
				return lineTrans[2];
			default:
				return null;
			}
		}

		private void SetupTitle()
		{
			if (!string.IsNullOrEmpty(_titleTextID))
			{
				base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_titleTextID);
			}
		}

		private void SetupCompleteIcon()
		{
			if (!string.IsNullOrEmpty(_completeIconPrefabPath))
			{
				base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_completeIconPrefabPath);
			}
		}

		private void OnDropItemButtonClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}
	}
}
