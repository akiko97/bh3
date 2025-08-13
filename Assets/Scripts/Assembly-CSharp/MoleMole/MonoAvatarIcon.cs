using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAvatarIcon : MonoBehaviour
	{
		private const string _border_path_jixie = "SpriteOutput/AvatarIcon/AvatarAttrJiXie";

		private const string _border_path_shengwu = "SpriteOutput/AvatarIcon/AvatarAttrShengWu";

		private const string _border_path_yineng = "SpriteOutput/AvatarIcon/AvatarAttrYiNeng";

		private AvatarDataItem _avatarData;

		private bool _isSelected;

		public static string bg_path_jixie = "SpriteOutput/AvatarIcon/AttrJiXie";

		public static string bg_path_shengwu = "SpriteOutput/AvatarIcon/AttrShengWu";

		public static string bg_path_yineng = "SpriteOutput/AvatarIcon/AttrYiNeng";

		private EndlessAvatarHp _avatarHp;

		public void SetupView(AvatarDataItem avatarDataItem, bool isSelected, EndlessAvatarHp avatarHP = null)
		{
			_avatarData = avatarDataItem;
			_avatarHp = avatarHP;
			base.transform.Find("Panel").gameObject.SetActive(false);
			base.transform.Find("BG").GetComponent<Image>().sprite = GetBGSprite();
			base.transform.Find("Frame").GetComponent<Image>().sprite = GetBorderSprite();
			base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.IconPath);
			base.transform.Find("PopUp").gameObject.SetActive(_avatarData.CanStarUp);
			base.transform.Find("LockImg").gameObject.SetActive(!_avatarData.UnLocked);
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)1);
			bool active = memberList.Count > 0 && memberList.Contains(_avatarData.avatarID);
			bool flag = memberList.Count > 0 && _avatarData.avatarID == memberList[0];
			base.transform.Find("FlagImg").gameObject.SetActive(active);
			base.transform.Find("FlagImg").GetComponent<Image>().color = ((!flag) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("Yellow"));
			base.transform.Find("AvatarStar").gameObject.SetActive(_avatarData.UnLocked);
			if (_avatarData.UnLocked)
			{
				SetAvatarStar();
			}
			base.transform.Find("LvText").GetComponent<Text>().text = ((!_avatarData.UnLocked) ? LocalizationGeneralLogic.GetText("Menu_AvatarLocked") : ("Lv." + _avatarData.level));
			base.transform.Find("HPRemain").gameObject.SetActive(avatarHP != null);
			base.transform.Find("Icon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
			base.transform.Find("FlashHint").gameObject.SetActive(false);
			base.transform.Find("LvText").gameObject.SetActive(true);
			SetUpAvatarDispatched(Singleton<IslandModule>.Instance.IsAvatarDispatched(_avatarData.avatarID));
			if (avatarHP != null)
			{
				base.transform.Find("HPRemain").GetComponent<MonoRemainHP>().SetAvatarHPData(_avatarHp, EndlessAvatarDieCallBack);
			}
			SetupSelectedView(isSelected);
		}

		public void SetupSelectedView(bool isSelected)
		{
			_isSelected = isSelected;
			base.transform.Find("Button").GetComponent<Button>().interactable = !_isSelected;
		}

		public void OnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarIconChange, _avatarData.avatarID));
		}

		private Sprite GetBGSprite()
		{
			switch ((EntityNature)_avatarData.Attribute)
			{
			case EntityNature.Mechanic:
				return Miscs.GetSpriteByPrefab(bg_path_jixie);
			case EntityNature.Biology:
				return Miscs.GetSpriteByPrefab(bg_path_shengwu);
			case EntityNature.Psycho:
				return Miscs.GetSpriteByPrefab(bg_path_yineng);
			default:
				return null;
			}
		}

		private Sprite GetBorderSprite()
		{
			switch ((EntityNature)_avatarData.Attribute)
			{
			case EntityNature.Mechanic:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrJiXie");
			case EntityNature.Biology:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrShengWu");
			case EntityNature.Psycho:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrYiNeng");
			default:
				return null;
			}
		}

		private void SetAvatarStar()
		{
			for (int i = 1; i < 6; i++)
			{
				string text = string.Format("{0}/{1}", "AvatarStar", i);
				base.transform.Find(text).gameObject.SetActive(i == _avatarData.star);
			}
		}

		private void EndlessAvatarDieCallBack(bool avatarDie)
		{
			base.transform.Find("Panel").gameObject.SetActive(avatarDie);
			base.transform.Find("Icon").GetComponent<Image>().color = ((!avatarDie) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout"));
			base.transform.Find("FlashHint").gameObject.SetActive(avatarDie);
			base.transform.Find("LvText").gameObject.SetActive(!avatarDie);
		}

		private void SetUpAvatarDispatched(bool isDispatched)
		{
			base.transform.Find("Icon").GetComponent<Image>().color = ((!isDispatched) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout"));
		}
	}
}
