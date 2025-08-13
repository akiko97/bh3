using System;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class ItemDetailDialogContext : BaseDialogContext
	{
		public readonly StorageDataItemBase storageItem;

		public readonly bool hideActionBtns;

		public ItemDetailDialogContext(StorageDataItemBase storageItem, bool hideActionBtns = false)
		{
			config = new ContextPattern
			{
				contextName = "ItemDetailDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ItemDetailDialog",
				ignoreNotify = true
			};
			this.storageItem = storageItem;
			this.hideActionBtns = hideActionBtns;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/RarityUpBtn").GetComponent<Button>(), OnRarityUpBtnCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			SetupRarityView();
			base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(storageItem.GetImagePath());
			Transform transform = base.view.transform.Find("Dialog/Content/Star/EquipStar");
			if (storageItem is AvatarFragmentDataItem)
			{
				transform.gameObject.SetActive(false);
			}
			else
			{
				transform.gameObject.SetActive(true);
				transform.GetComponent<MonoEquipStar>().SetupView(storageItem.rarity);
			}
			base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = storageItem.GetDisplayTitle();
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = GetAllDesc();
			base.view.transform.Find("Dialog/Content/Num/Text").GetComponent<Text>().text = storageItem.number.ToString();
			base.view.transform.Find("Dialog/Content/Num").gameObject.SetActive(!hideActionBtns);
			base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(!hideActionBtns && storageItem.GetEvoStorageItem() != null);
			return false;
		}

		public void OnRarityUpBtnCallBack()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StorageEvoPageContext(storageItem));
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}

		private void SetupRarityView()
		{
			string hexString = MiscData.Config.ItemRarityColorList[storageItem.rarity];
			base.view.transform.Find("Dialog/Content/Icon").GetComponent<Image>().color = Miscs.ParseColor(hexString);
		}

		private string GetAllDesc()
		{
			string text = UIUtil.ProcessStrWithNewLine(storageItem.GetDescription());
			MaterialDataItem materialDataItem = storageItem as MaterialDataItem;
			if (materialDataItem != null)
			{
				string bGDescription = materialDataItem.GetBGDescription();
				return string.Format("{0}{1}<color=#a8a8a8ff>{2}</color>", text, Environment.NewLine, bGDescription);
			}
			return text;
		}
	}
}
