using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class AvatarOverviewPageContext : BasePageContext
	{
		public enum PageType
		{
			Show = 0,
			TeamEdit = 1,
			GalTouchReplace = 2
		}

		private const string STIGMATA_ICON_EMPTY_PATH = "SpriteOutput/StigmataSmallIcon/Icon_add";

		private List<AvatarDataItem> showAvatarList;

		public PageType type;

		public int selectedAvatarID;

		public bool showAvatarRemainHP;

		public StageType levelType;

		public int teamEditIndex;

		public VentureDataItem ventureData;

		public AvatarOverviewPageContext()
		{
			config = new ContextPattern
			{
				contextName = "AvatarOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/AvatarOverviewPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			showSpaceShip = true;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 25:
				return SetupView();
			case 30:
				return OnAvatarStarUpRsp(pkt.getData<AvatarStarUpRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SelectAvtarIconChange)
			{
				return UpdateSelectedAvatar((int)ntf.body);
			}
			if (ntf.type == NotifyTypes.UnlockAvatar)
			{
				int avatarID = (int)ntf.body;
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(avatarID));
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("3dPanel").GetComponent<Button>(), OnAvatar3DPanelClick);
			BindViewCallback(base.view.transform.Find("Info/Lock/Right/InfoRow_4/UnlockBtn").GetComponent<Button>(), OnAvatarUnlockBtnClick);
			BindViewCallback(base.view.transform.Find("ListPanel/NextBtn").GetComponent<Button>(), OnNextBtnClick);
			BindViewCallback(base.view.transform.Find("ListPanel/PrevBtn").GetComponent<Button>(), OnPrevBtnClick);
			BindViewCallback(base.view.transform.Find("Info/Unlock/Left/Info_2").GetComponent<Button>(), OnLvInfoClick);
			BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Weapon").GetComponent<Button>(), OnWeaponInfoClick);
			BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Stigmata").GetComponent<Button>(), OnStigmataInfoClick);
			BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<Button>(), OnSkillInfoClick);
			BindViewCallback(base.view.transform.Find("Info/Unlock/Left/Info_1").GetComponent<Button>(), OnAvatarInfoClick);
			BindViewCallback(base.view.transform.Find("Info/Lock/Left").GetComponent<Button>(), OnAvatarInfoClick);
		}

		protected override bool SetupView()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (Singleton<AvatarModule>.Instance.UserAvatarList.Count == 0)
			{
				return false;
			}
			SetupAvatarListPanel();
			if (selectedAvatarID == 0)
			{
				selectedAvatarID = showAvatarList[0].avatarID;
				if (type == PageType.TeamEdit)
				{
					List<int> memberIdList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(levelType);
					AvatarDataItem avatarDataItem = showAvatarList.Find((AvatarDataItem x) => x.UnLocked && !memberIdList.Contains(x.avatarID) && !Singleton<IslandModule>.Instance.IsAvatarDispatched(x.avatarID));
					if (avatarDataItem != null)
					{
						selectedAvatarID = avatarDataItem.avatarID;
					}
				}
			}
			SetupSelectedAvatarInfo();
			base.view.transform.Find("TeamEditBtn").gameObject.SetActive(PageType.TeamEdit == type);
			base.view.transform.Find("GalReplaceBtn").gameObject.SetActive(PageType.GalTouchReplace == type);
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			UIUtil.Create3DAvatarByPage(avatarByID, MiscData.PageInfoKey.AvatarOverviewPage);
			base.view.transform.Find("ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
		}

		public void OnAvatarUnlockBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestAvatarStarUp(selectedAvatarID);
		}

		public void OnNextBtnClick()
		{
			base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToNextPage();
		}

		public void OnPrevBtnClick()
		{
			base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToPrevPage();
		}

		public void OnInTeamBtnClick()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Singleton<PlayerModule>.Instance.playerData.SetTeamMember(levelType, teamEditIndex, selectedAvatarID);
			Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(levelType);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TeamMemberChanged));
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnOutTeamBtnClick()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Singleton<PlayerModule>.Instance.playerData.RemoveTeamMember(levelType, teamEditIndex);
			Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(levelType);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TeamMemberChanged));
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnInVentureDispatchBtnClick()
		{
			if (teamEditIndex > ventureData.selectedAvatarList.Count)
			{
				ventureData.selectedAvatarList.Add(selectedAvatarID);
			}
			else
			{
				ventureData.selectedAvatarList[teamEditIndex - 1] = selectedAvatarID;
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged));
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnOutVentureDispatchClick()
		{
			ventureData.selectedAvatarList.RemoveAt(teamEditIndex - 1);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged));
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnReplaceBtnClick()
		{
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			if (avatarByID != null && avatarByID.UnLocked)
			{
				PlayerUITempSaveData uiTempSaveData = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData;
				uiTempSaveData.lastSelectedAvatarID = selectedAvatarID;
				Singleton<GalTouchModule>.Instance.ChangeAvatar(selectedAvatarID);
				Singleton<MainUIManager>.Instance.BackPage();
			}
		}

		public void OnAvatar3DPanelClick()
		{
			if (type != PageType.GalTouchReplace)
			{
				ShowDetailWithTab("LvUpTab");
			}
		}

		public void OnLvInfoClick()
		{
			if (type != PageType.GalTouchReplace)
			{
				ShowDetailWithTab("LvUpTab");
			}
		}

		public void OnWeaponInfoClick()
		{
			if (type != PageType.GalTouchReplace)
			{
				ShowDetailWithTab("WeaponTab");
			}
		}

		public void OnStigmataInfoClick()
		{
			if (type != PageType.GalTouchReplace)
			{
				ShowDetailWithTab("StigmataTab");
			}
		}

		public void OnSkillInfoClick()
		{
			if (type != PageType.GalTouchReplace)
			{
				ShowDetailWithTab("SkillTab");
			}
		}

		public void OnAvatarInfoClick()
		{
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(avatarByID));
		}

		private void ShowDetailWithTab(string tabString)
		{
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			if (avatarByID.UnLocked)
			{
				Singleton<MainUIManager>.Instance.ShowPage(new AvatarDetailPageContext(avatarByID, tabString));
				UIUtil.SetAvatarTattooVisible(tabString == "StigmataTab", avatarByID);
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(avatarByID));
			}
		}

		public bool OnAvatarStarUpRsp(AvatarStarUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
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

		private bool UpdateSelectedAvatar(int avatarId)
		{
			selectedAvatarID = avatarId;
			SetupSelectedAvatarInfo();
			PostAvatarChangeAudioPattern(avatarId / 100);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PlayAvtarChangeEffect));
			return false;
		}

		private void SetupSelectedAvatarInfo()
		{
			base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			Transform transform = base.view.transform.Find("Info/Unlock");
			Transform transform2 = base.view.transform.Find("Info/Lock");
			CheckLockByMissionTutorial();
			if (avatarByID.UnLocked)
			{
				transform.gameObject.SetActive(true);
				transform2.gameObject.SetActive(false);
				SetupClassName(transform.Find("Left/Info_1/ClassName"), avatarByID);
				transform.Find("Left/Info_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
				transform.Find("Left/Info_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
				transform.Find("Left/Info_2/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarByID.star);
				transform.Find("Left/Info_2/LevelText").GetComponent<Text>().text = "LV." + avatarByID.level;
				transform.Find("Left/Info_2/Combat/NumText").GetComponent<Text>().text = Mathf.FloorToInt(avatarByID.CombatNum).ToString();
				transform.Find("Left/Info_2/PopUp").gameObject.SetActive(avatarByID.CanStarUp);
				WeaponDataItem weapon = avatarByID.GetWeapon();
				transform.Find("Right/Weapon/Name").GetComponent<Text>().text = ((weapon != null) ? weapon.GetDisplayTitle() : string.Empty);
				SetupStigmataSmallIcons(transform.Find("Right/Stigmata/Icons"), avatarByID);
				transform.Find("Right/Skill/SkillPoint/PointNum").GetComponent<Text>().text = "+" + avatarByID.GetSkillPointAddNum();
				if (avatarByID.LevelTutorialID != 0 && !Singleton<LevelTutorialModule>.Instance.IsTutorialIDFinish(avatarByID.LevelTutorialID))
				{
					Singleton<ApplicationManager>.Instance.StartCoroutine(ShowAvatarTutorialDialog(avatarByID));
				}
			}
			else
			{
				transform.gameObject.SetActive(false);
				transform2.gameObject.SetActive(true);
				SetupClassName(transform2.Find("Left/ClassName"), avatarByID);
				transform2.Find("Left/DescContent").GetComponent<Text>().text = avatarByID.Desc;
				transform2.Find("Right/InfoRow_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
				transform2.Find("Right/InfoRow_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
				string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[avatarByID.WeaponBaseTypeList[0]];
				transform2.Find("Right/InfoRow_2/WeaponTypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				transform2.Find("Right/InfoRow_3/Fragment/NumText").GetComponent<Text>().text = avatarByID.fragment + "/" + avatarByID.MaxFragment;
				transform2.Find("Right/InfoRow_3/Fragment/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(avatarByID.fragment, avatarByID.MaxFragment, 0f);
				transform2.Find("Right/InfoRow_4/UnlockBtn").GetComponent<Button>().interactable = avatarByID.CanStarUp;
				MonoButtonWwiseEvent monoButtonWwiseEvent = transform2.Find("Right/InfoRow_4/UnlockBtn").GetComponent<MonoButtonWwiseEvent>();
				if (monoButtonWwiseEvent == null)
				{
					monoButtonWwiseEvent = transform2.Find("Right/InfoRow_4/UnlockBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
				}
				monoButtonWwiseEvent.eventName = ((!avatarByID.CanStarUp) ? "UI_Gen_Select_Negative" : "UI_Click");
				transform2.Find("Right/InfoRow_4/UnlockBtn/PopUp").gameObject.SetActive(avatarByID.CanStarUp);
			}
			UIUtil.Create3DAvatarByPage(avatarByID, MiscData.PageInfoKey.AvatarOverviewPage);
			if (type == PageType.TeamEdit)
			{
				SetupTeamEdit();
			}
			else if (type == PageType.Show)
			{
				SetUITempSaveData();
			}
			else if (type == PageType.GalTouchReplace)
			{
				SetupGalReplace();
			}
		}

		private void CheckLockByMissionTutorial()
		{
			bool flag = UnlockUIDataReaderExtend.UnLockByMission(1) && UnlockUIDataReaderExtend.UnlockByTutorial(1);
			base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<Button>().interactable = flag;
			base.view.transform.Find("Info/Unlock/Right/Skill/Lock").gameObject.SetActive(!flag);
			base.view.transform.Find("Info/Unlock/Right/Skill/SkillPoint/PointNum").gameObject.SetActive(flag);
			base.view.transform.Find("Info/Unlock/Right/Skill/SkillPoint/SkillPtLabel").gameObject.SetActive(flag);
			MonoButtonWwiseEvent monoButtonWwiseEvent = base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<MonoButtonWwiseEvent>();
			if (monoButtonWwiseEvent == null)
			{
				monoButtonWwiseEvent = base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.AddComponent<MonoButtonWwiseEvent>();
			}
			monoButtonWwiseEvent.eventName = ((!flag) ? "UI_Gen_Select_Negative" : "UI_Click");
		}

		private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
		{
			parent.Find("Dot").gameObject.SetActive(!avatarSelected.IsEasterner());
			if (avatarSelected.IsEasterner())
			{
				parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassLastName;
				parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
				parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
				parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
			}
			else
			{
				parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
				parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
				parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
				parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
			}
		}

		private void SetupStigmataSmallIcons(Transform parent, AvatarDataItem avatarSelected)
		{
			List<StigmataDataItem> stigmataList = avatarSelected.GetStigmataList();
			for (int i = 1; i <= stigmataList.Count; i++)
			{
				Transform transform = parent.Find(i.ToString());
				if (stigmataList[i - 1] != null)
				{
					transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(stigmataList[i - 1].GetSmallIconPath());
				}
				else
				{
					transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StigmataSmallIcon/Icon_add");
				}
			}
		}

		private void SetupAvatarListPanel()
		{
			if (type == PageType.TeamEdit)
			{
				showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll((AvatarDataItem x) => x.UnLocked && !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId));
			}
			else
			{
				showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll((AvatarDataItem x) => !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId));
			}
			showAvatarList.Sort(CompareByDefault);
			base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().Init(OnChange, showAvatarList.Count);
			if (type == PageType.Show)
			{
				ApplyTempSaveData();
			}
			base.view.transform.Find("ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
		}

		private void OnChange(Transform trans, int index)
		{
			bool isSelected = showAvatarList[index].avatarID == selectedAvatarID;
			AvatarDataItem avatarDataItem = showAvatarList[index];
			trans.GetComponent<MonoAvatarIcon>().SetupView(avatarDataItem, isSelected, (!showAvatarRemainHP) ? null : Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarDataItem.avatarID));
		}

		private void SetupTeamEdit()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Invalid comparison between Unknown and I4
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(levelType);
			bool flag = selectedAvatarID != 0 && memberList.Contains(selectedAvatarID);
			Button component = base.view.transform.Find("TeamEditBtn").GetComponent<Button>();
			component.gameObject.SetActive(true);
			if (teamEditIndex <= memberList.Count && selectedAvatarID == memberList[teamEditIndex - 1])
			{
				BindViewCallback(component, OnOutTeamBtnClick);
				component.interactable = memberList.Count > 1;
				string textID = "Menu_GetOutTeam";
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			}
			else
			{
				BindViewCallback(component, OnInTeamBtnClick);
				component.interactable = !flag;
				string textID2 = ((!flag) ? "Menu_EnterTeam" : "Menu_AlreadyInTeam");
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID2);
			}
			if ((int)levelType == 4 && Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(selectedAvatarID).is_die)
			{
				component.interactable = false;
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessEnergyUseUp");
			}
			else if (!flag && Singleton<IslandModule>.Instance.IsAvatarDispatched(selectedAvatarID))
			{
				component.interactable = false;
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarAlreadyDispatched");
			}
			base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(true);
			base.view.transform.Find("Info/RightEdge").gameObject.SetActive(true);
			base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(true);
		}

		private void SetupGalReplace()
		{
			Button component = base.view.transform.Find("GalReplaceBtn").GetComponent<Button>();
			BindViewCallback(component, OnReplaceBtnClick);
			component.interactable = selectedAvatarID != Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			if (avatarByID != null)
			{
				component.interactable = component.interactable && avatarByID.UnLocked;
			}
			base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(false);
			base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(false);
			base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(false);
			base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(false);
			base.view.transform.Find("Info/RightEdge").gameObject.SetActive(false);
			base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(false);
		}

		private IEnumerator ShowAvatarTutorialDialog(AvatarDataItem avatar)
		{
			Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(avatar.LevelTutorialID);
			yield return null;
			AvatarDataItem avatar2 = default(AvatarDataItem);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
			{
				type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarTutorial", avatar.ShortName),
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						LevelTutorialMetaData levelTutorialMetaDataByKey = LevelTutorialMetaDataReader.GetLevelTutorialMetaDataByKey(avatar2.LevelTutorialID);
						Singleton<LevelScoreManager>.Create();
						Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(avatar2.avatarID, levelTutorialMetaDataByKey.lua);
						Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true);
					}
				}
			});
		}

		private int CompareByDefault(AvatarDataItem lemb, AvatarDataItem remb)
		{
			if (!lemb.UnLocked && remb.UnLocked)
			{
				return 1;
			}
			if (!remb.UnLocked && lemb.UnLocked)
			{
				return -1;
			}
			return (!lemb.UnLocked) ? CompareByFragment(lemb, remb) : CompareByStar(lemb, remb);
		}

		private int CompareByFragment(AvatarDataItem lemb, AvatarDataItem remb)
		{
			int num = -(lemb.fragment - remb.fragment);
			if (num != 0)
			{
				return num;
			}
			return CompareByID(lemb, remb);
		}

		private int CompareByStar(AvatarDataItem lemb, AvatarDataItem remb)
		{
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)1);
			int num = memberList.IndexOf(lemb.avatarID);
			int num2 = memberList.IndexOf(remb.avatarID);
			if (num == -1 && num2 >= 0)
			{
				return 1;
			}
			if (num2 == -1 && num >= 0)
			{
				return -1;
			}
			if (num >= 0 && num2 >= 0)
			{
				return num - num2;
			}
			int num3 = -(lemb.star - remb.star);
			if (num3 != 0)
			{
				return num3;
			}
			return CompareByID(lemb, remb);
		}

		private int CompareByID(AvatarDataItem lemb, AvatarDataItem remb)
		{
			return lemb.avatarID - remb.avatarID;
		}

		private void PostAvatarChangeAudioPattern(int type)
		{
			string text = null;
			switch (type)
			{
			case 1:
				text = "VO_M_Kia_04_Selected";
				break;
			case 2:
				text = "VO_M_Mei_04_Selected";
				break;
			case 3:
				text = "VO_M_Bro_04_Selected";
				break;
			}
			if (text != null)
			{
				Singleton<WwiseAudioManager>.Instance.Post(text);
			}
		}

		private void SetUITempSaveData()
		{
			PlayerUITempSaveData uiTempSaveData = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData;
			uiTempSaveData.lastSelectedAvatarID = selectedAvatarID;
			uiTempSaveData.avatarOverviewPageScrollerPos = base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().GetNormalizedPosition();
			base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(true);
			base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(true);
			base.view.transform.Find("Info/RightEdge").gameObject.SetActive(true);
			base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(true);
		}

		private void ApplyTempSaveData()
		{
			PlayerUITempSaveData uiTempSaveData = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData;
			if (uiTempSaveData.lastSelectedAvatarID != 0)
			{
				base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().SetNormalizedPosition(uiTempSaveData.avatarOverviewPageScrollerPos);
			}
		}
	}
}
