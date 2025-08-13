using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAvatarDetailWeaponTab : MonoBehaviour
	{
		private FriendDetailDataItem _userData;

		private AvatarDataItem _avatarData;

		private bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarData)
		{
			_isRemoteAvatar = false;
			_avatarData = avatarData;
			base.transform.Find("Info/ChangeBtn").gameObject.SetActive(true);
			SetupWeapon(_avatarData.GetWeapon());
		}

		public void SetupView(FriendDetailDataItem userData)
		{
			_isRemoteAvatar = true;
			_userData = userData;
			_avatarData = _userData.leaderAvatar;
			base.transform.Find("Info/ChangeBtn").gameObject.SetActive(false);
			SetupWeapon(_userData.leaderAvatar.GetWeapon());
		}

		public void OnChangeBtnClick()
		{
			if (!_isRemoteAvatar)
			{
				AvatarChangeEquipPageContext context = new AvatarChangeEquipPageContext(_avatarData, _avatarData.GetWeapon(), (EquipmentSlot)1);
				Singleton<MainUIManager>.Instance.ShowPage(context);
			}
		}

		public void OnContentClick(BaseEventData data = null)
		{
			if (_isRemoteAvatar)
			{
				WeaponDataItem weapon = _userData.leaderAvatar.GetWeapon();
				if (weapon != null)
				{
					Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(weapon, true));
				}
			}
			else
			{
				WeaponDataItem weapon2 = _avatarData.GetWeapon();
				if (weapon2 == null)
				{
					OnChangeBtnClick();
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(weapon2));
				}
			}
		}

		private void SetupWeapon(WeaponDataItem weaponData)
		{
			base.transform.Find("Info/Title/Equipment").gameObject.SetActive(weaponData != null);
			base.transform.Find("Info/Content/Equipment").gameObject.SetActive(weaponData != null);
			if (weaponData != null)
			{
				base.transform.Find("Info/Title/Equipment/Name").GetComponent<Text>().text = weaponData.GetDisplayTitle();
				string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[weaponData.GetBaseType()];
				base.transform.Find("Info/Title/Equipment/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				base.transform.Find("Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(weaponData, true);
				base.transform.Find("Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = weaponData.GetCost().ToString();
				base.transform.Find("Info/Content/Equipment/Lv").GetComponent<Text>().text = "LV." + weaponData.level;
				base.transform.Find("Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(weaponData.rarity, weaponData.GetMaxRarity());
				base.transform.Find("Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(weaponData.GetSubRarity(), weaponData.GetMaxSubRarity() - 1);
			}
		}
	}
}
