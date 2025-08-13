using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class LevelPreparePageContext : BasePageContext
	{
		private const int MAX_TEAM_MEMBER_NUM = 3;

		public readonly LevelDataItem level;

		private List<FriendBriefDataItem> _showDataList;

		private FriendBriefDataItem _selectedHelper;

		private int _playerUidToShow;

		private bool _isWaitingLevelBegin;

		private StageBeginRsp _stageBeginRsp;

		private FriendDetailDataItem _helperDetailData;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		public LevelPreparePageContext(LevelDataItem level)
		{
			config = new ContextPattern
			{
				contextName = "LevelPreparePageContext",
				viewPrefabPath = "UI/Menus/Page/Map/LevelPreparePage",
				cacheType = ViewCacheType.AlwaysCached
			};
			this.level = level;
			_playerUidToShow = -1;
			_isWaitingLevelBegin = false;
			_showDataList = new List<FriendBriefDataItem>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 44:
				return OnStageBeginRsp(pkt.getData<StageBeginRsp>());
			case 73:
				return OnPlayerDetailRsp(pkt.getData<GetPlayerDetailDataRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.TeamMemberChanged)
			{
				SetupMyTeam();
				SetupLeaderSkill();
				return false;
			}
			if (ntf.type == NotifyTypes.ShowStaminaExchangeInfo2)
			{
				return ShowStaminaExchangeDialog();
			}
			return false;
		}

		public bool ShowStaminaExchangeDialog()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange2"));
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnOkButtonCallBack);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = level.StaminaCost.ToString();
			base.view.transform.Find("BG/Pic").GetComponent<Image>().sprite = level.GetDetailPicSprite();
			SetupMyTeam();
			SetupHelpers();
			SetupLeaderSkill();
			base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn");
			base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().RegisterCallback(OnRefreshTeammateUI);
			base.view.transform.Find("DynamicLv").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
			base.view.transform.Find("LevelDebugButton").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			SetupMyTeam();
			SetupLeaderSkill();
			base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn");
			base.OnLandedFromBackPage();
		}

		public override void Destroy()
		{
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
			base.Destroy();
		}

		public void OnOkButtonCallBack()
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			if (Singleton<PlayerModule>.Instance.playerData.teamLevel < level.UnlockPlayerLevel)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Err_PlayerLvLack", level.UnlockPlayerLevel),
					type = GeneralDialogContext.ButtonType.SingleButton
				});
				return;
			}
			bool flag = false;
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			foreach (int item in memberList)
			{
				if (Singleton<IslandModule>.Instance.IsAvatarDispatched(item))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarDispatchedCannotFight"),
					type = GeneralDialogContext.ButtonType.SingleButton
				});
				return;
			}
			if (level.IsMultMode)
			{
				int count = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType).Count;
				if (count < level.MinEnterAvatarNum)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						title = LocalizationGeneralLogic.GetText("Menu_Tips"),
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_TeamMemberLackHint"),
						type = GeneralDialogContext.ButtonType.SingleButton
					});
					return;
				}
				if (count < 3 && count < Singleton<AvatarModule>.Instance.UserAvatarList.Count)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						title = LocalizationGeneralLogic.GetText("Menu_Tips"),
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_TeamMemberCanAddHint"),
						type = GeneralDialogContext.ButtonType.DoubleButton,
						buttonCallBack = OnMulModeHintDialogButtonClick
					});
					return;
				}
			}
			RequestToEnterLevel();
		}

		public bool OnStageBeginRsp(StageBeginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Invalid comparison between Unknown and I4
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Invalid comparison between Unknown and I4
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				_stageBeginRsp = rsp;
				if (_selectedHelper == null || _helperDetailData != null)
				{
					DoBeginLevel();
				}
			}
			else
			{
				ResetWaitPacketData();
				if ((int)rsp.retcode == 4)
				{
					Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo2);
				}
				else
				{
					GeneralDialogContext generalDialogContext = new GeneralDialogContext();
					generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
					generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
					GeneralDialogContext generalDialogContext2 = generalDialogContext;
					generalDialogContext2.desc = (((int)rsp.retcode != 3) ? LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode) : LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode, level.UnlockPlayerLevel));
					Singleton<MainUIManager>.Instance.ShowDialog(generalDialogContext2);
				}
			}
			return false;
		}

		private bool OnPlayerDetailRsp(GetPlayerDetailDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (_isWaitingLevelBegin)
				{
					_helperDetailData = new FriendDetailDataItem(rsp.detail);
					if (_stageBeginRsp != null)
					{
						DoBeginLevel();
					}
				}
				else if (_playerUidToShow == (int)rsp.detail.uid)
				{
					_playerUidToShow = -1;
					FriendDetailDataItem detailData = new FriendDetailDataItem(rsp.detail);
					return UIUtil.ShowFriendDetailInfo(detailData);
				}
			}
			else if (_isWaitingLevelBegin)
			{
				ResetWaitPacketData();
			}
			return false;
		}

		private void SetupMyTeam()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			InitTeam();
			Transform transform = base.view.transform.Find("TeamPanel/Team");
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			MonoSwitchTeammateAnimPlugin component = base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>();
			for (int i = 1; i <= 3; i++)
			{
				AvatarDataItem avatarData = ((i > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i - 1]));
				GameObject gameObject = transform.Find(i.ToString()).gameObject;
				MonoTeamMember component2 = gameObject.GetComponent<MonoTeamMember>();
				component2.SetupView(level.LevelType, i, component, avatarData, base.view.GetComponent<RectTransform>());
				component2.RegisterCallback(OnRefreshTeammateUI, StartSwitchAnim_Handler);
			}
			base.view.transform.Find("TeamPanel/MulModeHint").gameObject.SetActive(level.IsMultMode);
			if (level.IsMultMode)
			{
				base.view.transform.Find("TeamPanel/MulModeHint/Num").GetComponent<Text>().text = level.MinEnterAvatarNum.ToString();
			}
			SetupLevelTips();
		}

		private void InitTeam()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)level.LevelType != 1)
			{
				List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
				if (memberList.Count == 0)
				{
					List<int> memberList2 = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)1);
					Singleton<PlayerModule>.Instance.playerData.SetTeamMember(level.LevelType, memberList2);
					Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(level.LevelType);
				}
			}
		}

		private void SetupHelpers()
		{
			base.view.transform.Find("FriendsPanel/MulModeHint").gameObject.SetActive(level.IsMultMode);
			_showDataList = GetHelperList();
			base.view.transform.Find("FriendsPanel/FriendScrollView").GetComponent<MonoGridScroller>().Init(OnChange, _showDataList.Count);
		}

		private void SetupLevelTips()
		{
			int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			if (teamLevel < MiscData.Config.BasicConfig.MinPlayerPunishLevel)
			{
				base.view.transform.Find("LvTips").gameObject.SetActive(false);
				return;
			}
			int num = Mathf.Clamp(level.RecommandLv - teamLevel, 0, 10);
			int playerPunishLevelDifferenceStepOne = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepOne;
			int playerPunishLevelDifferenceStepTwo = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepTwo;
			int playerPunishLevelDifferenceStepThree = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepThree;
			if (num >= playerPunishLevelDifferenceStepOne && num < playerPunishLevelDifferenceStepTwo)
			{
				base.view.transform.Find("LvTips").gameObject.SetActive(false);
			}
			else if (num >= playerPunishLevelDifferenceStepTwo && num < playerPunishLevelDifferenceStepThree)
			{
				base.view.transform.Find("LvTips").gameObject.SetActive(true);
				base.view.transform.Find("LvTips").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipYellowBgColor");
				base.view.transform.Find("LvTips/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Level_Tips1");
				base.view.transform.Find("LvTips/Text").GetComponent<Text>().color = MiscData.GetColor("PrepareLevelPunishTipYellowTxtColor");
				base.view.transform.Find("LvTips/Arrow").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipYellowBgColor");
			}
			else
			{
				base.view.transform.Find("LvTips").gameObject.SetActive(true);
				base.view.transform.Find("LvTips").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipRedBgColor");
				base.view.transform.Find("LvTips/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Level_Tips2");
				base.view.transform.Find("LvTips/Text").GetComponent<Text>().color = MiscData.GetColor("PrepareLevelPunishTipRedTxtColor");
				base.view.transform.Find("LvTips/Arrow").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipRedBgColor");
			}
		}

		private void OnChange(Transform trans, int index)
		{
			bool selected = _showDataList[index] == _selectedHelper;
			trans.GetComponent<MonoHelperFrameRow>().SetupView(_showDataList[index], selected, OnFrameBtnClick, OnIconBtnClick);
		}

		private void SetupLeaderSkill()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			if (memberList.Count > 0)
			{
				int avatarID = memberList[0];
				AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID);
				AvatarSkillDataItem leaderSkill = avatarByID.GetLeaderSkill();
				if (leaderSkill.UnLocked)
				{
					base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(true);
					base.view.transform.Find("TeamPanel/Skills/Self/SkillName").GetComponent<Text>().text = leaderSkill.SkillName;
					base.view.transform.Find("TeamPanel/Skills/Self/Desc").GetComponent<Text>().text = leaderSkill.SkillShortInfo;
				}
				else
				{
					base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(false);
				}
			}
			else
			{
				base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(false);
			}
			SetupHelperSkill();
		}

		private void SetupHelperSkill()
		{
			Transform transform = base.view.transform.Find("TeamPanel/Skills/Friend");
			if (_selectedHelper == null || _selectedHelper.AvatarLeaderSkill == null)
			{
				transform.gameObject.SetActive(false);
				return;
			}
			AvatarSkillDataItem avatarLeaderSkill = _selectedHelper.AvatarLeaderSkill;
			if (!avatarLeaderSkill.UnLocked)
			{
				transform.gameObject.SetActive(false);
				return;
			}
			transform.gameObject.SetActive(true);
			bool flag = Singleton<FriendModule>.Instance.IsMyFriend(_selectedHelper.uid);
			Color color;
			if (flag)
			{
				UIUtil.TryParseHexString("#88c700FF", out color);
			}
			else
			{
				UIUtil.TryParseHexString("#a0a0a0FF", out color);
			}
			transform.Find("BG/Select").gameObject.SetActive(flag);
			transform.Find("BG/Unable").gameObject.SetActive(!flag);
			Text component = transform.Find("SkillName").GetComponent<Text>();
			component.text = avatarLeaderSkill.SkillName;
			component.color = color;
			transform.Find("Desc").GetComponent<Text>().text = avatarLeaderSkill.SkillShortInfo;
			string text = ((!flag) ? LocalizationGeneralLogic.GetText("LevelPreparePage_FriendSkillHint_NotFriend") : LocalizationGeneralLogic.GetText("LevelPreparePage_FriendSkillHint_IsFriend"));
			Text component2 = transform.Find("Hint").GetComponent<Text>();
			component2.text = text;
			component2.color = color;
			Image component3 = transform.Find("Line").GetComponent<Image>();
			component3.color = color;
		}

		private List<FriendBriefDataItem> GetHelperList()
		{
			List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
			FriendBriefDataItem oneStrangeHelper = Singleton<FriendModule>.Instance.GetOneStrangeHelper();
			List<FriendBriefDataItem> friendsList = Singleton<FriendModule>.Instance.friendsList;
			List<FriendBriefDataItem> list2 = new List<FriendBriefDataItem>();
			List<FriendBriefDataItem> list3 = new List<FriendBriefDataItem>();
			foreach (FriendBriefDataItem item in friendsList)
			{
				if (Singleton<FriendModule>.Instance.isHelperFrozen(item.uid))
				{
					list3.Add(item);
				}
				else
				{
					list2.Add(item);
				}
			}
			list2.Sort((FriendBriefDataItem o1, FriendBriefDataItem o2) => o2.level - o1.level);
			list3.Sort((FriendBriefDataItem o1, FriendBriefDataItem o2) => Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(o1.uid).CompareTo(Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(o2.uid)));
			if (oneStrangeHelper != null)
			{
				list.Add(oneStrangeHelper);
			}
			list.AddRange(list2);
			list.AddRange(list3);
			return list;
		}

		private bool IsFriendNumOver()
		{
			int count = Singleton<FriendModule>.Instance.friendsList.Count;
			int maxFriendFinal = Singleton<PlayerModule>.Instance.playerData.maxFriendFinal;
			return count > maxFriendFinal;
		}

		private void DoBeginLevel()
		{
			if (Singleton<LevelScoreManager>.Instance == null)
			{
				Singleton<LevelScoreManager>.Create();
			}
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			Transform transform = base.view.transform.Find("LevelDebugPanel");
			if (transform != null)
			{
				instance.isDebugDynamicLevel = transform.GetComponent<MonoLevelDebug>().useDynamicLevel;
			}
			instance.collectAntiCheatData = _stageBeginRsp.is_collect_cheat_data;
			instance.signKey = _stageBeginRsp.sign_key;
			int progress = (int)(_stageBeginRsp.progressSpecified ? _stageBeginRsp.progress : 0);
			LevelDataItem levelDataItem = ((!_stageBeginRsp.stage_idSpecified) ? level : Singleton<LevelModule>.Instance.GetLevelById((int)_stageBeginRsp.stage_id));
			instance.SetLevelBeginIntent(levelDataItem, progress, _stageBeginRsp.drop_item_list, level.BattleType, _helperDetailData);
			ResetWaitPacketData();
			Singleton<MainUIManager>.Instance.PopTopPageOnly();
			ChapterSelectPageContext chapterSelectPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext as ChapterSelectPageContext;
			if (chapterSelectPageContext != null)
			{
				chapterSelectPageContext.OnDoLevelBegin();
			}
			bool toKeepContextStack = Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas;
			Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", toKeepContextStack, true);
		}

		private void ResetWaitPacketData()
		{
			_isWaitingLevelBegin = false;
			_stageBeginRsp = null;
			_helperDetailData = null;
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
		}

		private void OnFrameBtnClick(FriendBriefDataItem friendBriefData)
		{
			bool flag = Singleton<FriendModule>.Instance.IsMyFriend(friendBriefData.uid);
			if (IsFriendNumOver() && flag)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new FriendNumOverDialogContext());
				return;
			}
			if (_selectedHelper != null && _selectedHelper.uid == friendBriefData.uid)
			{
				_selectedHelper = null;
			}
			else
			{
				_selectedHelper = friendBriefData;
			}
			SetupHelperSkill();
			base.view.transform.Find("FriendsPanel/FriendScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
		}

		private void OnIconBtnClick(FriendBriefDataItem friendBriefData)
		{
			FriendDetailDataItem friendDetailDataItem = Singleton<FriendModule>.Instance.TryGetFriendDetailData(friendBriefData.uid);
			if (friendDetailDataItem == null)
			{
				_playerUidToShow = friendBriefData.uid;
				Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(friendBriefData.uid);
			}
			else
			{
				UIUtil.ShowFriendDetailInfo(friendDetailDataItem);
			}
		}

		private void RequestToEnterLevel()
		{
			if (_loadingWheelDialogContext == null)
			{
				_loadingWheelDialogContext = new LoadingWheelWidgetContext
				{
					ignoreMaxWaitTime = true
				};
				Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
			}
			_isWaitingLevelBegin = true;
			int num = ((_selectedHelper != null) ? _selectedHelper.uid : 0);
			if (_selectedHelper != null)
			{
				_helperDetailData = Singleton<FriendModule>.Instance.TryGetFriendDetailData(_selectedHelper.uid);
				if (_helperDetailData == null)
				{
					Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(num);
				}
			}
			Singleton<NetworkManager>.Instance.RequestLevelBeginReq(level, num);
		}

		private void OnMulModeHintDialogButtonClick(bool isOk)
		{
			if (isOk)
			{
				RequestToEnterLevel();
			}
		}

		private void OnRefreshTeammateUI(int num, bool bSelfSkill)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = base.view.transform.Find("TeamPanel/Team");
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			AvatarDataItem avatarData = ((num > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[num - 1]));
			GameObject gameObject = transform.Find(num.ToString()).gameObject;
			MonoTeamMember component = gameObject.GetComponent<MonoTeamMember>();
			component.SetupView(level.LevelType, num, base.view.GetComponent<MonoSwitchTeammateAnimPlugin>(), avatarData, base.view.GetComponent<RectTransform>());
			if (bSelfSkill)
			{
				SetupLeaderSkill();
			}
		}

		private void StartSwitchAnim_Handler(int dataIndex, int fromIndex, int toIndex)
		{
			base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().StartSwitchAnim(dataIndex, fromIndex, toIndex);
		}
	}
}
