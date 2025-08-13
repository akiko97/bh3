using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class LevelResultDialogContext : BaseDialogContext
	{
		private const int MAX_TEAM_MEMBER_NUM = 3;

		private const int DROP_ITEM_ANI_MAX_COUNT = 9;

		private const string DROP_ANI_CALL_BACK_STR = "Play_drop";

		private const string DROP_ITEM_SCALE_07 = "DropItemScale07";

		private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";

		private StageEndRsp _stageEndRsp;

		private FriendDetailDataItem _helperInfo;

		private LevelDataItem _levelData;

		private int _dropItemAniCount;

		private MonoGridScroller _dropScroller;

		private List<DropItem> _dropItemList;

		private DropItem _normalDropItem;

		private DropItem _fastDropItem;

		private DropItem _sonicDropItem;

		private PlayerLevelUpDialogContext _playerLevelUpDialogContext;

		private SequenceDialogManager _playerLevelUpAndAvatarNewSkillDialogManager;

		private bool _avatarCanUnlockSkillDialogShowAlready;

		private SequenceAnimationManager _leftPanelAnimationManager;

		private SequenceAnimationManager _dropPanelBGAnimationManager;

		private SequenceAnimationManager _dropItemAnimationManager;

		private SequenceDialogManager _dropNewItemDialogManager;

		private bool _leftPanelAnimationsEnd;

		private bool _playerAvatarDialogsEnd;

		private bool _dropPanelBGAniamtionEnd;

		private AddFriendDialogContext _addFriendDialog;

		private bool _levelSuccess;

		private FriendDetailDataItem _friendDetailData;

		public Action onDestory;

		public LevelResultDialogContext(StageEndRsp rsp = null)
		{
			config = new ContextPattern
			{
				contextName = "LevelResultDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/LevelResultDialog",
				cacheType = ViewCacheType.DontCache
			};
			findViewSavedInScene = true;
			_stageEndRsp = rsp;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.PlayerLevelUp)
			{
				return OnPlayerLevelUpNotify(ntf);
			}
			if (ntf.type == NotifyTypes.AvatarLevelUp)
			{
				return OnAvatarLevelUpNotify(ntf);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("RewardPanel/OKBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("RewardPanel/ExpPanel/Helper/BattleFriendInfoRow/DetailButton").GetComponent<Button>(), OnHelperClick);
			BindViewCallback(base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn").GetComponent<Button>(), OnAddFriendBtnClick);
		}

		protected override bool SetupView()
		{
			if (Singleton<LevelScoreManager>.Instance == null)
			{
				base.Destroy();
				return false;
			}
			base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
			_friendDetailData = Singleton<LevelScoreManager>.Instance.friendDetailItem;
			InitAnimationAndDialogManager();
			SetupTitle();
			OnStageEndRsp(Singleton<LevelScoreManager>.Instance.stageEndRsp);
			SetupFriendDialog();
			return false;
		}

		private bool OnStageEndRsp(StageEndRsp rsp)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			_stageEndRsp = rsp;
			if ((int)rsp.retcode == 0)
			{
				if (_levelSuccess)
				{
					LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
					if (rsp.stage_idSpecified)
					{
					}
					_leftPanelAnimationManager.AddAnimation(base.view.transform.Find("Title").GetComponent<MonoAnimationinSequence>());
					SetupRewardPanel(_stageEndRsp);
					ShowRewardPanel();
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private void OnHelperClick()
		{
			if (_helperInfo != null)
			{
				base.view.transform.gameObject.SetActive(false);
				UIUtil.ShowFriendDetailInfo(_helperInfo, true, base.view.transform);
			}
		}

		private void OnAddFriendBtnClick()
		{
			if (_helperInfo != null)
			{
				Singleton<NetworkManager>.Instance.RequestAddFriend(_helperInfo.uid);
				base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_RequestSend");
				base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn").GetComponent<Button>().interactable = false;
			}
		}

		private void OnLoseNextBtnClick()
		{
			Close();
		}

		private bool OnDropAniNofity(Notify ntf)
		{
			if (_dropItemAniCount < 9 && _dropItemAniCount < _dropItemList.Count)
			{
				Animation component = _dropScroller.GetItemTransByIndex(_dropItemAniCount).GetComponent<Animation>();
				component.Play();
				_dropItemAniCount++;
			}
			return false;
		}

		private bool OnPlayerLevelUpNotify(Notify ntf)
		{
			Transform transform = base.view.transform.Find("RewardPanel/ExpPanel");
			Transform transform2 = transform.Find("PlayerExp/InfoRowLv");
			transform2.Find("LevelLabel").GetComponent<Text>().text = "LV." + Singleton<PlayerModule>.Instance.playerData.teamLevel;
			_playerLevelUpAndAvatarNewSkillDialogManager.StartShow();
			return false;
		}

		private bool OnAvatarLevelUpNotify(Notify ntf)
		{
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			Transform transform = base.view.transform.Find("RewardPanel/ExpPanel");
			for (int i = 0; i < 3; i++)
			{
				Transform child = transform.Find("AvatarExp/Team").GetChild(i);
				AvatarDataItem avatarDataItem = ((i >= instance.memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(instance.memberList[i].avatarID));
				if (avatarDataItem != null)
				{
					child.Find("Content/LVNum").GetComponent<Text>().text = avatarDataItem.level.ToString();
				}
			}
			return false;
		}

		private bool OnAvatarCanUnlockSkillNotify(Notify ntf)
		{
			if (!_avatarCanUnlockSkillDialogShowAlready)
			{
				_avatarCanUnlockSkillDialogShowAlready = true;
				if (_playerLevelUpAndAvatarNewSkillDialogManager != null)
				{
					if (_playerLevelUpDialogContext != null)
					{
						_playerLevelUpAndAvatarNewSkillDialogManager.AddAsFirstDialog(_playerLevelUpDialogContext);
					}
					else
					{
						_playerLevelUpAndAvatarNewSkillDialogManager.StartShow();
					}
				}
			}
			return false;
		}

		private void InitAnimationAndDialogManager()
		{
			_leftPanelAnimationManager = new SequenceAnimationManager(OnLeftPanelAnimationsEnd);
			_playerLevelUpAndAvatarNewSkillDialogManager = new SequenceDialogManager(OnPlayerAvatarDialogsEnd);
			_dropPanelBGAnimationManager = new SequenceAnimationManager(OnDropPanelBGAniamtionEnd);
			_dropNewItemDialogManager = new SequenceDialogManager(OnDropNewItemDialogsEnd);
			_dropItemAnimationManager = new SequenceAnimationManager(OnItemPanelAnimationEnd);
			_dropScroller = base.view.transform.Find("RewardPanel/DropPanel/Drops/ScrollView").GetComponent<MonoGridScroller>();
			_leftPanelAnimationsEnd = false;
			_playerAvatarDialogsEnd = false;
			_dropPanelBGAniamtionEnd = false;
		}

		private void SetupTitle()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected I4, but got Unknown
			_levelSuccess = (int)Singleton<LevelScoreManager>.Instance.endStatus == 1;
			_levelData = Singleton<LevelModule>.Instance.GetLevelById(Singleton<LevelScoreManager>.Instance.LevelId);
			if (_levelData != null)
			{
				base.view.transform.Find("Title/LevelInfo").gameObject.SetActive(true);
				StageType levelType = _levelData.LevelType;
				switch ((int)levelType - 1)
				{
				case 0:
				{
					ActDataItem actDataItem = new ActDataItem(_levelData.ActID);
					base.view.transform.Find("Title/LevelInfo/ActName").GetComponent<Text>().text = actDataItem.actTitle + " " + actDataItem.actName;
					break;
				}
				case 1:
				case 2:
				{
					WeekDayActivityDataItem weekDayActivityByID = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(_levelData.ActID);
					base.view.transform.Find("Title/LevelInfo/ActName").GetComponent<Text>().text = weekDayActivityByID.GetActitityTitle();
					break;
				}
				}
				base.view.transform.Find("Title/LevelInfo/LevelName").GetComponent<Text>().text = _levelData.Title;
			}
			else
			{
				base.view.transform.Find("Title/LevelInfo").gameObject.SetActive(false);
			}
		}

		private void SetupFriendDialog()
		{
			if (HasFriendToAdd())
			{
				_addFriendDialog = new AddFriendDialogContext(_friendDetailData);
			}
		}

		private bool HasFriendToAdd()
		{
			if (_friendDetailData == null)
			{
				return false;
			}
			return !Singleton<FriendModule>.Instance.IsMyFriend(_friendDetailData.uid);
		}

		private void SetupRewardPanel(StageEndRsp rsp)
		{
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			PlayerModule instance2 = Singleton<PlayerModule>.Instance;
			Transform transform = base.view.transform.Find("RewardPanel/ExpPanel");
			Transform transform2 = transform.Find("PlayerExp/InfoRowLv");
			transform2.Find("LevelLabel").GetComponent<Text>().text = "LV." + instance.playerLevelBefore;
			transform2.Find("Exp/AddExp").GetComponent<Text>().text = rsp.player_exp_reward.ToString();
			transform2.Find("Exp/TiltSlider/").GetComponent<MonoMaskSlider>().UpdateValue(instance.playerExpBefore, instance2.playerData.TeamMaxExp, 0f);
			transform2.Find("Exp/MaxNumText").GetComponent<Text>().text = instance2.playerData.TeamMaxExp.ToString();
			transform2.Find("Exp/NumText").GetComponent<Text>().text = instance2.playerData.teamExp.ToString();
			if (instance.playerLevelBefore < instance2.playerData.teamLevel)
			{
				_playerLevelUpDialogContext = new PlayerLevelUpDialogContext();
				_playerLevelUpAndAvatarNewSkillDialogManager.AddDialog(_playerLevelUpDialogContext);
			}
			_leftPanelAnimationManager.AddAnimation(transform.Find("PlayerExp").GetComponent<MonoAnimationinSequence>());
			for (int i = 0; i < 3; i++)
			{
				Transform child = transform.Find("AvatarExp/Team").GetChild(i);
				Transform child2 = transform.Find("AvatarExp/Exps").GetChild(i);
				AvatarDataItem avatarDataItem = ((i >= instance.memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(instance.memberList[i].avatarID));
				if (avatarDataItem == null)
				{
					child.gameObject.SetActive(false);
					child2.gameObject.SetActive(false);
					child.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
					continue;
				}
				AvatarDataItem avatarDataItem2 = instance.memberList[i];
				child.Find("Content").gameObject.SetActive(true);
				child.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[avatarDataItem.Attribute]);
				child.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarDataItem.AvatarTachie);
				child.Find("Content/LVNum").GetComponent<Text>().text = avatarDataItem2.level.ToString();
				child.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarDataItem.star);
				child2.Find("AddExp").gameObject.SetActive(false);
				if (rsp.avatar_exp_rewardSpecified)
				{
					child2.Find("AddExp").gameObject.SetActive(true);
					child2.Find("AddExp").GetComponent<Text>().text = rsp.avatar_exp_reward.ToString();
				}
				child2.Find("TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(avatarDataItem.exp, avatarDataItem.MaxExp, 0f);
				if (avatarDataItem.level != avatarDataItem2.level || avatarDataItem.star != avatarDataItem2.star)
				{
					UIUtil.UpdateAvatarSkillStatusInLocalData(avatarDataItem);
				}
				List<KeyValuePair<string, bool>> canUnlockSkillNameList = Singleton<AvatarModule>.Instance.GetCanUnlockSkillNameList(avatarDataItem.avatarID, avatarDataItem2.level, avatarDataItem2.star, avatarDataItem.level, avatarDataItem.star);
				foreach (KeyValuePair<string, bool> item in canUnlockSkillNameList)
				{
					_playerLevelUpAndAvatarNewSkillDialogManager.AddDialog(new AvatarNewSkillCanUnlockDialogContext(avatarDataItem.FullName, item.Key, item.Value));
				}
			}
			_leftPanelAnimationManager.AddAnimation(transform.Find("AvatarExp").GetComponent<MonoAnimationinSequence>());
			_helperInfo = instance.friendDetailItem;
			SetTotalDropList(rsp, out _dropItemList, out _normalDropItem, out _fastDropItem, out _sonicDropItem);
			foreach (DropItem dropItem in _dropItemList)
			{
				if (Singleton<StorageModule>.Instance.IsItemNew((int)dropItem.item_id))
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)dropItem.item_id);
					dummyStorageDataItem.level = (int)dropItem.level;
					dummyStorageDataItem.number = (int)dropItem.num;
					_dropNewItemDialogManager.AddDialog(new DropNewItemDialogContext(dummyStorageDataItem));
				}
			}
			Transform transform3 = base.view.transform.Find("RewardPanel/DropPanel");
			_dropPanelBGAnimationManager.AddAnimation(transform3.GetComponent<MonoAnimationinSequence>());
			transform3.Find("Drops/ScrollView").GetComponent<MonoGridScroller>().Init(delegate(Transform trans, int index)
			{
				OnScrollerChange(trans, index);
			}, _dropItemList.Count);
			foreach (Transform item2 in transform3.Find("Drops/ScrollView/Content"))
			{
				MonoAnimationinSequence component = item2.GetComponent<MonoAnimationinSequence>();
				if (component != null)
				{
					component.animationName = "DropItemScale07";
				}
			}
			_dropPanelBGAnimationManager.AddAllChildrenInTransform(transform3.Find("Drops/ScrollView/Content"));
			transform3.Find("Reward").gameObject.SetActive(true);
			string text = ((!rsp.scoin_rewardSpecified) ? "0" : rsp.scoin_reward.ToString());
			transform3.Find("Reward/Num/Num").GetComponent<Text>().text = text;
		}

		private void ShowRewardPanel()
		{
			base.view.transform.Find("RewardPanel").gameObject.SetActive(true);
			_leftPanelAnimationManager.StartPlay();
		}

		private void Close()
		{
			Destroy();
		}

		public override void Destroy()
		{
			if (Singleton<LevelScoreManager>.Instance == null)
			{
				base.Destroy();
				return;
			}
			bool hasNuclearActivityBefore = Singleton<LevelScoreManager>.Instance.hasNuclearActivityBefore;
			bool flag = _levelData != null && Singleton<LevelScoreManager>.Instance.HasNuclearActivity(_levelData.levelId);
			bool flag2 = !hasNuclearActivityBefore && flag;
			Singleton<LevelScoreManager>.Destroy();
			Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			base.Destroy();
			if (onDestory != null)
			{
				onDestory();
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StageEnd, flag2));
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			MonoLevelDropIconButtonBox component = trans.GetComponent<MonoLevelDropIconButtonBox>();
			MonoLevelDropIconButton component2 = trans.Find("Item").GetComponent<MonoLevelDropIconButton>();
			Vector2 cellSize = _dropScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
			trans.SetLocalScaleX(cellSize.x / component2.width);
			trans.SetLocalScaleY(cellSize.y / component2.height);
			DropItem val = _dropItemList[index];
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)val.item_id);
			dummyStorageDataItem.level = (int)val.level;
			dummyStorageDataItem.number = (int)val.num;
			component2.SetupView(dummyStorageDataItem, OnDropItemBtnClick, true, true);
			if (val == _normalDropItem)
			{
				component.SetupTypeView(MonoLevelDropIconButtonBox.Type.NormalFinishChallengeReward, _dropPanelBGAniamtionEnd);
			}
			else if (val == _fastDropItem)
			{
				component.SetupTypeView(MonoLevelDropIconButtonBox.Type.FastFinishChallengeReward, _dropPanelBGAniamtionEnd);
			}
			else if (val == _sonicDropItem)
			{
				component.SetupTypeView(MonoLevelDropIconButtonBox.Type.SonicFinishChallengeReward, _dropPanelBGAniamtionEnd);
			}
			else
			{
				component.SetupTypeView(MonoLevelDropIconButtonBox.Type.DefaultDrop, _dropPanelBGAniamtionEnd);
			}
		}

		private void OnDropItemBtnClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}

		private void OnLeftPanelAnimationsEnd()
		{
			_leftPanelAnimationsEnd = true;
			if (!_dropPanelBGAniamtionEnd && _playerAvatarDialogsEnd)
			{
				_dropPanelBGAniamtionEnd = true;
				_dropPanelBGAnimationManager.StartPlay();
			}
		}

		private void OnPlayerAvatarDialogsEnd()
		{
			_playerAvatarDialogsEnd = true;
			if (!_dropPanelBGAniamtionEnd && _leftPanelAnimationsEnd)
			{
				_dropPanelBGAniamtionEnd = true;
				_dropPanelBGAnimationManager.StartPlay();
			}
		}

		private void OnDropPanelBGAniamtionEnd()
		{
			Transform transform = base.view.transform.Find("RewardPanel/DropPanel");
			foreach (Transform item in transform.Find("Drops/ScrollView/Content"))
			{
				MonoAnimationinSequence component = item.GetComponent<MonoAnimationinSequence>();
				if (component != null)
				{
					component.animationName = item.GetComponent<MonoLevelDropIconButtonBox>().GetOpenAnimationName();
				}
			}
			_dropItemAnimationManager.AddAllChildrenInTransform(transform.Find("Drops/ScrollView/Content"));
			_dropItemAnimationManager.StartPlay();
		}

		private void OnItemPanelAnimationEnd()
		{
			Transform transform = base.view.transform.Find("RewardPanel/DropPanel");
			foreach (Transform item in transform.Find("Drops/ScrollView/Content"))
			{
				item.GetComponent<MonoLevelDropIconButtonBox>().SetItemAfterAnimation();
			}
			_dropNewItemDialogManager.StartShow(0.6f);
		}

		private void OnDropNewItemDialogsEnd()
		{
			Transform transform = base.view.transform.Find("RewardPanel/DropPanel");
			MonoLevelDropIconButtonBox[] componentsInChildren = transform.Find("Drops/ScrollView/Content").GetComponentsInChildren<MonoLevelDropIconButtonBox>();
			foreach (MonoLevelDropIconButtonBox monoLevelDropIconButtonBox in componentsInChildren)
			{
				monoLevelDropIconButtonBox.SetOpenStatusView(true);
			}
			base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
			if (_addFriendDialog != null)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(_addFriendDialog);
			}
		}

		private void SetTotalDropList(StageEndRsp rsp, out List<DropItem> totalList, out DropItem normalDropItem, out DropItem fastDropItem, out DropItem sonicDropItem)
		{
			totalList = Singleton<LevelScoreManager>.Instance.GetTotalDropList();
			normalDropItem = null;
			fastDropItem = null;
			sonicDropItem = null;
			List<int> configChallengeIds = Singleton<LevelScoreManager>.Instance.configChallengeIds;
			LevelMetaData levelMeta = LevelMetaDataReader.TryGetLevelMetaDataByKey((int)rsp.stage_id);
			foreach (StageSpecialChallengeData item in rsp.special_challenge_list)
			{
				int challenge_index = (int)item.challenge_index;
				if (challenge_index < configChallengeIds.Count)
				{
					LevelChallengeDataItem levelChallengeDataItem = new LevelChallengeDataItem(configChallengeIds[challenge_index], levelMeta);
					if (levelChallengeDataItem.IsFinishStageNomalChallenge())
					{
						normalDropItem = item.drop_item;
						totalList.Add(normalDropItem);
					}
					else if (levelChallengeDataItem.IsFinishStageFastChallenge())
					{
						fastDropItem = item.drop_item;
						totalList.Add(fastDropItem);
					}
					else if (levelChallengeDataItem.IsFinishStageVeryFastChallenge())
					{
						sonicDropItem = item.drop_item;
						totalList.Add(sonicDropItem);
					}
				}
			}
		}
	}
}
