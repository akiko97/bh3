using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class StorageItemSellDialogContext : BaseDialogContext
	{
		public StorageDataItemBase storageDataItem;

		private GameObject _oneItemSellInfoPanel;

		private GameObject _multipItemSellInfoPanel;

		private int _sellItemNumber;

		private int _maxItemNumber;

		public StorageItemSellDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "StorageItemSellDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SellItemDialog"
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 34)
			{
				return SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/ActionPanel/ConfirmButton").GetComponent<Button>(), OnConfirmButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/ActionPanel/CancelButton").GetComponent<Button>(), OnCancelButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/IncreaseButton").GetComponent<Button>(), OnIncreaseButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/DecreaseButton").GetComponent<Button>(), OnDecreaseButtonCallBack);
		}

		protected override bool SetupView()
		{
			Init();
			base.view.transform.Find("Dialog/ItemButton").GetComponent<MonoStorageItemIcon>().SetupView(storageDataItem, base.view.transform.Find("Dialog"), null, -1, storageDataItem.GetType());
			bool flag = storageDataItem is MaterialDataItem;
			_oneItemSellInfoPanel.SetActive(!flag);
			_multipItemSellInfoPanel.SetActive(flag);
			if (flag)
			{
				MaterialDataItem materialDataItem = storageDataItem as MaterialDataItem;
				_sellItemNumber = 1;
				_maxItemNumber = materialDataItem.number;
				OnSellNumChange();
			}
			else
			{
				base.view.transform.Find("Dialog/OneItemSellInfoPanel/CoinGotNumber").GetComponent<Text>().text = Mathf.FloorToInt(storageDataItem.GetPriceForSell()).ToString();
			}
			return false;
		}

		public void OnConfirmButtonCallBack()
		{
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			if (!(storageDataItem is MaterialDataItem))
			{
				list.Add(storageDataItem);
			}
			else
			{
				StorageDataItemBase storageDataItemBase = new MaterialDataItem(ItemMetaDataReader.GetItemMetaDataByKey(storageDataItem.ID));
				storageDataItemBase.number = _sellItemNumber;
				list.Add(storageDataItemBase);
			}
			Singleton<NetworkManager>.Instance.RequestEquipmentSell(list);
			if (!(storageDataItem is MaterialDataItem) || storageDataItem.number <= _sellItemNumber)
			{
				Singleton<MainUIManager>.Instance.BackPageTo("StorageShowPageContext");
				Destroy();
			}
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void OnCancelButtonCallBack()
		{
			Destroy();
		}

		public void OnIncreaseButtonCallBack()
		{
			if (_sellItemNumber < _maxItemNumber)
			{
				_sellItemNumber++;
				OnSellNumChange();
			}
		}

		public void OnDecreaseButtonCallBack()
		{
			if (_sellItemNumber > 1)
			{
				_sellItemNumber--;
				OnSellNumChange();
			}
		}

		private void Init()
		{
			_oneItemSellInfoPanel = base.view.transform.Find("Dialog/OneItemSellInfoPanel").gameObject;
			_multipItemSellInfoPanel = base.view.transform.Find("Dialog/MultipleItemSellInfoPanel").gameObject;
		}

		private void OnSellNumChange()
		{
			base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/Text").GetComponent<Text>().text = _sellItemNumber.ToString();
			base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/CoinNum/CoinGotNumber").GetComponent<Text>().text = (storageDataItem.GetPriceForSell() * (float)_sellItemNumber).ToString();
		}
	}
}
