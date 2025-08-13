using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarSkillIconButton : MonoBehaviour
	{
		private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

		private AvatarDataItem _avatarData;

		private AvatarSkillDataItem _skillData;

		private bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarDataItem, AvatarSkillDataItem skillData, bool isRemoteAvatar)
		{
			_avatarData = avatarDataItem;
			_skillData = skillData;
			_isRemoteAvatar = isRemoteAvatar;
			Text component = base.transform.Find("Info/NameText").GetComponent<Text>();
			component.text = _skillData.SkillName;
			component.color = ((!_skillData.UnLocked) ? MiscData.GetColor("TextGrey") : Color.white);
			Transform transform = base.transform.Find("SkillIcon/UnLockInfo");
			transform.gameObject.SetActive(!_skillData.UnLocked);
			Transform transform2 = transform.Find("UnLockStar");
			Transform transform3 = transform.Find("UnLockLv");
			transform2.gameObject.SetActive(false);
			transform3.gameObject.SetActive(false);
			if (transform.gameObject.activeSelf)
			{
				base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("SkillBtnGrey");
				bool flag = _avatarData.level < _skillData.UnLockLv;
				if (_avatarData.star < _skillData.UnLockStar)
				{
					transform2.gameObject.SetActive(true);
					transform3.gameObject.SetActive(false);
					transform2.Find("Star").GetComponent<MonoAvatarStar>().SetupView(_skillData.UnLockStar);
				}
				else if (flag)
				{
					transform2.gameObject.SetActive(false);
					transform3.gameObject.SetActive(true);
					transform3.Find("Lv").GetComponent<Text>().text = "Lv." + _skillData.UnLockLv;
				}
			}
			int levelSum = _skillData.GetLevelSum();
			int maxLevelSum = _skillData.GetMaxLevelSum();
			Text component2 = base.transform.Find("Info/Row/AddPtText").GetComponent<Text>();
			component2.gameObject.SetActive(_skillData.UnLocked);
			if (levelSum == maxLevelSum)
			{
				component2.text = "MAX";
			}
			else
			{
				component2.text = ((levelSum <= 0) ? string.Empty : ("+" + levelSum));
			}
			SetupIcon();
			SetupSkillPopUp();
		}

		private void SetupIcon()
		{
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(_skillData.IconPath);
			base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
			string empty = string.Empty;
			if (_skillData.UnLocked)
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
			base.transform.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(empty);
		}

		public void OnClick()
		{
			if (_skillData.UnLocked)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarSkillIconChange, _skillData.skillID));
			}
			else if (!_isRemoteAvatar)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new AvatarSkillDialogContext(_avatarData, _skillData));
			}
		}

		private void SetupSkillPopUp()
		{
			if (!_isRemoteAvatar && _skillData.UnLocked)
			{
				bool active = false;
				bool active2 = false;
				Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
				foreach (AvatarSubSkillDataItem avatarSubSkill in _skillData.avatarSubSkillList)
				{
					if (subSkillStatusDict.ContainsKey(avatarSubSkill.subSkillID))
					{
						active = true;
					}
					if (avatarSubSkill.UnLocked && avatarSubSkill.level < avatarSubSkill.MaxLv && _avatarData.level >= avatarSubSkill.LvUpNeedAvatarLevel && _avatarData.star >= avatarSubSkill.GetUpLevelStarNeed())
					{
						active2 = true;
					}
				}
				base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(active);
				base.transform.Find("Upgradable").gameObject.SetActive(active2);
			}
			else
			{
				base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(false);
				base.transform.Find("Upgradable").gameObject.SetActive(false);
			}
		}
	}
}
