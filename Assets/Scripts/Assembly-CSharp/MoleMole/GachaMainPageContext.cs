using System;
using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class GachaMainPageContext : BasePageContext
	{
		public enum GachaAmountType
		{
			GachaOne = 0,
			GachaTen = 1
		}

		public class WaitGachaRsp
		{
			public bool _waiting;

			public GachaType _gachaType;

			public GachaAmountType _amountType;

			public Action<GachaType, GachaAmountType> _callback;

			public void Start(GachaType gacha, GachaAmountType amount, Action<GachaType, GachaAmountType> callback)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				if (!_waiting)
				{
					_waiting = true;
					_gachaType = gacha;
					_amountType = amount;
					_callback = callback;
					ShowWheel();
				}
			}

			public void End()
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if (_waiting)
				{
					_waiting = false;
					_callback(_gachaType, _amountType);
				}
			}

			private void ShowWheel()
			{
				LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext(59);
				loadingWheelWidgetContext.ignoreMaxWaitTime = true;
				Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
			}
		}

		public const string HCOIN_TAB = "HcoinTab";

		public const string SPECIAL_TAB = "SpecialTab";

		public const string FRIEND_TAB = "FriendTab";

		private List<StorageDataItemBase> _gachaGotList;

		private List<GachaItem> _gachaItemList;

		private SequenceDialogManager _dropItemShowDialogManager;

		private SequenceDialogManager _unLockAvatarDialogManager;

		private GachaType _currentGachaType;

		private TabManager _tabManager;

		private GachaDisplayInfo _displayInfo;

		private string _currentTabKey = "HcoinTab";

		private int _cost;

		private WaitGachaRsp _waitGachaRsp;

		private int[] _avatarCardIDs = new int[12]
		{
			60101, 60102, 60103, 60104, 60201, 60202, 60203, 60204, 60301, 60302,
			60303, 60304
		};

		public GachaMainPageContext()
		{
			config = new ContextPattern
			{
				contextName = "GachaMainPageContext",
				viewPrefabPath = "UI/Menus/Page/Gacha/GachaMainPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_gachaGotList = new List<StorageDataItemBase>();
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_waitGachaRsp = new WaitGachaRsp();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 63:
				return OnGachaDisplayInfoGot(pkt.getData<GetGachaDisplayRsp>());
			case 59:
				return OnGachaRsp(pkt.getData<GachaRsp>());
			case 216:
				return SetupView();
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.DownloadResAssetSucc)
			{
				return SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), OnHcoinTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), OnSpecialTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), OnFriendTabBtnClick);
			BindViewCallback(base.view.transform.Find("HCoinTab/ActBtns/One/Btn").GetComponent<Button>(), OnHCoinOneGachaBtnClick);
			BindViewCallback(base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn").GetComponent<Button>(), OnHcoinTenGachaBtnClick);
			BindViewCallback(base.view.transform.Find("SpecialTab/ActBtns/One/Btn").GetComponent<Button>(), OnSpecialOneGachaBtnClick);
			BindViewCallback(base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn").GetComponent<Button>(), OnSpecialTenGachaBtnClick);
			BindViewCallback(base.view.transform.Find("FriendPointTab/ActBtns/One/Btn").GetComponent<Button>(), OnFriendOneGachaBtnClick);
			BindViewCallback(base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn").GetComponent<Button>(), OnFriendTenGachaBtnClick);
			BindViewCallback(base.view.transform.Find("HCoinTab/SupplyImg/Pic").GetComponent<Button>(), OnHcoinDetailBtnClick);
			BindViewCallback(base.view.transform.Find("SpecialTab/SupplyImg/Pic").GetComponent<Button>(), OnSpecialDetailBtnClick);
		}

		protected override bool SetupView()
		{
			_displayInfo = Singleton<GachaModule>.Instance.GachaDisplay;
			if (_displayInfo == null)
			{
				base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
				base.view.transform.Find("HCoinTab").gameObject.SetActive(false);
				base.view.transform.Find("SpecialTab").gameObject.SetActive(false);
				base.view.transform.Find("FriendPointTab").gameObject.SetActive(false);
				base.view.transform.Find("TabBtns").gameObject.SetActive(false);
				Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
				return false;
			}
			base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
			UpdateView();
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			bool flag = _displayInfo.specialGachaData != null && TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(_displayInfo.specialGachaData.data_expire_time);
			base.view.transform.Find("SpecialTab").gameObject.SetActive(flag && _currentTabKey == "SpecialTab");
			base.view.transform.Find("TabBtns/TabBtn_2").gameObject.SetActive(flag);
			base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
			if (_unLockAvatarDialogManager != null && _unLockAvatarDialogManager.GetDialogNum() > 0)
			{
				_unLockAvatarDialogManager.StartShow();
			}
			LetterSpacing[] componentsInChildren = base.view.transform.GetComponentsInChildren<LetterSpacing>();
			foreach (LetterSpacing letterSpacing in componentsInChildren)
			{
				if (letterSpacing.autoFixLine)
				{
					letterSpacing.AccommodateText();
				}
			}
		}

		private void OnHcoinTabBtnClick()
		{
			_currentTabKey = "HcoinTab";
			_tabManager.ShowTab(_currentTabKey);
		}

		private void OnSpecialTabBtnClick()
		{
			_currentTabKey = "SpecialTab";
			_tabManager.ShowTab(_currentTabKey);
		}

		private void OnFriendTabBtnClick()
		{
			_currentTabKey = "FriendTab";
			_tabManager.ShowTab(_currentTabKey);
		}

		private void OnHCoinOneGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)2;
			int ticket_material_id = (int)_displayInfo.hcoinGachaData.ticket_material_id;
			if (TicketConditionCheck(ticket_material_id, 1))
			{
				_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaOne, TriggerStageGachaBox);
				Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, 1);
				_cost = 1;
			}
		}

		private void OnHcoinTenGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)2;
			int ticket_material_id = (int)_displayInfo.hcoinGachaData.ticket_material_id;
			if (TicketConditionCheck(ticket_material_id, 10))
			{
				_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaTen, TriggerStageGachaBox);
				Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, 10);
				_cost = 10;
			}
		}

		private void OnSpecialOneGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)3;
			if (_displayInfo.specialGachaData != null && TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(_displayInfo.specialGachaData.data_expire_time))
			{
				int ticket_material_id = (int)_displayInfo.specialGachaData.ticket_material_id;
				if (TicketConditionCheck(ticket_material_id, 1))
				{
					_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaOne, TriggerStageGachaBox);
					Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, 1);
					_cost = 1;
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint")));
				Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
			}
		}

		private void OnSpecialTenGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)3;
			if (_displayInfo.specialGachaData != null && TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(_displayInfo.specialGachaData.data_expire_time))
			{
				int ticket_material_id = (int)_displayInfo.specialGachaData.ticket_material_id;
				if (TicketConditionCheck(ticket_material_id, 10))
				{
					_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaTen, TriggerStageGachaBox);
					Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, 10);
					_cost = 10;
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint")));
				Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
			}
		}

		private void OnFriendOneGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)1;
			int friends_point_cost = (int)_displayInfo.friendPointGachaData.friends_point_cost;
			if (FriendConditionCheck(friends_point_cost))
			{
				_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaOne, TriggerStageGachaBox);
				Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, 1);
				_cost = friends_point_cost;
			}
		}

		private void OnFriendTenGachaBtnClick()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			_currentGachaType = (GachaType)1;
			int cost;
			int maxFriendPointGachaTime = GetMaxFriendPointGachaTime(out cost);
			if (FriendConditionCheck(cost))
			{
				_waitGachaRsp.Start(_currentGachaType, GachaAmountType.GachaTen, TriggerStageGachaBox);
				Singleton<NetworkManager>.Instance.RequestGacha(_currentGachaType, maxFriendPointGachaTime);
				_cost = cost;
			}
		}

		private void OnSpecialDetailBtnClick()
		{
			if (_displayInfo.specialGachaData != null && TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(_displayInfo.specialGachaData.data_expire_time))
			{
				Singleton<MainUIManager>.Instance.ShowPage(new GachaDetailPageContext(_displayInfo.specialGachaData.common_data, 3));
				return;
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint")));
			Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
		}

		private void OnHcoinDetailBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new GachaDetailPageContext(_displayInfo.hcoinGachaData.common_data, 2));
		}

		public bool OnGachaDisplayInfoGot(GetGachaDisplayRsp rsp)
		{
			SetupView();
			TryToDoTutorial();
			return false;
		}

		private bool OnGachaRsp(GachaRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				_gachaGotList.Clear();
				_gachaItemList = rsp.item_list;
				_dropItemShowDialogManager = new SequenceDialogManager(ShowGachaResultPage);
				_unLockAvatarDialogManager = new SequenceDialogManager(ClearUnlockAvatarDialogManagerContent);
				foreach (GachaItem item in rsp.item_list)
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.item_id, (int)item.level);
					if (dummyStorageDataItem == null)
					{
						continue;
					}
					dummyStorageDataItem.number = (int)item.num;
					if (dummyStorageDataItem is AvatarCardDataItem)
					{
						if (item.split_fragment_numSpecified)
						{
							(dummyStorageDataItem as AvatarCardDataItem).SpliteToFragment((int)item.split_fragment_num);
						}
						else
						{
							AvatarCardDataItem avatarCardDataItem = dummyStorageDataItem as AvatarCardDataItem;
							int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarCardDataItem.ID).avatarID;
							AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(avatarID, true);
							_unLockAvatarDialogManager.AddDialog(dialogContext);
						}
					}
					_gachaGotList.Add(dummyStorageDataItem);
					List<Tuple<StorageDataItemBase, bool>> list = new List<Tuple<StorageDataItemBase, bool>>();
					list.Add(new Tuple<StorageDataItemBase, bool>(dummyStorageDataItem, item.is_rare_drop));
					if (item.gift_item_idSpecified)
					{
						StorageDataItemBase dummyStorageDataItem2 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.gift_item_id, (int)item.gift_level);
						if (dummyStorageDataItem2 != null)
						{
							dummyStorageDataItem2.number = (int)item.gift_num;
							list.Add(new Tuple<StorageDataItemBase, bool>(dummyStorageDataItem2, false));
							_gachaGotList.Add(dummyStorageDataItem2);
						}
					}
					_dropItemShowDialogManager.AddDialog(new DropNewItemDialogContextV2(list));
				}
				_waitGachaRsp.End();
			}
			else if ((int)rsp.retcode == 3)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Return_GachaTitcketLack"),
					desc = LocalizationGeneralLogic.GetText("Menu_Return_GachaTitcketLack")
				});
			}
			UpdateView();
			return false;
		}

		private void TriggerStageGachaBox(GachaType type, GachaAmountType gachaAmountType)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
			Singleton<MainUIManager>.Instance.SceneCanvas.gameObject.SetActive(false);
			GameObject original = Miscs.LoadResource<GameObject>("Entities/DynamicObject/StageGacha3D");
			MonoStageGacha component = UnityEngine.Object.Instantiate(original).GetComponent<MonoStageGacha>();
			GameObject uiCamera = GameObject.Find("UICamera");
			GameObject spaceShipObj = Singleton<MainUIManager>.Instance.GetMainCanvas().GetSpaceShipObj();
			SuperDebug.VeryImportantAssert(spaceShipObj != null, SpaceshipNullAssertFailedCallback);
			component.Init(spaceShipObj, uiCamera, type, gachaAmountType, BeginDropAnimationAfterBoxAnimation);
		}

		private string SpaceshipNullAssertFailedCallback()
		{
			return "spaceship is null when TriggerStageGachaBox currentCanvas: " + Singleton<MainUIManager>.Instance.GetMainCanvas().ToString() + " currentPage: " + Singleton<MainUIManager>.Instance.CurrentPageContext.ToString() + " pageStack: " + Singleton<MainUIManager>.Instance.GetAllPageNamesInStack();
		}

		private void BeginDropAnimationAfterBoxAnimation()
		{
			Singleton<MainUIManager>.Instance.SceneCanvas.gameObject.SetActive(true);
			_dropItemShowDialogManager.StartShow();
		}

		private void ShowGachaResultPage()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Singleton<MainUIManager>.Instance.ShowPage(new GachaResultPageContext(_displayInfo, _currentGachaType, _gachaGotList, _gachaItemList, _cost));
		}

		private void ClearUnlockAvatarDialogManagerContent()
		{
			if (_unLockAvatarDialogManager != null)
			{
				_unLockAvatarDialogManager.ClearDialogs();
			}
		}

		private void ShowAppStoreCommentPage()
		{
			if (_gachaItemList == null || _gachaItemList.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (GachaItem gachaItem in _gachaItemList)
			{
				if (gachaItem != null && gachaItem.is_rare_dropSpecified && gachaItem.is_rare_drop)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				int aPP_STORE_COMMENT_ID_ = CommonIDModule.APP_STORE_COMMENT_ID_1;
				UIUtil.ShowAppStoreComment(aPP_STORE_COMMENT_ID_);
			}
		}

		private void OnTabSetActive(bool selected, GameObject contentGo, Button tabButton)
		{
			tabButton.interactable = !selected;
			tabButton.transform.Find("Unselect").gameObject.SetActive(!selected);
			tabButton.transform.Find("Select").gameObject.SetActive(selected);
			contentGo.SetActive(selected);
			if (!selected)
			{
				return;
			}
			LetterSpacing[] componentsInChildren = contentGo.GetComponentsInChildren<LetterSpacing>();
			foreach (LetterSpacing letterSpacing in componentsInChildren)
			{
				if (letterSpacing.autoFixLine)
				{
					letterSpacing.AccommodateText();
				}
			}
		}

		private void SetupHcoinTab()
		{
			HcoinGachaData hcoinGachaData = _displayInfo.hcoinGachaData;
			StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID((int)hcoinGachaData.ticket_material_id);
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)hcoinGachaData.ticket_material_id);
			string gachaTicketIconPath = MiscData.GetGachaTicketIconPath((int)hcoinGachaData.ticket_material_id);
			Sprite sprite = ((!string.IsNullOrEmpty(gachaTicketIconPath)) ? Miscs.GetSpriteByPrefab(gachaTicketIconPath) : null);
			if (sprite == null)
			{
				sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
			}
			base.view.transform.Find("HCoinTab/InfoPanel/Title/Time").GetComponent<Text>().text = hcoinGachaData.common_data.title;
			base.view.transform.Find("HCoinTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(hcoinGachaData.common_data.content);
			base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1]);
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[10]);
			base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + 1;
			base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Cost/Icon").GetComponent<Image>().sprite = sprite;
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + 10;
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Cost/Icon").GetComponent<Image>().sprite = sprite;
			base.view.transform.Find("HCoinTab/ActBtns/One/Scoin").gameObject.SetActive(false);
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Scoin").gameObject.SetActive(false);
			base.view.transform.Find("HCoinTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_HCoinGachaNote");
			base.view.transform.Find("HCoinTab/ActBtns/One/Added/Num").GetComponent<Text>().text = "2";
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_HCoinGachaNote");
			base.view.transform.Find("HCoinTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = "20";
			base.view.transform.Find("HCoinTab/TicketRemain/Num").GetComponent<Text>().text = ((storageDataItemBase != null) ? storageDataItemBase.number.ToString() : "0");
			base.view.transform.Find("HCoinTab/TicketRemain/Icon").GetComponent<Image>().sprite = sprite;
			UIUtil.TrySetupEventSprite(base.view.transform.Find("HCoinTab/SupplyImg/Pic").GetComponent<Image>(), hcoinGachaData.common_data.supply_image);
			UIUtil.TrySetupEventSprite(base.view.transform.Find("HCoinTab/InfoPanel/Title/Image").GetComponent<Image>(), hcoinGachaData.common_data.title_image);
			base.view.transform.Find("HCoinTab/RemainTime").gameObject.SetActive(hcoinGachaData.data_expire_timeSpecified);
			if (hcoinGachaData.data_expire_timeSpecified)
			{
				base.view.transform.Find("HCoinTab/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(Miscs.GetDateTimeFromTimeStamp(hcoinGachaData.data_expire_time), null, OnGachaDisplayDataExpired);
			}
			_tabManager.SetTab("HcoinTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("HCoinTab").gameObject);
		}

		private void SetupSpeicalTab()
		{
			HcoinGachaData specialGachaData = _displayInfo.specialGachaData;
			bool flag = specialGachaData != null && TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(specialGachaData.data_expire_time);
			base.view.transform.Find("SpecialTab").gameObject.SetActive(flag);
			base.view.transform.Find("TabBtns/TabBtn_2").gameObject.SetActive(flag);
			if (!flag)
			{
				if (_currentTabKey == "SpecialTab")
				{
					_currentTabKey = "HcoinTab";
				}
				return;
			}
			StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID((int)specialGachaData.ticket_material_id);
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)specialGachaData.ticket_material_id);
			string gachaTicketIconPath = MiscData.GetGachaTicketIconPath((int)specialGachaData.ticket_material_id);
			Sprite sprite = ((!string.IsNullOrEmpty(gachaTicketIconPath)) ? Miscs.GetSpriteByPrefab(gachaTicketIconPath) : null);
			if (sprite == null)
			{
				sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
			}
			base.view.transform.Find("SpecialTab/InfoPanel/Title/Time").GetComponent<Text>().text = specialGachaData.common_data.title;
			base.view.transform.Find("SpecialTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(specialGachaData.common_data.content);
			base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1]);
			base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[10]);
			base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + 1;
			base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Cost/Icon").GetComponent<Image>().sprite = sprite;
			base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + 10;
			base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Cost/Icon").GetComponent<Image>().sprite = sprite;
			base.view.transform.Find("SpecialTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_SpecialGachaNote");
			base.view.transform.Find("SpecialTab/ActBtns/One/Added/Num").GetComponent<Text>().text = "2";
			base.view.transform.Find("SpecialTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_SpecialGachaNote");
			base.view.transform.Find("SpecialTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = "20";
			base.view.transform.Find("SpecialTab/TicketRemain/Num").GetComponent<Text>().text = ((storageDataItemBase != null) ? storageDataItemBase.number.ToString() : "0");
			base.view.transform.Find("SpecialTab/TicketRemain/Icon").GetComponent<Image>().sprite = sprite;
			UIUtil.TrySetupEventSprite(base.view.transform.Find("SpecialTab/SupplyImg/Pic").GetComponent<Image>(), specialGachaData.common_data.supply_image);
			UIUtil.TrySetupEventSprite(base.view.transform.Find("SpecialTab/InfoPanel/Title/Image").GetComponent<Image>(), specialGachaData.common_data.title_image);
			base.view.transform.Find("SpecialTab/RemainTime").gameObject.SetActive(specialGachaData.data_expire_timeSpecified);
			if (specialGachaData.data_expire_timeSpecified)
			{
				base.view.transform.Find("SpecialTab/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(Miscs.GetDateTimeFromTimeStamp(specialGachaData.data_expire_time), null, OnGachaDisplayDataExpired);
			}
			_tabManager.SetTab("SpecialTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("SpecialTab").gameObject);
		}

		private void SetupFriendTab()
		{
			FriendsPointGachaData friendPointGachaData = _displayInfo.friendPointGachaData;
			base.view.transform.Find("FriendPointTab/InfoPanel/Title/Time").GetComponent<Text>().text = friendPointGachaData.common_data.title;
			base.view.transform.Find("FriendPointTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(friendPointGachaData.common_data.content);
			base.view.transform.Find("FriendPointTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + friendPointGachaData.friends_point_cost;
			int cost;
			int maxFriendPointGachaTime = GetMaxFriendPointGachaTime(out cost);
			base.view.transform.Find("FriendPointTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1]);
			base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[maxFriendPointGachaTime]);
			base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + cost;
			base.view.transform.Find("FriendPointTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = maxFriendPointGachaTime.ToString();
			base.view.transform.Find("FriendPointTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FriendGachaNote");
			base.view.transform.Find("FriendPointTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FriendGachaNote");
			base.view.transform.Find("FriendPointTab/FriendPointRemain/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.friendsPoint.ToString();
			UIUtil.TrySetupEventSprite(base.view.transform.Find("FriendPointTab/SupplyImg/Pic").GetComponent<Image>(), friendPointGachaData.common_data.supply_image);
			UIUtil.TrySetupEventSprite(base.view.transform.Find("FriendPointTab/InfoPanel/Title/Image").GetComponent<Image>(), friendPointGachaData.common_data.title_image);
			_tabManager.SetTab("FriendTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("FriendPointTab").gameObject);
		}

		private bool TicketConditionCheck(int ticketID, int num)
		{
			if (IsStorageFull())
			{
				return false;
			}
			MaterialDataItem materialDataItem = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(ticketID);
			if (materialDataItem == null || materialDataItem.number < num)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GachaTicketLackDialogContext(ticketID, num));
				return false;
			}
			return true;
		}

		private bool FriendConditionCheck(int needFriendPoint)
		{
			if (IsStorageFull())
			{
				return false;
			}
			if (Singleton<PlayerModule>.Instance.playerData.friendsPoint < needFriendPoint)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					desc = LocalizationGeneralLogic.GetText("Err_FriendPointShortage")
				});
				return false;
			}
			return true;
		}

		private bool IsStorageFull()
		{
			if (Singleton<StorageModule>.Instance.IsFull())
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ClearStorageHintDialog());
				return true;
			}
			return false;
		}

		private int GetMaxFriendPointGachaTime(out int cost)
		{
			int friendsPoint = Singleton<PlayerModule>.Instance.playerData.friendsPoint;
			int friends_point_cost = (int)_displayInfo.friendPointGachaData.friends_point_cost;
			int num = friendsPoint / friends_point_cost;
			if (num > 10)
			{
				num = 10;
			}
			else if (num < 2)
			{
				num = 2;
			}
			cost = friends_point_cost * num;
			return num;
		}

		private void HackGachaItems(List<GachaItem> itemList)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			itemList.Clear();
			for (int i = 0; i < _avatarCardIDs.Length; i++)
			{
				GachaItem val = new GachaItem();
				val.item_id = (uint)_avatarCardIDs[i];
				itemList.Add(val);
			}
		}

		private bool UpdateView()
		{
			base.view.transform.Find("TabBtns").gameObject.SetActive(true);
			SetupHcoinTab();
			SetupSpeicalTab();
			SetupFriendTab();
			_tabManager.ShowTab(_currentTabKey);
			return false;
		}

		private void OnGachaDisplayDataExpired()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint")));
			Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
		}
	}
}
