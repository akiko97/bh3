using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDropNewItemShow : MonoBehaviour
	{
		public enum DropNewItemType
		{
			NormalItem = 1,
			AvatarCard = 2
		}

		private StorageDataItemBase _itemData;

		private DropNewItemType _itemType = DropNewItemType.NormalItem;

		public void SetupView(StorageDataItemBase itemData)
		{
			_itemData = itemData;
			if (itemData is AvatarCardDataItem)
			{
				_itemType = DropNewItemType.AvatarCard;
			}
			SetupEffect();
			SetupItemInfo();
			PostOpenningAudioEvent();
		}

		private void SetupEffect()
		{
			if (_itemType == DropNewItemType.NormalItem)
			{
				base.transform.Find("GachaStart/Green").gameObject.SetActive(false);
				base.transform.Find("GachaStart/Blue").gameObject.SetActive(false);
				base.transform.Find("GachaStart/Purple").gameObject.SetActive(false);
				base.transform.Find("GachaStart/Orange").gameObject.SetActive(false);
				base.transform.Find("GachaStart/" + MiscData.Config.RarityColor[_itemData.rarity]).gameObject.SetActive(true);
				base.transform.Find("Effect/Green").gameObject.SetActive(false);
				base.transform.Find("Effect/Blue").gameObject.SetActive(false);
				base.transform.Find("Effect/Purple").gameObject.SetActive(false);
				base.transform.Find("Effect/Orange").gameObject.SetActive(false);
				base.transform.Find("Effect/" + MiscData.Config.RarityColor[_itemData.rarity]).gameObject.SetActive(true);
			}
		}

		private void SetupItemInfo()
		{
			base.transform.Find("Item/TitleRare/Green").gameObject.SetActive(false);
			base.transform.Find("Item/TitleRare/Blue").gameObject.SetActive(false);
			base.transform.Find("Item/TitleRare/Purple").gameObject.SetActive(false);
			base.transform.Find("Item/TitleRare/Orange").gameObject.SetActive(false);
			base.transform.Find("Item/TitleRare/" + MiscData.Config.RarityColor[_itemData.rarity]).gameObject.SetActive(true);
			if (_itemType == DropNewItemType.NormalItem)
			{
				base.transform.Find("Item/Title/DescPanel/Desc").GetComponent<Text>().text = _itemData.GetDisplayTitle();
				base.transform.Find("Item/StigmataIcon").gameObject.SetActive(false);
				base.transform.Find("Item/3dModel").gameObject.SetActive(false);
				base.transform.Find("Item/OtherIcon").gameObject.SetActive(false);
				if (_itemData is WeaponDataItem)
				{
					base.transform.Find("Item/3dModel").gameObject.SetActive(true);
					base.transform.Find("Item/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(_itemData as WeaponDataItem);
				}
				else if (_itemData is StigmataDataItem)
				{
					base.transform.Find("Item/StigmataIcon").gameObject.SetActive(true);
					base.transform.Find("Item/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(_itemData as StigmataDataItem);
				}
				else
				{
					string prefabPath = ((!(_itemData is EndlessToolDataItem)) ? _itemData.GetImagePath() : (_itemData as EndlessToolDataItem).GetIconPath());
					base.transform.Find("Item/OtherIcon").gameObject.SetActive(true);
					base.transform.Find("Item/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				}
				string text = LocalizationGeneralLogic.GetText("Menu_Material");
				if (_itemData is StigmataDataItem)
				{
					text = LocalizationGeneralLogic.GetText("Menu_Stigmata");
				}
				else if (_itemData is WeaponDataItem)
				{
					text = LocalizationGeneralLogic.GetText("Menu_Weapon");
				}
				base.transform.Find("Item/Title/DescPanel/Label").GetComponent<Text>().text = text;
				Transform transform = base.transform.Find("Item/Stars");
				if (_itemData is AvatarFragmentDataItem)
				{
					transform.gameObject.SetActive(false);
				}
				else
				{
					transform.gameObject.SetActive(true);
					for (int i = 0; i < transform.childCount; i++)
					{
						Transform child = transform.GetChild(i);
						child.gameObject.SetActive(i < _itemData.rarity);
					}
				}
				Color color = Miscs.ParseColor(MiscData.Config.DropItemBracketColorList[_itemData.rarity]);
				base.transform.Find("Item/Title/DescPanel/L").GetComponent<Image>().color = color;
				base.transform.Find("Item/Title/DescPanel/R").GetComponent<Image>().color = color;
			}
			else
			{
				base.transform.Find("Item/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_itemData.GetImagePath());
				base.transform.Find("Item/Title/DescPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_AvatarCard");
				int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(_itemData.ID).avatarID;
				int star = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarID).star;
				base.transform.Find("Item/Stars/AvatarStar/1").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[star]);
				base.transform.Find("Item/Title/DescPanel/Desc").GetComponent<Text>().text = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID).FullName;
				Color color2 = Miscs.ParseColor(MiscData.Config.DropItemBracketColorList[0]);
				base.transform.Find("Item/Title/DescPanel/L").GetComponent<Image>().color = color2;
				base.transform.Find("Item/Title/DescPanel/R").GetComponent<Image>().color = color2;
			}
		}

		private void PostOpenningAudioEvent()
		{
			if (_itemData is StigmataDataItem)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Item_Tattoo_PTL_Display");
			}
			else if (_itemData is AvatarCardDataItem)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large");
			}
			else
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small");
			}
		}

		public void ResetRectMaskForStigmata()
		{
			if (_itemData is StigmataDataItem)
			{
				base.transform.Find("Item/StigmataIcon").GetComponent<RectMask>().SetGraphicDirty();
			}
		}
	}
}
