using System;
using System.Collections;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class FadeAnimation : MonoBehaviour
	{
		public int materialId;

		public string mainColorName;

		public float fadeInDuration;

		public AnimationCurve fadeInCurve;

		public float fadeOutDuration;

		public AnimationCurve fadeOutCurve;

		private Material _material;

		private Color _normalColor;

		private bool _bFadeIn;

		private bool _bFadeOut;

		private float _elapse;

		private void Start()
		{
			Init();
		}

		private void Update()
		{
			if (_bFadeIn)
			{
				_elapse += Time.deltaTime;
				if (_elapse > fadeInDuration)
				{
					_bFadeIn = false;
					_material.SetColor(mainColorName, _normalColor);
				}
				else
				{
					Color normalColor = _normalColor;
					normalColor.a = Mathf.Lerp(0f, _normalColor.a, fadeInCurve.Evaluate(_elapse / fadeInDuration));
					_material.SetColor(mainColorName, normalColor);
				}
			}
			if (_bFadeOut)
			{
				_elapse += Time.deltaTime;
				if (_elapse > fadeOutDuration)
				{
					_bFadeOut = false;
					Color normalColor2 = _normalColor;
					normalColor2.a = 0f;
					_material.SetColor(mainColorName, normalColor2);
				}
				else
				{
					Color normalColor3 = _normalColor;
					normalColor3.a = Mathf.Lerp(_normalColor.a, 0f, fadeOutCurve.Evaluate(_elapse / fadeOutDuration));
					_material.SetColor(mainColorName, normalColor3);
				}
			}
		}

		public void StartFadeOut(Action action)
		{
			_bFadeOut = true;
			_elapse = 0f;
			StartCoroutine(NotifyEnd(action));
		}

		private IEnumerator NotifyEnd(Action action)
		{
			yield return new WaitForSeconds(fadeOutDuration);
			action();
		}

		private void Init()
		{
			_material = GetComponent<Renderer>().materials[materialId];
			_normalColor = _material.GetColor(mainColorName);
			Color normalColor = _normalColor;
			normalColor.a = 0f;
			_material.SetColor(mainColorName, normalColor);
			_bFadeIn = true;
			_elapse = 0f;
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				UnityEngine.Object.DestroyImmediate(_material);
			}
		}
	}
}
