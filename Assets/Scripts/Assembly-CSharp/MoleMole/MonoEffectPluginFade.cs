using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginFade : BaseMonoEffectPlugin
	{
		private enum State
		{
			FadingIn = 0,
			FadingHold = 1,
			FadingOut = 2
		}

		public float FadeInTime;

		public float FadeHoldTime;

		public float FadeOutTime;

		[Header("Tick this to hold forever and only uses FadeIn/FadeOut")]
		public bool HoldForever;

		[Header("Select which properties to fade.")]
		public bool FadeTintColor = true;

		public bool FadeEmissionColor;

		public bool FadeMainAlpha;

		public bool FadeOutlineAlpha;

		public bool FadeMainColor;

		public bool FadeOpaqueness;

		private float _timer;

		private List<IAlphaFader> _faders;

		private State _state;

		public override void Setup()
		{
			if (FadeInTime > 0f)
			{
				LerpAllFaders(0f);
				_state = State.FadingIn;
			}
			else
			{
				LerpAllFaders(1f);
				_state = State.FadingHold;
			}
			_timer = 0f;
		}

		protected override void Awake()
		{
			base.Awake();
			SetupFaders();
			_timer = 0f;
		}

		private void SetupFaders()
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			_faders = new List<IAlphaFader>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer is ParticleSystemRenderer)
				{
					GraphicsUtils.CreateAndAssignInstancedMaterial(renderer, renderer.sharedMaterial);
					if (FadeTintColor && renderer.sharedMaterial.HasProperty("_TintColor"))
					{
						_faders.Add(new ColorSharedMaterialFader(renderer, "_TintColor"));
					}
					if (FadeEmissionColor && renderer.sharedMaterial.HasProperty("_EmissionColor"))
					{
						_faders.Add(new ColorSharedMaterialFader(renderer, "_EmissionColor"));
					}
					if (FadeMainAlpha && renderer.sharedMaterial.HasProperty("_MainAlpha"))
					{
						_faders.Add(new FloatSharedMaterialFader(renderer, "_MainAlpha"));
					}
					if (FadeOutlineAlpha && renderer.sharedMaterial.HasProperty("_OutlineAlpha"))
					{
						_faders.Add(new FloatSharedMaterialFader(renderer, "_OutlineAlpha"));
					}
					if (FadeMainColor && renderer.sharedMaterial.HasProperty("_Color"))
					{
						_faders.Add(new ColorSharedMaterialFader(renderer, "_Color"));
					}
					if (FadeOpaqueness && renderer.sharedMaterial.HasProperty("_Opaqueness"))
					{
						_faders.Add(new FloatSharedMaterialFader(renderer, "_Opaqueness"));
					}
				}
				else
				{
					if (FadeTintColor && renderer.sharedMaterial.HasProperty("_TintColor"))
					{
						_faders.Add(new ColorRendererFader(renderer, "_TintColor"));
					}
					if (FadeEmissionColor && renderer.sharedMaterial.HasProperty("_EmissionColor"))
					{
						_faders.Add(new ColorRendererFader(renderer, "_EmissionColor"));
					}
					if (FadeMainAlpha && renderer.sharedMaterial.HasProperty("_MainAlpha"))
					{
						_faders.Add(new FloatRendererFader(renderer, "_MainAlpha"));
					}
					if (FadeOutlineAlpha && renderer.sharedMaterial.HasProperty("_OutlineAlpha"))
					{
						_faders.Add(new FloatRendererFader(renderer, "_OutlineAlpha"));
					}
					if (FadeMainColor && renderer.sharedMaterial.HasProperty("_Color"))
					{
						_faders.Add(new ColorRendererFader(renderer, "_Color"));
					}
					if (FadeOpaqueness && renderer.sharedMaterial.HasProperty("_Opaqueness"))
					{
						_faders.Add(new FloatRendererFader(renderer, "_Opaqueness"));
					}
				}
			}
		}

		private void LerpAllFaders(float t)
		{
			for (int i = 0; i < _faders.Count; i++)
			{
				_faders[i].LerpAlpha(t);
			}
		}

		public void Update()
		{
			if (_state == State.FadingIn)
			{
				_timer += Time.deltaTime * _effect.TimeScale;
				LerpAllFaders(Mathf.Clamp01(_timer / FadeInTime));
				if (_timer > FadeInTime)
				{
					_timer = 0f;
					_state = State.FadingHold;
					LerpAllFaders(1f);
				}
			}
			else if (_state == State.FadingHold)
			{
				_timer += Time.deltaTime * _effect.TimeScale;
				if (!HoldForever && _timer > FadeHoldTime)
				{
					_timer = 0f;
					_state = State.FadingOut;
				}
			}
			else if (_state == State.FadingOut)
			{
				_timer += Time.deltaTime * _effect.TimeScale;
				LerpAllFaders(Mathf.Clamp01(1f - _timer / FadeOutTime));
			}
		}

		public override bool IsToBeRemove()
		{
			return _state == State.FadingOut && _timer > FadeOutTime;
		}

		public override void SetDestroy()
		{
			_state = State.FadingOut;
			_timer = 0f;
		}

		private void OnDestroy()
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GraphicsUtils.TryCleanRendererInstancedMaterial(componentsInChildren[i]);
			}
		}
	}
}
