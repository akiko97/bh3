using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class EndlessPreparePageContext : BasePageContext
	{
		private const int MAX_TEAM_MEMBER_NUM = 3;

		private EndlessStageBeginRsp _stageBeginRsp;

		private EndlessGroupMetaData _groupMetaData;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		private bool _enterBattleDirrectly;

		public EndlessPreparePageContext(bool enterBattleDirrectly = false)
		{
			config = new ContextPattern
			{
				contextName = "EndlessPreparePageContext",
				viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessPreparePage"
			};
			_enterBattleDirrectly = enterBattleDirrectly;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 142:
				return OnStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());
			case 150:
				return OnGetEndlessAvatarHpRsp(pkt.getData<GetEndlessAvatarHpRsp>());
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
				SetupFightBtnInterable();
			}
			if (ntf.type == NotifyTypes.EndlessActivityEnd || ntf.type == NotifyTypes.EndlessActivitySettlement || ntf.type == NotifyTypes.EndlessActivityBegin)
			{
				Singleton<MainUIManager>.Instance.BackPage();
			}
			if (ntf.type == NotifyTypes.EndlessAppliedToolChange)
			{
				return SetupInfoPanel();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Action/Btn").GetComponent<Button>(), OnOkButtonCallBack);
		}

		protected override bool SetupView()
		{
			Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
			_groupMetaData = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
			SetupMyTeam();
			SetupLeaderSkill();
			SetupInfoPanel();
			SetupDropList();
			SetupFightBtnInterable();
			base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().RegisterCallback(OnRefreshTeammateUI);
			if (_enterBattleDirrectly)
			{
				RequestToEnterLevel();
			}
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			SetupMyTeam();
			SetupLeaderSkill();
			SetupInfoPanel();
			base.OnLandedFromBackPage();
		}

		public void OnOkButtonCallBack()
		{
			if (Singleton<EndlessModule>.Instance.SelfInvisible())
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
				{
					type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_InvisibleItemWillLoseEffectHint"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							RequestToEnterLevel();
						}
					}
				});
			}
			else
			{
				RequestToEnterLevel();
			}
		}

		public bool OnStageBeginRsp(EndlessStageBeginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				_stageBeginRsp = rsp;
				DoBeginLevel();
			}
			else
			{
				ResetWaitPacketData();
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				GeneralDialogContext generalDialogContext2 = generalDialogContext;
				generalDialogContext2.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(generalDialogContext2);
			}
			return false;
		}

		public bool OnGetEndlessAvatarHpRsp(GetEndlessAvatarHpRsp rsp)
		{
			SetupMyTeam();
			return false;
		}

		private void SetupMyTeam()
		{
			InitTeam();
			Transform transform = base.view.transform.Find("TeamPanel/Team");
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
			MonoSwitchTeammateAnimPlugin component = base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>();
			for (int i = 1; i <= 3; i++)
			{
				AvatarDataItem avatarData = ((i > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i - 1]));
				GameObject gameObject = transform.Find(i.ToString()).gameObject;
				MonoTeamMember component2 = gameObject.GetComponent<MonoTeamMember>();
				component2.SetupView((StageType)4, i, component, avatarData, base.view.GetComponent<RectTransform>());
				component2.RegisterCallback(OnRefreshTeammateUI, StartSwitchAnim_Handler);
			}
		}

		private void InitTeam()
		{
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
			HashSet<int> hashSet = new HashSet<int>(memberList);
			if (hashSet.Count == 0)
			{
				return;
			}
			HashSet<int> hashSet2 = new HashSet<int>();
			for (int i = 0; i < memberList.Count; i++)
			{
				int num = memberList[i];
				EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(num);
				if (endlessAvatarHPData.is_dieSpecified && endlessAvatarHPData.is_die)
				{
					hashSet2.Add(num);
				}
			}
			hashSet.ExceptWith(hashSet2);
			Singleton<PlayerModule>.Instance.playerData.SetTeamMember((StageType)4, new List<int>(hashSet));
			Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam((StageType)4);
		}

		private void SetupLeaderSkill()
		{
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
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
		}

		private bool SetupInfoPanel()
		{
			base.view.transform.Find("InfoPanel/Title/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_groupMetaData.groupName);
			base.view.transform.Find("InfoPanel/Title/GroupName").GetComponent<Text>().color = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[_groupMetaData.groupLevel]);
			base.view.transform.Find("InfoPanel/Title/FloorNum").GetComponent<Text>().text = (Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1).ToString();
			base.view.transform.Find("InfoPanel/ApplyToolsScrollView/").GetComponent<MonoGridScroller>().Init(OnScrollerChange, Singleton<EndlessModule>.Instance.GetAppliedToolDataList().Count);
			base.view.transform.Find("InfoPanel/Rank/Rank").GetComponent<Text>().text = Singleton<EndlessModule>.Instance.CurrentRank.ToString();
			return false;
		}

		private void SetupDropList()
		{
			string empty = string.Empty;
			int num = Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1;
			EndlessDropMetaData dropDataList = GetDropDataList(num);
			if (dropDataList == null)
			{
				base.view.transform.Find("InfoPanel/Drop").gameObject.SetActive(false);
				return;
			}
			List<int> firstDropDisplayList = dropDataList.firstDropDisplayList;
			List<int> dropDisplayList = dropDataList.dropDisplayList;
			List<int> list;
			if (Singleton<EndlessModule>.Instance.maxLevelEverReach < num && firstDropDisplayList != null)
			{
				list = firstDropDisplayList;
				empty = LocalizationGeneralLogic.GetText("Menu_FirstDropList");
			}
			else
			{
				list = dropDisplayList;
				empty = LocalizationGeneralLogic.GetText("Menu_DisplayDropList");
			}
			base.view.transform.Find("InfoPanel/Drop/Title/Text").GetComponent<Text>().text = empty;
			Transform transform = base.view.transform.Find("InfoPanel/Drop/Drops/ScollerContent");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				bool flag = i < list.Count;
				child.gameObject.SetActive(flag);
				if (flag)
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(list[i]);
					child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true);
				}
			}
		}

		private EndlessDropMetaData GetDropDataList(int level)
		{
			int currentGroupLevel = Singleton<EndlessModule>.Instance.currentGroupLevel;
			int num = Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1;
			List<EndlessDropMetaData> itemList = EndlessDropMetaDataReader.GetItemList();
			itemList.Sort((EndlessDropMetaData left, EndlessDropMetaData right) => (left.group != right.group) ? (left.group - right.group) : (left.level - right.level));
			for (int num2 = 0; num2 < itemList.Count - 1; num2++)
			{
				EndlessDropMetaData endlessDropMetaData = itemList[num2];
				EndlessDropMetaData endlessDropMetaData2 = itemList[num2 + 1];
				if (endlessDropMetaData.group >= currentGroupLevel)
				{
					if (endlessDropMetaData.group > currentGroupLevel)
					{
						return endlessDropMetaData;
					}
					if (endlessDropMetaData.level == num)
					{
						return endlessDropMetaData;
					}
					if (endlessDropMetaData.level < num && ((endlessDropMetaData2.group == currentGroupLevel && endlessDropMetaData2.level > num) || endlessDropMetaData2.group > currentGroupLevel))
					{
						return endlessDropMetaData;
					}
				}
			}
			return null;
		}

		private bool SetupFightBtnInterable()
		{
			bool flag = true;
			if (Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4).Count < 1)
			{
				flag = false;
			}
			if (TimeUtil.Now < Singleton<EndlessModule>.Instance.BeginTime || TimeUtil.Now >= Singleton<EndlessModule>.Instance.EndTime)
			{
				flag = false;
			}
			base.view.transform.Find("Action/Btn").GetComponent<Button>().interactable = flag;
			return flag;
		}

		private void DoBeginLevel()
		{
			Singleton<LevelScoreManager>.Create();
			int num = (int)((!_stageBeginRsp.progressSpecified) ? 1 : (_stageBeginRsp.progress + 1));
			int hardLevel = Mathf.FloorToInt(_groupMetaData.baseHardLevel + (float)(num - 1) * _groupMetaData.deltaHardLevel);
			List<string> list = new List<string>();
			foreach (uint item in _stageBeginRsp.effect_item_id_list)
			{
				EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem((int)item);
				if (endlessToolDataItem.ParamString != null)
				{
					list.Add(endlessToolDataItem.ParamString);
				}
			}
			Singleton<LevelScoreManager>.Instance.SetEndlessLevelBeginIntent(num, hardLevel, list, _stageBeginRsp, MiscData.Config.BasicConfig.EndlessInitTimer);
			ResetWaitPacketData();
			BackPage();
			Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true);
		}

		private void ResetWaitPacketData()
		{
			_stageBeginRsp = null;
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
		}

		private void RequestToEnterLevel()
		{
			_loadingWheelDialogContext = new LoadingWheelWidgetContext();
			_loadingWheelDialogContext.ignoreMaxWaitTime = true;
			Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
			Singleton<NetworkManager>.Instance.RequestEndlessLevelBeginReq();
		}

		private void OnRefreshTeammateUI(int num, bool bSelfSkill)
		{
			Transform transform = base.view.transform.Find("TeamPanel/Team");
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
			AvatarDataItem avatarData = ((num > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[num - 1]));
			GameObject gameObject = transform.Find(num.ToString()).gameObject;
			MonoTeamMember component = gameObject.GetComponent<MonoTeamMember>();
			component.SetupView((StageType)4, num, base.view.GetComponent<MonoSwitchTeammateAnimPlugin>(), avatarData, base.view.GetComponent<RectTransform>());
			if (bSelfSkill)
			{
				SetupLeaderSkill();
			}
		}

		private void StartSwitchAnim_Handler(int dataIndex, int fromIndex, int toIndex)
		{
			base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().StartSwitchAnim(dataIndex, fromIndex, toIndex);
		}

		private void SetActionButtonInteractable(bool interactable)
		{
			base.view.transform.Find("Action/Btn").GetComponent<Button>().interactable = interactable;
		}

		private void OnScrollerChange(Transform toolTrans, int index)
		{
			EndlessToolDataItem itemData = Singleton<EndlessModule>.Instance.GetAppliedToolDataList()[index];
			toolTrans.GetComponent<MonoAppliedEndlessItem>().SetupView(itemData);
		}
	}
}
