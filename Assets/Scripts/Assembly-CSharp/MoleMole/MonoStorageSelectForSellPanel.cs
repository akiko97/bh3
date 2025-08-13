using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStorageSelectForSellPanel : MonoBehaviour
	{
		public const int MAX_SELL_NUM = 20;

		private bool _isMultiSell;

		private Dictionary<int, StorageDataItemBase> _sellItemMap;

		private int _sellNum;

		public void SetupView(bool isMultiSell)
		{
			_isMultiSell = isMultiSell;
			_sellItemMap = new Dictionary<int, StorageDataItemBase>();
			_sellNum = 0;
			base.transform.Find("SingleNum").gameObject.SetActive(!isMultiSell);
			base.transform.Find("MultiNum").gameObject.SetActive(isMultiSell);
			UpdateDataView();
		}

		public void RefreshOnItemButonClick(StorageDataItemBase item)
		{
			int idForKey = item.GetIdForKey();
			if (_sellItemMap.ContainsKey(idForKey))
			{
				_sellItemMap.Remove(idForKey);
				if (_isMultiSell)
				{
					_sellNum = 0;
				}
			}
			else if (_isMultiSell)
			{
				_sellItemMap.Clear();
				_sellItemMap.Add(idForKey, item);
				_sellNum = 1;
			}
			else if (_sellItemMap.Count < 20)
			{
				_sellItemMap.Add(idForKey, item);
			}
			UpdateDataView();
		}

		public bool IsItemInSelectedMap(StorageDataItemBase item)
		{
			int idForKey = item.GetIdForKey();
			return _sellItemMap.ContainsKey(idForKey);
		}

		public void OnSellPanelCloseBtnClick()
		{
			Close();
		}

		public void OnSellPanelOKBtnClick()
		{
			if (_sellItemMap.Values.Count == 0)
			{
				return;
			}
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			int number = ((!_isMultiSell) ? 1 : _sellNum);
			foreach (StorageDataItemBase value in _sellItemMap.Values)
			{
				StorageDataItemBase storageDataItemBase = value.Clone();
				storageDataItemBase.number = number;
				list.Add(storageDataItemBase);
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new SellConfirmDialogContext(list));
		}

		public void OnSellDecreaseBtnClick()
		{
			if (_sellItemMap.Count != 0 && _sellNum > 1)
			{
				_sellNum--;
				UpdateDataView();
			}
		}

		public void OnSellIncreaseBtnClick()
		{
			if (_sellItemMap.Count != 0)
			{
				StorageDataItemBase storageDataItemBase = _sellItemMap.Values.First();
				if (_sellNum < storageDataItemBase.number)
				{
					_sellNum++;
					UpdateDataView();
				}
			}
		}

		public void OnSellMaxBtnClick()
		{
			if (_sellItemMap.Count != 0)
			{
				StorageDataItemBase storageDataItemBase = _sellItemMap.Values.First();
				_sellNum = storageDataItemBase.number;
				UpdateDataView();
			}
		}

		private void UpdateDataView()
		{
			int num = CalculateTotalSCoinSell();
			if (!_isMultiSell)
			{
				Transform transform = base.transform.Find("SingleNum");
				transform.Find("SCoin/Num").GetComponent<Text>().text = num.ToString();
				int count = _sellItemMap.Keys.Count;
				transform.Find("Select/Num").GetComponent<Text>().text = count + "/" + 20;
				int equipmentSizeLimit = Singleton<PlayerModule>.Instance.playerData.equipmentSizeLimit;
				int currentCapacity = Singleton<StorageModule>.Instance.GetCurrentCapacity();
				transform.Find("Capacity/Num").GetComponent<Text>().text = (currentCapacity - count).ToString() + "/" + equipmentSizeLimit;
			}
			else
			{
				Transform transform2 = base.transform.Find("MultiNum");
				transform2.Find("SCoin/Num").GetComponent<Text>().text = num.ToString();
				transform2.Find("SellNum/Text").GetComponent<Text>().text = _sellNum.ToString();
			}
		}

		private int CalculateTotalSCoinSell()
		{
			float num = 0f;
			int num2 = ((!_isMultiSell) ? 1 : _sellNum);
			foreach (StorageDataItemBase value in _sellItemMap.Values)
			{
				num += value.GetPriceForSell() * (float)num2;
			}
			return Mathf.FloorToInt(num);
		}

		private void Close()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSellViewActive, false));
		}
	}
}
