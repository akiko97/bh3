using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class EndlessMainPageContext : BasePageContext
	{
		public enum ViewStatus
		{
			ShowCurrentGroup = 0,
			ShowTopGroup = 1
		}

		private const string BATTLE_REPORT_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/BattleReportRow";

		private Color _groupColor;

		private ViewStatus _viewStatus;

		public EndlessMainPageContext()
		{
			config = new ContextPattern
			{
				contextName = "EndlessMainPageContext",
				viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessMainPage"
			};
			_viewStatus = ViewStatus.ShowCurrentGroup;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 140:
			case 220:
				return SetupView();
			case 151:
				return OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());
			case 146:
				return OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());
			case 153:
				return OnEndlessWarInfoNotify(pkt.getData<EndlessWarInfoNotify>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.EndlessActivityEnd)
			{
				return OnEndlessEnd();
			}
			if (ntf.type == NotifyTypes.EndlessActivityEnd)
			{
				return OnEndlessSettlement();
			}
			if (ntf.type == NotifyTypes.EndlessActivityEnd)
			{
				return OnEndlessBegin();
			}
			if (ntf.type == NotifyTypes.EndlessAppliedToolChange)
			{
				return SetupRank();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/Prepare").GetComponent<Button>(), OnPrepareBtnClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/Tool").GetComponent<Button>(), OnUseToolBtnClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/Shop").GetComponent<Button>(), OnShopBtnClick);
			BindViewCallback(base.view.transform.Find("GroupPanel/InfoBtn").GetComponent<Button>(), OnInfoBtnClick);
		}

		protected override bool SetupView()
		{
			_groupColor = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[(int)Singleton<EndlessModule>.Instance.endlessData.group_level]);
			SetupGroupList();
			SetupRank();
			SetupWarReportList();
			SetAllButtonsInteractable(Singleton<EndlessModule>.Instance.GetEndlessActivityStatus() == EndlessActivityStatus.InProgress);
			CheckEndlessReward();
			CheckIfBombBurst();
			Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
			Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			CheckCurrentEndlessDataValid();
			CheckEndlessReward();
			Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
		}

		private void OnPrepareBtnClick()
		{
			Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
			Singleton<MainUIManager>.Instance.ShowPage(new EndlessPreparePageContext());
		}

		private void OnUseToolBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new EndlessUseToolPageContext());
		}

		private void OnInfoBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new EndlessInfoDialogContext());
		}

		private void OnShopBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext(UIShopType.SHOP_ENDLESS));
		}

		private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
		{
			CheckIfBombBurst();
			SetupRank();
			return false;
		}

		private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.reward_list.Count > 0)
			{
				ShowActivityRewardDialog(rsp);
			}
			return false;
		}

		private bool OnEndlessWarInfoNotify(EndlessWarInfoNotify rsp)
		{
			InsertNewWarReport(rsp.war_info);
			return false;
		}

		private bool OnEndlessEnd()
		{
			SetAllButtonsInteractable(false);
			return false;
		}

		private bool OnEndlessSettlement()
		{
			Singleton<NetworkManager>.Instance.RequestEndlessData();
			Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(140, BackPage));
			return false;
		}

		private bool OnEndlessBegin()
		{
			SetAllButtonsInteractable(true);
			return false;
		}

		private void SetupGroupList()
		{
			EndlessModule instance = Singleton<EndlessModule>.Instance;
			int currentGroupLevel = instance.currentGroupLevel;
			List<EndlessGroupMetaData> itemList = EndlessGroupMetaDataReader.GetItemList();
			Transform transform = base.view.transform.Find("GroupPanel/GroupListPanel/GroupList");
			int cur_top_group_level = (int)instance.endlessData.cur_top_group_level;
			bool flag = instance.endlessData.top_group_player_numSpecified && instance.endlessData.top_group_promote_unlock_player_numSpecified;
			float num = instance.endlessData.top_group_player_num;
			float num2 = instance.endlessData.top_group_promote_unlock_player_num;
			bool flag2 = instance.CanSeeTopGroupInfo();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= itemList.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				EndlessGroupMetaData endlessGroupMetaData = itemList[i];
				bool flag3 = currentGroupLevel == endlessGroupMetaData.groupLevel;
				child.Find("CurrentGroup").gameObject.SetActive((_viewStatus != ViewStatus.ShowCurrentGroup) ? (endlessGroupMetaData.groupLevel == itemList.Count) : flag3);
				string prefabPath = ((endlessGroupMetaData.groupLevel > cur_top_group_level) ? MiscData.Config.EndlessGroupUnopenPrefabPath[endlessGroupMetaData.groupLevel] : ((endlessGroupMetaData.groupLevel != currentGroupLevel) ? MiscData.Config.EndlessGroupUnselectPrefabPath[endlessGroupMetaData.groupLevel] : MiscData.Config.EndlessGroupSelectPrefabPath[endlessGroupMetaData.groupLevel]));
				child.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				if (flag2)
				{
					Button component = child.Find("Button").GetComponent<Button>();
					if (flag3)
					{
						component.gameObject.SetActive(true);
						BindViewCallback(component, OnCurrentButtonClick);
					}
					else if (endlessGroupMetaData.groupLevel == instance.TopGroupLevel)
					{
						child.Find("Button").GetComponent<Button>().gameObject.SetActive(true);
						child.Find("Particle").GetComponent<ParticleSystem>().gameObject.SetActive(true);
						BindViewCallback(component, OnTopGroupButtonClick);
					}
				}
				else if (endlessGroupMetaData.groupLevel == cur_top_group_level + 1)
				{
					child.Find("Button").GetComponent<Button>().gameObject.SetActive(true);
					BindViewCallback(child.Find("Button").GetComponent<Button>(), ShowNextGroupHint);
				}
				else
				{
					child.Find("Button").GetComponent<Button>().gameObject.SetActive(false);
				}
				bool flag4 = endlessGroupMetaData.groupLevel == cur_top_group_level + 1;
				child.Find("Slider").gameObject.SetActive(flag4 && flag);
				child.Find("Slider/Fill").GetComponent<Image>().fillAmount = num / num2;
			}
			if (instance.CanRequestTopGroupInfo())
			{
				Singleton<NetworkManager>.Instance.RequestGetEndlessTopGroup();
			}
		}

		private bool SetupRank()
		{
			EndlessModule instance = Singleton<EndlessModule>.Instance;
			int num = (int)instance.endlessData.group_level;
			if (_viewStatus == ViewStatus.ShowTopGroup)
			{
				num = instance.TopGroupLevel;
			}
			base.view.transform.gameObject.GetComponent<Animator>().SetBool("IsInTopGroup", num == instance.TopGroupLevel);
			EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(num);
			base.view.transform.Find("GroupPanel/RankPanel/GroupName/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(endlessGroupMetaDataByKey.groupName);
			base.view.transform.Find("GroupPanel/RankPanel/GroupName/GroupName").GetComponent<Text>().color = _groupColor;
			base.view.transform.Find("GroupPanel/RankPanel/BracketTop/Line").GetComponent<Image>().color = _groupColor;
			base.view.transform.Find("GroupPanel/RankPanel/BracketTop/LineCur").GetComponent<Image>().color = _groupColor;
			base.view.transform.Find("GroupPanel/RankPanel/BracketBottom/Line").GetComponent<Image>().color = _groupColor;
			base.view.transform.Find("GroupPanel/RankPanel/BracketBottom/LineCur").GetComponent<Image>().color = _groupColor;
			int num2 = 0;
			int num3 = 0;
			Transform transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/PromoteRank");
			if (endlessGroupMetaDataByKey.promoteRank == 0)
			{
				transform.gameObject.SetActive(false);
			}
			else
			{
				transform.gameObject.SetActive(true);
				List<int> promoteRank = instance.GetPromoteRank(_viewStatus);
				transform.Find("RankRewardRow/Title").GetComponent<Text>().text = ((instance.currentGroupLevel != (int)instance.endlessData.cur_top_group_level) ? LocalizationGeneralLogic.GetText("Menu_Title_EndlessPromoteArea") : LocalizationGeneralLogic.GetText("Menu_Title_EndlessStayArea"));
				num3 = promoteRank.Count;
				transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.prototeRewardID, promoteRank, _viewStatus);
				num2 += num3;
			}
			transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/NormalRank");
			List<int> normalRank = instance.GetNormalRank(_viewStatus);
			num3 = normalRank.Count;
			transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.normalRewardID, normalRank, _viewStatus);
			num2 += num3;
			transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/DemoteRank");
			if (endlessGroupMetaDataByKey.groupLevel > 1)
			{
				transform.gameObject.SetActive(true);
				List<int> demoteRank = instance.GetDemoteRank(_viewStatus);
				num3 = demoteRank.Count;
				transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.demoteRewardID, demoteRank, _viewStatus);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
			return false;
		}

		private void SetupWarReportList()
		{
			Transform transform = base.view.transform.Find("BattlerReport/ReportList/Content");
			transform.DestroyChildren();
			List<EndlessWarInfo> list = ((_viewStatus != ViewStatus.ShowCurrentGroup) ? Singleton<EndlessModule>.Instance.topGroupData.war_info_list : new List<EndlessWarInfo>(Singleton<EndlessModule>.Instance.warInfoList));
			for (int i = 0; i < list.Count && i < 20; i++)
			{
				EndlessWarInfo battleInfo = list[i];
				Transform transform2 = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/BattleReportRow")).transform;
				transform2.GetComponent<MonoBattleReportRow>().SetupView(battleInfo, _viewStatus);
				transform2.SetParent(transform, false);
			}
			base.view.transform.Find("BattlerReport/ReportList").GetComponent<MonoReportList>().Init();
		}

		private void InsertNewWarReport(EndlessWarInfo warInfo)
		{
			if (_viewStatus != ViewStatus.ShowTopGroup)
			{
				Transform parent = base.view.transform.Find("BattlerReport/ReportList/Content");
				Transform transform = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/BattleReportRow")).transform;
				transform.GetComponent<MonoBattleReportRow>().SetupView(warInfo, ViewStatus.ShowCurrentGroup);
				transform.SetParent(parent, false);
				transform.SetAsFirstSibling();
				base.view.transform.Find("BattlerReport/ReportList").GetComponent<MonoReportList>().Init();
			}
		}

		private void ShowActivityRewardDialog(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.reward_list.Count > 0)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new EndlessSettlementDialogContext(rsp));
				}
				Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = null;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		private void SetAllButtonsInteractable(bool interactable)
		{
			if (_viewStatus == ViewStatus.ShowTopGroup)
			{
				base.view.transform.Find("ActionBtns").gameObject.SetActive(false);
				return;
			}
			base.view.transform.Find("ActionBtns").gameObject.SetActive(true);
			Button[] componentsInChildren = base.view.transform.Find("ActionBtns").GetComponentsInChildren<Button>();
			foreach (Button button in componentsInChildren)
			{
				if (_viewStatus == ViewStatus.ShowCurrentGroup)
				{
					button.interactable = interactable;
				}
			}
		}

		private void CheckEndlessReward()
		{
			if (Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData != null)
			{
				ShowActivityRewardDialog(Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData);
			}
		}

		private bool CheckIfBombBurst()
		{
			EndlessToolDataItem justBurstBombData = Singleton<EndlessModule>.Instance.justBurstBombData;
			if (justBurstBombData != null)
			{
				Singleton<MainUIManager>.Instance.ShowUIEffect(config.contextName, justBurstBombData.EffectPrefatPath);
				string text = LocalizationGeneralLogic.GetText("Menu_Desc_ExplodedByOther", justBurstBombData.GetDisplayTitle());
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text));
				Singleton<EndlessModule>.Instance.justBurstBombData = null;
			}
			return false;
		}

		private void CheckCurrentEndlessDataValid()
		{
			if (TimeUtil.Now > Singleton<EndlessModule>.Instance.SettlementTime)
			{
				Singleton<NetworkManager>.Instance.RequestEndlessData();
			}
		}

		private void ShowNextGroupHint()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_EndlessNextGroupOpenHint")));
		}

		private void OnTopGroupButtonClick()
		{
			_viewStatus = ViewStatus.ShowTopGroup;
			SetupView();
		}

		private void OnCurrentButtonClick()
		{
			_viewStatus = ViewStatus.ShowCurrentGroup;
			SetupView();
		}
	}
}
