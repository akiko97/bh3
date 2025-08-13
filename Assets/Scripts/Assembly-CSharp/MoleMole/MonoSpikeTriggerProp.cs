using UnityEngine;

namespace MoleMole
{
	public class MonoSpikeTriggerProp : MonoTriggerUnitFieldProp
	{
		private const float TakeEffecctDelay = 0.7f;

		private const float LoseEffecctDelay = 0.2f;

		private string _enableAnimationName = "Enable";

		private string _disableAnimationName = "Disable";

		private float timer;

		private float effectDelayTimer;

		private bool _isAttacking;

		private bool _isInContineousState;

		private float _CD = 2f;

		private float _duration = 2f;

		private Animation[] _animations;

		public override void InitUnitFieldPropRange(int numberX, int numberZ)
		{
			base.InitUnitFieldPropRange(numberX, numberZ);
			_animations = GetComponentsInChildren<Animation>();
			_CD = ((!(config.PropArguments.CD > 0f)) ? _CD : config.PropArguments.CD);
			timer = _CD;
			_duration = ((!(config.PropArguments.EffectDuration > 0f)) ? _duration : config.PropArguments.EffectDuration);
		}

		protected override void Update()
		{
			base.Update();
			if (_isInContineousState)
			{
				return;
			}
			timer -= Time.deltaTime * TimeScale;
			if (timer < 0f)
			{
				if (_isAttacking)
				{
					timer = _duration;
					DisableSprike();
					effectDelayTimer = 0.2f;
				}
				else
				{
					timer = _CD;
					EnableSprike();
					effectDelayTimer = 0.7f;
				}
				_isAttacking = !_isAttacking;
			}
			if (!(effectDelayTimer > 0f))
			{
				return;
			}
			effectDelayTimer -= Time.deltaTime * TimeScale;
			if (effectDelayTimer < 0f)
			{
				if (_isAttacking)
				{
					EnableCollider();
				}
				else
				{
					DisableCollider();
				}
			}
		}

		private void EnableSprike()
		{
			if (!_triggerCollider.enabled)
			{
				Animation[] animations = _animations;
				foreach (Animation animation in animations)
				{
					animation.Play(_enableAnimationName);
				}
			}
		}

		private void DisableSprike()
		{
			if (_triggerCollider.enabled)
			{
				Animation[] animations = _animations;
				foreach (Animation animation in animations)
				{
					animation.Play(_disableAnimationName);
				}
			}
		}

		protected override void OnTimeScaleChanged(float newTimeScale)
		{
			Animation[] animations = _animations;
			foreach (Animation animation in animations)
			{
				foreach (AnimationState item in animation)
				{
					item.speed = newTimeScale;
				}
			}
		}

		public void SetContinuousState(bool Active)
		{
			_isInContineousState = true;
			if (Active)
			{
				Animation[] animations = _animations;
				foreach (Animation animation in animations)
				{
					animation.Play(_enableAnimationName);
				}
				ClearInsideColliders();
				_triggerCollider.enabled = true;
			}
			else
			{
				Animation[] animations2 = _animations;
				foreach (Animation animation2 in animations2)
				{
					animation2.Play(_disableAnimationName);
				}
				_triggerCollider.enabled = false;
			}
		}

		private void EnableCollider()
		{
			ClearInsideColliders();
			_triggerCollider.enabled = true;
		}

		private void DisableCollider()
		{
			ClearInsideColliders();
			_triggerCollider.enabled = false;
		}

		public void SetSpikePropDurationAndCD(float duration, float cd)
		{
			_isInContineousState = false;
			_duration = duration;
			_CD = cd;
		}
	}
}
