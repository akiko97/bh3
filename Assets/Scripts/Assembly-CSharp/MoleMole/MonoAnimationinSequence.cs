using System;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Animation))]
	public class MonoAnimationinSequence : MonoBehaviour
	{
		private OnAnimationEnd _onAnimationEnd;

		private Action<Transform> _endCallBack;

		public string animationName;

		private Animation _animation;

		private bool _needReset;

		public string audioPattern;

		public float audioPatternDelay;

		private float _timer = -1f;

		private void Awake()
		{
			_animation = GetComponent<Animation>();
		}

		public void SetAnimationEndCallBack(OnAnimationEnd callBack, Action<Transform> endCallBack = null)
		{
			_onAnimationEnd = callBack;
			_endCallBack = endCallBack;
		}

		public void AnimationEnd()
		{
			if (_endCallBack != null)
			{
				_endCallBack(base.transform);
			}
			if (_onAnimationEnd != null)
			{
				_onAnimationEnd();
			}
		}

		private void Update()
		{
			if (_timer >= 0f)
			{
				_timer -= Time.deltaTime;
				if (_timer < 0f)
				{
					Singleton<WwiseAudioManager>.Instance.Post(audioPattern);
				}
			}
		}

		public void TryResetToAnimationFirstFrame()
		{
			if (!_needReset)
			{
				_needReset = true;
				return;
			}
			AnimationState animationState;
			if (!string.IsNullOrEmpty(animationName))
			{
				_animation.clip = _animation[animationName].clip;
				animationState = _animation[animationName];
			}
			else
			{
				animationState = _animation[_animation.clip.name];
			}
			animationState.enabled = true;
			animationState.time = 0f;
			animationState.weight = 1f;
			_animation.Sample();
			animationState.enabled = false;
		}

		public void Play()
		{
			if (string.IsNullOrEmpty(animationName))
			{
				base.transform.GetComponent<Animation>().Play();
			}
			else
			{
				base.transform.GetComponent<Animation>().PlayQueued(animationName);
			}
			if (!string.IsNullOrEmpty(audioPattern))
			{
				if (audioPatternDelay == 0f)
				{
					Singleton<WwiseAudioManager>.Instance.Post(audioPattern);
				}
				else
				{
					_timer = audioPatternDelay;
				}
			}
		}
	}
}
