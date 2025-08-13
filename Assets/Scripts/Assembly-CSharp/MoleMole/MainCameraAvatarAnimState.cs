using UnityEngine;

namespace MoleMole
{
	public class MainCameraAvatarAnimState : BaseMainCameraState
	{
		private Animation _animation;

		private BaseMonoAvatar _avatar;

		private BaseMainCameraState _nextState;

		private bool _exitTransitionLerp;

		private bool _isFirstFrame;

		private bool _isLastFrameTimeScaleZero;

		private float _sampleTimer;

		private bool _isInterrupted;

		private MonoSimpleAnimation _simpleAnimationComponent;

		public MainCameraAvatarAnimState(MonoMainCamera camera)
			: base(camera)
		{
			_nextState = _owner.followState;
			_exitTransitionLerp = false;
		}

		public override void Enter()
		{
			if (_isInterrupted)
			{
				_animation.enabled = true;
				_isInterrupted = false;
				return;
			}
			_isFirstFrame = true;
			_sampleTimer = 0f;
			if (_simpleAnimationComponent != null)
			{
				if (_simpleAnimationComponent.initClipZNear > 0f)
				{
					_owner.cameraComponent.nearClipPlane = Mathf.Max(0.01f, _simpleAnimationComponent.initClipZNear);
				}
				if (_simpleAnimationComponent.initFOV > 0f)
				{
					_owner.cameraComponent.fieldOfView = _simpleAnimationComponent.initFOV;
				}
				_simpleAnimationComponent.selfUpdateKeyedRotation = false;
				if (_simpleAnimationComponent.useKeyedDirectionalLightRotation)
				{
					_owner.SetNeedLerpDirectionalLight(false);
				}
			}
		}

		private void DoExit()
		{
			if (_exitTransitionLerp)
			{
				_owner.TransitWithLerp(_nextState, 1f);
			}
			else
			{
				_owner.Transit(_nextState);
			}
		}

		public override void Update()
		{
			if (_simpleAnimationComponent != null)
			{
				_simpleAnimationComponent.SyncRotation();
			}
			if (_isFirstFrame)
			{
				SyncByAnimation();
				_animation[_animation.clip.name].time = 0f;
				_isFirstFrame = false;
			}
			else if (_animation != null && _animation.isPlaying)
			{
				if (_avatar != null)
				{
					if (_simpleAnimationComponent != null && _simpleAnimationComponent.hasPushedLevelTimeScale)
					{
						SyncByAnimation();
						if (Singleton<LevelManager>.Instance.IsPaused())
						{
							_animation[_animation.clip.name].speed = 0f;
							return;
						}
						_animation[_animation.clip.name].speed = 1f;
						_sampleTimer += Time.deltaTime;
						return;
					}
					_sampleTimer += Time.deltaTime * _avatar.TimeScale;
					if (_isLastFrameTimeScaleZero && _avatar.TimeScale != 0f)
					{
						_sampleTimer += 1f / 60f;
					}
					_animation[_animation.clip.name].time = _sampleTimer;
					_animation.Sample();
					SyncByAnimation();
					_isLastFrameTimeScaleZero = _avatar.TimeScale == 0f;
				}
				else
				{
					DoExit();
				}
			}
			else
			{
				DoExit();
			}
		}

		public override void Exit()
		{
			if (_isInterrupted)
			{
				_animation.enabled = false;
				return;
			}
			_owner.cameraComponent.nearClipPlane = _owner.originalNearClip;
			_owner.cameraComponent.fieldOfView = _owner.originalFOV;
			if (_animation != null)
			{
				_animation.Stop();
				Object.Destroy(_animation.gameObject);
			}
			_owner.SetNeedLerpDirectionalLight(true);
		}

		public void SetupFollowAvatarAnim(Animation animation, BaseMonoAvatar avatar)
		{
			_animation = animation;
			_avatar = avatar;
			_simpleAnimationComponent = animation.GetComponent<MonoSimpleAnimation>();
		}

		public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
		{
			_nextState = nextState;
			_exitTransitionLerp = exitTransitionLerp;
		}

		public void SetInterrupt()
		{
			_isInterrupted = true;
		}

		private void SyncByAnimation()
		{
			cameraPosition = _animation.transform.position;
			if (_simpleAnimationComponent.useAnimRotation)
			{
				if (_simpleAnimationComponent.realAnimGameObject != null)
				{
					cameraForward = _simpleAnimationComponent.realAnimGameObject.transform.forward;
				}
				else
				{
					cameraForward = _animation.transform.forward;
				}
			}
			else
			{
				cameraForward = _animation.transform.forward;
			}
			if (_simpleAnimationComponent != null)
			{
				if (_simpleAnimationComponent.useKeyedFOV)
				{
					cameraFOV = _simpleAnimationComponent.keyedFOV;
				}
				else
				{
					cameraFOV = _owner.originalFOV;
				}
				if (_simpleAnimationComponent.useKeyedDirectionalLightRotation)
				{
					_owner.directionalLight.transform.rotation = _simpleAnimationComponent.GetLightRotation();
				}
			}
		}
	}
}
