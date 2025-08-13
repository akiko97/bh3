using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class StorageItemDetailPageContext : BasePageContext
	{
		public const string SIGMATA_SKILL_TAB = "S_Skill_Tab";

		public const string SIGMATA_SET_SKILL_TAB = "S_Suit_Skill_Tab";

		public const string SIGMATA_AFFIX_SKILL_TAB = "S_Affix_Skill_Tab";

		public StorageDataItemBase storageItem;

		public readonly bool hideActionBtns;

		public bool showIdentifyBtnOnly;

		public readonly bool unlock;

		private TabManager _stigmataTabManager;

		public AvatarDataItem uiEquipOwner;

		public StorageItemDetailPageContext(StorageDataItemBase storageItem, bool hideActionBtns = false, bool unlock = true)
		{
			config = new ContextPattern
			{
				contextName = "StorageItemDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/Storage/WeaponDetailPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			if (storageItem is StigmataDataItem)
			{
				config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataDetailPage";
			}
			this.storageItem = storageItem;
			this.hideActionBtns = hideActionBtns;
			this.unlock = unlock;
			_stigmataTabManager = new TabManager();
			_stigmataTabManager.onSetActive += OnTabSetActive;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.EquipPowerupOrEvo)
			{
				SetupItemData((StorageDataItemBase)ntf.body);
			}
			else if (ntf.type == NotifyTypes.RefreshStigmataDetailView)
			{
				SetupView();
				_stigmataTabManager.ShowTab("S_Affix_Skill_Tab");
				TryToDoTutorial();
			}
			else if (ntf.type == NotifyTypes.StigmataNewAffix)
			{
				SetupView();
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 75:
				SetupItemProtectedStatus();
				break;
			case 192:
				return OnIdentifyStigmataAffixRsp(pkt.getData<IdentifyStigmataAffixRsp>());
			case 40:
			case 136:
				return SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>(), OnPowerUpButtonClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>(), OnUpRarityButtonClick);
			if (storageItem is StigmataDataItem)
			{
				BindViewCallback(base.view.transform.Find("Info/Content/LockButton").GetComponent<Button>(), OnLockButtonClick);
				BindViewCallback(base.view.transform.Find("Info/IdentifyBtn").GetComponent<Button>(), OnIdentifyBtnClick);
				BindViewCallback(base.view.transform.Find("Info/Content/BtnInEquip").GetComponent<Button>(), OnOwnerAvatarBtnClick);
			}
			else
			{
				BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/LockButton").GetComponent<Button>(), OnLockButtonClick);
				BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip").GetComponent<Button>(), OnOwnerAvatarBtnClick);
			}
			if (storageItem is StigmataDataItem)
			{
				BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_1").GetComponent<Button>(), OnNaturalSkillTabButtonClick);
				BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_2").GetComponent<Button>(), OnSuitSkillTabButtonClick);
				BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_3").GetComponent<Button>(), OnAffixSkillTabButtonClick);
				BindViewCallback(base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>(), OnNewAffixBtnClick);
			}
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().interactable = storageItem.level < storageItem.GetMaxLevel();
			base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>().interactable = storageItem.GetEvoStorageItem() != null;
			SetupInfoPanel();
			SetupAttributesPanel();
			SetupSkillsPanel();
			SetupLvInfoPanel();
			Transform transform = ((!(storageItem is StigmataDataItem)) ? base.view.transform.Find("Info/Info/Content/Equipment/LockButton") : base.view.transform.Find("Info/Content/LockButton"));
			transform.gameObject.SetActive(!hideActionBtns);
			LetterSpacing[] componentsInChildren = base.view.transform.GetComponentsInChildren<LetterSpacing>();
			foreach (LetterSpacing letterSpacing in componentsInChildren)
			{
				if (letterSpacing.autoFixLine)
				{
					letterSpacing.AccommodateText();
				}
			}
			base.view.transform.Find("ActionBtns").gameObject.SetActive(!hideActionBtns);
			return false;
		}

		public void SetupItemData(StorageDataItemBase itemData)
		{
			storageItem = itemData;
			SetupView();
		}

		public override void Destroy()
		{
			if (storageItem != null && storageItem is WeaponDataItem && base.view != null)
			{
				Transform transform = base.view.transform.Find("Info/Info/Content/Equipment/3dModel");
				transform.GetComponent<MonoWeaponRenderImage>().CleanUp();
			}
			base.Destroy();
		}

		public bool OnIdentifyStigmataAffixRsp(IdentifyStigmataAffixRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				base.view.transform.Find("Info/IdentifyBtn").GetComponent<DragObject>().OnIdentifyStigmataAffixSucc();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)));
			}
			return false;
		}

		public void OnPowerUpButtonClick()
		{
			if (storageItem.level < storageItem.GetMaxLevel())
			{
				Singleton<MainUIManager>.Instance.ShowPage(new StoragePowerUpPageContext(storageItem)
				{
					uiEquipOwner = uiEquipOwner
				});
			}
		}

		public void OnUpRarityButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StorageEvoPageContext(storageItem)
			{
				uiEquipOwner = uiEquipOwner
			});
		}

		public void OnNewAffixBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StigmataNewAffixPageContext(storageItem as StigmataDataItem));
		}

		public void OnLockButtonClick()
		{
			Singleton<NetworkManager>.Instance.RequestChangeEquipmentProtectdStatus(storageItem);
		}

		public void OnIdentifyBtnClick()
		{
		}

		public void OnSellButtonClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new StorageItemSellDialogContext
			{
				storageDataItem = storageItem
			});
		}

		public void OnCloseBtnClick()
		{
			ShowStigmataInfo(false);
		}

		public void OnOpenBtnClick()
		{
			ShowStigmataInfo(true);
		}

		public void OnNaturalSkillTabButtonClick()
		{
			_stigmataTabManager.ShowTab("S_Skill_Tab");
		}

		public void OnSuitSkillTabButtonClick()
		{
			_stigmataTabManager.ShowTab("S_Suit_Skill_Tab");
		}

		public void OnAffixSkillTabButtonClick()
		{
			_stigmataTabManager.ShowTab("S_Affix_Skill_Tab");
		}

		public void OnOwnerAvatarBtnClick()
		{
			AvatarDataItem avatarData = Singleton<AvatarModule>.Instance.TryGetAvatarByID(storageItem.avatarID);
			string defaultTab = "WeaponTab";
			if (storageItem is StigmataDataItem)
			{
				defaultTab = "StigmataTab";
			}
			Singleton<MainUIManager>.Instance.ShowPage(new AvatarDetailPageContext(avatarData, defaultTab));
		}

		private void SetupInfoPanel()
		{
			if (storageItem is WeaponDataItem)
			{
				SetupWeapon();
			}
			else if (storageItem is StigmataDataItem)
			{
				SetupStigmata();
			}
		}

		private void SetupWeapon()
		{
			string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[storageItem.GetBaseType()];
			base.view.transform.Find("Info/Info/Title/Equipment/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
			base.view.transform.Find("Info/Info/Title/Equipment/Name").GetComponent<Text>().text = storageItem.GetDisplayTitle();
			base.view.transform.Find("Info/Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = storageItem.GetCost().ToString();
			MonoEquipSubStar component = base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(storageItem.rarity, storageItem.GetMaxRarity());
			MonoEquipSubStar component2 = base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(storageItem.GetSubRarity(), storageItem.GetMaxSubRarity() - 1);
			base.view.transform.Find("Info/Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(storageItem as WeaponDataItem);
			SetupItemProtectedStatus();
			base.view.transform.Find("Desc/Content/Text").GetComponent<Text>().text = storageItem.GetDescription();
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(storageItem.avatarID);
			base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip").gameObject.SetActive(avatarDataItem != null);
			if (avatarDataItem != null)
			{
				base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip/EquipChara").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarDataItem.IconPath);
			}
		}

		private void SetupStigmata()
		{
			StigmataDataItem stigmataDataItem = storageItem as StigmataDataItem;
			base.view.transform.Find("Info/Content/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(stigmataDataItem.GetSmallIconPath());
			string text = storageItem.GetDisplayTitle();
			if (stigmataDataItem.IsAffixIdentify)
			{
				string affixName = stigmataDataItem.GetAffixName();
				if (!string.IsNullOrEmpty(affixName))
				{
					text = MiscData.AddColor("Blue", affixName) + " " + text;
				}
			}
			else
			{
				text = MiscData.AddColor("WarningRed", stigmataDataItem.GetAffixName()) + " " + text;
			}
			base.view.transform.Find("Info/Content/Equipment/Title/Name").GetComponent<Text>().text = text;
			base.view.transform.Find("Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = storageItem.GetCost().ToString();
			SetupItemProtectedStatus();
			MonoEquipSubStar component = base.view.transform.Find("Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(storageItem.rarity, storageItem.GetMaxRarity());
			MonoEquipSubStar component2 = base.view.transform.Find("Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(storageItem.GetSubRarity(), storageItem.GetMaxSubRarity() - 1);
			base.view.transform.Find("Info/Figure").GetComponent<MonoStigmataFigure>().SetupViewWithIdentifyStatus(stigmataDataItem, hideActionBtns && !unlock);
			if (stigmataDataItem.IsAffixIdentify)
			{
				bool flag = stigmataDataItem.CanRefine && Singleton<TutorialModule>.Instance != null && UnlockUIDataReaderExtend.UnlockByTutorial(5);
				base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>().interactable = flag;
				bool flag2 = Singleton<StorageModule>.Instance.GetStigmatasCanUseForNewAffix(stigmataDataItem).Count > 0;
				base.view.transform.Find("ActionBtns/NewAffixBtn/PopUp").gameObject.SetActive(flag2 && flag);
			}
			else
			{
				base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().interactable = false;
				base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>().interactable = false;
				base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>().interactable = false;
				base.view.transform.Find("ActionBtns/NewAffixBtn/PopUp").gameObject.SetActive(false);
			}
			base.view.transform.Find("Info/Figure/InfoMark").gameObject.SetActive(stigmataDataItem.IsAffixIdentify && unlock);
			base.view.transform.Find("Info/IdentifyBtn").gameObject.SetActive((!hideActionBtns || showIdentifyBtnOnly) && !stigmataDataItem.IsAffixIdentify);
			base.view.transform.Find("Info/IdentifyBtn").GetComponent<DragObject>().Init(storageItem);
			base.view.transform.Find("Info/Content/Equipment/Desc").GetComponent<Text>().text = storageItem.GetDescription();
			base.view.transform.Find("Info/Content").gameObject.SetActive(true);
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(storageItem.avatarID);
			base.view.transform.Find("Info/Content/BtnInEquip").gameObject.SetActive(avatarDataItem != null);
			if (avatarDataItem != null)
			{
				base.view.transform.Find("Info/Content/BtnInEquip/EquipChara").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarDataItem.IconPath);
			}
		}

		private void ShowStigmataInfo(bool active)
		{
			base.view.transform.Find("Info/Content").gameObject.SetActive(active);
			base.view.transform.Find("Info/CloseBtn").gameObject.SetActive(active);
			base.view.transform.Find("Info/OpenBtn").gameObject.SetActive(!active);
		}

		private void SetupItemProtectedStatus()
		{
			Transform transform = ((!(storageItem is StigmataDataItem)) ? base.view.transform.Find("Info/Info/Content/Equipment/LockButton") : base.view.transform.Find("Info/Content/LockButton"));
			bool isProtected = storageItem.isProtected;
			transform.Find("LockedImage").gameObject.SetActive(isProtected);
			transform.Find("UnlockedImage").gameObject.SetActive(!isProtected);
		}

		private void SetupAttributesPanel()
		{
			MonoAttributeDisplay component = base.view.transform.Find("Attributes/InfoPanel/BasicStatus").GetComponent<MonoAttributeDisplay>();
			component.SetupView(storageItem, uiEquipOwner);
		}

		private void SetupLvInfoPanel()
		{
			base.view.transform.Find("Lv/InfoRowLv/Lv/CurrentLevelNum").GetComponent<Text>().text = storageItem.level.ToString();
			base.view.transform.Find("Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().text = storageItem.GetMaxLevel().ToString();
			if (storageItem.level == storageItem.GetMaxLevel())
			{
				base.view.transform.Find("Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().color = MiscData.GetColor("Yellow");
			}
			base.view.transform.Find("Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = storageItem.exp.ToString();
			base.view.transform.Find("Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = storageItem.GetMaxExp().ToString();
			base.view.transform.Find("Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(storageItem.exp, storageItem.GetMaxExp(), 0f);
		}

		private void SetupSkillsPanel()
		{
			if (storageItem is WeaponDataItem)
			{
				SetupWeaponSkills();
			}
			else if (storageItem is StigmataDataItem)
			{
				SetupStigmataSkills();
			}
		}

		private void SetupWeaponSkills()
		{
			List<EquipSkillDataItem> skills = (storageItem as WeaponDataItem).skills;
			base.view.transform.Find("Skills").gameObject.SetActive(skills.Count > 0);
			if (skills.Count > 0)
			{
				MonoEquipSkillPanel component = base.view.transform.Find("Skills/Info/NaturalSkills/Content").GetComponent<MonoEquipSkillPanel>();
				component.SetupView(skills, storageItem.level);
			}
		}

		private void SetupStigmataSkills()
		{
			StigmataDataItem stigmataData = storageItem as StigmataDataItem;
			_stigmataTabManager.Clear();
			SetupNaturalSkillsTab(stigmataData);
			SetupAffixSkillsTab(stigmataData);
			SetupSuitSkillsTab(stigmataData);
			List<string> keys = _stigmataTabManager.GetKeys();
			if (keys.Count > 0)
			{
				_stigmataTabManager.ShowTab(keys[0]);
			}
		}

		private void SetupNaturalSkillsTab(StigmataDataItem stigmataData)
		{
			List<EquipSkillDataItem> skills = stigmataData.skills;
			if (skills.Count > 0)
			{
				MonoEquipSkillPanel component = base.view.transform.Find("Skills/Info/NaturalSkills/Content").GetComponent<MonoEquipSkillPanel>();
				component.SetupView(skills, stigmataData.level);
				_stigmataTabManager.SetTab("S_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("Skills/Info/NaturalSkills").gameObject);
				base.view.transform.Find("Skills/TabBtns/TabBtn_1").gameObject.SetActive(true);
			}
			else
			{
				base.view.transform.Find("Skills/TabBtns/TabBtn_1").gameObject.SetActive(false);
				base.view.transform.Find("Skills/Info/NaturalSkills").gameObject.SetActive(false);
			}
		}

		private void SetupSuitSkillsTab(StigmataDataItem stigmataData)
		{
			SortedDictionary<int, EquipSkillDataItem> allSetSkills = stigmataData.GetAllSetSkills();
			if (allSetSkills.Count > 0)
			{
				MonoStigmataSetSkillPanel component = base.view.transform.Find("Skills/Info/SuitSkills/Content").GetComponent<MonoStigmataSetSkillPanel>();
				component.SetupView(stigmataData, allSetSkills);
				_stigmataTabManager.SetTab("S_Suit_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("Skills/Info/SuitSkills").gameObject);
				base.view.transform.Find("Skills/TabBtns/TabBtn_2").gameObject.SetActive(true);
			}
			else
			{
				base.view.transform.Find("Skills/TabBtns/TabBtn_2").gameObject.SetActive(false);
				base.view.transform.Find("Skills/Info/SuitSkills").gameObject.SetActive(false);
			}
		}

		private void SetupAffixSkillsTab(StigmataDataItem stigmataData)
		{
			List<StigmataDataItem.AffixSkillData> affixSkillList = stigmataData.GetAffixSkillList();
			if (!stigmataData.IsAffixIdentify || affixSkillList.Count > 0)
			{
				MonoStigmataAffixSkillPanel component = base.view.transform.Find("Skills/Info/AffixSkills/Content").GetComponent<MonoStigmataAffixSkillPanel>();
				component.SetupView(stigmataData, affixSkillList);
				_stigmataTabManager.SetTab("S_Affix_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("Skills/Info/AffixSkills").gameObject);
				base.view.transform.Find("Skills/TabBtns/TabBtn_3").gameObject.SetActive(true);
			}
			else
			{
				base.view.transform.Find("Skills/TabBtns/TabBtn_3").gameObject.SetActive(false);
				base.view.transform.Find("Skills/Info/AffixSkills").gameObject.SetActive(false);
			}
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}
	}
}
