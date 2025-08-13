using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAvatarStigmataSlot : MonoBehaviour
	{
		private AvatarDataItem _avatarData;

		private EquipmentSlot _slot;

		private int _index;

		private bool _isRemoteAvatar;

		private StigmataDataItem _stigmataData;

		public void SetupView(AvatarDataItem avatarDataItem, EquipmentSlot slot, int index, bool isRemoteAvatar)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			_avatarData = avatarDataItem;
			_slot = slot;
			_index = index;
			_isRemoteAvatar = isRemoteAvatar;
			_stigmataData = _avatarData.GetStigmata(slot);
			Transform transform = base.transform.Find("Content/Stigmata");
			if (_stigmataData == null)
			{
				transform.gameObject.SetActive(false);
				string textID = "Menu_StigmataSlot_" + _index;
				base.transform.Find("Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			}
			else
			{
				transform.gameObject.SetActive(true);
				base.transform.Find("Title/Text").GetComponent<Text>().text = _stigmataData.GetDisplayTitle();
				SetupStigmata(transform, _stigmataData);
			}
			base.transform.Find("ChangeBtn").gameObject.SetActive(!_isRemoteAvatar);
		}

		public void OnChangeBtnCallBack()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!_isRemoteAvatar)
			{
				if (GetFilterList().Count > 0)
				{
					AvatarChangeEquipPageContext context = new AvatarChangeEquipPageContext(_avatarData, _stigmataData, _slot);
					Singleton<MainUIManager>.Instance.ShowPage(context);
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_NoSuitableStigmata")));
				}
			}
		}

		public void OnContentClick(BaseEventData data = null)
		{
			if (_isRemoteAvatar)
			{
				if (_stigmataData != null)
				{
					Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(_stigmataData, true));
				}
			}
			else if (_stigmataData == null)
			{
				OnChangeBtnCallBack();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(_stigmataData)
				{
					uiEquipOwner = _avatarData
				});
			}
		}

		private void SetupStigmata(Transform trans, StigmataDataItem stigmata)
		{
			SetStigmataImage(trans.Find("MaskPanel/Figure").GetComponent<RectTransform>(), stigmata);
			trans.Find("LvText").GetComponent<Text>().text = "LV." + stigmata.level;
			trans.Find("Cost/Num").GetComponent<Text>().text = stigmata.GetCost().ToString();
			trans.Find("Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(stigmata.rarity, stigmata.GetMaxRarity());
			trans.Find("Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(stigmata.GetSubRarity(), stigmata.GetMaxSubRarity() - 1);
		}

		private List<StorageDataItemBase> GetFilterList()
		{
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.UserStorageItemList.FindAll((StorageDataItemBase x) => Filter(x));
			list.Sort(StorageDataItemBase.CompareToRarityDesc);
			return list;
		}

		private bool Filter(StorageDataItemBase item)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected I4, but got Unknown
			bool flag = false;
			bool flag2 = false;
			EquipmentSlot slot = _slot;
			switch ((int)slot - 1)
			{
			case 0:
				flag = item.GetType() == typeof(WeaponDataItem);
				flag2 = item.GetBaseType() == _avatarData.WeaponBaseTypeList[0];
				break;
			case 1:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 1;
				break;
			case 2:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 2;
				break;
			case 3:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 3;
				break;
			}
			return flag && flag2;
		}

		private void SetStigmataImage(RectTransform imageTrans, StigmataDataItem stigmata)
		{
			imageTrans.GetComponent<MonoStigmataFigure>().SetupView(stigmata);
			imageTrans.transform.Find("PrefContainer").localScale = Vector3.one * stigmata.GetScale();
			imageTrans.anchoredPosition = new Vector2(stigmata.GetOffesetX(), stigmata.GetOffesetY());
		}
	}
}
