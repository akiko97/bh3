using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class CabinDetailPageContext : BasePageContext
	{
		public const string POWER_TAB = "PowerTab";

		public const string TREE_TAB = "TreeTab";

		public const string ENHANCE_TAB = "EnhanceTab";

		public const string VENTURE_TAB = "VentureTab";

		public const string MISC_TAB = "MiscTab";

		public const string MISC_OVERVIEW_TAB = "MiscOverviewTab";

		public const string COLLECT_TAB = "CollectTab";

		private const string AVATAR_ENHANCE_INFO_PREFAB_PATH = "UI/Menus/Widget/Island/AvatarEnhanceInfo";

		private const string TREE_TAB_PREFAB_PATH = "UI/Menus/Page/Island/TreeTab";

		private const string MISC_TAB_PREFAB_PATH = "UI/Menus/Page/Island/MiscTab";

		private const string COLLECT_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinCollectBG";

		private const string COLLECT_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinCollectBlack";

		private const string ENHANCE_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinEnhanceBG";

		private const string ENHANCE_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinEnhanceBlack";

		private const string Misc_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinMiscBG";

		private const string Misc_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinMiscBlack";

		private const string POWER_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinPowerBG";

		private const string POWER_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinPowerBlack";

		private const string VENTURE_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinVentureBG";

		private const string VENTURE_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinVentureBlack";

		private int _animatorCollectTrigger = Animator.StringToHash("CollectTab");

		private int _animatorPowerTrigger = Animator.StringToHash("PowerTab");

		private int _animatorMiscTrigger = Animator.StringToHash("MiscTab");

		private int _animatorMiscOverviewTrigger = Animator.StringToHash("MiscOverviewTab");

		private int _animatorVentureTrigger = Animator.StringToHash("VentureTab");

		private int _animatorEnhanceTrigger = Animator.StringToHash("EnhanceTab");

		private int _animatorTreeTrigger = Animator.StringToHash("TreeTab");

		private Dictionary<string, int> _animatorTriggerDict;

		private Animator _animator;

		public static readonly string[] TAB_KEY = new string[7] { "PowerTab", "TreeTab", "EnhanceTab", "VentureTab", "MiscTab", "MiscOverviewTab", "CollectTab" };

		public string defaultTab = string.Empty;

		private TabManager _tabManager;

		private CabinDataItemBase _cabinData;

		private int _playerLevelBefore;

		private CanvasTimer _triggerCameraTimer;

		private Vector2 _infoPos;

		private List<VentureDataItem> _ventureList;

		private MiscSubTab _currentMiscSubTab;

		private CanvasTimer _collectCabinTimer;

		private float FETCH_SCOIN_MISSION_RATIO_TOTAL = 200f;

		private bool _bCacheSpawn;

		private Dictionary<MiscSubTab, Transform> _subTabDict = new Dictionary<MiscSubTab, Transform>();

		private StorageDataItemBase _selectedItem;

		private List<StorageDataItemBase> _showItemList;

		private Transform _iconEffect;

		private float _iconEffectDuration = 2.5f;

		public CabinDetailPageContext(CabinDataItemBase cabinData, bool bCacheSpawn = false)
		{
			config = new ContextPattern
			{
				contextName = "CabinDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/Island/CabinDetailPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_cabinData = cabinData;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_bCacheSpawn = bCacheSpawn;
		}

		private void SetupUIBG()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected I4, but got Unknown
			Image component = base.view.transform.Find("BG/BG").GetComponent<Image>();
			Image component2 = base.view.transform.Find("BG/Black").GetComponent<Image>();
			string prefabPath = string.Empty;
			string prefabPath2 = string.Empty;
			CabinType cabinType = _cabinData.cabinType;
			switch ((int)cabinType - 1)
			{
			case 0:
				prefabPath = "SpriteOutput/CabinBG/CabinPowerBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinPowerBlack";
				break;
			case 1:
				prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
				break;
			case 5:
				prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
				break;
			case 6:
				prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
				break;
			case 4:
				prefabPath = "SpriteOutput/CabinBG/CabinVentureBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinVentureBlack";
				break;
			case 3:
				prefabPath = "SpriteOutput/CabinBG/CabinMiscBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinMiscBlack";
				break;
			case 2:
				prefabPath = "SpriteOutput/CabinBG/CabinCollectBG";
				prefabPath2 = "SpriteOutput/CabinBG/CabinCollectBlack";
				break;
			}
			component.sprite = Miscs.GetSpriteByPrefab(prefabPath);
			component2.sprite = Miscs.GetSpriteByPrefab(prefabPath2);
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SetSellViewActive)
			{
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 157)
			{
				SetupView();
			}
			if (cmdId == 167)
			{
				OnResetCabinTechRsp(pkt.getData<ResetCabinTechRsp>());
			}
			if (cmdId == 165)
			{
				OnAddCabinTechRsp(pkt.getData<AddCabinTechRsp>());
			}
			if (cmdId == 169)
			{
				OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());
			}
			if (cmdId == 11)
			{
				OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());
			}
			if (cmdId == 176)
			{
				OnGetIslandVentureRewardRsp(pkt.getData<GetIslandVentureRewardRsp>());
			}
			if (cmdId == 170)
			{
				RefreshVentureTabContent();
			}
			if (cmdId == 180)
			{
				OnGetIslandDisjoinEquipmentRsp(pkt.getData<IslandDisjoinEquipmentRsp>());
			}
			if (cmdId == 172)
			{
				OnRefreshIslandVentureRsp(pkt.getData<RefreshIslandVentureRsp>());
			}
			if (cmdId == 184)
			{
				OnGetCollectCabinRsp(pkt.getData<GetCollectCabinRsp>());
			}
			if (cmdId == 182)
			{
				OnIslandCollectRsp(pkt.getData<IslandCollectRsp>());
			}
			if (cmdId == 210)
			{
				OnSpeedUpIslandVentureRsp(pkt.getData<SpeedUpIslandVentureRsp>());
			}
			if (cmdId == 222)
			{
				OnGetRefreshIslandVentureInfoRsp(pkt.getData<GetRefreshIslandVentureInfoRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnPower").GetComponent<Button>(), OnPowerTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnTree").GetComponent<Button>(), OnTreeTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnEnhance").GetComponent<Button>(), OnEnhanceTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnVenture").GetComponent<Button>(), OnVentureTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnMisc").GetComponent<Button>(), OnMiscTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnMiscOverview").GetComponent<Button>(), OnMiscOverviewTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtnCollect").GetComponent<Button>(), OnCollectTabBtnClick);
			BindViewCallback(base.view.transform.Find("VentureTab/InfoPanel/Refresh").GetComponent<Button>(), OnRefreshVentureBtnClick);
			BindViewCallback(base.view.transform.Find("CollectTab/FetchBtn/Btn").GetComponent<Button>(), OnFetchScoinBtnClick);
			BindViewCallback(base.view.transform.Find("TreeTab/ResetBtn").GetComponent<Button>(), OnResetPowerBtnClick);
			BindViewCallback(base.view.transform.Find("MiscTab/FirstSubTab/AddBtn").GetComponent<Button>(), OnMiscAddBtnClick);
			BindViewCallback(base.view.transform.Find("MiscTab/SelectSubTab/DisjointBtn").GetComponent<Button>(), OnMiscDisjointBtnClick);
			BindViewCallback(base.view.transform.Find("MiscTab/PreviewSubTab/DisjointBtn").GetComponent<Button>(), OnMiscDisjointFinalBtnClick);
			BindViewCallback(base.view.transform.Find("MiscTab/SelectSubTab/CancelBtn").GetComponent<Button>(), OnMiscSelectCancelBtnClick);
			BindViewCallback(base.view.transform.Find("MiscTab/PreviewSubTab/CancelBtn").GetComponent<Button>(), OnMiscPreviewCancelBtnClick);
		}

		private void TriggerSceneCamera(bool enable)
		{
			GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().TriggerCameraObj(enable);
		}

		protected override bool SetupView()
		{
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Invalid comparison between Unknown and I4
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Expected I4, but got Unknown
			_animator = base.view.transform.GetComponent<Animator>();
			_animatorTriggerDict = new Dictionary<string, int>();
			_animatorTriggerDict["CollectTab"] = _animatorCollectTrigger;
			_animatorTriggerDict["PowerTab"] = _animatorPowerTrigger;
			_animatorTriggerDict["MiscTab"] = _animatorMiscTrigger;
			_animatorTriggerDict["MiscOverviewTab"] = _animatorMiscOverviewTrigger;
			_animatorTriggerDict["VentureTab"] = _animatorVentureTrigger;
			_animatorTriggerDict["EnhanceTab"] = _animatorEnhanceTrigger;
			_animatorTriggerDict["TreeTab"] = _animatorTreeTrigger;
			SetupUIBG();
			foreach (Transform item in base.view.transform.Find("EffectContainer"))
			{
				ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			Transform transform2 = base.view.transform.Find("InfoPanel/Info/");
			transform2.Find("CabinName").GetComponent<Text>().text = _cabinData.GetCabinName();
			transform2.Find("Lv/Lv").GetComponent<Text>().text = "Lv." + _cabinData.level;
			transform2.Find("ExtendGrade").GetComponent<MonoCabinExtendGrade>().SetupView(_cabinData.extendGrade);
			SetAllTabVisible(false);
			if ((int)_cabinData.cabinType != 1)
			{
				SetupTreeTab();
			}
			CabinType cabinType = _cabinData.cabinType;
			switch ((int)cabinType - 1)
			{
			case 0:
				SetupPowerTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "PowerTab";
				}
				break;
			case 1:
				SetupEnhanceTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "EnhanceTab";
				}
				break;
			case 5:
				SetupEnhanceTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "EnhanceTab";
				}
				break;
			case 6:
				SetupEnhanceTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "EnhanceTab";
				}
				break;
			case 4:
				SetupVentureTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "VentureTab";
				}
				break;
			case 3:
				SetupMiscTab();
				SetupMiscOverviewTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "MiscOverviewTab";
				}
				break;
			case 2:
				SetupCollectTab();
				if (string.IsNullOrEmpty(defaultTab))
				{
					defaultTab = "CollectTab";
				}
				break;
			}
			if (string.IsNullOrEmpty(defaultTab))
			{
				defaultTab = "TreeTab";
			}
			_tabManager.ShowTab(defaultTab);
			_animator.ResetTrigger(_animatorTriggerDict[defaultTab]);
			_animator.SetTrigger(_animatorTriggerDict[defaultTab]);
			if (!_bCacheSpawn)
			{
				_triggerCameraTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
				_triggerCameraTimer.timeUpCallback = delegate
				{
					TriggerSceneCamera(false);
				};
			}
			return false;
		}

		public override void BackToMainMenuPage()
		{
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true);
		}

		public override void BackPage()
		{
			Singleton<IslandModule>.Instance.RegisterVentureInProgress();
			base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>().ClearNodes();
			if (_triggerCameraTimer != null)
			{
				_triggerCameraTimer.Destroy();
			}
			TriggerSceneCamera(true);
			string[] tAB_KEY = TAB_KEY;
			foreach (string name in tAB_KEY)
			{
				base.view.transform.Find(name).gameObject.SetActive(false);
			}
			base.BackPage();
			base.BackPage();
		}

		protected override void OnSetActive(bool enabled)
		{
			base.OnSetActive(enabled);
			if (!enabled)
			{
				string[] tAB_KEY = TAB_KEY;
				foreach (string name in tAB_KEY)
				{
					base.view.transform.Find(name).gameObject.SetActive(false);
				}
			}
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			foreach (Transform item in base.view.transform.Find("EffectContainer"))
			{
				ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			_animator.ResetTrigger(_animatorTriggerDict[defaultTab]);
			_animator.SetTrigger(_animatorTriggerDict[defaultTab]);
		}

		private void OnPowerTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "PowerTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnTreeTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "TreeTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
			MonoTechTreeUI component = base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>();
			component.SetOriginPosition();
		}

		private void OnEnhanceTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "EnhanceTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnVentureTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "VentureTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnMiscOverviewTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "MiscOverviewTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnMiscTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "MiscTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
			SwitchMiscSubTab(MiscSubTab.First);
		}

		private void OnCollectTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = (defaultTab = "CollectTab");
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnResetPowerBtnClick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)_cabinData.cabinType == 5 && Singleton<IslandModule>.Instance.GetVentureInProgressNum() > 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_CanNotResetTechTreeWhenVentureInProgress"),
					type = GeneralDialogContext.ButtonType.SingleButton
				});
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ResetTechTreeDialogContext(_cabinData));
			}
		}

		private void ClearOnChangedTab(string tabBefore, string tabAfter)
		{
			int num = _animatorTriggerDict[tabAfter];
			_animator.ResetTrigger(_animatorTriggerDict[tabBefore]);
			_animator.ResetTrigger(num);
			_animator.SetTrigger(num);
		}

		private void OnRefreshVentureBtnClick()
		{
			int times = Singleton<IslandModule>.Instance.VentureRefreshTimes + 1;
			if (!(_cabinData as CabinVentureDataItem).GetRefreshCost(times))
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_ReachRefreshVentureMaxTimes")
				});
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestGetRefreshIslandVentureInfo();
				Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(222));
			}
		}

		private void OnFetchScoinBtnClick()
		{
			CabinCollectDataItem cabinCollectDataItem = _cabinData as CabinCollectDataItem;
			if (cabinCollectDataItem.CanFetchScoin())
			{
				Singleton<NetworkManager>.Instance.RequestIslandCollect();
				return;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.SingleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = "No scoin can fetch"
			});
		}

		private bool OnGetMainDataRsp(GetMainDataRsp rsp)
		{
			if (rsp.scoinSpecified && _tabManager.GetShowingTabKey() == "MiscTab" && _currentMiscSubTab == MiscSubTab.Preview)
			{
				RefreshSubTab_Preview();
			}
			return false;
		}

		private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				RefreshVentureTabContent();
			}
			return false;
		}

		private void RefreshVentureTabContent()
		{
			SetupVentureTabContent();
			bool active = Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0;
			base.view.transform.Find("TabBtns/TabBtnVenture/PopUp").gameObject.SetActive(active);
		}

		private bool OnGetIslandVentureRewardRsp(GetIslandVentureRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new RewardGotDialogContext(rsp.reward_list[0], _playerLevelBefore, rsp.drop_item_list, "Menu_Title_GotVentureReward", string.Empty));
			}
			return false;
		}

		private bool OnAddCabinTechRsp(AddCabinTechRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				RefreshTreeUI();
			}
			return false;
		}

		private bool OnResetCabinTechRsp(ResetCabinTechRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				RefreshTreeUI();
			}
			return false;
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			if (!_bCacheSpawn)
			{
				btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
				btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
				btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
				btn.interactable = !active;
				go.SetActive(active);
			}
		}

		private void SetupPowerTab()
		{
			GameObject gameObject = base.view.transform.Find("PowerTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnPower").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "PowerTab";
			_tabManager.SetTab(key, component, gameObject);
			SetupPowerTabContent();
		}

		private void SetupPowerTabContent()
		{
			Transform transform = base.view.transform.Find("PowerTab/PowerInfo");
			int usedPowerCost = Singleton<IslandModule>.Instance.GetUsedPowerCost();
			int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
			transform.Find("CurrentPower/PowerCost/Current").GetComponent<Text>().text = usedPowerCost.ToString();
			transform.Find("CurrentPower/PowerCost/Max").GetComponent<Text>().text = maxPowerCost.ToString();
			base.view.transform.Find("PowerTab/Power/Fill").GetComponent<Image>().fillAmount = (float)usedPowerCost / (float)maxPowerCost;
			transform.Find("MaxInfo/Current/Num").GetComponent<Text>().text = Singleton<IslandModule>.Instance.GetMaxPowerCost().ToString();
			int nextLevelMaxPowerCost = Singleton<IslandModule>.Instance.GetNextLevelMaxPowerCost();
			bool flag = nextLevelMaxPowerCost > 0;
			transform.Find("MaxInfo/Next").gameObject.SetActive(flag);
			if (flag)
			{
				transform.Find("MaxInfo/Next/Num").GetComponent<Text>().text = nextLevelMaxPowerCost.ToString();
			}
		}

		private void SetupCollectTab()
		{
			GameObject gameObject = base.view.transform.Find("CollectTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnCollect").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "CollectTab";
			_tabManager.SetTab(key, component, gameObject);
			SetupCollectTabContent();
		}

		private void SetupCollectTabContent()
		{
			CabinCollectDataItem cabinCollectDataItem = _cabinData as CabinCollectDataItem;
			Transform transform = base.view.transform.Find("CollectTab/InfoPanel");
			transform.Find("Speed/Num").GetComponent<Text>().text = Mathf.FloorToInt(cabinCollectDataItem.speed).ToString();
			transform.Find("TopLimit/Num").GetComponent<Text>().text = cabinCollectDataItem.topLimit.ToString();
			transform.Find("CrtRatio/Num").GetComponent<Text>().text = string.Format("{0:0%}", cabinCollectDataItem.crtRatio);
			transform.Find("ExtraRatio/Num").GetComponent<Text>().text = string.Format("{0:0%}", cabinCollectDataItem.crtExtraRatio);
			base.view.transform.Find("CollectTab/Scoin/Fill").GetComponent<Image>().fillAmount = Mathf.Clamp01((float)cabinCollectDataItem.currentScoinAmount / cabinCollectDataItem.topLimit);
			base.view.transform.Find("CollectTab/Scoin/Num").GetComponent<Text>().text = cabinCollectDataItem.currentScoinAmount.ToString();
			if (_collectCabinTimer != null)
			{
				_collectCabinTimer.Destroy();
			}
			if (cabinCollectDataItem.canUpdateScoinLate)
			{
				_collectCabinTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer((float)(cabinCollectDataItem.nextScoinUpdateTime - TimeUtil.Now).TotalSeconds, 0f);
				_collectCabinTimer.timeUpCallback = delegate
				{
					Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
				};
			}
			base.view.transform.Find("CollectTab/FetchBtn/Btn").GetComponent<Button>().interactable = cabinCollectDataItem.CanFetchScoin();
			int num = 5;
			Transform transform2 = base.view.transform.Find("CollectTab/InfoPanel");
			Transform transform3 = base.view.transform.Find("CollectTab/InfoPanel/Accessory");
			int num2 = Singleton<IslandModule>.Instance.GetDropMaterialPackageNum();
			if (num2 > num)
			{
				num2 = num;
			}
			if (num2 > 0 || cabinCollectDataItem.dropItems.Count > 0)
			{
				transform3.gameObject.SetActive(true);
				_infoPos.x = transform2.GetComponent<RectTransform>().anchoredPosition.x;
				_infoPos.y = -40f;
				transform2.GetComponent<RectTransform>().anchoredPosition = _infoPos;
			}
			else
			{
				transform3.gameObject.SetActive(false);
				_infoPos.x = transform2.GetComponent<RectTransform>().anchoredPosition.x;
				_infoPos.y = -112f;
				transform2.GetComponent<RectTransform>().anchoredPosition = _infoPos;
			}
			for (int num3 = 0; num3 < num; num3++)
			{
				Transform transform4 = transform3.Find(string.Format("MaterialList/{0}", (num3 + 1).ToString()));
				if (num3 < cabinCollectDataItem.dropItems.Count)
				{
					transform4.gameObject.SetActive(true);
					int item_id = (int)cabinCollectDataItem.dropItems[num3].item_id;
					int level = (int)cabinCollectDataItem.dropItems[num3].level;
					StorageDataItemBase itemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item_id, level);
					if (itemData == null)
					{
						continue;
					}
					transform4.Find("Empty").gameObject.SetActive(false);
					transform4.Find("BG").gameObject.SetActive(true);
					transform4.Find("BG/Unselected/FrameBottom").gameObject.SetActive(true);
					transform4.Find("Text").gameObject.SetActive(true);
					transform4.Find("Text").GetComponent<Text>().text = string.Format("x{0}", (int)cabinCollectDataItem.dropItems[num3].num);
					transform4.Find("ItemIcon").gameObject.SetActive(true);
					transform4.Find("Star").gameObject.SetActive(true);
					transform4.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemData.GetIconPath());
					transform4.Find("Star").GetComponent<MonoItemIconStar>().SetupView(itemData.rarity, itemData.rarity);
					if (itemData is EndlessToolDataItem)
					{
						transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
						continue;
					}
					BindViewCallback(transform4.Find("ShowDetailBtn").GetComponent<Button>(), delegate
					{
						ShowDetailDialog(itemData);
					});
				}
				else if (num3 < num2)
				{
					transform4.gameObject.SetActive(true);
					transform4.Find("Empty").gameObject.SetActive(true);
					transform4.Find("BG").gameObject.SetActive(false);
					transform4.Find("BG/Unselected/FrameBottom").gameObject.SetActive(false);
					transform4.Find("Text").gameObject.SetActive(false);
					transform4.Find("ItemIcon").gameObject.SetActive(false);
					transform4.Find("Star").gameObject.SetActive(false);
					transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
				}
				else
				{
					transform4.gameObject.SetActive(false);
					transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
				}
			}
		}

		private void ShowDetailDialog(StorageDataItemBase item)
		{
			UIUtil.ShowItemDetail(item, true);
		}

		private void HackDropItems()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			CabinCollectDataItem cabinCollectDataItem = _cabinData as CabinCollectDataItem;
			cabinCollectDataItem.dropItems = new List<DropItem>();
			DropItem val = new DropItem();
			val.item_id = 4001u;
			cabinCollectDataItem.dropItems.Add(val);
			val = new DropItem();
			val.item_id = 1007u;
			cabinCollectDataItem.dropItems.Add(val);
		}

		private void SetupMiscOverviewTab()
		{
			GameObject gameObject = base.view.transform.Find("MiscOverviewTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnMiscOverview").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "MiscOverviewTab";
			_tabManager.SetTab(key, component, gameObject);
			SetupMiscOverviewTabContent();
		}

		private void SetupMiscOverviewTabContent()
		{
			int count = Singleton<FriendModule>.Instance.friendsList.Count;
			int maxFriendAdd = Singleton<IslandModule>.Instance.GetMaxFriendAdd();
			int num = Singleton<PlayerModule>.Instance.playerData.maxFriend + maxFriendAdd;
			base.view.transform.Find("MiscOverviewTab/Content/1/Friend/Fill").GetComponent<Image>().fillAmount = (float)count / (float)num;
			base.view.transform.Find("MiscOverviewTab/Content/1/Friend/Num").GetComponent<Text>().text = count.ToString();
			base.view.transform.Find("MiscOverviewTab/Content/1/Plus").GetComponent<Text>().text = string.Format("+{0}", maxFriendAdd);
			int skillPoint = Singleton<PlayerModule>.Instance.playerData.skillPoint;
			int skillPointLimit = Singleton<PlayerModule>.Instance.playerData.skillPointLimit;
			int skillPointAdd = Singleton<IslandModule>.Instance.GetSkillPointAdd();
			base.view.transform.Find("MiscOverviewTab/Content/2/Skill/Fill").GetComponent<Image>().fillAmount = (float)skillPoint / (float)skillPointLimit;
			base.view.transform.Find("MiscOverviewTab/Content/2/Skill/Num").GetComponent<Text>().text = skillPoint.ToString();
			base.view.transform.Find("MiscOverviewTab/Content/2/Plus").GetComponent<Text>().text = string.Format("+{0}", skillPointAdd);
		}

		private void SetupMiscTab()
		{
			GameObject gameObject = base.view.transform.Find("MiscTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnMisc").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "MiscTab";
			_tabManager.SetTab(key, component, gameObject);
			_iconEffect = base.view.transform.Find("MiscTab/IconEffect");
			SetupMiscTabContent();
		}

		private void SetupMiscTabContent()
		{
			_subTabDict[MiscSubTab.First] = base.view.transform.Find("MiscTab/FirstSubTab");
			_subTabDict[MiscSubTab.Select] = base.view.transform.Find("MiscTab/SelectSubTab");
			_subTabDict[MiscSubTab.Preview] = base.view.transform.Find("MiscTab/PreviewSubTab");
			SwitchMiscSubTab(MiscSubTab.First);
		}

		private void OnMiscAddBtnClick()
		{
			_showItemList = GetFilterList();
			if (_showItemList.Count <= 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_CabinNoDisjointWeapon")
				});
			}
			else
			{
				SwitchMiscSubTab(MiscSubTab.Select);
			}
		}

		private void OnMiscDisjointBtnClick()
		{
			SwitchMiscSubTab(MiscSubTab.Preview);
		}

		private void OnMiscDisjointFinalBtnClick()
		{
			int scoin = Singleton<PlayerModule>.Instance.playerData.scoin;
			int needSCoin = CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(_selectedItem.ID).NeedSCoin;
			if (scoin < needSCoin)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new SCoinExchangeDialogContext());
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestIslandDisjoinEquipment((EquipmentType)3, (uint)_selectedItem.uid);
			}
		}

		private bool OnGetIslandDisjoinEquipmentRsp(IslandDisjoinEquipmentRsp rsp)
		{
			SwitchMiscSubTab(MiscSubTab.First);
			Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Decompose_Item");
			_iconEffect.gameObject.SetActive(true);
			MonoItemIconButton component = _iconEffect.GetComponent<MonoItemIconButton>();
			component.SetupView(_selectedItem);
			Singleton<ApplicationManager>.Instance.StartCoroutine(ShowDisjoinRsp(rsp));
			return false;
		}

		private IEnumerator ShowDisjoinRsp(IslandDisjoinEquipmentRsp rsp)
		{
			yield return new WaitForSeconds(_iconEffectDuration);
			_iconEffect.gameObject.SetActive(false);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.SingleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Tips"),
				desc = (((int)rsp.retcode != 0) ? ((Enum)rsp.retcode).ToString() : LocalizationGeneralLogic.GetText("Menu_Title_IslandDisjoinSucc"))
			});
		}

		private bool OnRefreshIslandVentureRsp(RefreshIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = ((Enum)rsp.retcode).ToString()
				});
			}
			return false;
		}

		private bool OnGetCollectCabinRsp(GetCollectCabinRsp rsp)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (_cabinData is CabinCollectDataItem && (int)rsp.retcode == 0)
			{
				SetupCollectTab();
			}
			return false;
		}

		private bool OnIslandCollectRsp(IslandCollectRsp rsp)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Transform transform = base.view.transform.Find("EffectContainer/IslandCollectionGoldCoin");
				int fetchScoin = (int)rsp.add_scoin;
				CabinCollectDataItem cabinCollectDataItem = _cabinData as CabinCollectDataItem;
				float burstRate = ((!rsp.is_extraSpecified || !rsp.is_extra) ? 1f : cabinCollectDataItem.crtExtraRatio);
				ParticleSystem component = transform.GetComponent<ParticleSystem>();
				ParticleSystem.EmissionModule emission = component.emission;
				emission.rate = new ParticleSystem.MinMaxCurve
				{
					constantMax = Mathf.Clamp((float)fetchScoin / cabinCollectDataItem.topLimit, 0.1f, 1f) * FETCH_SCOIN_MISSION_RATIO_TOTAL
				};
				Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Collect_Gold");
				PlayEffect(transform);
				CanvasTimer canvasTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.2f, 0f);
				canvasTimer.timeUpCallback = delegate
				{
					ShowGetScoinHintDialog(fetchScoin, burstRate, rsp.drop_item_list);
				};
			}
			return false;
		}

		private bool OnSpeedUpIslandVentureRsp(SpeedUpIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0 && (int)_cabinData.cabinType == 5)
			{
				SetupVentureTabContent();
			}
			return false;
		}

		private bool OnGetRefreshIslandVentureInfoRsp(GetRefreshIslandVentureInfoRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				int refresh_price = (int)rsp.refresh_price;
				if (refresh_price > Singleton<PlayerModule>.Instance.playerData.hcoin)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						type = GeneralDialogContext.ButtonType.SingleButton,
						title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
						desc = LocalizationGeneralLogic.GetText("10029")
					});
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						type = GeneralDialogContext.ButtonType.DoubleButton,
						title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshVentureListHint", refresh_price),
						buttonCallBack = OnRefreshVentureListConfirmed
					});
				}
			}
			return false;
		}

		private void OnMiscSelectCancelBtnClick()
		{
			SwitchMiscSubTab(MiscSubTab.First);
		}

		private void OnMiscPreviewCancelBtnClick()
		{
			SwitchMiscSubTab(MiscSubTab.Select);
		}

		private void SwitchMiscSubTab(MiscSubTab tab)
		{
			_currentMiscSubTab = tab;
			foreach (int value in Enum.GetValues(typeof(MiscSubTab)))
			{
				_subTabDict[(MiscSubTab)value].gameObject.SetActive(value == (int)tab);
			}
			switch (tab)
			{
			case MiscSubTab.First:
				RefreshSubTab_First();
				break;
			case MiscSubTab.Select:
				RefreshSubTab_Select();
				break;
			case MiscSubTab.Preview:
				RefreshSubTab_Preview();
				break;
			}
		}

		private void RefreshSubTab_First()
		{
			bool flag = _cabinData._techTree.AbilityUnLock((CabinTechEffectType)6);
			string text = ((!flag) ? LocalizationGeneralLogic.GetText("Menu_Desc_CabinMiscDisjointDisable") : LocalizationGeneralLogic.GetText("Menu_Desc_CabinMiscDisjointEnable"));
			base.view.transform.Find("MiscTab/FirstSubTab/Title").GetComponent<Text>().text = text;
			base.view.transform.Find("MiscTab/FirstSubTab/AddBtn").gameObject.SetActive(flag);
		}

		private void RefreshSubTab_Select()
		{
			if (_showItemList.Count > 0)
			{
				_selectedItem = _showItemList[0];
				_subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().Init(OnChange, _showItemList.Count);
				UpdateSelectInfo();
			}
		}

		private void OnChange(Transform trans, int index)
		{
			StorageDataItemBase storageDataItemBase = _showItemList[index];
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.showProtected = true;
			bool isSelected = storageDataItemBase == _selectedItem;
			bool bUsed = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(storageDataItemBase.avatarID) != null;
			component.SetupView(storageDataItemBase, MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected, false, bUsed);
			component.SetClickCallback(OnItemClick);
		}

		private List<StorageDataItemBase> GetFilterList()
		{
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.GetAllUserWeapons().FindAll((StorageDataItemBase x) => Filter(x));
			list.Sort(StorageDataItemBase.CompareToRarityDesc);
			return list;
		}

		private bool Filter(StorageDataItemBase item)
		{
			bool flag = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.avatarID) != null;
			bool flag2 = CabinDisjointEquipmentMetaDataReader.TryGetCabinDisjointEquipmentMetaDataByKey(item.ID) != null;
			return !flag && flag2 && !item.isProtected;
		}

		private void OnItemClick(StorageDataItemBase item, bool selected)
		{
			if (!selected)
			{
				_selectedItem = item;
				UpdateSelectInfo();
				_subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
			}
		}

		private void UpdateSelectInfo()
		{
			_subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/SelectedEquip/Name").GetComponent<Text>().text = _selectedItem.GetDisplayTitle();
			_subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/SelectedEquip/Lv").GetComponent<Text>().text = "LV." + _selectedItem.level;
		}

		private void RefreshSubTab_Preview()
		{
			MonoItemIconButton component = _subTabDict[MiscSubTab.Preview].Find("Input").GetComponent<MonoItemIconButton>();
			component.SetupView(_selectedItem);
			MonoMiscDisjointOutputUI component2 = _subTabDict[MiscSubTab.Preview].Find("ScrollView/Content").GetComponent<MonoMiscDisjointOutputUI>();
			component2.SetupView(_selectedItem);
			int scoin = Singleton<PlayerModule>.Instance.playerData.scoin;
			int needSCoin = CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(_selectedItem.ID).NeedSCoin;
			Text component3 = _subTabDict[MiscSubTab.Preview].Find("DisjointBtn/Cost/Content/Text").GetComponent<Text>();
			component3.text = needSCoin.ToString();
			Color color = ((scoin >= needSCoin) ? Color.white : Color.red);
			component3.color = color;
		}

		private void SetupTreeTab()
		{
			GameObject gameObject = base.view.transform.Find("TreeTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnTree").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "TreeTab";
			_tabManager.SetTab(key, component, gameObject);
			MonoTechTreeUI component2 = gameObject.transform.Find("ScrollView").GetComponent<MonoTechTreeUI>();
			if (string.IsNullOrEmpty(defaultTab))
			{
				component2.ClearNodes();
			}
			if (!component2.HasChildren())
			{
				component2.InitNodes(_cabinData._techTree);
			}
			RefreshPowerInfo();
			component2.SetOriginPosition();
		}

		private void RefreshTreeUI()
		{
			base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>().RefreshUI();
			RefreshPowerInfo();
		}

		private void RefreshPowerInfo()
		{
			int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
			int usedPowerCost = Singleton<IslandModule>.Instance.GetUsedPowerCost();
			int usedPower = _cabinData.GetUsedPower();
			float fillAmount = (float)usedPowerCost / (float)maxPowerCost;
			float fillAmount2 = (float)usedPower / (float)maxPowerCost;
			base.view.transform.Find("TreeTab/PowerInfo/PowerCircle/AllUsed").GetComponent<Image>().fillAmount = fillAmount;
			base.view.transform.Find("TreeTab/PowerInfo/PowerCircle/ThisUsed").GetComponent<Image>().fillAmount = fillAmount2;
			base.view.transform.Find("TreeTab/PowerInfo/Line1/Used").GetComponent<Text>().text = usedPowerCost.ToString();
			base.view.transform.Find("TreeTab/PowerInfo/Line1/Max").GetComponent<Text>().text = maxPowerCost.ToString();
			base.view.transform.Find("TreeTab/PowerInfo/Line2/Used").GetComponent<Text>().text = usedPower.ToString();
		}

		private void SetupEnhanceTab()
		{
			GameObject gameObject = base.view.transform.Find("EnhanceTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnEnhance").GetComponent<Button>();
			component.gameObject.SetActive(true);
			string key = "EnhanceTab";
			_tabManager.SetTab(key, component, gameObject);
			SetupEnhanceTabContent();
		}

		private void SetupEnhanceTabContent()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected I4, but got Unknown
			Transform transform = base.view.transform.Find("EnhanceTab/EnhanceList/Content");
			transform.DestroyChildren();
			Transform transform2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/Island/AvatarEnhanceInfo")).transform;
			transform2.SetParent(transform, false);
			CabinAvatarEnhanceDataItem cabinAvatarEnhanceDataItem = _cabinData as CabinAvatarEnhanceDataItem;
			int avatarClassID = (int)cabinAvatarEnhanceDataItem._classType;
			transform2.GetComponent<MonoAvatarEnhance>().SetupView(avatarClassID);
		}

		private void SetupVentureTab()
		{
			GameObject gameObject = base.view.transform.Find("VentureTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/TabBtnVenture").GetComponent<Button>();
			component.gameObject.SetActive(true);
			bool active = Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0;
			base.view.transform.Find("TabBtns/TabBtnVenture/PopUp").gameObject.SetActive(active);
			string key = "VentureTab";
			_tabManager.SetTab(key, component, gameObject);
			SetupVentureTabContent();
			Singleton<IslandModule>.Instance.UnRegisterVentureInProgress();
		}

		private void SetupVentureTabContent()
		{
			_ventureList = Singleton<IslandModule>.Instance.GetVentureList();
			base.view.transform.Find("VentureTab/ScrollView").GetComponent<MonoGridScroller>().Init(OnVentureScrollChange, _ventureList.Count);
			CabinVentureDataItem cabinVentureDataItem = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)5) as CabinVentureDataItem;
			base.view.transform.Find("VentureTab/InfoPanel/InProgress/Num").GetComponent<Text>().text = string.Format("{0}/{1}", Singleton<IslandModule>.Instance.GetVentureInProgressNum(), cabinVentureDataItem.GetMaxVentureNumInProgress());
			base.view.transform.Find("VentureTab/InfoPanel/AutoRefresh/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(TimeUtil.DailyUpdateTime);
		}

		private void SetAllTabVisible(bool isVisible)
		{
			string[] tAB_KEY = TAB_KEY;
			foreach (string name in tAB_KEY)
			{
				base.view.transform.Find(name).gameObject.SetActive(isVisible);
			}
			foreach (Transform item in base.view.transform.Find("TabBtns"))
			{
				item.gameObject.SetActive(isVisible);
			}
		}

		private void OnVentureScrollChange(Transform ventureTrans, int index)
		{
			VentureDataItem ventureData = _ventureList[index];
			ventureTrans.GetComponent<MonoVentureInfoRow>().SetupView(ventureData, OnVentureFetchBtnClick, OnVentureGoBtnClick, OnVentureCancelBtnClick, OnVentureSpeedUpBtnClick);
		}

		private void OnVentureFetchBtnClick(VentureDataItem ventureData)
		{
			_playerLevelBefore = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			Singleton<NetworkManager>.Instance.RequestGetIslandVentureReward(ventureData.VentureID);
		}

		private void OnVentureGoBtnClick(VentureDataItem ventureData)
		{
			Singleton<MainUIManager>.Instance.ShowPage(new VentureDispatchPageContext(ventureData));
		}

		private void OnVentureCancelBtnClick(VentureDataItem ventureData)
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				title = LocalizationGeneralLogic.GetText("Menu_Tips"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_CancelVentureHint", ventureData.GetStaminaReturnOnCancel()),
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						Singleton<NetworkManager>.Instance.RequestCancelDispatchIslandVenture(ventureData.VentureID);
					}
				}
			});
		}

		private void OnVentureSpeedUpBtnClick(VentureDataItem ventureData)
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new VentureSpeedUpDialogContext(ventureData));
		}

		private void OnRefreshVentureListConfirmed(bool isConfirmed)
		{
			if (isConfirmed)
			{
				Singleton<NetworkManager>.Instance.RequestRefreshIslandVenture();
			}
		}

		private void PlayEffect(Transform effectTrans)
		{
			ParticleSystem[] componentsInChildren = effectTrans.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
		}

		private void ShowGetScoinHintDialog(int scoinNum, float burstRate, List<DropItem> dropItems)
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new IslandCollectGotDialogContext(scoinNum, burstRate, dropItems));
		}
	}
}
