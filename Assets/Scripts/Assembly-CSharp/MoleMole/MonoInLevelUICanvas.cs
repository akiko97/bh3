using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoInLevelUICanvas : BaseMonoCanvas
	{
		public enum FadeState
		{
			Normal = 0,
			FadeIn = 1,
			FadeOut = 2
		}

		private const float HP_BAR_SHOWING_TIME = float.MaxValue;

		private const string FADE_IN_STATE_NAME = "StageTransitFadeIn";

		private const string FADE_OUT_STATE_NAME = "StageTransitFadeout";

		private const string MOVIE_BAR_PANEL_NAME = "StageMovieBarPanel";

		private const float MOVIE_BAR_FADE_DURATION = 0.15f;

		private const float TRANSIT_FADE_DURATION = 0.18f;

		public InLevelMainPageContext mainPageContext;

		public GameObject inLevelMainPage;

		private MonoInLevelLock _lockEffect;

		private EntityTimer _showHPTimer;

		private MonsterActor _activeTargetMonster;

		private AchieveUnlockInLevelContext _achieveUnlockInLevelContext;

		private GameObject _movieBarPanel;

		private CanvasGroup _movieBarCanvasGroup;

		private float _movieBarFadeTimer;

		private float _movieBarAlphaFrom;

		private float _movieBarAlphaTo;

		private static Color TRANSIT_BLACK = new Color(0f, 0f, 0f, 1f);

		private static Color TRANSIT_WHITE = new Color(0f, 0f, 0f, 0f);

		private GameObject _stageTransitPanel;

		private Image _stageTransitImage;

		private float _stageTransitFadeTimer;

		private float _stageTransitFadeTimeSpan;

		private Action _fadeEndCallback;

		private MonoVideoPlayer _videoPlayer;

		private FadeState _fadeState;

		private Color _stageTransitColorFrom;

		private Color _stageTransitColorTo;

		public HintArrowManager hintArrowManager { get; private set; }

		public MonoVideoPlayer VideoPlayer
		{
			get
			{
				return _videoPlayer;
			}
		}

		private void Awake()
		{
			hintArrowManager = new HintArrowManager();
			_showHPTimer = new EntityTimer(float.MaxValue);
			_showHPTimer.SetActive(false);
			base.transform.Find("TopPanels/StageTransitPanel").gameObject.SetActive(true);
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
			_movieBarPanel = base.transform.Find("TopPanels/StageMovieBarPanel").gameObject;
			_movieBarCanvasGroup = _movieBarPanel.GetComponent<CanvasGroup>();
			_stageTransitPanel = base.transform.Find("TopPanels/StageTransitPanel").gameObject;
			_stageTransitImage = _stageTransitPanel.GetComponent<Image>();
			_fadeState = FadeState.Normal;
			_stageTransitFadeTimeSpan = 0.18f;
			_fadeEndCallback = null;
		}

		public override void Start()
		{
			Canvas component = GetComponent<Canvas>();
			component.worldCamera = UnityEngine.Object.FindObjectOfType<MonoInLevelUICamera>().GetComponent<Camera>();
			inLevelMainPage.SetActive(true);
			mainPageContext = new InLevelMainPageContext(inLevelMainPage);
			Singleton<MainUIManager>.Instance.ShowPage(mainPageContext);
			mainPageContext.view.name = "InLevelMainPage";
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.Start));
			FadeOutStageTransitPanel(0.18f, true);
			_lockEffect = mainPageContext.view.transform.Find("InLevel_Lock_02").GetComponent<MonoInLevelLock>();
			_lockEffect.gameObject.SetActive(false);
			hintArrowManager.InitAtStart();
			_achieveUnlockInLevelContext = new AchieveUnlockInLevelContext();
			Singleton<MainUIManager>.Instance.ShowWidget(_achieveUnlockInLevelContext);
			_videoPlayer = GameObject.Find("VideoPlayer").GetComponent<MonoVideoPlayer>();
			base.Start();
		}

		public override void Update()
		{
			base.Update();
			hintArrowManager.Core();
			_lockEffect.Core();
			_showHPTimer.Core(1f);
			if (_showHPTimer.isTimeUp || _activeTargetMonster == null || _activeTargetMonster.monster == null || !_activeTargetMonster.monster.IsActive())
			{
				if (mainPageContext.view != null)
				{
					mainPageContext.HideMonsterStatus();
				}
				_showHPTimer.Reset(false);
			}
			if ((_fadeState == FadeState.FadeIn || _fadeState == FadeState.FadeOut) && _stageTransitFadeTimer >= 0f)
			{
				_stageTransitImage.color = Color.Lerp(_stageTransitColorFrom, _stageTransitColorTo, (_stageTransitFadeTimeSpan - _stageTransitFadeTimer) / _stageTransitFadeTimeSpan);
				_stageTransitFadeTimer -= Time.deltaTime;
				if (_stageTransitFadeTimer <= 0f)
				{
					_stageTransitImage.color = _stageTransitColorTo;
					_fadeState = FadeState.Normal;
					_stageTransitFadeTimer = 0f;
					if (_stageTransitColorTo == TRANSIT_WHITE)
					{
						_stageTransitPanel.gameObject.SetActive(false);
					}
					if (_fadeEndCallback != null)
					{
						_fadeEndCallback();
					}
				}
			}
			if (!(_movieBarFadeTimer > 0f))
			{
				return;
			}
			_movieBarCanvasGroup.alpha = Mathf.Lerp(_movieBarAlphaFrom, _movieBarAlphaTo, (0.15f - _movieBarFadeTimer) / 0.15f);
			_movieBarFadeTimer -= Time.deltaTime;
			if (_movieBarFadeTimer <= 0f)
			{
				_movieBarCanvasGroup.alpha = _movieBarAlphaTo;
				if (_movieBarAlphaTo == 0f)
				{
					_movieBarCanvasGroup.gameObject.SetActive(false);
				}
			}
		}

		public override void OnDestroy()
		{
			Singleton<NotifyManager>.Instance.RemoveContext(_achieveUnlockInLevelContext);
			_achieveUnlockInLevelContext.Destroy();
			_achieveUnlockInLevelContext = null;
		}

		public void OnUpdateLocalAvatar(uint runtimeID, uint oldRuntimeId = 0)
		{
			if (oldRuntimeId != 0)
			{
				BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(oldRuntimeId);
				avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Remove(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			}
			BaseMonoAvatar avatarByRuntimeID2 = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			avatarByRuntimeID2.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Combine(avatarByRuntimeID2.onAttackTargetChanged, new Action<BaseMonoEntity>(OnUpdateAttackTarget));
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			AvatarActor avatarBefore = ((oldRuntimeId != 0) ? ((AvatarActor)Singleton<EventManager>.Instance.GetActor(oldRuntimeId)) : null);
			mainPageContext.OnLocalAvatarChanged(avatarBefore, actor);
		}

		public void OnUpdateLocalAvatarAbilityDisplay(uint runtimeID, uint oldRuntimeID = 0)
		{
			bool flag = false;
			if (oldRuntimeID != 0)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(oldRuntimeID);
				if (actor.abilityPlugin.HasDisplayFloat("IsOverheat"))
				{
					flag = true;
					actor.abilityPlugin.SubDetachDisplayFloat("IsOverheat", mainPageContext.OnIsOverheatChanged);
					actor.abilityPlugin.SubDetachDisplayFloat("OverheatRatio", mainPageContext.OnOverheatRatioChanged);
				}
			}
			AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			if (actor2.abilityPlugin.HasDisplayFloat("IsOverheat"))
			{
				if (!flag)
				{
					mainPageContext.SetOverHeatViewActive(true);
				}
				float curValue = 0f;
				float curValue2 = 0f;
				float ceiling = 0f;
				float floor = 0f;
				actor2.abilityPlugin.SubAttachDisplayFloat("IsOverheat", mainPageContext.OnIsOverheatChanged, ref curValue, ref floor, ref ceiling);
				actor2.abilityPlugin.SubAttachDisplayFloat("OverheatRatio", mainPageContext.OnOverheatRatioChanged, ref curValue2, ref floor, ref ceiling);
				mainPageContext.UpdateOverHeatView(curValue > 0f, curValue2);
			}
			else if (flag)
			{
				mainPageContext.SetOverHeatViewActive(false);
			}
		}

		private void OnUpdateAttackTarget(BaseMonoEntity entity)
		{
			UpdateLockEntity(entity);
			MonsterActor activeTargetMonster = _activeTargetMonster;
			if (entity != null)
			{
				BaseMonoEntity baseMonoEntity = null;
				if (entity is BaseMonoMonster)
				{
					baseMonoEntity = entity;
				}
				else if (entity is MonoBodyPartEntity && ((MonoBodyPartEntity)entity).owner is BaseMonoMonster)
				{
					baseMonoEntity = ((MonoBodyPartEntity)entity).owner;
				}
				if (baseMonoEntity != null)
				{
					MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoEntity.GetRuntimeID());
					_activeTargetMonster = actor;
					_showHPTimer.Reset(false);
				}
			}
			MonsterActor activeTargetMonster2 = _activeTargetMonster;
			if (activeTargetMonster != activeTargetMonster2)
			{
				mainPageContext.OnTargetMonsterChange(activeTargetMonster, activeTargetMonster2);
			}
		}

		private void UpdateLockEntity(BaseMonoEntity entity)
		{
			_lockEffect.SetLockFollowTarget(entity);
		}

		public void FadeOutStageTransitPanel(float fadeDuration = 0.18f, bool instant = false, Action fadeStartCallback = null, Action fadeEndCallback = null)
		{
			if (_stageTransitImage.color.a == 1f)
			{
				_fadeState = FadeState.FadeOut;
				if (fadeStartCallback != null)
				{
					fadeStartCallback();
				}
				if (fadeEndCallback != null)
				{
					fadeEndCallback();
				}
				return;
			}
			_stageTransitPanel.SetActive(true);
			_stageTransitPanel.transform.SetAsLastSibling();
			if (fadeStartCallback != null)
			{
				fadeStartCallback();
			}
			if (fadeEndCallback != null)
			{
				_fadeEndCallback = fadeEndCallback;
			}
			if (instant)
			{
				_stageTransitFadeTimer = 0f;
				_stageTransitImage.color = TRANSIT_BLACK;
				if (_fadeEndCallback != null)
				{
					_fadeEndCallback();
				}
			}
			else
			{
				_stageTransitColorFrom = TRANSIT_WHITE;
				_stageTransitColorTo = TRANSIT_BLACK;
				_stageTransitFadeTimeSpan = fadeDuration;
				_stageTransitFadeTimer = _stageTransitFadeTimeSpan;
				_stageTransitImage.color = TRANSIT_WHITE;
				_fadeState = FadeState.FadeOut;
			}
		}

		public void FadeInStageTransitPanel(float fadeDuration = 0.18f, bool instant = false, Action fadeStartCallback = null, Action fadeEndCallback = null)
		{
			if (_stageTransitImage.color.a == 0f)
			{
				if (fadeStartCallback != null)
				{
					fadeStartCallback();
				}
				if (fadeEndCallback != null)
				{
					fadeEndCallback();
				}
			}
			_stageTransitPanel.SetActive(true);
			if (!_stageTransitPanel.activeSelf)
			{
				return;
			}
			if (fadeStartCallback != null)
			{
				fadeStartCallback();
			}
			if (fadeEndCallback != null)
			{
				_fadeEndCallback = fadeEndCallback;
			}
			if (instant)
			{
				_stageTransitFadeTimer = 0f;
				_stageTransitImage.color = TRANSIT_WHITE;
				_stageTransitPanel.SetActive(false);
				if (_fadeEndCallback != null)
				{
					_fadeEndCallback();
				}
			}
			else
			{
				_stageTransitColorFrom = TRANSIT_BLACK;
				_stageTransitColorTo = TRANSIT_WHITE;
				_stageTransitFadeTimeSpan = fadeDuration;
				_stageTransitFadeTimer = _stageTransitFadeTimeSpan;
				_stageTransitImage.color = TRANSIT_BLACK;
				_fadeState = FadeState.FadeIn;
			}
		}

		public bool IsStageTransitPanelFading()
		{
			return _stageTransitFadeTimer > 0f && (_fadeState == FadeState.FadeIn || _fadeState == FadeState.FadeOut);
		}

		public void ShowMovieBar(bool instant)
		{
			if (_movieBarPanel.activeSelf && _movieBarCanvasGroup.alpha == 1f)
			{
				return;
			}
			_movieBarPanel.SetActive(true);
			_movieBarPanel.transform.SetAsLastSibling();
			if (instant)
			{
				_movieBarFadeTimer = 0f;
				_movieBarCanvasGroup.alpha = 1f;
				return;
			}
			_movieBarAlphaFrom = 0f;
			_movieBarAlphaTo = 1f;
			if (_movieBarFadeTimer <= 0f)
			{
				_movieBarFadeTimer = 0.15f;
				_movieBarCanvasGroup.alpha = 0f;
			}
		}

		public void HideMovieBar(bool instant)
		{
			if (!_movieBarPanel.activeSelf)
			{
				return;
			}
			if (instant)
			{
				_movieBarFadeTimer = 0f;
				_movieBarPanel.SetActive(false);
				return;
			}
			_movieBarAlphaFrom = 1f;
			_movieBarAlphaTo = 0f;
			if (_movieBarFadeTimer <= 0f)
			{
				_movieBarFadeTimer = 0.15f;
				_movieBarCanvasGroup.alpha = 1f;
			}
		}

		public void SetWhiteTransitPanelActive(bool enable)
		{
			base.transform.Find("TopPanels/WhiteTransitPanel").gameObject.SetActive(enable);
		}

		public void SetInLevelUIActive(bool active)
		{
			BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
			if (currentPageContext != null)
			{
				if (currentPageContext is InLevelMainPageContext)
				{
					InLevelMainPageContext inLevelMainPageContext = currentPageContext as InLevelMainPageContext;
					inLevelMainPageContext.SetInLevelMainPageActive(active);
				}
				else
				{
					Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(active);
				}
			}
		}

		public void StartPlayVideo(CgDataItem cgDataItem)
		{
			Action fadeEndCallback = delegate
			{
				PlayVideo(cgDataItem);
			};
			FadeOutStageTransitPanel(0.18f, false, null, fadeEndCallback);
		}

		public override void PlayVideo(CgDataItem cgDataItem)
		{
			if (_videoPlayer != null)
			{
				bool withSkipBtn = false;
				if (cgDataItem != null)
				{
					withSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(cgDataItem.cgID);
				}
				MonoVideoPlayer videoPlayer = _videoPlayer;
				Action<CgDataItem> onVideoBeginCallback = OnInLevelVideoBeginCallback;
				videoPlayer.LoadOrPlayVideo(cgDataItem, null, onVideoBeginCallback, OnInLevelVideoEndCallback, MonoVideoPlayer.VideoControlType.Play, withSkipBtn);
			}
		}

		public void LoadVideo(CgDataItem cgDataItem)
		{
			if (_videoPlayer != null)
			{
				bool withSkipBtn = false;
				if (cgDataItem != null)
				{
					withSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(cgDataItem.cgID);
				}
				MonoVideoPlayer videoPlayer = _videoPlayer;
				Action<CgDataItem> onVideoBeginCallback = OnInLevelVideoBeginCallback;
				videoPlayer.LoadOrPlayVideo(cgDataItem, null, onVideoBeginCallback, OnInLevelVideoEndCallback, MonoVideoPlayer.VideoControlType.Load, withSkipBtn);
			}
		}

		public void OnInLevelVideoBeginCallback(CgDataItem cgDataItem)
		{
			Singleton<LevelManager>.Instance.SetPause(true);
		}

		public void OnInLevelVideoEndCallback(CgDataItem cgDataItem)
		{
			Singleton<LevelManager>.Instance.SetPause(false);
			Action fadeEndCallback = delegate
			{
				Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
				Singleton<EventManager>.Instance.FireEvent(new EvtVideoState((uint)cgDataItem.cgID, EvtVideoState.State.Finish));
			};
			FadeInStageTransitPanel(0.18f, false, null, fadeEndCallback);
		}
	}
}
