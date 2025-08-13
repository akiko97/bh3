using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarDetailLvUpTab : MonoBehaviour
	{
		private FriendDetailDataItem _userData;

		private AvatarDataItem _avatarData;

		protected bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarData)
		{
			_isRemoteAvatar = false;
			_avatarData = avatarData;
			SetupAvatarBasicStatus(base.transform.Find("BasicStatus/InfoPanel/BasicStatus"));
			base.transform.Find("BasicStatus/InfoPanel/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(_avatarData.star);
			base.transform.Find("Introduction").gameObject.SetActive(false);
			base.transform.Find("Star").gameObject.SetActive(true);
			SetupTiltSlider(base.transform.Find("Star/InfoRowFragment/Fragment"), avatarData.fragment, avatarData.MaxFragment);
			base.transform.Find("Star/StarUpBtn").GetComponent<Button>().interactable = avatarData.CanStarUp;
			base.transform.Find("Star/StarUpBtn/PopUp").gameObject.SetActive(avatarData.CanStarUp);
			MonoButtonWwiseEvent monoButtonWwiseEvent = null;
			monoButtonWwiseEvent = base.transform.Find("Star/StarUpBtn").GetComponent<MonoButtonWwiseEvent>();
			if (monoButtonWwiseEvent == null)
			{
				monoButtonWwiseEvent = base.transform.Find("Star/StarUpBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
			}
			monoButtonWwiseEvent.eventName = ((!base.transform.Find("Star/StarUpBtn").GetComponent<Button>().interactable) ? "UI_Gen_Select_Negative" : "UI_Click");
			base.transform.Find("Lv").gameObject.SetActive(true);
			SetupTiltSlider(base.transform.Find("Lv/InfoRowLv/Exp"), avatarData.exp, avatarData.MaxExp);
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			base.transform.Find("Lv/LvUpBtn").GetComponent<Button>().interactable = _avatarData.level < playerData.AvatarLevelLimit || _avatarData.exp < _avatarData.MaxExp;
			monoButtonWwiseEvent = base.transform.Find("Lv/LvUpBtn").GetComponent<MonoButtonWwiseEvent>();
			if (monoButtonWwiseEvent == null)
			{
				monoButtonWwiseEvent = base.transform.Find("Lv/LvUpBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
			}
			monoButtonWwiseEvent.eventName = ((!base.transform.Find("Lv/LvUpBtn").GetComponent<Button>().interactable) ? "UI_Gen_Select_Negative" : "UI_Click");
		}

		public void SetupView(FriendDetailDataItem userData)
		{
			_isRemoteAvatar = true;
			_userData = userData;
			_avatarData = _userData.leaderAvatar;
			SetupAvatarBasicStatus(base.transform.Find("BasicStatus/InfoPanel/BasicStatus"));
			base.transform.Find("BasicStatus/InfoPanel/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(_avatarData.star);
			base.transform.Find("Star").gameObject.SetActive(false);
			base.transform.Find("Lv").gameObject.SetActive(false);
			base.transform.Find("Introduction").gameObject.SetActive(true);
			base.transform.Find("Introduction/InfoPanel/Desc").GetComponent<Text>().text = _userData.Desc;
			bool flag = Singleton<FriendModule>.Instance.IsMyFriend(_userData.uid);
			base.transform.Find("Introduction/AddBtn").gameObject.SetActive(!flag);
			base.transform.Find("Introduction/DeleteBtn").gameObject.SetActive(flag);
		}

		public void OnLvUpBtnClick()
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			bool flag = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial().Count > 0;
			bool flag2 = _avatarData.level < playerData.AvatarLevelLimit || _avatarData.exp < _avatarData.MaxExp;
			if (flag && flag2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new MaterialUseDialogContext(_avatarData));
				return;
			}
			string text = LocalizationGeneralLogic.GetText("Err_Unknown");
			if (!flag)
			{
				text = LocalizationGeneralLogic.GetText("Err_NoLvUpItem");
			}
			else if (!flag2)
			{
				text = LocalizationGeneralLogic.GetText("Err_AvatarLevelLimit", playerData.teamLevel, playerData.AvatarLevelLimit);
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.SingleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = text
			});
		}

		public void OnStarUpBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestAvatarStarUp(_avatarData.avatarID);
		}

		public void OnDeleteFriendBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Action_DeleteFriend"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConfirmDeleteFriend"),
				okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_Delete"),
				buttonCallBack = DeleteFriendConfirmCallBack
			});
		}

		public void OnAddFriendBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestAddFriend(_userData.uid);
		}

		public void OnShowInfoPopUpBtnClick()
		{
			base.transform.Find("InfoPopup").gameObject.SetActive(true);
			base.transform.Find("InfoPopUpCloseBtn").gameObject.SetActive(true);
		}

		public void OnCloseInfoPopUpBtnClick()
		{
			base.transform.Find("InfoPopup").gameObject.SetActive(false);
			base.transform.Find("InfoPopUpCloseBtn").gameObject.SetActive(false);
		}

		private void SetupAvatarBasicStatus(Transform trans)
		{
			trans.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalHPUI).ToString();
			trans.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalSPUI).ToString();
			trans.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalAttackUI).ToString();
			trans.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalDefenseUI).ToString();
			trans.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalCriticalUI).ToString();
		}

		private void SetupTiltSlider(Transform trans, float value, float maxValue)
		{
			float num = value;
			float maxValue2 = maxValue;
			string text = value.ToString();
			string text2 = maxValue.ToString();
			if (maxValue == 0f)
			{
				maxValue2 = num;
				text2 = "MAX";
			}
			trans.Find("NumText").GetComponent<Text>().text = text;
			trans.Find("MaxNumText").GetComponent<Text>().text = text2;
			trans.Find("TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(num, maxValue2, 0f);
		}

		private void DeleteFriendConfirmCallBack(bool isConfirm)
		{
			if (isConfirm)
			{
				Singleton<NetworkManager>.Instance.RequestDelFriend(_userData.uid);
			}
		}
	}
}
