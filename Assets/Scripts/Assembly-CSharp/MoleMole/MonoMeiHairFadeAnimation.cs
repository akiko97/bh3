using System;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class MonoMeiHairFadeAnimation : MonoBehaviour
	{
		[Serializable]
		public class FadeParams
		{
			public float offset = 1f;

			public float range = 0.01f;

			public float value = 1f;

			public bool IsValidFade()
			{
				return (double)value < 0.9999;
			}

			public static FadeParams Lerp(FadeParams params1, FadeParams params2, float t)
			{
				FadeParams fadeParams = new FadeParams();
				if (!params1.IsValidFade())
				{
					fadeParams.offset = params2.offset;
					fadeParams.range = params2.range;
				}
				if (!params2.IsValidFade())
				{
					fadeParams.offset = params1.offset;
					fadeParams.range = params1.range;
				}
				else
				{
					fadeParams.offset = Mathf.Lerp(params1.offset, params2.offset, t);
					fadeParams.range = Mathf.Lerp(params1.range, params2.range, t);
				}
				fadeParams.value = Mathf.Lerp(params1.value, params2.value, t);
				return fadeParams;
			}
		}

		private static readonly string NORMAL_SHADER_PATH = "miHoYo/Character/Avatar";

		private static readonly string FADING_SHADER_PATH = "miHoYo/Character/Avatar UI Hair";

		private Shader _normalShader;

		private Shader _fadingShader;

		public float fadeDuration;

		public FadeParams paramsForWeapon;

		public FadeParams paramsForStigmata;

		private float _timer;

		private FadeParams _originalParams;

		private FadeParams _currentParams;

		private FadeParams _fromParams;

		private FadeParams _toParams;

		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		public void CancelFade()
		{
			FadeToParams(_originalParams);
		}

		public void FadeForWeaponTab()
		{
			FadeToParams(paramsForWeapon);
		}

		public void FadeForStigmataTab()
		{
			FadeToParams(paramsForStigmata);
		}

		private void FadeToParams(FadeParams targetParams)
		{
			_timer = 0f;
			_fromParams = _currentParams;
			_toParams = targetParams;
		}

		private void Start()
		{
			_originalParams = new FadeParams();
			_originalParams.value = 1f;
			_currentParams = _originalParams;
			_fromParams = _originalParams;
			_timer = 0f;
			_renderer = GetComponent<Renderer>();
			_mpb = new MaterialPropertyBlock();
			_normalShader = Shader.Find(NORMAL_SHADER_PATH);
			_fadingShader = Shader.Find(FADING_SHADER_PATH);
		}

		private void Update()
		{
			if (!(_timer < fadeDuration) || _toParams == null)
			{
				return;
			}
			_timer += Time.deltaTime;
			FadeParams fadeParams = FadeParams.Lerp(_fromParams, _toParams, Mathf.Clamp01(_timer / fadeDuration));
			bool flag = (double)_currentParams.value < 0.99;
			bool flag2 = (double)fadeParams.value < 0.99;
			if (flag2 && !flag)
			{
				for (int i = 0; i < _renderer.materials.Length; i++)
				{
					_renderer.materials[i].shader = _fadingShader;
				}
			}
			else if (!flag2 && flag)
			{
				for (int j = 0; j < _renderer.materials.Length; j++)
				{
					_renderer.materials[j].shader = _normalShader;
				}
			}
			_currentParams = fadeParams;
			ApplyFade();
		}

		private void ApplyFade()
		{
			_renderer.GetPropertyBlock(_mpb);
			_mpb.SetFloat("_DirectionalFadeOffset", _currentParams.offset);
			_mpb.SetFloat("_DirectionalFadeRange", _currentParams.range);
			_mpb.SetFloat("_DirectionalFadeValue", _currentParams.value);
			_renderer.SetPropertyBlock(_mpb);
		}
	}
}
