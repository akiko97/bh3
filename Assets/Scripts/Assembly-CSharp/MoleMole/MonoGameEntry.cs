using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MonoGameEntry : BaseMonoCanvas
	{
		private enum Status
		{
			Default = 0,
			WaitingRetryLoadDataAsset = 1,
			WaitingRetryLoadEventAsset = 2
		}

		public const int MAX_ASSET_BUNDLE_RETRY_TIMES = 4;

		public const float RETRY_ASSET_BUNDLE_WAIT_SECONDS = 2f;

		public GameObject GameEntryPage;

		public int assetbundleRetryTimes;

		public Avatar3dModelContext avatar3dModelContext;

		private GameEntryPageContext _pageContext;

		private ElevatorModelContext _elevatorModelContext;

		private SpaceShipModelContext _spaceshipModelContext;

		private Coroutine _waitLoadSpaceshipCoroutine;

		private Coroutine _waitAvatarModelCoroutine;

		private Coroutine _waitSplashFadeoutCoroutine;

		private Coroutine _waitBeforeLoginCoroutine;

		private Status _status;

		private float _retryAssetBundleWaitTimer;

		private MediaPlayer _videoPlayer;

		private DisplayIMGUI _videoDisplay;

		private bool _startFirstLevel;

		public GameObject spaceshipGO { get; set; }

		public float warshipDefaultYPos { get; set; }

		public void Awake()
		{
			GraphicsSettingUtil.SetTargetFrameRate(60);
			GeneralLogicManager.InitOnGameStart();
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
			assetbundleRetryTimes = 0;
			_status = Status.Default;
			_retryAssetBundleWaitTimer = 0f;
			_videoPlayer = base.transform.Find("Video/VideoPlayer").GetComponent<MediaPlayer>();
			_videoPlayer.Events.AddListener(OnVideoEvent);
			_videoPlayer.gameObject.SetActive(false);
			_videoDisplay = base.transform.Find("Video/VideoDisplay").GetComponent<DisplayIMGUI>();
			_videoDisplay.gameObject.SetActive(false);
			base.transform.Find("Video/BlackPanel").gameObject.SetActive(false);
			base.transform.Find("Video").gameObject.SetActive(false);
		}

		public override void Start()
		{
			GameObject view = GameObject.Find("StartLoading_Model");
			_elevatorModelContext = new ElevatorModelContext(view);
			Singleton<MainUIManager>.Instance.ShowWidget(_elevatorModelContext);
			avatar3dModelContext = new Avatar3dModelContext();
			Singleton<MainUIManager>.Instance.ShowWidget(avatar3dModelContext, UIType.Root);
			_pageContext = new GameEntryPageContext(GameEntryPage);
			Singleton<MainUIManager>.Instance.ShowPage(_pageContext);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.FadeOutGameEntrySplash));
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[3] { "All_In_One_Bank", "BK_Global", "BK_Events" });
			Singleton<WwiseAudioManager>.Instance.ManualPrepareBank("BK_GameEntry");
			Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Start_Alarm");
			string text = LocalizationGeneralLogic.GetText("Menu_ConnectGlobalDispatch");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
			_waitSplashFadeoutCoroutine = StartCoroutine(WaitSplashFadeout());
			PostStartHandleBenchmark();
			AudioSettingData.ApplySettingConfig();
			if (Singleton<EffectManager>.Instance == null)
			{
				Singleton<EffectManager>.Create();
				Singleton<EffectManager>.Instance.InitAtAwake();
				Singleton<EffectManager>.Instance.InitAtStart();
			}
			base.Start();
		}

		public override void Update()
		{
			base.Update();
			if (_status == Status.WaitingRetryLoadDataAsset)
			{
				_retryAssetBundleWaitTimer += Time.fixedDeltaTime;
				if (_retryAssetBundleWaitTimer >= 2f)
				{
					_status = Status.Default;
					_retryAssetBundleWaitTimer = 0f;
					RetryCheckAndLoadDataAsset();
				}
			}
			else if (_status == Status.WaitingRetryLoadEventAsset)
			{
				_retryAssetBundleWaitTimer += Time.fixedDeltaTime;
				if (_retryAssetBundleWaitTimer >= 2f)
				{
					_status = Status.Default;
					_retryAssetBundleWaitTimer = 0f;
					RetryCheckAndLoadEventAsset();
				}
			}
		}

		public override void OnDestroy()
		{
			StopWaitLoadSpaceship();
			StopWaitAvatarModel();
			StopWaitSplashFadeout();
			StopWaitBeforeLogin();
			if (_startFirstLevel)
			{
				if (_spaceshipModelContext != null && _spaceshipModelContext.view != null)
				{
					UnityEngine.Object.Destroy(_spaceshipModelContext.view);
				}
				if (avatar3dModelContext != null && avatar3dModelContext.view != null)
				{
					UnityEngine.Object.Destroy(avatar3dModelContext.view);
				}
			}
			base.OnDestroy();
		}

		public void OnRestartGame()
		{
			StopWaitLoadSpaceship();
			StopWaitAvatarModel();
			StopWaitSplashFadeout();
			StopWaitBeforeLogin();
			if (_spaceshipModelContext != null && _spaceshipModelContext.view != null)
			{
				UnityEngine.Object.Destroy(_spaceshipModelContext.view);
			}
			if (avatar3dModelContext != null && avatar3dModelContext.view != null)
			{
				UnityEngine.Object.Destroy(avatar3dModelContext.view);
			}
		}

		public void ConnentGlobalDispatch()
		{
			StartCoroutine(Singleton<NetworkManager>.Instance.ConnectGlobalDispatchServer(ConnectDispatch));
		}

		public void ConnectDispatch()
		{
			StartCoroutine(Singleton<NetworkManager>.Instance.ConnectDispatchServer(DispatchConnectCallback));
		}

		private void DispatchConnectCallback()
		{
			Singleton<AccountManager>.Instance.manager.SetupByDispatchServerData();
			if (GlobalVars.DataUseAssetBundle)
			{
				CheckAndLoadDataAsset();
			}
			else
			{
				OnDataAssetReady();
			}
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.showVersionText)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowVersionText));
			}
		}

		private void CheckAndLoadDataAsset()
		{
			Singleton<AssetBundleManager>.Instance.Loader.LoadVersionFile(BundleType.DATA_FILE, OnLoadingDataVersionProgress, delegate(bool success)
			{
				if (success)
				{
					StartCoroutine(Singleton<AssetBundleManager>.Instance.Loader.StartDownloadAssetBundle(BundleType.DATA_FILE, OnLoadingDataAssetBundleProgress, null, OnLoadingDataAssetBundleComplete));
				}
				else
				{
					OnLoadingDataAssetBundleComplete(false);
				}
			});
		}

		private void OnDataAssetReady()
		{
			GeneralLogicManager.InitOnDataAssetReady(true, AfterDataAssetReady);
		}

		private void AfterDataAssetReady()
		{
			if (GlobalVars.ResourceUseAssetBundle)
			{
				CheckAndLoadEventAsset();
			}
			else
			{
				OnEventAssetReady();
			}
		}

		private void OnLoadingDataVersionProgress(long current, long total, long delta, float speed)
		{
			string text = LocalizationGeneralLogic.GetText("Menu_CheckDataAsset");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
		}

		private void OnLoadingDataAssetBundleProgress(long current, long total, long delta, float speed)
		{
			float item = (float)current / (float)total;
			string text = LocalizationGeneralLogic.GetText("Menu_DownloadDataAsset");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, true, item)));
		}

		private void OnLoadingDataAssetBundleComplete(bool result)
		{
			if (result)
			{
				assetbundleRetryTimes = 0;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLoadAssetText, false));
				OnDataAssetReady();
			}
			else
			{
				TriggerRetryCheckAndLoadDataAsset();
			}
		}

		private void TriggerRetryCheckAndLoadDataAsset()
		{
			_status = Status.WaitingRetryLoadDataAsset;
			_retryAssetBundleWaitTimer = 0f;
		}

		private void RetryCheckAndLoadDataAsset()
		{
			assetbundleRetryTimes++;
			if (assetbundleRetryTimes > 4)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_NetError"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_DownloadDataAssetErr"),
					notDestroyAfterTouchBG = true,
					buttonCallBack = delegate
					{
						CheckAndLoadDataAsset();
						TriggerRetryCheckAndLoadDataAsset();
					}
				});
			}
			else
			{
				CheckAndLoadDataAsset();
			}
		}

		private void CheckAndLoadEventAsset()
		{
			Singleton<AssetBundleManager>.Instance.Loader.LoadVersionFile(BundleType.RESOURCE_FILE, OnLoadingEventVersionProgress, delegate(bool success)
			{
				if (success)
				{
					StartCoroutine(Singleton<AssetBundleManager>.Instance.Loader.StartDownloadAssetBundle(BundleType.RESOURCE_FILE, OnLoadingEventAssetBundleProgress, null, OnLoadingEventAssetBundleComplete));
				}
				else
				{
					OnLoadingEventAssetBundleComplete(false);
				}
			});
		}

		private void OnEventAssetReady()
		{
			Singleton<AssetBundleManager>.Instance.UpdateEventSVNVersion();
			_waitBeforeLoginCoroutine = StartCoroutine(WaitBeforeLogin());
		}

		private void OnLoadingEventVersionProgress(long current, long total, long delta, float speed)
		{
			string text = LocalizationGeneralLogic.GetText("Menu_CheckEventAsset");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
		}

		private void OnLoadingEventAssetBundleProgress(long current, long total, long delta, float speed)
		{
			float item = (float)current / (float)total;
			string text = LocalizationGeneralLogic.GetText("Menu_DownloadEventAsset");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, true, item)));
		}

		private void OnLoadingEventAssetBundleComplete(bool result)
		{
			if (result)
			{
				assetbundleRetryTimes = 0;
				_status = Status.Default;
				_retryAssetBundleWaitTimer = 0f;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLoadAssetText, false));
				OnEventAssetReady();
			}
			else
			{
				TriggerRetryCheckAndLoadEventAsset();
			}
		}

		private void TriggerRetryCheckAndLoadEventAsset()
		{
			_status = Status.WaitingRetryLoadEventAsset;
			_retryAssetBundleWaitTimer = 0f;
		}

		private void RetryCheckAndLoadEventAsset()
		{
			assetbundleRetryTimes++;
			if (assetbundleRetryTimes > 4)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_NetError"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_DownloadEventAssetErr"),
					notDestroyAfterTouchBG = true,
					buttonCallBack = delegate
					{
						CheckAndLoadEventAsset();
						TriggerRetryCheckAndLoadEventAsset();
					}
				});
			}
			else
			{
				CheckAndLoadEventAsset();
			}
		}

		public void OnPlayerLogin(bool isFirstTime)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(false, string.Empty, false, 0f)));
			UIUtil.SpaceshipCheckWeather();
			Singleton<IslandModule>.Instance.InitTechTree();
			if (isFirstTime)
			{
				PlayVideo();
			}
			else
			{
				_waitAvatarModelCoroutine = StartCoroutine(WaitCreateAvatarModel());
				DisableVideo();
			}
			if (Singleton<RealTimeWeatherManager>.Instance.Available)
			{
				ProcessRealtimeWeatherUpdate();
			}
			Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ReportUIStatistics();
		}

		public void OnElevatorFloorAnimEvent(int phase)
		{
			if (phase == 1)
			{
				GraphicsSettingUtil.EnableUIAvatarsDynamicBone(false);
			}
		}

		public void OnElevatorFloorPhase1AnimOver()
		{
			_elevatorModelContext.HideSomeParts();
			Animation component = spaceshipGO.transform.Find("Warship").GetComponent<Animation>();
			component.Play("WarshipFall");
			_elevatorModelContext.PlayFloorPhase2Animation();
			SetUIAvatarStandOnSpaceship();
		}

		public void OnElevatorFloorPhase2AnimOver()
		{
			GraphicsSettingUtil.EnableUIAvatarsDynamicBone(true);
			Camera main = Camera.main;
			Animation component = main.gameObject.GetComponent<Animation>();
			component.Play("BeforeEnterSpaceship");
		}

		public void OnElevatorDoorAnimOver()
		{
			Camera main = Camera.main;
			Animation component = main.gameObject.GetComponent<Animation>();
			component.Play("EnterSpaceship");
			_elevatorModelContext.PlayBackAnimation();
		}

		public void OnCameraBeforeEnterSpaceshipAnimOver()
		{
			_elevatorModelContext.SetDescImage(ElevatorModelContext.DescImageType.Confirmed);
			_elevatorModelContext.PlayDoorAnimation();
			Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Door_Open");
		}

		public void OnCameraEnterSpaceshipAnimEvent(int phase)
		{
			if (phase == 1)
			{
				Singleton<WwiseAudioManager>.Instance.Post("VO_M_Con_12_CPT_On_Bridge");
			}
			switch (phase)
			{
			case 2:
				TriggerAvatarModelTurnAround();
				break;
			case 3:
				Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
				Singleton<MainMenuBGM>.Instance.TryEnterMainMenu();
				Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithoutSpaceship");
				break;
			}
		}

		private IEnumerator WaitSplashFadeout()
		{
			while (!_pageContext.IsSplashFadeOut)
			{
				yield return null;
			}
			_waitSplashFadeoutCoroutine = null;
			if (!GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				ConnentGlobalDispatch();
				yield break;
			}
			FakePacketHelper.FakeConnectDispatch();
			OnDataAssetReady();
		}

		private IEnumerator WaitBeforeLogin()
		{
			_waitLoadSpaceshipCoroutine = StartCoroutine(WaitLoadSpaceShip());
			yield return _waitLoadSpaceshipCoroutine;
			bool hasLastLoginUser = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId != 0;
			bool isLoginByAccount = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
			string text = ((hasLastLoginUser || isLoginByAccount) ? "ENTRY_PREPARED" : "ENTRY_PREPARED_LOGIN");
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, LocalizationGeneralLogic.GetText(text), true, 1f)));
			_elevatorModelContext.SetDescImage(ElevatorModelContext.DescImageType.Identifying);
			Singleton<AccountManager>.Instance.manager.LoginUI();
			_waitBeforeLoginCoroutine = null;
		}

		private IEnumerator WaitLoadSpaceShip()
		{
			AsyncAssetRequst resReq = Miscs.LoadResourceAsync("Stage/MainMenu_SpaceShip/MainMenu_SpaceShip");
			yield return resReq.operation;
			spaceshipGO = UnityEngine.Object.Instantiate((GameObject)resReq.asset);
			spaceshipGO.name = "MainMenu_SpaceShip";
			warshipDefaultYPos = spaceshipGO.transform.Find("Warship").position.y;
			spaceshipGO.transform.Find("Warship").localPosition = new Vector3(0f, 10f, 0f);
			UnityEngine.Object.DontDestroyOnLoad(spaceshipGO);
			_spaceshipModelContext = new SpaceShipModelContext(uiMainCamera: Camera.main.gameObject, view: spaceshipGO);
			Singleton<MainUIManager>.Instance.ShowWidget(_spaceshipModelContext);
			_waitLoadSpaceshipCoroutine = null;
		}

		private IEnumerator WaitCreateAvatarModel()
		{
			List<Transform> avatars = avatar3dModelContext.GetAllAvatars();
			while (avatars.Count == 0)
			{
				yield return null;
				avatars = avatar3dModelContext.GetAllAvatars();
			}
			float timer = 0f;
			while (Singleton<MissionModule>.Instance == null || !Singleton<MissionModule>.Instance.missionDataReceived)
			{
				yield return null;
				timer += Time.deltaTime;
				if (timer > 3f)
				{
					Singleton<NetworkManager>.Instance.RequestGetMissionData();
					timer = 0f;
				}
			}
			_elevatorModelContext.PlayFloorPhase1Animation();
			Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_End");
			_waitAvatarModelCoroutine = null;
		}

		private void SetUIAvatarStandOnSpaceship()
		{
			int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(readyToTouchAvatarID);
			avatar3dModelContext.SetStandOnSpaceship(avatarByID.avatarID);
		}

		private void TriggerAvatarModelTurnAround()
		{
			int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(readyToTouchAvatarID);
			avatar3dModelContext.TriggerAvatarTurnAround(avatarByID.avatarID);
		}

		private void StopWaitAvatarModel()
		{
			if (_waitAvatarModelCoroutine != null)
			{
				StopCoroutine(_waitAvatarModelCoroutine);
				_waitAvatarModelCoroutine = null;
			}
		}

		private void StopWaitSplashFadeout()
		{
			if (_waitSplashFadeoutCoroutine != null)
			{
				StopCoroutine(_waitSplashFadeoutCoroutine);
				_waitSplashFadeoutCoroutine = null;
			}
		}

		private void StopWaitLoadSpaceship()
		{
			if (_waitLoadSpaceshipCoroutine != null)
			{
				StopCoroutine(_waitLoadSpaceshipCoroutine);
				_waitLoadSpaceshipCoroutine = null;
			}
		}

		private void StopWaitBeforeLogin()
		{
			if (_waitBeforeLoginCoroutine != null)
			{
				StopCoroutine(_waitBeforeLoginCoroutine);
				_waitBeforeLoginCoroutine = null;
			}
		}

		private void PostStartHandleBenchmark()
		{
			if (GlobalVars.IS_BENCHMARK)
			{
				Screen.sleepTimeout = -1;
				SuperDebug.CloseAllDebugs();
				GameObject gameObject = new GameObject();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.name = "__Benchmark";
				gameObject.AddComponent<MonoBenchmarkSwitches>();
			}
		}

		private void PlayVideo()
		{
			_pageContext.FadeOutVideo();
			base.transform.Find("Video").gameObject.SetActive(true);
			_videoPlayer.gameObject.SetActive(true);
			try
			{
				CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItemList()[0];
				string text = string.Format("Video/{0}.mp4", cgDataItem.cgPath);
				if (_videoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, text, false))
				{
					Singleton<ApplicationManager>.Instance.InvokeNextFrame(delegate
					{
						Singleton<MainUIManager>.Instance.LockUI(true, float.MaxValue);
					});
				}
				else
				{
					SuperDebug.VeryImportantError("open video failed! path=" + text);
					OnVideoFinished();
				}
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError(ex.ToString());
				OnVideoFinished();
			}
		}

		private void StartFirstLevel()
		{
			try
			{
				int cgID = Singleton<CGModule>.Instance.GetCgDataItemList()[0].cgID;
				Singleton<CGModule>.Instance.MarkCGIDFinish(cgID);
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError(ex.ToString());
			}
			_startFirstLevel = _pageContext.StartFirstLevel();
			if (!_startFirstLevel)
			{
				DisableVideo();
				_pageContext.DisableVideoFadeOut();
				_waitAvatarModelCoroutine = StartCoroutine(WaitCreateAvatarModel());
			}
		}

		private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, RenderHeads.Media.AVProVideo.ErrorCode ec)
		{
			switch (et)
			{
			case MediaPlayerEvent.EventType.ReadyToPlay:
				OnVideoReady();
				break;
			case MediaPlayerEvent.EventType.FirstFrameReady:
				OnVideoStarted();
				break;
			case MediaPlayerEvent.EventType.FinishedPlaying:
			case MediaPlayerEvent.EventType.Error:
				OnVideoFinished();
				break;
			case MediaPlayerEvent.EventType.Started:
			case MediaPlayerEvent.EventType.Closing:
				break;
			}
		}

		private void OnVideoReady()
		{
			try
			{
				AudioVolumeParamSetting(0f);
				Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_End");
				_videoPlayer.Play();
				Singleton<MainUIManager>.Instance.LockUI(true, float.MaxValue);
				Screen.sleepTimeout = -1;
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError(ex.ToString());
				OnVideoFinished();
			}
		}

		private void OnVideoStarted()
		{
			_videoDisplay.gameObject.SetActive(true);
			base.transform.Find("Video/BlackPanel").gameObject.SetActive(true);
		}

		private void OnVideoFinished()
		{
			try
			{
				AudioVolumeParamSetting(3f);
				DisableVideo();
				Screen.sleepTimeout = -2;
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError(ex.ToString());
			}
			finally
			{
				StartFirstLevel();
			}
		}

		private void DisableVideo()
		{
			_videoPlayer.Events.RemoveAllListeners();
			_videoPlayer.gameObject.SetActive(false);
			_videoDisplay.gameObject.SetActive(false);
			base.transform.Find("Video").gameObject.SetActive(false);
		}

		private void AudioVolumeParamSetting(float vol)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_BGM", vol);
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_SE", vol);
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_Voice", vol);
		}

		private void ProcessRealtimeWeatherUpdate()
		{
			if (Singleton<RealTimeWeatherManager>.Instance.IsWeatherInfoExpired())
			{
				Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo();
			}
		}
	}
}
