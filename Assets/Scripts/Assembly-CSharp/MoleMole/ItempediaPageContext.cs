using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class ItempediaPageContext : BasePageContext
	{
		private class TabInfo
		{
			public List<ItempediaDataAdapter> itemList;

			public MonoGridScroller scroller;

			public MonoScrollerFadeManager fadeManager;

			public StorageModule.StorageSortType sortType = StorageModule.StorageSortType.Rarity_ASC;
		}

		private class MemorizeInfo
		{
			public string lastTabName;
		}

		private const string WEAPON_TAB = "WeaponTab";

		private const string STIGMATA_TAB = "StigmataTab";

		private const string ITEM_TAB = "ItemTab";

		private static MemorizeInfo _tabMemorizedInfo;

		public string defaultTab = "WeaponTab";

		private TabManager _tabManager;

		private TabInfo _weaponTabInfo = new TabInfo();

		private TabInfo _stigmataTabInfo = new TabInfo();

		private TabInfo _itemTabInfo = new TabInfo();

		private TabInfo _currentTabInfo;

		private Button _sortButton;

		private GameObject _sortPanel;

		private MonoItempediaSortButton _sortButtonRarity;

		private MonoItempediaSortButton _sortButtonType;

		private MonoItempediaSortButton _sortButtonLevel;

		private MonoItempediaSortButton _sortButtonCost;

		private MonoItempediaSortButton _sortButtonSuite;

		public ItempediaPageContext()
		{
			config = new ContextPattern
			{
				contextName = "ItempediaPageContext",
				viewPrefabPath = "UI/Menus/Page/Itempedia/ItempediaPage"
			};
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), OnWeaponTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), OnStigmataTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), OnItemTabBtnClick);
			BindViewCallback(base.view.transform.Find("SortBtn").GetComponent<Button>(), OnSortBtnClick);
			BindViewCallback(base.view.transform.Find("SortPanel/BG").GetComponent<Button>(), OnSortBGClick);
		}

		protected override bool SetupView()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : "WeaponTab");
			_tabManager.Clear();
			GameObject gameObject = null;
			Button button = null;
			gameObject = base.view.transform.Find("WeaponTab").gameObject;
			button = base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>();
			_tabManager.SetTab("WeaponTab", button, gameObject);
			gameObject = base.view.transform.Find("StigmataTab").gameObject;
			button = base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>();
			_tabManager.SetTab("StigmataTab", button, gameObject);
			gameObject = base.view.transform.Find("ItemTab").gameObject;
			button = base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>();
			_tabManager.SetTab("ItemTab", button, gameObject);
			_tabManager.ShowTab(text);
			base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in");
			_sortButtonRarity = base.view.transform.Find("SortPanel/Content/SortFuncBtnRarity").GetComponent<MonoItempediaSortButton>();
			_sortButtonType = base.view.transform.Find("SortPanel/Content/SortFuncBtnType").GetComponent<MonoItempediaSortButton>();
			_sortButtonLevel = base.view.transform.Find("SortPanel/Content/SortFuncBtnLevel").GetComponent<MonoItempediaSortButton>();
			_sortButtonCost = base.view.transform.Find("SortPanel/Content/SortFuncBtnCost").GetComponent<MonoItempediaSortButton>();
			_sortButtonSuite = base.view.transform.Find("SortPanel/Content/SortFuncBtnSuite").GetComponent<MonoItempediaSortButton>();
			_sortButton = base.view.transform.Find("SortBtn").GetComponent<Button>();
			_sortPanel = base.view.transform.Find("SortPanel").gameObject;
			_sortButtonRarity.SetClickCallback(OnSortByRarityClicked);
			_sortButtonType.SetClickCallback(OnSortByTypeClicked);
			_sortButtonLevel.SetClickCallback(OnSortByLevelClicked);
			_sortButtonCost.SetClickCallback(OnSortByCostClicked);
			_sortButtonSuite.SetClickCallback(OnSortBySuiteClicked);
			_weaponTabInfo.scroller = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoGridScroller>();
			_stigmataTabInfo.scroller = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoGridScroller>();
			_itemTabInfo.scroller = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoGridScroller>();
			_weaponTabInfo.fadeManager = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_stigmataTabInfo.fadeManager = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_itemTabInfo.fadeManager = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_stigmataTabInfo.sortType = StorageModule.StorageSortType.Suite_ASC;
			SetupTabDataList(_weaponTabInfo, WeaponMetaDataReader.GetItemList().ToArray());
			SetupTabDataList(_stigmataTabInfo, StigmataMetaDataReader.GetItemList().ToArray());
			SetupTabDataList(_itemTabInfo, ItemMetaDataReader.GetItemList().ToArray());
			SortItems(_weaponTabInfo);
			SortItems(_stigmataTabInfo);
			SortItems(_itemTabInfo);
			SetupTab(_weaponTabInfo);
			SetupTab(_stigmataTabInfo);
			SetupTab(_itemTabInfo);
			switch ((_tabMemorizedInfo != null) ? _tabMemorizedInfo.lastTabName : text)
			{
			case "WeaponTab":
				OnWeaponTabBtnClick();
				break;
			case "StigmataTab":
				OnStigmataTabBtnClick();
				break;
			case "ItemTab":
				OnItemTabBtnClick();
				break;
			}
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in");
			PlayCurrentTabAnimation();
		}

		private void SetMemorizeTabName(string name)
		{
			if (_tabMemorizedInfo == null)
			{
				_tabMemorizedInfo = new MemorizeInfo();
			}
			_tabMemorizedInfo.lastTabName = name;
		}

		private void SetCollection(string tabName)
		{
			GameObject gameObject = base.view.transform.Find("Collection").gameObject;
			Text component = base.view.transform.Find("Collection/Text").GetComponent<Text>();
			Text component2 = base.view.transform.Find("Collection/Title").GetComponent<Text>();
			int num = 0;
			int num2 = 0;
			switch (tabName)
			{
			case "WeaponTab":
				gameObject.SetActive(true);
				num = Singleton<ItempediaModule>.Instance.GetUnlockCountWeapon();
				num2 = Singleton<ItempediaModule>.Instance.GetAllWeaponCount();
				component2.text = LocalizationGeneralLogic.GetText("Menu_Desc_WeaponIndexCompleteness");
				break;
			case "StigmataTab":
				gameObject.SetActive(true);
				num = Singleton<ItempediaModule>.Instance.GetUnlockCountStigmata();
				num2 = Singleton<ItempediaModule>.Instance.GetAllStigmataCount();
				component2.text = LocalizationGeneralLogic.GetText("Menu_Desc_StigmataIndexCompleteness");
				break;
			case "ItemTab":
				gameObject.SetActive(false);
				break;
			}
			component.text = string.Format("{0}/{1}", num.ToString(), num2.ToString());
		}

		private void OnWeaponTabBtnClick()
		{
			_currentTabInfo = _weaponTabInfo;
			_tabManager.ShowTab("WeaponTab");
			SetMemorizeTabName("WeaponTab");
			PlayCurrentTabAnimation();
			SetCollection("WeaponTab");
		}

		private void OnStigmataTabBtnClick()
		{
			_currentTabInfo = _stigmataTabInfo;
			_tabManager.ShowTab("StigmataTab");
			SetMemorizeTabName("StigmataTab");
			PlayCurrentTabAnimation();
			SetCollection("StigmataTab");
		}

		private void OnItemTabBtnClick()
		{
			_currentTabInfo = _itemTabInfo;
			_tabManager.ShowTab("ItemTab");
			SetMemorizeTabName("ItemTab");
			PlayCurrentTabAnimation();
			SetCollection("ItemTab");
		}

		private void OnSortBtnClick()
		{
			SetupSortView(true);
		}

		private void OnSortBGClick()
		{
			SetupSortView(false);
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void OnScrollerChange(List<int> list, Transform trans, int index)
		{
		}

		private void SetupSortView(bool sortActive)
		{
			_sortPanel.SetActive(sortActive);
			_sortButton.interactable = !sortActive;
			if (sortActive)
			{
				string showingTabKey = _tabManager.GetShowingTabKey();
				bool flag = IsTabItemHeap(showingTabKey);
				_sortButtonLevel.gameObject.SetActive(false);
				_sortButtonCost.gameObject.SetActive(!flag);
				_sortButtonSuite.gameObject.SetActive(showingTabKey == "StigmataTab");
				_sortButtonRarity.SetupView((_currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC) | (_currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_DESC), _currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC);
				_sortButtonType.SetupView((_currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC) | (_currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_DESC), _currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC);
				_sortButtonLevel.SetupView((_currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC) | (_currentTabInfo.sortType == StorageModule.StorageSortType.Level_DESC), _currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC);
				_sortButtonCost.SetupView((_currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC) | (_currentTabInfo.sortType == StorageModule.StorageSortType.Cost_DESC), _currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC);
				_sortButtonSuite.SetupView((_currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC) | (_currentTabInfo.sortType == StorageModule.StorageSortType.Suite_DESC), _currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC);
			}
		}

		private bool IsTabItemHeap(string tabKey)
		{
			return tabKey != "WeaponTab" && tabKey != "StigmataTab";
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
			if (_currentTabInfo != null)
			{
				_currentTabInfo.fadeManager.Init(_currentTabInfo.scroller.GetItemDict(), null, IsStorageItemDataEqual);
				_currentTabInfo.fadeManager.Play();
			}
		}

		private bool IsItempediaDataEqual(RectTransform dataNew, RectTransform dataOld)
		{
			if (dataNew == null || dataOld == null)
			{
				return false;
			}
			MonoItempediaIconButton component = dataOld.GetComponent<MonoItempediaIconButton>();
			MonoItempediaIconButton component2 = dataNew.GetComponent<MonoItempediaIconButton>();
			return component2._item == component._item;
		}

		private void OnSortByRarityClicked()
		{
			if (_currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC)
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Rarity_DESC;
			}
			else
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Rarity_ASC;
			}
			SortItems(_currentTabInfo);
			SetupTab(_currentTabInfo);
			SetupSortView(true);
			PlayCurrentTabAnimation();
		}

		private void OnSortByTypeClicked()
		{
			if (_currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC)
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.BaseType_DESC;
			}
			else
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.BaseType_ASC;
			}
			SortItems(_currentTabInfo);
			SetupTab(_currentTabInfo);
			SetupSortView(true);
			PlayCurrentTabAnimation();
		}

		private void OnSortByLevelClicked()
		{
			if (_currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC)
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Level_DESC;
			}
			else
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Level_ASC;
			}
			SortItems(_currentTabInfo);
			SetupTab(_currentTabInfo);
			SetupSortView(true);
			PlayCurrentTabAnimation();
		}

		private void OnSortByCostClicked()
		{
			if (_currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC)
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Cost_DESC;
			}
			else
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Cost_ASC;
			}
			SortItems(_currentTabInfo);
			SetupTab(_currentTabInfo);
			SetupSortView(true);
			PlayCurrentTabAnimation();
		}

		private void OnSortBySuiteClicked()
		{
			if (_currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC)
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Suite_DESC;
			}
			else
			{
				_currentTabInfo.sortType = StorageModule.StorageSortType.Suite_ASC;
			}
			SortItems(_currentTabInfo);
			SetupTab(_currentTabInfo);
			SetupSortView(true);
			PlayCurrentTabAnimation();
		}

		private void SetupTabDataList(TabInfo info, object[] metaDataList)
		{
			if (metaDataList == null)
			{
				return;
			}
			info.itemList = new List<ItempediaDataAdapter>(metaDataList.Length);
			int i = 0;
			for (int num = metaDataList.Length; i < num; i++)
			{
				ItempediaDataAdapter itempediaDataAdapter = new ItempediaDataAdapter(metaDataList[i]);
				if (Singleton<ItempediaModule>.Instance.IsInItempedia(itempediaDataAdapter.ID))
				{
					info.itemList.Add(itempediaDataAdapter);
				}
			}
		}

		private void SetupTab(TabInfo info)
		{
			if (info.itemList != null)
			{
				SetupScrollView(info.itemList, info.scroller);
			}
		}

		private void SortItems(TabInfo tabInfo)
		{
			switch (tabInfo.sortType)
			{
			case StorageModule.StorageSortType.Rarity_ASC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToRarityAsc);
				break;
			case StorageModule.StorageSortType.Rarity_DESC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToRarityDesc);
				break;
			case StorageModule.StorageSortType.BaseType_ASC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToBaseTypeAsc);
				break;
			case StorageModule.StorageSortType.BaseType_DESC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToBaseTypeDesc);
				break;
			case StorageModule.StorageSortType.Level_ASC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToLevelAsc);
				break;
			case StorageModule.StorageSortType.Level_DESC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToLevelDesc);
				break;
			case StorageModule.StorageSortType.Cost_ASC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToCostAsc);
				break;
			case StorageModule.StorageSortType.Cost_DESC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToCostDesc);
				break;
			case StorageModule.StorageSortType.Suite_ASC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToSuiteAsc);
				break;
			case StorageModule.StorageSortType.Suite_DESC:
				tabInfo.itemList.Sort(ItempediaDataAdapter.CompareToSuiteDesc);
				break;
			}
		}

		private void SetupScrollView(List<ItempediaDataAdapter> dataList, MonoGridScroller scroller)
		{
			scroller.Init(delegate(Transform t, int i)
			{
				MonoItempediaIconButton component = t.GetComponent<MonoItempediaIconButton>();
				ItempediaDataAdapter itempediaDataAdapter = dataList[i];
				int[] allUnlockItems = Singleton<ItempediaModule>.Instance.GetAllUnlockItems();
				bool active = false;
				int j = 0;
				for (int num = allUnlockItems.Length; j < num; j++)
				{
					if (allUnlockItems[j] == itempediaDataAdapter.ID)
					{
						active = true;
						break;
					}
				}
				component.SetupView(itempediaDataAdapter, active);
				component.SetClickCallback(OnItemButtonClick);
			}, dataList.Count);
		}

		private void OnItemButtonClick(ItempediaDataAdapter item)
		{
			bool unlock = false;
			int[] allUnlockItems = Singleton<ItempediaModule>.Instance.GetAllUnlockItems();
			int i = 0;
			for (int num = allUnlockItems.Length; i < num; i++)
			{
				if (item.ID == allUnlockItems[i])
				{
					unlock = true;
					break;
				}
			}
			UIUtil.ShowItemDetail(item.GetDummyStorageItemData(), true, unlock);
		}
	}
}
