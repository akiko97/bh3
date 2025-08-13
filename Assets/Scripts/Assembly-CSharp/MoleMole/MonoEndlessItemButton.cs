using System;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoEndlessItemButton : MonoBehaviour
	{
		private EndlessItem _itemData;

		private EndlessToolDataItem _itemDataItem;

		private Action<EndlessItem> _itemClickCallback;

		private Action _itemUseClickCallback;

		private EndlessPlayerData _currentSelectPlayer;

		public void SetupView(EndlessItem itemData, bool isSelect = false, EndlessPlayerData selectPlayer = null, Action<EndlessItem> clickCallback = null, Action useClickCallback = null)
		{
			_itemData = itemData;
			_itemDataItem = new EndlessToolDataItem((int)_itemData.item_id, (int)_itemData.num);
			_itemClickCallback = clickCallback;
			_itemUseClickCallback = useClickCallback;
			base.transform.Find("VerticalLayout/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_itemDataItem.GetIconPath());
			base.transform.Find("VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = _itemDataItem.GetDisplayTitle();
			base.transform.Find("VerticalLayout/TopLine/NameRow/NumText").GetComponent<Text>().text = "x" + _itemDataItem.number;
			base.transform.Find("VerticalLayout/TopLine/Target/Self").gameObject.SetActive(_itemDataItem.ApplyToSelf);
			base.transform.Find("VerticalLayout/TopLine/Target/Other").gameObject.SetActive(!_itemDataItem.ApplyToSelf);
			base.transform.Find("VerticalLayout/AbstractText").GetComponent<Text>().text = _itemDataItem.GetDescription();
			base.transform.Find("VerticalLayout/DescText").GetComponent<Text>().text = _itemDataItem.GetDescription();
			base.transform.Find("SelectMark").gameObject.SetActive(isSelect);
			base.transform.Find("VerticalLayout/DescText").gameObject.SetActive(false);
			base.transform.Find("VerticalLayout/UseBtn").gameObject.SetActive(isSelect);
			base.transform.Find("VerticalLayout/UseBtn/Use").gameObject.SetActive(isSelect);
			base.transform.Find("VerticalLayout/UseBtn/Tip").gameObject.SetActive(false);
		}

		public void OnClick()
		{
			if (_itemClickCallback != null)
			{
				_itemClickCallback(_itemData);
			}
		}

		public void OnUseClick()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			if (!_itemDataItem.ApplyToSelf && (int)_itemDataItem.ToolType != 3)
			{
				base.transform.Find("VerticalLayout/UseBtn/Use").gameObject.SetActive(false);
				base.transform.Find("VerticalLayout/UseBtn/Tip").gameObject.SetActive(true);
			}
			if (_itemUseClickCallback != null)
			{
				_itemUseClickCallback();
			}
		}
	}
}
