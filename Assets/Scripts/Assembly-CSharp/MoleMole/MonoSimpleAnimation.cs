using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Animation))]
	[ExecuteInEditMode]
	public class MonoSimpleAnimation : MonoAuxObject
	{
		public enum TimeScaleMode
		{
			DoNothing = 0,
			IgnoreTimeScale = 1,
			UseLevelTimeScale = 2
		}

		[Serializable]
		public class CircleTrack
		{
			public float radius;

			public float angularSpeed;

			public float startAngle;

			public float elevation;

			public bool isAntiClockwise;

			public GameObject centerPos;
		}

		[Header("Set this to true and key")]
		public bool useKeyedRotation;

		[Header("Use animation to key this instead of transform.rotation for working constant tangent")]
		public Vector3 keyedRotation;

		public bool useAnimRotation;

		[Header("Use keyed Radial Blur")]
		public bool useKeyedRadialBlur;

		public Vector2 radialBlurCenter;

		public float radialBlurStrenth;

		public float radialBlurScatterScale;

		[Header("Use this Go postion and rotation")]
		public GameObject realAnimGameObject;

		[Header("Duplicate a fixed parent transform on start")]
		public bool useFixedParentAnchor;

		[Header("Set this to true and key")]
		public bool useKeyedFOV;

		[Header("Use animation to key this instead of maincamera fov")]
		public float keyedFOV;

		[Header("Use key Directional Light Forward")]
		public bool useKeyedDirectionalLightRotation;

		[Header("Use animation to key this to control directional light forward")]
		public Vector3 keyedDirectionalLightRotation;

		[HideInInspector]
		public bool hasPushedLevelTimeScale;

		[Header("Init FOV, only positive value works")]
		public float initFOV = -1f;

		[Header("Init Near Z Plane, only positive value works")]
		public float initClipZNear = -1f;

		[Header("Ignore Time Scale")]
		public TimeScaleMode timeScaleMode;

		[Header("Animated UI Graphics")]
		public Graphic[] graphics;

		[NonSerialized]
		public bool selfUpdateKeyedRotation = true;

		[Header("use circle track")]
		public bool UseSpecificCircleTrack;

		[Header("circle track parameter")]
		public CircleTrack circleTrack;

		private bool _hasSetCircleStartTime;

		private float _circleStartTime;

		private Vector3 _circleLookAtPos;

		private Animation _animation;

		private float _sampleTimer;

		private AnimationState _animationState;

		private float _lastTimeScale;

		private int _levelEntityTimescalIx = -1;

		public Transform ownedParent { get; private set; }

		private void Awake()
		{
			_animation = GetComponent<Animation>();
			if (timeScaleMode > TimeScaleMode.DoNothing)
			{
				_lastTimeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				_sampleTimer = 0f;
				_animationState = _animation[_animation.clip.name];
			}
		}

		private void Start()
		{
			if (useFixedParentAnchor)
			{
				if (!Application.isPlaying || ownedParent != null || base.transform.parent == null)
				{
					return;
				}
				GameObject gameObject = new GameObject();
				gameObject.name = "FixedCameraAnchor";
				ownedParent = gameObject.transform;
				ownedParent.position = base.transform.parent.transform.position;
				Vector3 forward = base.transform.parent.transform.forward;
				forward.y = 0f;
				ownedParent.forward = forward;
				base.transform.parent = ownedParent;
				base.transform.localPosition = Vector3.zero;
			}
			if (UseSpecificCircleTrack && circleTrack != null)
			{
				base.transform.localRotation = circleTrack.centerPos.transform.localRotation;
				_circleLookAtPos = circleTrack.centerPos.transform.position;
			}
		}

		public void SyncRotation()
		{
			if (useKeyedRotation)
			{
				base.transform.localRotation = Quaternion.Euler(keyedRotation);
			}
		}

		public void SyncRadialBlur()
		{
			if (useKeyedRadialBlur)
			{
				radialBlurCenter.x = Mathf.Clamp(radialBlurCenter.x, 0f, 1f);
				radialBlurCenter.y = Mathf.Clamp(radialBlurCenter.y, 0f, 1f);
				radialBlurStrenth = Mathf.Clamp(radialBlurStrenth, 0f, 10f);
				radialBlurScatterScale = Mathf.Clamp(radialBlurScatterScale, 0f, 2f);
			}
		}

		public void SetOwnedParent(Transform ownedParent)
		{
			this.ownedParent = ownedParent;
			base.transform.parent = this.ownedParent;
			base.transform.localPosition = Vector3.zero;
		}

		private void OnDestroy()
		{
			if (Application.isPlaying && ownedParent != null)
			{
				UnityEngine.Object.Destroy(ownedParent.gameObject);
			}
		}

		[AnimationCallback]
		private void TimeSlow(float duration)
		{
			Singleton<LevelManager>.Instance.levelActor.TimeSlow(duration);
		}

		[AnimationCallback]
		private void PushLevelTimeScale(float timescale)
		{
			hasPushedLevelTimeScale = true;
			_levelEntityTimescalIx = Singleton<LevelManager>.Instance.levelEntity.timeScaleStack.Push(timescale);
		}

		[AnimationCallback]
		private void PopLevelTimeScale()
		{
			hasPushedLevelTimeScale = false;
			Singleton<LevelManager>.Instance.levelEntity.timeScaleStack.Pop(_levelEntityTimescalIx);
		}

		private void Update()
		{
			if (selfUpdateKeyedRotation)
			{
				SyncRotation();
			}
			if (useKeyedRadialBlur)
			{
				SyncRadialBlur();
			}
			if (UseSpecificCircleTrack && circleTrack != null && Singleton<LevelManager>.Instance != null && Singleton<LevelManager>.Instance.levelEntity != null)
			{
				if (!_hasSetCircleStartTime)
				{
					_circleStartTime = Time.time;
					_hasSetCircleStartTime = true;
				}
				float num = circleTrack.startAngle + circleTrack.angularSpeed * (Time.time - _circleStartTime) * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				if (circleTrack.isAntiClockwise)
				{
					num *= -1f;
				}
				Vector3 vector = default(Vector3);
				vector.z = circleTrack.radius * Mathf.Cos(num * ((float)Math.PI / 180f)) * Mathf.Cos(circleTrack.elevation * ((float)Math.PI / 180f));
				vector.x = circleTrack.radius * Mathf.Sin(num * ((float)Math.PI / 180f)) * Mathf.Cos(circleTrack.elevation * ((float)Math.PI / 180f));
				vector.y = circleTrack.radius * Mathf.Sin(circleTrack.elevation * ((float)Math.PI / 180f));
				base.transform.localPosition = circleTrack.centerPos.transform.localPosition + vector;
				base.transform.LookAt(_circleLookAtPos);
			}
			if (timeScaleMode == TimeScaleMode.IgnoreTimeScale)
			{
				_sampleTimer += Time.unscaledDeltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				_animationState.time = _sampleTimer;
				_animation.Sample();
			}
			else if (timeScaleMode == TimeScaleMode.UseLevelTimeScale)
			{
				float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				if (_lastTimeScale != timeScale)
				{
					_animationState.speed = timeScale;
				}
				_lastTimeScale = timeScale;
			}
			if (graphics.Length > 0 && _animation.isPlaying)
			{
				for (int i = 0; i < graphics.Length; i++)
				{
					graphics[i].SetAllDirty();
				}
			}
		}

		[AnimationCallback]
		private void ShowCloseUpPanel(string monsterName)
		{
			Singleton<MainUIManager>.Instance.ShowPage(new MonsterCloseUpPageContext(monsterName));
		}

		[AnimationCallback]
		private void HideCloseUpPanel()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MonsterCloseUpEnd));
		}

		[AnimationCallback]
		private void HideGameObject(string childPath)
		{
			if (string.IsNullOrEmpty(childPath))
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.transform.Find(childPath).gameObject.SetActive(false);
			}
		}

		[AnimationCallback]
		public void SetInLevelUIDeactive()
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			if (inLevelUICanvas != null)
			{
				inLevelUICanvas.SetInLevelUIActive(false);
			}
		}

		public void SetInLevelUIActive()
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			if (inLevelUICanvas != null)
			{
				inLevelUICanvas.SetInLevelUIActive(true);
			}
		}

		[AnimationCallback]
		public void SetInLevelUIActiveInstant()
		{
			BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
			if (currentPageContext != null)
			{
				if (currentPageContext is InLevelMainPageContext)
				{
					InLevelMainPageContext inLevelMainPageContext = currentPageContext as InLevelMainPageContext;
					inLevelMainPageContext.SetInLevelMainPageActive(true, true);
				}
				else
				{
					Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(true);
				}
			}
		}

		[AnimationCallback]
		private void SetInLevelUIDeactiveInstant()
		{
			BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
			if (currentPageContext != null)
			{
				if (currentPageContext is InLevelMainPageContext)
				{
					InLevelMainPageContext inLevelMainPageContext = currentPageContext as InLevelMainPageContext;
					inLevelMainPageContext.SetInLevelMainPageActive(false, true);
				}
				else
				{
					Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(false);
				}
			}
		}

		[AnimationCallback]
		private void ActCameraShake(float shakeTime)
		{
			Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(shakeTime, 0.36f, 90f, 2, false, false);
		}

		[AnimationCallback]
		private void EndLevel(string winMsg)
		{
		}

		[AnimationCallback]
		private void FadeIn(float fadeDuration)
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			Action fadeEndCallback = delegate
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition));
			};
			inLevelUICanvas.FadeInStageTransitPanel(fadeDuration, false, null, fadeEndCallback);
		}

		[AnimationCallback]
		private void FadeOut(float fadeDuration)
		{
			MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
			Action fadeStartCallback = delegate
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition));
			};
			inLevelUICanvas.FadeOutStageTransitPanel(fadeDuration, false, fadeStartCallback);
		}

		[AnimationCallback]
		private void SetFloorReflectionHeight(float reflectionHeight)
		{
			Singleton<StageManager>.Instance.GetPerpStage().GetComponent<FloorReflection>().floorHeight = reflectionHeight;
		}

		[AnimationCallback]
		private void DebugBreak()
		{
			Debug.Break();
		}

		[AnimationCallback]
		private void MainCanvasFadeIn()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel();
		}

		[AnimationCallback]
		private void MainCanvasFadeOut()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel();
		}

		public Quaternion GetLightRotation()
		{
			if (ownedParent == null)
			{
				return Quaternion.Euler(keyedDirectionalLightRotation);
			}
			return ownedParent.transform.rotation * Quaternion.Euler(keyedDirectionalLightRotation);
		}
	}
}
