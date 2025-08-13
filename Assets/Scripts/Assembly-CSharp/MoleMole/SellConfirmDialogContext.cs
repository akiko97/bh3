using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class SellConfirmDialogContext : BaseDialogContext
	{
		public readonly List<StorageDataItemBase> sellList;

		public SellConfirmDialogContext(List<StorageDataItemBase> sellList)
		{
			config = new ContextPattern
			{
				contextName = "SellConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SellConfirmDialog",
				ignoreNotify = true
			};
			this.sellList = sellList;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/WarningText").gameObject.SetActive(HasRareItem());
			int num = CalculateTotalSCoinSell();
			base.view.transform.Find("Dialog/Content/SCoin/Num").GetComponent<Text>().text = num.ToString();
			return false;
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestEquipmentSell(sellList);
			Close();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSellViewActive, false));
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void Close()
		{
			Destroy();
		}

		private int CalculateTotalSCoinSell()
		{
			float num = 0f;
			foreach (StorageDataItemBase sell in sellList)
			{
				num += sell.GetPriceForSell() * (float)sell.number;
			}
			return Mathf.FloorToInt(num);
		}

		private bool HasRareItem()
		{
			return sellList.Find((StorageDataItemBase x) => x.rarity >= 3) != null;
		}
	}
}
