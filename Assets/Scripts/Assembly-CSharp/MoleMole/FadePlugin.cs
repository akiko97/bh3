using UnityEngine;

namespace MoleMole
{
	public class FadePlugin : BaseEntityFuncPlugin
	{
		public const int EXECUTION_ORDER = 9101;

		private float _from;

		private float _to;

		private float _fadeTime;

		private float _timer;

		private bool _active;

		private MaterialFader[] _faders;

		public FadePlugin(BaseMonoEntity entity)
			: base(entity)
		{
		}

		public void StartFade(float from, float to, float time)
		{
			if (_active)
			{
				return;
			}
			_from = from;
			_to = to;
			_fadeTime = time;
			_timer = 0f;
			_active = true;
			IFadeOff fadeOff = (IFadeOff)_entity;
			Material[] allMaterials = fadeOff.GetAllMaterials();
			_faders = new MaterialFader[allMaterials.Length * 2];
			for (int i = 0; i < allMaterials.Length; i++)
			{
				if (allMaterials[i].HasProperty("_MainAlpha"))
				{
					_faders[2 * i] = new FloatFader(allMaterials[i], "_MainAlpha");
					_faders[2 * i].LerpAlpha(_from);
					_faders[2 * i + 1] = new NopFader();
				}
				else if (allMaterials[i].HasProperty("_Color"))
				{
					_faders[2 * i] = new ColorFader(allMaterials[i], "_Color");
					_faders[2 * i].LerpAlpha(_from);
					_faders[2 * i + 1] = new ColorFader(allMaterials[i], "_OutlineColor");
					_faders[2 * i + 1].LerpAlpha(_from);
				}
				else
				{
					_faders[2 * i] = new NopFader();
					_faders[2 * i + 1] = new NopFader();
				}
			}
		}

		public override void FixedCore()
		{
		}

		public override void Core()
		{
			if (_active)
			{
				_timer += Time.deltaTime * _entity.TimeScale;
				LerpAlpha(Mathf.Lerp(_from, _to, _timer / _fadeTime));
				if (_timer > _fadeTime)
				{
					_active = false;
				}
			}
		}

		private void LerpAlpha(float t)
		{
			for (int i = 0; i < _faders.Length; i++)
			{
				_faders[i].LerpAlpha(t);
			}
		}

		public override bool IsActive()
		{
			return _active;
		}

		public bool IsLastActiveFadeOut()
		{
			return _to < _from;
		}
	}
}
