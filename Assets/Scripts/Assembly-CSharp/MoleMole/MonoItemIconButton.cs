using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoItemIconButton : MonoBehaviour
	{
		public enum SelectMode
		{
			None = 0,
			CheckWhenSelect = 1,
			SmallWhenUnSelect = 2,
			ConsumeMaterial = 3
		}

		public delegate void ClickCallBack(StorageDataItemBase item, bool selelcted = false);

		private const float UNSELECTED_SCALE_RATIO = 0.83f;

		public SelectMode selectMode;

		public bool showProtected;

		public bool blockSelect;

		private ClickCallBack _clickCallBack;

		public StorageDataItemBase _item;

		private bool _isSelected;

		private Action<StorageDataItemBase> _minusBtnCallBack;

		private int _selectNum;

		public void SetupView(StorageDataItemBase item, SelectMode selectMode = SelectMode.None, bool isSelected = false, bool bShowCostOver = false, bool bUsed = false)
		{
			_item = item;
			this.selectMode = selectMode;
			_isSelected = isSelected;
			base.transform.Find("SelectedMark").gameObject.SetActive(false);
			base.transform.Find("ProtectedMark").gameObject.SetActive(false);
			base.transform.Find("InteractiveMask").gameObject.SetActive(false);
			base.transform.Find("NotEnough").gameObject.SetActive(false);
			base.transform.Find("Star").gameObject.SetActive(false);
			base.transform.Find("StigmataType").gameObject.SetActive(false);
			base.transform.Find("UnidentifyText").gameObject.SetActive(false);
			base.transform.Find("QuestionMark").gameObject.SetActive(false);
			base.transform.Find("MinusBtn").gameObject.SetActive(false);
			if (_item == null)
			{
				base.transform.Find("ItemIcon").gameObject.SetActive(false);
				base.transform.Find("Text").gameObject.SetActive(false);
				return;
			}
			base.transform.Find("ItemIcon").gameObject.SetActive(true);
			base.transform.Find("Text").gameObject.SetActive(true);
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(item.GetIconPath());
			base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
			base.transform.Find("ItemIcon").GetComponent<Image>().color = Color.white;
			base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[item.rarity]);
			if (_item is WeaponDataItem || _item is StigmataDataItem)
			{
				base.transform.Find("Text").GetComponent<Text>().text = "LV." + item.level;
			}
			else if (_item is MaterialDataItem)
			{
				string text = "×" + item.number;
				if (selectMode == SelectMode.ConsumeMaterial)
				{
					StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(_item.ID);
					int num = ((storageDataItemBase != null) ? storageDataItemBase.number : 0);
					text = ((_item.number <= num) ? MiscData.AddColor("TotalBlack", num + " / " + _item.number) : (MiscData.AddColor("WarningRed", num + " / ") + MiscData.AddColor("TotalBlack", _item.number.ToString())));
				}
				base.transform.Find("Text").GetComponent<Text>().text = text;
			}
			else if (_item is AvatarFragmentDataItem)
			{
				base.transform.Find("Text").GetComponent<Text>().text = "×" + item.number;
			}
			else
			{
				base.transform.Find("Text").gameObject.SetActive(false);
			}
			SetupRarityView();
			SetupStigmataTypeIcon();
			if (selectMode != SelectMode.None)
			{
				SetupSelectedView(isSelected);
			}
			else
			{
				base.transform.Find("BG/Unselected").gameObject.SetActive(true);
				base.transform.Find("BG/Selected").gameObject.SetActive(false);
			}
			if (showProtected)
			{
				SetupProtectedView();
			}
			SetupBlockSelectView();
			SetupCostView(bShowCostOver);
			SetupUsedView(bUsed);
			base.transform.Find("ItemIcon/Icon").GetComponent<Image>().material = null;
			base.transform.Find("ItemIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
			if (_item is StigmataDataItem)
			{
				SetupStigmataAffixView((_item as StigmataDataItem).IsAffixIdentify);
			}
		}

		private void SetupCostView(bool bShowCostOver)
		{
			base.transform.Find("CostOver").gameObject.SetActive(bShowCostOver);
		}

		private void SetupUsedView(bool bUsed)
		{
			base.transform.Find("Used").gameObject.SetActive(bUsed);
		}

		private void SetupRarityView()
		{
			if (!(_item is AvatarFragmentDataItem))
			{
				base.transform.Find("Star").gameObject.SetActive(true);
				int maxStar = _item.rarity;
				if (_item is WeaponDataItem)
				{
					maxStar = (_item as WeaponDataItem).GetMaxRarity();
				}
				else if (_item is StigmataDataItem)
				{
					maxStar = (_item as StigmataDataItem).GetMaxRarity();
				}
				base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(_item.rarity, maxStar);
			}
		}

		private void SetupStigmataTypeIcon()
		{
			base.transform.Find("StigmataType").gameObject.SetActive(_item is StigmataDataItem);
			if (_item is StigmataDataItem)
			{
				base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[_item.GetBaseType()]);
			}
		}

		private void SetupStigmataAffixView(bool isIdentify)
		{
			base.transform.Find("UnidentifyText").gameObject.SetActive(!isIdentify);
			base.transform.Find("Text").gameObject.SetActive(isIdentify);
			Image component = base.transform.Find("ItemIcon/Icon").GetComponent<Image>();
			if (isIdentify)
			{
				component.material = null;
				component.color = Color.white;
			}
			else
			{
				Material material = Miscs.LoadResource<Material>("Material/ImageMonoColor");
				component.material = material;
				component.color = MiscData.GetColor("DarkBlue");
			}
			base.transform.Find("QuestionMark").gameObject.SetActive(!isIdentify);
		}

		private void SetupSelectedView(bool isSelected)
		{
			_isSelected = isSelected;
			if (selectMode == SelectMode.SmallWhenUnSelect)
			{
				base.transform.localScale = ((!_isSelected) ? (Vector3.one * 0.83f) : Vector3.one);
			}
			else
			{
				base.transform.Find("SelectedMark").gameObject.SetActive(_isSelected);
			}
			base.transform.Find("BG/Selected").gameObject.SetActive(isSelected);
			base.transform.Find("BG/Unselected").gameObject.SetActive(!isSelected);
		}

		private void SetupProtectedView()
		{
			base.transform.Find("ProtectedMark").gameObject.SetActive(_item.isProtected);
		}

		private void SetupBlockSelectView()
		{
			base.transform.Find("InteractiveMask").gameObject.SetActive(blockSelect);
			base.transform.GetComponent<Button>().interactable = !blockSelect;
		}

		public void ShowSelectedNum(int num)
		{
			_selectNum = num;
			base.transform.Find("SelectedMark/Image").gameObject.SetActive(false);
			base.transform.Find("SelectedMark/Num").gameObject.SetActive(true);
			base.transform.Find("SelectedMark/Num").GetComponent<Text>().text = "×" + num;
			base.transform.Find("MinusBtn").gameObject.SetActive(num > 0 && _item is MaterialDataItem);
		}

		public void SetMinusBtnCallBack(Action<StorageDataItemBase> minusBtnCallBack = null)
		{
			_minusBtnCallBack = minusBtnCallBack;
		}

		public void OnClick()
		{
			if (_clickCallBack != null)
			{
				_clickCallBack(_item);
			}
		}

		public void SetClickCallback(ClickCallBack callback)
		{
			_clickCallBack = callback;
		}

		public void OnMinusBtnClick()
		{
			_selectNum--;
			base.transform.Find("SelectedMark/Num").gameObject.SetActive(_selectNum > 0);
			base.transform.Find("MinusBtn").gameObject.SetActive(_selectNum > 0);
			if (_minusBtnCallBack != null)
			{
				_minusBtnCallBack(_item);
			}
		}

		private void OnDisable()
		{
		}
	}
}
