using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarSubSkillIconButton : MonoBehaviour
	{
		private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

		private AvatarDataItem _avatarData;

		private AvatarSkillDataItem _skillData;

		private AvatarSubSkillDataItem _subSkillData;

		private bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarDataItem, AvatarSkillDataItem skillData, AvatarSubSkillDataItem subSkillData, bool isRemoteAvatar)
		{
			_avatarData = avatarDataItem;
			_skillData = skillData;
			_subSkillData = subSkillData;
			_isRemoteAvatar = isRemoteAvatar;
			base.transform.Find("Row/NameText").GetComponent<Text>().text = _subSkillData.Name;
			base.transform.Find("RowText/DescText").GetComponent<Text>().text = _subSkillData.Info;
			SetupIcon();
			Transform transform = base.transform.Find("SkillIcon/UnLockInfo");
			bool flag = _avatarData.level < _subSkillData.UnlockLv;
			bool flag2 = _avatarData.star < _subSkillData.UnlockStar;
			transform.gameObject.SetActive(!_subSkillData.UnLocked);
			if (!_subSkillData.UnLocked)
			{
				Transform transform2 = transform.Find("UnLockStar");
				Transform transform3 = transform.Find("UnLockLv");
				base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("SkillBtnGrey");
				if (flag2)
				{
					transform2.gameObject.SetActive(true);
					transform3.gameObject.SetActive(false);
					transform2.Find("Star").GetComponent<MonoAvatarStar>().SetupView(_subSkillData.UnlockStar);
				}
				else if (flag)
				{
					transform2.gameObject.SetActive(false);
					transform3.gameObject.SetActive(true);
					transform3.Find("Lv").GetComponent<Text>().text = "Lv." + _subSkillData.UnlockLv;
				}
				else
				{
					transform2.gameObject.SetActive(false);
					transform3.gameObject.SetActive(false);
					base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("Blue");
				}
			}
			Transform transform4 = base.transform.Find("RowText/NextLv");
			Transform transform5 = base.transform.Find("RowText/NextStar");
			transform4.gameObject.SetActive(false);
			transform5.gameObject.SetActive(false);
			if (_subSkillData.UnLocked && _subSkillData.level < _subSkillData.MaxLv)
			{
				bool flag3 = _avatarData.level < _subSkillData.LvUpNeedAvatarLevel;
				if (_avatarData.star < _subSkillData.GetUpLevelStarNeed())
				{
					transform4.gameObject.SetActive(false);
					transform5.gameObject.SetActive(true);
					transform5.Find("Star").GetComponent<MonoAvatarStar>().SetupView(_subSkillData.GetUpLevelStarNeed());
				}
				else if (flag3)
				{
					transform4.gameObject.SetActive(true);
					transform5.gameObject.SetActive(false);
					transform4.Find("Lv").GetComponent<Text>().text = "Lv." + _subSkillData.LvUpNeedAvatarLevel;
				}
			}
			Text component = base.transform.Find("Row/AddPtText").GetComponent<Text>();
			component.gameObject.SetActive(_subSkillData.UnLocked);
			if (_subSkillData.level == _subSkillData.MaxLv)
			{
				component.text = "MAX";
			}
			else
			{
				component.text = ((_subSkillData.level <= 0) ? string.Empty : string.Format("+{0}", _subSkillData.level));
			}
			SetupSubSkillPopUp();
			if (_subSkillData.level == _subSkillData.MaxLv)
			{
				base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(false);
			}
			base.transform.Find("SkillIcon/Max").gameObject.SetActive(_subSkillData.level == _subSkillData.MaxLv);
			base.transform.Find("SkillIcon/Upgradable").gameObject.SetActive(!_isRemoteAvatar && _subSkillData.UnLocked && _subSkillData.level < _subSkillData.MaxLv && _avatarData.level >= _subSkillData.LvUpNeedAvatarLevel && _avatarData.star >= _subSkillData.GetUpLevelStarNeed());
		}

		public void OnClick()
		{
			if (!_isRemoteAvatar)
			{
				if (_subSkillData.level == _subSkillData.MaxLv)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_SubSkillMaxLv")));
					return;
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarSubSkillDialogContext(_avatarData, _skillData, _subSkillData));
				ClearSubSkillStatusInLocalData();
				SetupSubSkillPopUp();
			}
		}

		private void SetupIcon()
		{
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(_subSkillData.IconPath);
			base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (_subSkillData.UnLocked)
			{
				empty = "SkillBtnBlue";
				base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = null;
			}
			else
			{
				empty = "SkillBtnGrey";
				Material material = Miscs.LoadResource<Material>("Material/ImageGrayscale");
				base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = material;
			}
			empty2 = "Blue";
			base.transform.Find("Frame").GetComponent<Image>().color = MiscData.GetColor(empty2);
			base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor(empty2);
			base.transform.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(empty);
		}

		private void SetupSubSkillPopUp()
		{
			Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
			bool flag = subSkillStatusDict.ContainsKey(_subSkillData.subSkillID);
			base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(!_isRemoteAvatar && flag);
		}

		private void ClearSubSkillStatusInLocalData()
		{
			Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
			subSkillStatusDict.Remove(_subSkillData.subSkillID);
			Singleton<MiHoYoGameData>.Instance.Save();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate));
		}
	}
}
