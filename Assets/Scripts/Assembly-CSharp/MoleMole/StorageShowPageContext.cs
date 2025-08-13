using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class StorageShowPageContext : BasePageContext
	{
		public enum FeatureType
		{
			Normal = 0,
			SelectForSell = 1,
			SelectForPowerUp = 2
		}

		public const string WEAPON_TAB = "WeaponTab";

		public const string STIGMATA_TAB = "StigmataTab";

		public const string Item_TAB = "ItemTab";

		public const string Fragment_TAB = "FragmentTab";

		public static readonly string[] TAB_KEY = new string[4] { "WeaponTab", "StigmataTab", "ItemTab", "FragmentTab" };

		public string defaultTab = string.Empty;

		public FeatureType featureType;

		public StorageDataItemBase powerUpTarget;

		public List<StorageDataItemBase> selectedResources;

		private TabManager _tabManager;

		private Dictionary<string, List<StorageDataItemBase>> _tabItemList;

		private MonoStorageSelectForSellPanel _sellPanel;

		private MonoStorageSelectForPowerUp _powerUpPanel;

		private Dictionary<GameObject, MonoGridScroller> _scrollerDict;

		private Dictionary<GameObject, MonoScrollerFadeManager> _fadeManagerDict;

		private Dictionary<GameObject, Dictionary<int, RectTransform>> _itemBeforeDict;

		public StorageShowPageContext()
		{
			config = new ContextPattern
			{
				contextName = "StorageShowPageContext",
				viewPrefabPath = "UI/Menus/Page/Storage/StorageShowPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
			_tabItemList = new Dictionary<string, List<StorageDataItemBase>>();
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SetSellViewActive)
			{
				return OnSetSellViewActive((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetStorageSortType)
			{
				return OnSetSortType((StorageModule.StorageSortType)(int)ntf.body);
			}
			if (ntf.type == NotifyTypes.RefreshStorageShowing)
			{
				RefreshShowingTabItem();
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 27:
			case 28:
				SetupView();
				break;
			case 103:
				SetupFragmentTab();
				if (_tabManager.GetShowingTabKey() == "FragmentTab")
				{
					GameObject showingTabContent = _tabManager.GetShowingTabContent();
					if (_fadeManagerDict.ContainsKey(showingTabContent))
					{
						_fadeManagerDict[showingTabContent].Init(_scrollerDict[showingTabContent].GetItemDict(), null, IsStorageItemDataEqual);
						_fadeManagerDict[showingTabContent].Play();
						_itemBeforeDict[showingTabContent] = null;
					}
				}
				break;
			case 40:
			case 136:
				return SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), OnWeaponTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), OnStigmataTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), OnItemTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), OnFragmentTabBtnClick);
			BindViewCallback(base.view.transform.Find("SellBtn").GetComponent<Button>(), OnSellBtnClick);
			BindViewCallback(base.view.transform.Find("SortBtn").GetComponent<Button>(), OnSortBtnClick);
			BindViewCallback(base.view.transform.Find("SortPanel/BG").GetComponent<Button>(), OnSortBGClick);
		}

		protected override bool SetupView()
		{
			InitScroller();
			if (string.IsNullOrEmpty(defaultTab))
			{
				string value = Singleton<MiHoYoGameData>.Instance.LocalData.StorageShowTabName;
				if (string.IsNullOrEmpty(value))
				{
					value = "WeaponTab";
				}
				defaultTab = value;
			}
			SetupWeaponTab();
			SetupStigmataTab();
			SetupItemTab();
			SetupFragmentTab();
			_tabManager.ShowTab(defaultTab);
			SetupSortView(false);
			SetupSellView();
			SetupPowerUpView();
			base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in");
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			GameObject showingTabContent = _tabManager.GetShowingTabContent();
			if (_fadeManagerDict.ContainsKey(showingTabContent))
			{
				_fadeManagerDict[showingTabContent].Init(_scrollerDict[showingTabContent].GetItemDict(), null, IsStorageItemDataEqual);
				_fadeManagerDict[showingTabContent].Play();
				_itemBeforeDict[showingTabContent] = null;
			}
			base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in");
		}

		private void OnWeaponTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = "WeaponTab";
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnStigmataTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = "StigmataTab";
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnItemTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = "ItemTab";
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnFragmentTabBtnClick()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = "FragmentTab";
			_tabManager.ShowTab(text);
			ClearOnChangedTab(showingTabKey, text);
		}

		private void OnSellBtnClick()
		{
			featureType = FeatureType.SelectForSell;
			SetupSellView();
		}

		private void OnSortBtnClick()
		{
			SetupSortView(true);
		}

		private void OnSortBGClick()
		{
			SetupSortView(false);
		}

		private void ClearOnChangedTab(string tabBefore, string tabAfter)
		{
			switch (featureType)
			{
			case FeatureType.Normal:
				defaultTab = tabAfter;
				Singleton<MiHoYoGameData>.Instance.LocalData.StorageShowTabName = tabAfter;
				Singleton<MiHoYoGameData>.Instance.Save();
				break;
			case FeatureType.SelectForSell:
				featureType = FeatureType.Normal;
				RefreshTabItemByKey(tabBefore);
				SetupSellView();
				break;
			case FeatureType.SelectForPowerUp:
				if (_powerUpPanel != null)
				{
					_powerUpPanel.ClearModifyingItem();
					bool isMulti = IsTabItemHeap(_tabManager.GetShowingTabKey());
					_powerUpPanel.RefreshView(isMulti);
					RefreshShowingTabItem();
				}
				break;
			}
		}

		private bool OnSetSortType(StorageModule.StorageSortType sortType)
		{
			SetupSortView(true);
			SortByTab(_tabManager.GetShowingTabKey(), _tabManager.GetShowingTabContent());
			PlayCurrentTabAnimation();
			return false;
		}

		private bool OnSetSellViewActive(bool setActive)
		{
			featureType = (setActive ? FeatureType.SelectForSell : FeatureType.Normal);
			SetupSellView();
			return false;
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
			if (active && _fadeManagerDict.ContainsKey(go))
			{
				_fadeManagerDict[go].Init(_scrollerDict[go].GetItemDict(), null, IsStorageItemDataEqual);
				_fadeManagerDict[go].Play();
				_itemBeforeDict[go] = null;
			}
		}

		private void OnScrollerChange(List<StorageDataItemBase> list, Transform trans, int index)
		{
			StorageDataItemBase storageDataItemBase = list[index];
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.showProtected = true;
			component.blockSelect = false;
			if (featureType == FeatureType.SelectForSell)
			{
				component.blockSelect = !FilterForSell(storageDataItemBase);
			}
			else if (featureType == FeatureType.SelectForPowerUp)
			{
				component.blockSelect = !FilterForPowerUp(storageDataItemBase);
			}
			bool flag = featureType == FeatureType.SelectForSell && _sellPanel != null && _sellPanel.IsItemInSelectedMap(storageDataItemBase);
			bool flag2 = featureType == FeatureType.SelectForPowerUp && _powerUpPanel != null && _powerUpPanel.IsItemInSelectedMap(storageDataItemBase);
			bool bUsed = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(storageDataItemBase.avatarID) != null;
			component.SetupView(storageDataItemBase, MonoItemIconButton.SelectMode.CheckWhenSelect, flag || flag2, false, bUsed);
			component.SetClickCallback(OnItemButonClick);
			if (IsTabItemHeap(_tabManager.GetShowingTabKey()) && flag2)
			{
				component.SetMinusBtnCallBack(OnItemMinusBtnClick);
				component.ShowSelectedNum(_powerUpPanel.GetItemSelectNum(storageDataItemBase));
			}
		}

		private void OnItemButonClick(StorageDataItemBase item, bool selected)
		{
			if (featureType == FeatureType.SelectForSell)
			{
				_sellPanel.RefreshOnItemButonClick(item);
				RefreshShowingTabItem();
			}
			else if (featureType == FeatureType.SelectForPowerUp)
			{
				_powerUpPanel.RefreshOnItemButonClick(item);
				RefreshShowingTabItem();
			}
			else
			{
				UIUtil.ShowItemDetail(item);
			}
		}

		private void OnItemMinusBtnClick(StorageDataItemBase dataItem)
		{
			if (_powerUpPanel != null)
			{
				_powerUpPanel.OnDecreaseBtnClick(dataItem);
			}
		}

		private void SetupTab(string key, Button tabBtn, GameObject tabGo, List<StorageDataItemBase> list)
		{
			if (_tabItemList.ContainsKey(key))
			{
				_tabItemList[key] = list;
			}
			else
			{
				_tabItemList.Add(key, list);
			}
			StorageModule.StorageSortType key2 = Singleton<StorageModule>.Instance.sortTypeMap[key];
			_tabItemList[key].Sort(Singleton<StorageModule>.Instance.sortComparisionMap[key2]);
			SortByTab(key, tabGo);
			_tabManager.SetTab(key, tabBtn, tabGo);
		}

		private void SortByTab(string key, GameObject tabGo)
		{
			StorageModule.StorageSortType key2 = Singleton<StorageModule>.Instance.sortTypeMap[key];
			_tabItemList[key].Sort(Singleton<StorageModule>.Instance.sortComparisionMap[key2]);
			tabGo.transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(delegate(Transform trans, int index)
			{
				OnScrollerChange(_tabItemList[key], trans, index);
			}, _tabItemList[key].Count);
		}

		private void SetupWeaponTab()
		{
			GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.GetAllUserWeapons();
			if (featureType == FeatureType.SelectForPowerUp)
			{
				list = list.FindAll(FilterForPowerUp);
			}
			else if (featureType == FeatureType.SelectForSell)
			{
				list = list.FindAll(FilterForSell);
			}
			SetupTab("WeaponTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("WeaponTab").gameObject, list);
		}

		private void SetupStigmataTab()
		{
			GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.GetAllUserStigmata();
			if (featureType == FeatureType.SelectForPowerUp)
			{
				list = list.FindAll(FilterForPowerUp);
			}
			else if (featureType == FeatureType.SelectForSell)
			{
				list = list.FindAll(FilterForSell);
			}
			SetupTab("StigmataTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("StigmataTab").gameObject, list);
		}

		private void SetupItemTab()
		{
			GameObject gameObject = base.view.transform.Find("ItemTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.GetAllUserMaterial();
			if (featureType == FeatureType.SelectForPowerUp)
			{
				list = list.FindAll(FilterForPowerUp);
			}
			else if (featureType == FeatureType.SelectForSell)
			{
				list = list.FindAll(FilterForSell);
			}
			SetupTab("ItemTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("ItemTab").gameObject, list);
		}

		private void SetupFragmentTab()
		{
			GameObject gameObject = base.view.transform.Find("FragmentTab").gameObject;
			_itemBeforeDict[gameObject] = _scrollerDict[gameObject].GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			if (featureType == FeatureType.SelectForPowerUp)
			{
				base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(false);
				gameObject.SetActive(false);
			}
			else
			{
				base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(true);
				SetupTab("FragmentTab", base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), base.view.transform.Find("FragmentTab").gameObject, Singleton<StorageModule>.Instance.GetFragmentList());
			}
		}

		private bool FilterForPowerUp(StorageDataItemBase item)
		{
			return !item.isProtected && item.avatarID <= 0 && item != powerUpTarget && !Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(item.ID) && (!(item is MaterialDataItem) || ((MaterialDataItem)item).GetGearExp() > 0f);
		}

		private bool FilterForSell(StorageDataItemBase item)
		{
			return !item.isProtected && item.avatarID <= 0 && !Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(item.ID);
		}

		private void SetupSellView()
		{
			_sellPanel = base.view.transform.Find("SellPanel").GetComponent<MonoStorageSelectForSellPanel>();
			base.view.transform.Find("SellBtn").gameObject.SetActive(featureType == FeatureType.Normal);
			base.view.transform.Find("SellPanel").gameObject.SetActive(featureType == FeatureType.SelectForSell);
			if (featureType == FeatureType.SelectForSell)
			{
				bool isMultiSell = IsTabItemHeap(_tabManager.GetShowingTabKey());
				_sellPanel.SetupView(isMultiSell);
			}
			RefreshShowingTabItem();
		}

		private void SetupPowerUpView()
		{
			_powerUpPanel = base.view.transform.Find("PowerUpPanel").GetComponent<MonoStorageSelectForPowerUp>();
			base.view.transform.Find("SellBtn").gameObject.SetActive(featureType == FeatureType.Normal);
			base.view.transform.Find("PowerUpPanel").gameObject.SetActive(featureType == FeatureType.SelectForPowerUp);
			if (featureType == FeatureType.SelectForPowerUp)
			{
				bool isMulti = IsTabItemHeap(_tabManager.GetShowingTabKey());
				_powerUpPanel.GetComponent<MonoStorageSelectForPowerUp>().SetupView(selectedResources, isMulti, powerUpTarget);
			}
			RefreshShowingTabItem();
			base.view.transform.Find("NoItemCanUse").gameObject.SetActive(false);
			if (featureType != FeatureType.SelectForPowerUp)
			{
				return;
			}
			StorageModule instance = Singleton<StorageModule>.Instance;
			bool flag = instance.GetAllUserWeapons().Count > 0 && instance.GetAllUserStigmata().Count > 0;
			if (!flag)
			{
				foreach (StorageDataItemBase item in instance.GetAllUserMaterial())
				{
					if (item.GetGearExp() > 0f)
					{
						flag = true;
						break;
					}
				}
			}
			base.view.transform.Find("NoItemCanUse").gameObject.SetActive(!flag);
		}

		private void SetupSortView(bool sortActive)
		{
			base.view.transform.Find("SortPanel").gameObject.SetActive(sortActive);
			base.view.transform.Find("SortBtn").GetComponent<Button>().interactable = !sortActive;
			if (!sortActive)
			{
				return;
			}
			Transform transform = base.view.transform.Find("SortPanel/Content");
			bool flag = IsTabItemHeap(_tabManager.GetShowingTabKey());
			transform.Find("SortFuncBtnLevel").gameObject.SetActive(!flag);
			transform.Find("SortFuncBtnCost").gameObject.SetActive(!flag);
			transform.Find("SortFuncBtnTime").gameObject.SetActive(!flag);
			MonoStorageSortButton[] componentsInChildren = transform.GetComponentsInChildren<MonoStorageSortButton>();
			MonoStorageSortButton[] array = componentsInChildren;
			foreach (MonoStorageSortButton monoStorageSortButton in array)
			{
				if (monoStorageSortButton.gameObject.activeSelf)
				{
					monoStorageSortButton.SetupView(_tabManager.GetShowingTabKey());
				}
			}
		}

		private bool IsTabItemHeap(string tabKey)
		{
			return tabKey != "WeaponTab" && tabKey != "StigmataTab";
		}

		private void RefreshShowingTabItem()
		{
			_tabManager.GetShowingTabContent().transform.Find("ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
		}

		private void RefreshTabItemByKey(string key)
		{
			_tabManager.GetTabContent(key).transform.Find("ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
		}

		private void InitScroller()
		{
			_scrollerDict = new Dictionary<GameObject, MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("WeaponTab").gameObject] = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("StigmataTab").gameObject] = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("ItemTab").gameObject] = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoGridScroller>();
			_scrollerDict[base.view.transform.Find("FragmentTab").gameObject] = base.view.transform.Find("FragmentTab/ScrollView").GetComponent<MonoGridScroller>();
			_fadeManagerDict = new Dictionary<GameObject, MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("WeaponTab").gameObject] = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("StigmataTab").gameObject] = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("ItemTab").gameObject] = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeManagerDict[base.view.transform.Find("FragmentTab").gameObject] = base.view.transform.Find("FragmentTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_itemBeforeDict = new Dictionary<GameObject, Dictionary<int, RectTransform>>();
			_itemBeforeDict[base.view.transform.Find("WeaponTab").gameObject] = null;
			_itemBeforeDict[base.view.transform.Find("StigmataTab").gameObject] = null;
			_itemBeforeDict[base.view.transform.Find("ItemTab").gameObject] = null;
			_itemBeforeDict[base.view.transform.Find("FragmentTab").gameObject] = null;
		}

		private bool IsStorageItemDataEqual(RectTransform dataNew, RectTransform dataOld)
		{
			if (dataNew == null || dataOld == null)
			{
				return false;
			}
			MonoItemIconButton component = dataOld.GetComponent<MonoItemIconButton>();
			MonoItemIconButton component2 = dataNew.GetComponent<MonoItemIconButton>();
			return component2._item == component._item;
		}

		private void PlayCurrentTabAnimation()
		{
			GameObject showingTabContent = _tabManager.GetShowingTabContent();
			if (_fadeManagerDict.ContainsKey(showingTabContent))
			{
				_fadeManagerDict[showingTabContent].Init(_scrollerDict[showingTabContent].GetItemDict(), null, IsStorageItemDataEqual);
				_fadeManagerDict[showingTabContent].Play();
				_itemBeforeDict[showingTabContent] = null;
			}
		}
	}
}
