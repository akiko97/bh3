using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoHintArrow : MonoBehaviour
	{
		public enum EffectType
		{
			Twinkle = 0,
			Count = 1
		}

		public enum State
		{
			Visible = 0,
			Hidden = 1,
			FadingIn = 2,
			FadingOut = 3
		}

		private const string ARROW_IN_ANIM = "HintArrowIn";

		private const string ARROW_OUT_ANIM = "HintArrowOut";

		private Color _emissivOrigineColor = Color.white;

		private Color _twinkleColor = Color.gray;

		private uint _twinkleIntevalFrameCount = 2u;

		private uint _twinkleSumUpFrameCount;

		private uint _twinkleTotalFrameCount = 20u;

		private bool _isOriginColor = true;

		private bool _isInTwinkle;

		[NonSerialized]
		public uint listenRuntimID;

		[NonSerialized]
		public BaseMonoEntity listenEntity;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Animation _animation;

		private MaterialPropertyBlock _block;

		private bool _destroyUponFadeOut;

		private AnimationState _fadeInAnimState;

		private AnimationState _fadeOutAnimState;

		public State state { get; private set; }

		public void Init(uint listenRuntimID = 0, BaseMonoEntity listenEntity = null)
		{
			this.listenRuntimID = listenRuntimID;
			this.listenEntity = listenEntity;
			_block = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_block);
			_emissivOrigineColor = _renderer.sharedMaterial.GetColor(InLevelData.SHADER_TINTCOLOR);
			_block.SetColor(InLevelData.SHADER_TINTCOLOR, _emissivOrigineColor);
			_renderer.SetPropertyBlock(_block);
			_fadeInAnimState = _animation["HintArrowIn"];
			_fadeOutAnimState = _animation["HintArrowOut"];
			base.gameObject.SetActive(false);
			state = State.Hidden;
		}

		public bool IsToBeRemove()
		{
			return listenEntity == null || listenEntity.IsToBeRemove();
		}

		public void TriggerEffect(EffectType effectType)
		{
			if (state == State.FadingIn)
			{
				_animation.Stop();
			}
			else if (state == State.Hidden || state == State.FadingOut)
			{
				return;
			}
			if (effectType == EffectType.Twinkle)
			{
				_twinkleSumUpFrameCount = 0u;
				_isInTwinkle = true;
			}
		}

		private void TwinkleRecoverOrigin()
		{
			_renderer.GetPropertyBlock(_block);
			_block.SetColor(InLevelData.SHADER_TINTCOLOR, _emissivOrigineColor);
			_renderer.SetPropertyBlock(_block);
			_twinkleSumUpFrameCount = 0u;
			_isInTwinkle = false;
			_isOriginColor = true;
		}

		private void LateUpdateTwinkle()
		{
			if (_isInTwinkle)
			{
				if (_twinkleSumUpFrameCount % _twinkleIntevalFrameCount == 0)
				{
					_isOriginColor = !_isOriginColor;
				}
				_renderer.GetPropertyBlock(_block);
				_block.SetColor(InLevelData.SHADER_TINTCOLOR, (!_isOriginColor) ? _twinkleColor : _emissivOrigineColor);
				_renderer.SetPropertyBlock(_block);
				_twinkleSumUpFrameCount++;
				if (_twinkleSumUpFrameCount >= _twinkleTotalFrameCount)
				{
					TwinkleRecoverOrigin();
				}
			}
		}

		private void Update()
		{
			if (state == State.FadingIn)
			{
				if (_animation["HintArrowIn"].normalizedTime > 1f)
				{
					_animation.Stop("HintArrowIn");
					state = State.Visible;
				}
			}
			else if (state == State.FadingOut)
			{
				if (_animation["HintArrowOut"].normalizedTime > 1f)
				{
					if (_destroyUponFadeOut)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					else
					{
						base.gameObject.SetActive(false);
					}
					state = State.Hidden;
				}
			}
			else if (state != State.Visible)
			{
			}
		}

		private void LateUpdate()
		{
			if (state == State.Visible)
			{
				LateUpdateTwinkle();
			}
		}

		public void SetVisible(bool visible)
		{
			if (state == State.Visible && _isInTwinkle)
			{
				TwinkleRecoverOrigin();
			}
			if (visible)
			{
				if (!_destroyUponFadeOut && state != State.FadingIn && state != State.Visible && (state == State.Hidden || state == State.FadingOut))
				{
					base.gameObject.SetActive(true);
					_animation.Play("HintArrowIn");
					state = State.FadingIn;
				}
			}
			else if (state != State.FadingOut && state != State.Hidden)
			{
				_animation.Play("HintArrowOut");
				state = State.FadingOut;
			}
		}

		public void SetDestroyUponFadeOut()
		{
			_destroyUponFadeOut = true;
		}
	}
}
