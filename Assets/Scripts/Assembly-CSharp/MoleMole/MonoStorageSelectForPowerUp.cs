using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStorageSelectForPowerUp : MonoBehaviour
	{
		public const int MAX_SELECT_NUM = 6;

		private bool _isMulti;

		private Dictionary<KeyValuePair<Type, int>, StorageDataItemBase> _selectedItemMap;

		private List<StorageDataItemBase> _selectedItemList;

		private StorageDataItemBase _modifyingItem;

		private StorageDataItemBase _powerUpTarget;

		public void SetupView(List<StorageDataItemBase> selectedItemList, bool isMulti, StorageDataItemBase powerUpTarget)
		{
			_selectedItemList = selectedItemList;
			_powerUpTarget = powerUpTarget;
			_selectedItemMap = new Dictionary<KeyValuePair<Type, int>, StorageDataItemBase>();
			foreach (StorageDataItemBase selectedItem in _selectedItemList)
			{
				_selectedItemMap.Add(GetKeyByItem(selectedItem), selectedItem);
			}
			RefreshView(isMulti);
		}

		public void RefreshView(bool isMulti)
		{
			_isMulti = isMulti;
			UpdateDataView();
		}

		public void ClearModifyingItem()
		{
			_modifyingItem = null;
		}

		public int GetItemSelectNum(StorageDataItemBase item)
		{
			KeyValuePair<Type, int> keyByItem = GetKeyByItem(item);
			return _selectedItemMap.ContainsKey(keyByItem) ? _selectedItemMap[keyByItem].number : 0;
		}

		public void RefreshOnItemButonClick(StorageDataItemBase item)
		{
			KeyValuePair<Type, int> keyByItem = GetKeyByItem(item);
			if (_selectedItemMap.ContainsKey(keyByItem))
			{
				if (_isMulti)
				{
					_modifyingItem = _selectedItemMap[keyByItem];
					OnIncreaseBtnClick();
				}
				else
				{
					_selectedItemList.Remove(_selectedItemMap[keyByItem]);
					_selectedItemMap.Remove(keyByItem);
				}
			}
			else if (_selectedItemMap.Count < 6)
			{
				StorageDataItemBase storageDataItemBase = item.Clone();
				storageDataItemBase.number = 1;
				_selectedItemList.Add(storageDataItemBase);
				_selectedItemMap.Add(keyByItem, storageDataItemBase);
				if (_isMulti)
				{
					_modifyingItem = storageDataItemBase;
				}
			}
			UpdateDataView();
		}

		public bool IsItemInSelectedMap(StorageDataItemBase item)
		{
			KeyValuePair<Type, int> keyByItem = GetKeyByItem(item);
			return _selectedItemMap.ContainsKey(keyByItem);
		}

		public void OnPowerUpPanelCloseBtnClick()
		{
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
		}

		public void OnPowerUpPanelOKBtnClick()
		{
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
		}

		public void OnDecreaseBtnClick(StorageDataItemBase dataItem)
		{
			KeyValuePair<Type, int> keyByItem = GetKeyByItem(dataItem);
			if (_selectedItemMap.ContainsKey(keyByItem))
			{
				_modifyingItem = _selectedItemMap[keyByItem];
			}
			else
			{
				_modifyingItem = null;
			}
			if (_modifyingItem != null && _modifyingItem.number > 0)
			{
				_modifyingItem.number--;
				FixListOnModifyNumber(_modifyingItem);
				UpdateDataView();
			}
		}

		public void OnIncreaseBtnClick()
		{
			if (_modifyingItem != null)
			{
				int number = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(_modifyingItem.GetType(), _modifyingItem.GetIdForKey()).number;
				if (_modifyingItem.number < number)
				{
					_modifyingItem.number++;
					FixListOnModifyNumber(_modifyingItem);
					UpdateDataView();
				}
			}
		}

		public void OnMaxBtnClick()
		{
			if (_modifyingItem != null)
			{
				int number = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(_modifyingItem.GetType(), _modifyingItem.GetIdForKey()).number;
				_modifyingItem.number = number;
				FixListOnModifyNumber(_modifyingItem);
				UpdateDataView();
			}
		}

		public void OnClearBtnClick()
		{
			if (_modifyingItem != null)
			{
				_modifyingItem.number = 0;
				FixListOnModifyNumber(_modifyingItem);
				UpdateDataView();
			}
		}

		private void UpdateDataView()
		{
			float scoinNeed;
			float expGet;
			UIUtil.CalCulateExpFromItems(out scoinNeed, out expGet, _selectedItemList, _powerUpTarget);
			int num = UIUtil.CalculateLvWithExp(expGet, _powerUpTarget);
			bool flag = num >= _powerUpTarget.GetMaxLevel();
			Text component = base.transform.Find("Content/Exp/Num/Num").GetComponent<Text>();
			component.text = Mathf.RoundToInt(expGet).ToString();
			component.color = ((!flag) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("WarningRed"));
			base.transform.Find("Content/Exp/Num/MaxLabel").gameObject.SetActive(flag);
			bool flag2 = scoinNeed > (float)Singleton<PlayerModule>.Instance.playerData.scoin;
			Text component2 = base.transform.Find("Content/Scoin/Num").GetComponent<Text>();
			component2.text = Mathf.RoundToInt(scoinNeed).ToString();
			component2.color = ((!flag2) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("WarningRed"));
		}

		private void FixListOnModifyNumber(StorageDataItemBase item)
		{
			KeyValuePair<Type, int> keyByItem = GetKeyByItem(item);
			if (_selectedItemMap.ContainsKey(keyByItem))
			{
				if (item.number == 0)
				{
					_selectedItemList.Remove(_selectedItemMap[keyByItem]);
					_selectedItemMap.Remove(keyByItem);
				}
			}
			else if (item.number > 0)
			{
				_selectedItemList.Add(item);
				_selectedItemMap.Add(keyByItem, item);
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshStorageShowing));
		}

		private KeyValuePair<Type, int> GetKeyByItem(StorageDataItemBase item)
		{
			return new KeyValuePair<Type, int>(item.GetType(), item.GetIdForKey());
		}
	}
}
