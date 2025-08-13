using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class MonoMainCanvas : BaseMonoCanvas
	{
		public PlayerStatusWidgetContext playerBar;

		public Avatar3dModelContext avatar3dModelContext;

		private SpaceShipModelContext _spaceshipModelContext;

		private GalTouchContext _galTouchContext;

		private GameObject _spaceShip;

		private GameObject _mainCamera;

		private MonoVideoPlayer _videoPlayer;

		public MonoVideoPlayer VideoPlayer
		{
			get
			{
				return _videoPlayer;
			}
		}

		public void Awake()
		{
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
		}

		public override void Start()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			_mainCamera = GameObject.Find("MainCamera");
			playerBar = new PlayerStatusWidgetContext();
			Singleton<MainUIManager>.Instance.ShowWidget(playerBar);
			AudioSettingData.ApplySettingConfig();
			_videoPlayer = GameObject.Find("VideoPlayer").GetComponent<MonoVideoPlayer>();
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			if (instance != null)
			{
				if (!instance.isTryLevel && !instance.isDebugLevel && instance.stageEndRsp != null && (int)instance.stageEndRsp.retcode == 0 && instance.isLevelSuccess)
				{
					LevelResultDialogContext levelResultDialogContext = new LevelResultDialogContext();
					levelResultDialogContext.onDestory = ShowPage;
					LevelResultDialogContext dialogContext = levelResultDialogContext;
					Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
				}
				else
				{
					Singleton<LevelScoreManager>.Destroy();
					ShowPage();
				}
			}
			else
			{
				ShowPage();
			}
			GraphicsSettingData.ApplySettingConfig();
			Camera main = Camera.main;
			if (main != null)
			{
				PostFX component = main.GetComponent<PostFX>();
				if (component != null)
				{
					component.WriteDepthTexture = false;
				}
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene));
			Resources.UnloadUnusedAssets();
			base.Start();
		}

		public override void PlayVideo(CgDataItem cgDataItem)
		{
			if (_videoPlayer != null)
			{
				_videoPlayer.LoadOrPlayVideo(cgDataItem);
			}
		}

		public override void Update()
		{
			if (Singleton<RealTimeWeatherManager>.Instance.Available)
			{
				ProcessRealtimeWeatherUpdate();
			}
			base.Update();
		}

		public override void OnDestroy()
		{
			if (_spaceshipModelContext != null && _spaceshipModelContext.view != null)
			{
				Object.Destroy(_spaceshipModelContext.view);
			}
			if (avatar3dModelContext != null && avatar3dModelContext.view != null)
			{
				Object.Destroy(avatar3dModelContext.view);
			}
			if (_galTouchContext != null)
			{
				_galTouchContext.Destroy();
			}
		}

		public override GameObject GetSpaceShipObj()
		{
			return _spaceShip;
		}

		public override void ClearAllWidgetContext()
		{
			if (playerBar != null)
			{
				playerBar.Destroy();
				playerBar = null;
			}
		}

		public void InitMainPageContexts()
		{
			if (avatar3dModelContext == null)
			{
				GameObject view = GameObject.Find("AvatarContainer");
				avatar3dModelContext = new Avatar3dModelContext(view);
				Singleton<MainUIManager>.Instance.ShowWidget(avatar3dModelContext, UIType.Root);
			}
			if (_spaceshipModelContext == null)
			{
				GameObject gameObject = GameObject.Find("MainMenu_SpaceShip");
				if (gameObject == null)
				{
					gameObject = Object.Instantiate((GameObject)Miscs.LoadResource("Stage/MainMenu_SpaceShip/MainMenu_SpaceShip"));
					gameObject.name = "MainMenu_SpaceShip";
				}
				_spaceShip = gameObject;
				_spaceshipModelContext = new SpaceShipModelContext(_spaceShip, _mainCamera);
				Singleton<MainUIManager>.Instance.ShowWidget(_spaceshipModelContext);
			}
			if (_galTouchContext == null)
			{
				_galTouchContext = new GalTouchContext();
				Singleton<MainUIManager>.Instance.ShowWidget(_galTouchContext);
			}
		}

		private IEnumerator ApplyGraphicsSetting()
		{
			_mainCamera.SetActive(true);
			GraphicsSettingData.ApplySettingConfig();
			yield return null;
			yield return null;
			if (_mainCamera.activeSelf && (_spaceshipModelContext == null || !_spaceshipModelContext.view.activeSelf))
			{
				_mainCamera.SetActive(false);
			}
		}

		private void ProcessRealtimeWeatherUpdate()
		{
			if (Singleton<RealTimeWeatherManager>.Instance.IsReadyToRetryQuery())
			{
				Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo();
			}
		}

		private void ShowPage()
		{
			if (Singleton<MainUIManager>.Instance.HasContextInStash())
			{
				Singleton<MainUIManager>.Instance.CreateContextFromStash();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowPage(new MainPageContext());
			}
		}
	}
}
