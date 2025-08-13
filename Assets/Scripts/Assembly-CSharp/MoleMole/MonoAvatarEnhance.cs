using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAvatarEnhance : MonoBehaviour
	{
		private int _avatarClassID;

		private float _hpAdd;

		private float _spAdd;

		private float _atkAdd;

		private float _defAdd;

		private float _crtAdd;

		public void SetupView(int avatarClassID)
		{
			_avatarClassID = avatarClassID;
			base.transform.Find("AvatarInfo/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EnhanceSpeicalAvatarAttr", GetAvatarClassName());
			base.transform.Find("AvatarInfo/AvatarImage").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.IslandAvatarEnhanceClassImage[avatarClassID]);
			GetAddInfo();
			SetAttrAddInfo();
			ShowAttrAddInfo(base.transform.Find("AvatarInfo").GetComponent<Toggle>().isOn);
		}

		private void GetAddInfo()
		{
			CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(_avatarClassID);
			_hpAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)1) * 100f;
			_spAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)2) * 100f;
			_atkAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)3) * 100f;
			_crtAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)5) * 100f;
			_defAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance((AvatarAttrType)4) * 100f;
		}

		public void ShowAttrAddInfo(bool isOn)
		{
			base.transform.Find("AttrEnhanceList").gameObject.SetActive(isOn);
			base.transform.Find("AvatarInfo/Icon/Spread").gameObject.SetActive(isOn);
			base.transform.Find("AvatarInfo/Icon/Unspread").gameObject.SetActive(!isOn);
		}

		private string GetAvatarClassName()
		{
			switch (_avatarClassID)
			{
			case 1:
				return "KIANA";
			case 2:
				return "MEI";
			case 3:
				return "BRONYA";
			case 4:
				return "HIMEKO";
			case 5:
				return "FUKA";
			default:
				return string.Empty;
			}
		}

		private void SetAttrAddInfo()
		{
			Transform transform = base.transform.Find("AttrEnhanceList");
			transform.Find("HpEnhance").gameObject.SetActive(_hpAdd > 0f);
			if (_hpAdd > 0f)
			{
				transform.Find("HpEnhance/Num").GetComponent<Text>().text = _hpAdd.ToString("0.00") + "%";
			}
			transform.Find("SpEnhance").gameObject.SetActive(_spAdd > 0f);
			if (_spAdd > 0f)
			{
				transform.Find("SpEnhance/Num").GetComponent<Text>().text = _spAdd.ToString("0.00") + "%";
			}
			transform.Find("AtkEnhance").gameObject.SetActive(_atkAdd > 0f);
			if (_atkAdd > 0f)
			{
				transform.Find("AtkEnhance/Num").GetComponent<Text>().text = _atkAdd.ToString("0.00") + "%";
			}
			transform.Find("DefEnhance").gameObject.SetActive(_defAdd > 0f);
			if (_defAdd > 0f)
			{
				transform.Find("DefEnhance/Num").GetComponent<Text>().text = _defAdd.ToString("0.00") + "%";
			}
			transform.Find("CrtEnhance").gameObject.SetActive(_crtAdd > 0f);
			if (_crtAdd > 0f)
			{
				transform.Find("CrtEnhance/Num").GetComponent<Text>().text = _crtAdd.ToString("0.00") + "%";
			}
			bool flag = _hpAdd + _spAdd + _atkAdd + _defAdd + _crtAdd > 0f;
			transform.Find("NoEnhance").gameObject.SetActive(!flag);
		}
	}
}
