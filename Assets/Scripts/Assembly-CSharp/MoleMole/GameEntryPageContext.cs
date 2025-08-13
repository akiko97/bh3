using System;
using System.Collections;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class GameEntryPageContext : BasePageContext
	{
		public enum DescImageType
		{
			None = 0,
			Loading = 1,
			Identifying = 2,
			Confirmed = 3
		}

		private Coroutine _splashFadeOutCoroutine;

		private int _splashFadeOutFrameCount = 30;

		private int _transitBlackFadeOutFrameCount = 60;

		private int _transitBlackVideoFadeOutFrameCount = 15;

		private bool _hasLogin;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		private bool _agreementShowed;

		public bool _accountReady;

		public bool IsSplashFadeOut { get; private set; }

		public GameEntryPageContext(GameObject view)
		{
			config = new ContextPattern
			{
				contextName = "GameEntryContext"
			};
			showSpaceShip = true;
			base.view = view;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 25:
			case 48:
				return OnGetAvatarReletedInfo();
			case 5:
				return OnGetPlayerTokenRsp(pkt.getData<GetPlayerTokenRsp>());
			case 7:
				return OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());
			case 44:
				return OnStageBeginRsp(pkt.getData<StageBeginRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.ShowMihoyoLoginUI)
			{
				return ShowMihoyoLoginUI();
			}
			if (ntf.type == NotifyTypes.MihoyoAccountLoginSuccess)
			{
				return OnMihoyoAccountLoginSuccess();
			}
			if (ntf.type == NotifyTypes.ShowLoadAssetText)
			{
				return ShowLoadAssetText((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetLoadAssetText)
			{
				return SetLoadAssetText((Tuple<bool, string, bool, float>)ntf.body);
			}
			if (ntf.type == NotifyTypes.FadeOutGameEntrySplash)
			{
				return FadeOutSplash();
			}
			if (ntf.type == NotifyTypes.SDKAccountLoginSuccess)
			{
				return OnSDKAccountLoginSuccess();
			}
			if (ntf.type == NotifyTypes.ShowVersionText)
			{
				return OnShowVersionText();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("DebugBtn").GetComponent<Button>(), OnDebugBtnClick);
			BindViewCallback(base.view.transform.Find("FullScreenLogin"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/LoginBtn").GetComponent<Button>(), OnLoginBtnClick);
			BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/RigsterBtn").GetComponent<Button>(), OnRigsterBtnClick);
			BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/TryUserBtn").GetComponent<Button>(), OnTryUserBtnClick);
			BindViewCallback(base.view.transform.Find("LoginPanel/LastUser/LogoutBtn").GetComponent<Button>(), OnLogoutBtnClick);
			BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/ForbidNewUser/LoginBtn").GetComponent<Button>(), OnLoginBtnClick);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("LoginPanel").gameObject.SetActive(false);
			base.view.transform.Find("DebugBtn").gameObject.SetActive(false);
			return false;
		}

		public override void Destroy()
		{
			StopSplashFadeOut();
			base.Destroy();
		}

		public void OnTryUserBtnClick()
		{
			bool flag = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId != 0;
			bool flag2 = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
			if (!flag && !flag2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_GuestLogin"),
					notDestroyAfterTouchBG = true,
					buttonCallBack = delegate(bool isOK)
					{
						if (isOK)
						{
							_agreementShowed = true;
							Singleton<MainUIManager>.Instance.ShowDialog(new AgreementDialogContext
							{
								buttonCallBack = delegate(bool agree)
								{
									if (agree)
									{
										TryUserLogin();
									}
								}
							});
						}
					}
				});
			}
			else
			{
				TryUserLogin();
			}
		}

		private void TryUserLogin()
		{
			Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
			Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			Singleton<AccountManager>.Instance.manager.Reset();
			Singleton<NetworkManager>.Instance.LoginGameServer();
		}

		public void OnLoginBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoLoginDialogContext());
		}

		public void OnRigsterBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoRegisterDialogContext());
		}

		public void OnLogoutBtnClick()
		{
			base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(false);
			base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(true);
			base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("ENTRY_PREPARED_LOGIN");
			_accountReady = false;
		}

		public void OnBGClick(BaseEventData data = null)
		{
			if (!_hasLogin && _accountReady)
			{
				Singleton<NetworkManager>.Instance.LoginGameServer();
			}
		}

		public void OnDebugBtnClick()
		{
			GlobalVars.DISABLE_NETWORK_DEBUG = true;
			SuperDebug.DEBUG_SWITCH[6] = true;
			FakePacketHelper.LoadFromFile();
			Singleton<NetworkManager>.Instance.LoginGameServer();
		}

		public bool ShowMihoyoLoginUI()
		{
			base.view.transform.Find("LoginPanel").gameObject.SetActive(true);
			GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
			bool flag = generalLocalData.LastLoginUserId != 0;
			bool flag2 = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
			_accountReady = flag || flag2;
			base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(flag || flag2);
			base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(!flag && !flag2);
			string textID = ((flag || flag2) ? "ENTRY_PREPARED" : "ENTRY_PREPARED_LOGIN");
			base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			if (flag2)
			{
				string accountName = Singleton<AccountManager>.Instance.manager.GetAccountName();
				base.view.transform.Find("LoginPanel/LastUser/Info/Name").GetComponent<Text>().text = accountName;
				base.view.transform.Find("LoginPanel/LastUser/Info/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_User");
			}
			else if (flag)
			{
				string text = generalLocalData.LastLoginUserId.ToString();
				base.view.transform.Find("LoginPanel/LastUser/Info/Name").GetComponent<Text>().text = text;
				base.view.transform.Find("LoginPanel/LastUser/Info/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_TryUser");
			}
			bool forbidNewUser = Singleton<NetworkManager>.Instance.DispatchSeverData.forbidNewUser;
			base.view.transform.Find("LoginPanel/EmptyUser/ForbidNewUser").gameObject.SetActive(forbidNewUser);
			base.view.transform.Find("LoginPanel/EmptyUser/Normal").gameObject.SetActive(!forbidNewUser);
			return false;
		}

		public bool OnMihoyoAccountLoginSuccess()
		{
			string accountName = Singleton<AccountManager>.Instance.manager.GetAccountName();
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_WelcomeBack") + " " + accountName));
			ShowMihoyoLoginUI();
			return false;
		}

		public bool OnSDKAccountLoginSuccess()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_WelcomeBack")));
			_accountReady = true;
			return false;
		}

		private bool OnGetPlayerTokenRsp(GetPlayerTokenRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode != 0)
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
				if ((int)rsp.retcode == 3)
				{
					Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					Singleton<AccountManager>.Instance.manager.Reset();
					ShowMihoyoLoginUI();
				}
			}
			return false;
		}

		private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Invalid comparison between Unknown and I4
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Invalid comparison between Unknown and I4
			if (_hasLogin)
			{
				return false;
			}
			if ((int)rsp.retcode == 0)
			{
				base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(false);
				base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(false);
				MonoGameEntry gameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (rsp.is_first_login && !_agreementShowed)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new AgreementDialogContext
					{
						buttonCallBack = delegate(bool agree)
						{
							if (agree)
							{
								gameEntry.OnPlayerLogin(rsp.is_first_login);
							}
							else
							{
								ApplicationManager.Quit();
							}
						}
					});
				}
				else
				{
					gameEntry.OnPlayerLogin(rsp.is_first_login);
				}
				_hasLogin = true;
			}
			else if ((int)rsp.retcode != 4)
			{
				string desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				if ((int)rsp.retcode == 3)
				{
					DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(rsp.black_list_end_time);
					desc = LocalizationGeneralLogic.GetText("Menu_BlackList", Singleton<PlayerModule>.Instance.playerData.userId, rsp.msg, dateTimeFromTimeStamp.ToString("yyyy-MM-dd HH:mm:ss"));
				}
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				generalDialogContext.desc = desc;
				generalDialogContext.notDestroyAfterTouchBG = true;
				GeneralDialogContext dialogContext = generalDialogContext;
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
			return false;
		}

		public bool OnStageBeginRsp(StageBeginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				DoBeginLevel(rsp);
			}
			return false;
		}

		private bool ShowLoadAssetText(bool active)
		{
			return false;
		}

		private bool SetLoadAssetText(Tuple<bool, string, bool, float> loadingInfo)
		{
			base.view.transform.Find("LoadingPanel").gameObject.SetActive(loadingInfo.Item1);
			base.view.transform.Find("LoadingPanel/Desc").GetComponent<Text>().text = loadingInfo.Item2;
			base.view.transform.Find("LoadingPanel/Progress").gameObject.SetActive(loadingInfo.Item3);
			base.view.transform.Find("LoadingPanel/Progress").GetComponent<Slider>().value = loadingInfo.Item4;
			if (loadingInfo.Item2 == LocalizationGeneralLogic.GetText("ENTRY_PREPARED_LOGIN") || loadingInfo.Item2 == LocalizationGeneralLogic.GetText("ENTRY_PREPARED"))
			{
				base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = loadingInfo.Item2;
				base.view.transform.Find("LoadingPanel").GetComponent<Animation>().Play();
			}
			return false;
		}

		private bool FadeOutSplash()
		{
			_splashFadeOutCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(FadeOutSplashImp());
			return false;
		}

		public bool FadeOutVideo()
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(FadeOutVideoImp());
			return false;
		}

		private bool OnGetAvatarReletedInfo()
		{
			SetupAvatar3dModel();
			return false;
		}

		private bool OnShowVersionText()
		{
			base.view.transform.Find("VersionText").gameObject.SetActive(true);
			string text = "v" + Singleton<NetworkManager>.Instance.GetGameVersion();
			if (GlobalVars.DataUseAssetBundle)
			{
				text += "-asb";
			}
			base.view.transform.Find("VersionText").GetComponent<Text>().text = text;
			return false;
		}

		private IEnumerator FadeOutSplashImp()
		{
			yield return null;
			float lerpBegin = 1f;
			float lerpEnd = 0f;
			Image image = base.view.transform.Find("Splash").GetComponent<Image>();
			Color temp = Color.white;
			int count = 0;
			while (count <= _splashFadeOutFrameCount)
			{
				float t = (float)count / (float)_splashFadeOutFrameCount;
				float alpha = Mathf.Lerp(lerpBegin, lerpEnd, Mathf.Clamp01(t));
				temp.r = image.color.r;
				temp.g = image.color.g;
				temp.b = image.color.b;
				temp.a = alpha;
				image.color = temp;
				count++;
				yield return null;
			}
			image.gameObject.SetActive(false);
			count = 0;
			Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Start");
			image = base.view.transform.Find("TransitBlack").GetComponent<Image>();
			while (count <= _transitBlackFadeOutFrameCount)
			{
				if (count == 0 || count == 2)
				{
					count++;
					yield return null;
				}
				else if (count == 1)
				{
					GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
					LocalDataVersion.LoadFromFile();
					GraphicsSettingData.ReloadFromFile();
					GraphicsSettingData.ApplySettingConfig();
					count++;
					yield return null;
				}
				else
				{
					float t2 = (float)count / (float)_transitBlackFadeOutFrameCount;
					float alpha2 = Mathf.Lerp(lerpBegin, lerpEnd, Mathf.Clamp01(t2));
					temp.r = image.color.r;
					temp.g = image.color.g;
					temp.b = image.color.b;
					temp.a = alpha2;
					image.color = temp;
					count++;
					yield return null;
				}
			}
			base.view.transform.Find("Splash").gameObject.SetActive(false);
			base.view.transform.Find("TransitBlack").gameObject.SetActive(false);
			IsSplashFadeOut = true;
			_splashFadeOutCoroutine = null;
		}

		private IEnumerator FadeOutVideoImp()
		{
			base.view.transform.Find("TransitBlack").gameObject.SetActive(true);
			Image image = base.view.transform.Find("TransitBlack").GetComponent<Image>();
			Color color = Color.white;
			int count = 0;
			while (count <= _transitBlackVideoFadeOutFrameCount)
			{
				float t = (float)count / (float)_transitBlackVideoFadeOutFrameCount;
				float a = Mathf.Lerp(0f, 1f, Mathf.Clamp01(t));
				color.r = image.color.r;
				color.g = image.color.g;
				color.b = image.color.b;
				color.a = a;
				image.color = color;
				count++;
				yield return null;
			}
		}

		private void StopSplashFadeOut()
		{
			if (_splashFadeOutCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_splashFadeOutCoroutine);
				_splashFadeOutCoroutine = null;
			}
		}

		private bool SetupAvatar3dModel()
		{
			int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(readyToTouchAvatarID);
			if (avatarDataItem == null || !avatarDataItem.UnLocked)
			{
				avatarDataItem = Singleton<AvatarModule>.Instance.UserAvatarList.Find((AvatarDataItem x) => x.UnLocked);
			}
			UIUtil.Create3DAvatarByPage(avatarDataItem, MiscData.PageInfoKey.GameEntryPage);
			Singleton<GalTouchModule>.Instance.ChangeAvatar(avatarDataItem.avatarID);
			return false;
		}

		public bool StartFirstLevel()
		{
			try
			{
				LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(10101);
				if (levelDataItem == null)
				{
					return false;
				}
				_loadingWheelDialogContext = new LoadingWheelWidgetContext();
				Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
				Singleton<NetworkManager>.Instance.RequestLevelBeginReq(levelDataItem);
				return true;
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError(ex.ToString());
				return false;
			}
		}

		private void DoBeginLevel(StageBeginRsp rsp)
		{
			if (Singleton<LevelScoreManager>.Instance == null)
			{
				Singleton<LevelScoreManager>.Create();
			}
			Singleton<LevelScoreManager>.Instance.collectAntiCheatData = rsp.is_collect_cheat_data;
			Singleton<LevelScoreManager>.Instance.signKey = rsp.sign_key;
			LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(10101);
			Singleton<LevelScoreManager>.Instance.SetLevelBeginIntent(levelDataItem, 0, rsp.drop_item_list, levelDataItem.BattleType);
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
			Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", false, true, true, null, false);
		}

		public void DisableVideoFadeOut()
		{
			base.view.transform.Find("TransitBlack").gameObject.SetActive(false);
		}
	}
}
