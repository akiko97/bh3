using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class FollowSlowMotionKill : BaseFollowShortState
	{
		private enum State
		{
			Entering = 0,
			During = 1,
			Exiting = 2
		}

		private class ParamsBase
		{
		}

		private class LookAtPositionParams : ParamsBase
		{
			public Vector3 position;

			public bool mute;
		}

		private SlowMotionEffect[] _effects;

		private static readonly int MAX_EFFECT_NUM = 2;

		private float _lastRadiusOffset;

		private float _lastElevationOffset;

		protected float _origElevation;

		protected float _origRadius;

		private bool _hasUserControled;

		private State _state;

		private ParamsBase _followingShortState;

		public FollowSlowMotionKill(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = false;
			_effects = new SlowMotionEffect[MAX_EFFECT_NUM];
			for (int i = 0; i < _effects.Length; i++)
			{
				_effects[i] = new SlowMotionEffect();
			}
		}

		public void SetSlowMotionKill(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
		{
			foreach (SlowMotionEffect item in ActiveEffectEnum())
			{
				if (!item.OverDuration(0.5f))
				{
					return;
				}
			}
			SlowMotionEffect[] effects = _effects;
			foreach (SlowMotionEffect slowMotionEffect2 in effects)
			{
				if (!slowMotionEffect2.active)
				{
					slowMotionEffect2.Set(config, distTarget, distCamera);
					slowMotionEffect2.active = true;
					if (base.active)
					{
						slowMotionEffect2.Enter(_owner);
					}
					break;
				}
			}
		}

		private IEnumerable ActiveEffectEnum()
		{
			SlowMotionEffect[] effects = _effects;
			foreach (SlowMotionEffect effect in effects)
			{
				if (effect.active)
				{
					yield return effect;
				}
			}
		}

		private bool HasActiveEffect()
		{
			SlowMotionEffect[] effects = _effects;
			foreach (SlowMotionEffect slowMotionEffect in effects)
			{
				if (slowMotionEffect.active)
				{
					return true;
				}
			}
			return false;
		}

		private void ClearEffects()
		{
			SlowMotionEffect[] effects = _effects;
			foreach (SlowMotionEffect slowMotionEffect in effects)
			{
				slowMotionEffect.active = false;
			}
		}

		public void SetFollowingLookAtPosition(Vector3 position, bool mute)
		{
			LookAtPositionParams lookAtPositionParams = new LookAtPositionParams();
			lookAtPositionParams.position = position;
			lookAtPositionParams.mute = mute;
			_followingShortState = lookAtPositionParams;
		}

		private void SetFollowingShortState()
		{
			if (_followingShortState != null && _followingShortState.GetType() == typeof(LookAtPositionParams))
			{
				LookAtPositionParams lookAtPositionParams = _followingShortState as LookAtPositionParams;
				_owner.mainCamera.FollowLookAtPosition(lookAtPositionParams.position, lookAtPositionParams.mute, true);
			}
		}

		public override void Enter()
		{
			if (_owner.followAvatarControlledRotate.active && _owner.followAvatarControlledRotate.IsExiting())
			{
				_owner.TransitBaseState(_owner.followAvatarState);
			}
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelAndStopAtCurrentState();
			}
			_origElevation = _owner.recoverState.GetOriginalElevation();
			_origRadius = _owner.recoverState.GetOriginalRadius();
			_lastRadiusOffset = 0f;
			_lastElevationOffset = 0f;
			_state = State.Entering;
			_hasUserControled = false;
			foreach (SlowMotionEffect item in ActiveEffectEnum())
			{
				item.Enter(_owner);
			}
			Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_Start");
		}

		private SlowMotionEffect.OutParams UpdateAndBlendEffectes()
		{
			if (!HasActiveEffect())
			{
				return null;
			}
			int num = 0;
			SlowMotionEffect.OutParams outParams = new SlowMotionEffect.OutParams();
			outParams.timeScale = 1f;
			outParams.anchorDeltaPolar = 0f;
			outParams.anchorElevationOffset = 0f;
			outParams.anchorRadiusOffset = 0f;
			outParams.forwardDeltaAngle = 0f;
			foreach (SlowMotionEffect item in ActiveEffectEnum())
			{
				num++;
				item.Update();
				SlowMotionEffect.OutParams outParams2 = item.outParams;
				outParams.timeScale *= outParams2.timeScale;
				outParams.anchorDeltaPolar += outParams2.anchorDeltaPolar;
				outParams.anchorElevationOffset += outParams2.anchorElevationOffset;
				outParams.anchorRadiusOffset += outParams2.anchorRadiusOffset;
				outParams.forwardDeltaAngle += outParams2.forwardDeltaAngle;
			}
			outParams.anchorDeltaPolar /= num;
			outParams.anchorElevationOffset /= num;
			outParams.anchorRadiusOffset /= num;
			outParams.forwardDeltaAngle /= num;
			return outParams;
		}

		public override void Update()
		{
			base.isSkippingBaseState = !_owner.followAvatarControlledRotate.active;
			if (_owner.followAvatarState.active)
			{
				_owner.followAvatarState.CancelEnteringLerp();
			}
		}

		public override void PostUpdate()
		{
			if (_state == State.Entering)
			{
				_state = State.During;
			}
			else if (_state == State.During)
			{
				SlowMotionEffect.OutParams outParams = UpdateAndBlendEffectes();
				if (outParams != null)
				{
					Time.timeScale = outParams.timeScale;
					Time.fixedDeltaTime = 0.02f * Time.timeScale;
					if (!_owner.followAvatarControlledRotate.active && !_hasUserControled)
					{
						_owner.anchorPolar += outParams.anchorDeltaPolar;
						_owner.anchorRadius += outParams.anchorRadiusOffset - _lastRadiusOffset;
						_lastRadiusOffset = outParams.anchorRadiusOffset;
						_owner.anchorElevation += outParams.anchorElevationOffset - _lastElevationOffset;
						float b = (0f - Mathf.Asin(Mathf.Max(_owner.followCenterY - 0.3f, 0f) / Mathf.Max(_owner.anchorRadius * _owner.cameraLocateRatio, 0.1f))) * 57.29578f;
						_owner.anchorElevation = Mathf.Max(_owner.anchorElevation, b);
						_lastElevationOffset = outParams.anchorElevationOffset;
						_owner.forwardDeltaAngle = outParams.forwardDeltaAngle;
					}
					else
					{
						_hasUserControled = true;
					}
				}
				if (!HasActiveEffect())
				{
					_state = State.Exiting;
				}
				_owner.needLerpPositionThisFrame = false;
				_owner.needLerpForwardThisFrame = false;
			}
			else if (_state == State.Exiting)
			{
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
				if (_followingShortState != null)
				{
					SetFollowingShortState();
				}
				else
				{
					End();
				}
			}
		}

		public override void Exit()
		{
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_End");
			if (!_owner.followAvatarControlledRotate.active)
			{
				_owner.recoverState.TryRecover();
			}
		}
	}
}
