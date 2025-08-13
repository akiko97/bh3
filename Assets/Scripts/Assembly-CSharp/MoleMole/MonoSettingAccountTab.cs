using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSettingAccountTab : MonoBehaviour
	{
		public void SetupView()
		{
			bool flag = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
			base.transform.Find("LogoutBtn").gameObject.SetActive(flag);
			base.transform.Find("BindAccountBtn").gameObject.SetActive(!flag);
			base.transform.Find("Content/Info/AccountName").gameObject.SetActive(flag);
			base.transform.Find("Content/Info/TryUserLabel").gameObject.SetActive(!flag);
			if (flag)
			{
				base.transform.Find("Content/Info/AccountName/Text").GetComponent<Text>().text = Singleton<AccountManager>.Instance.manager.GetAccountName();
			}
			base.transform.Find("Content/Info/PlayerId/Text").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
			GeneralLocalDataItem.AccountData account = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account;
			bool active = !flag || string.IsNullOrEmpty(account.email);
			bool active2 = flag && !string.IsNullOrEmpty(account.email) && !account.isEmailVerify;
			bool active3 = flag && !string.IsNullOrEmpty(account.email) && account.isEmailVerify;
			bool flag2 = !flag || string.IsNullOrEmpty(account.mobile);
			base.transform.Find("Content/Security/BindEmail/BindBtn").gameObject.SetActive(flag && string.IsNullOrEmpty(account.email));
			base.transform.Find("Content/Security/BindEmail/Status/NotVerify").gameObject.SetActive(active2);
			base.transform.Find("Content/Security/BindEmail/Status/NotBind").gameObject.SetActive(active);
			base.transform.Find("Content/Security/BindEmail/Status/AlreadyBind").gameObject.SetActive(active3);
			Button component = base.transform.Find("Content/Security/BindEmail/VerifyBtn").GetComponent<Button>();
			component.gameObject.SetActive(active2);
			component.interactable = !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply;
			base.transform.Find("Content/Security/BindMobile/BindBtn").gameObject.SetActive(flag && string.IsNullOrEmpty(account.mobile));
			base.transform.Find("Content/Security/BindMobile/Status/AlreadyBind").gameObject.SetActive(!flag2);
			base.transform.Find("Content/Security/BindMobile/Status/NotBind").gameObject.SetActive(flag2);
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				base.transform.Find("Content/ExchangeReward").gameObject.SetActive(false);
			}
			else
			{
				base.transform.Find("Content/ExchangeReward").gameObject.SetActive(true);
			}
		}

		public void OnLogoutBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Logout"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_Logout"),
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext());
						Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
						Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
						GeneralLogicManager.RestartGame();
					}
				}
			});
		}

		public void OnBindAccountBtnClick()
		{
			Singleton<AccountManager>.Instance.manager.BindUI();
		}

		public void OnBindEmailBtnClick()
		{
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				string url = string.Format(theOriginalAccountManager.ORIGINAL_BIND_EMAIL_URL + "?uid={0}&token={1}", accountUid, accountToken);
				WebViewGeneralLogic.LoadUrl(url);
			}
		}

		public void OnBindMobileBtnClick()
		{
			string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
			string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
			TheOriginalAccountManager theOriginalAccountManager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
			if (theOriginalAccountManager != null)
			{
				string url = string.Format(theOriginalAccountManager.ORIGINAL_BIND_MOBILE_URL + "?uid={0}&token={1}", accountUid, accountToken);
				WebViewGeneralLogic.LoadUrl(url);
			}
		}

		public void OnVerifyEmailApplyBtnClick()
		{
			Singleton<AccountManager>.Instance.manager.VerifyEmailApply();
		}

		public void OnCDKEYValueChanged()
		{
		}

		public void OnExchangePresentBtnClick()
		{
		}

		public bool CheckNeedSave()
		{
			return false;
		}
	}
}
