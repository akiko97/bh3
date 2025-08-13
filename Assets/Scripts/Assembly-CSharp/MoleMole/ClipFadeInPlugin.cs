using UnityEngine;

namespace MoleMole
{
	public class ClipFadeInPlugin : BaseEntityFuncPlugin
	{
		public const int EXECUTION_ORDER = 9102;

		private float _clipMaxHeight;

		private float _fadeTime;

		private float _timer;

		private bool _active;

		private string EFFECT_SHADER_PATH = "miHoYo/Character/Plane_Clip";

		private Shader[] _originShaders;

		private Material[] _materialList;

		public ClipFadeInPlugin(BaseMonoEntity entity)
			: base(entity)
		{
		}

		public override void FixedCore()
		{
		}

		public override void Core()
		{
			if (_active)
			{
				_timer += Time.deltaTime * _entity.TimeScale;
				LerpHeigh(Mathf.Lerp(0f - _clipMaxHeight, 0f, _timer / _fadeTime));
				if (_timer > _fadeTime)
				{
					EndFade();
				}
			}
		}

		public void StartFade(float clipMaxHeight, float time)
		{
			_clipMaxHeight = clipMaxHeight;
			_fadeTime = time;
			_active = true;
			IFadeOff fadeOff = (IFadeOff)_entity;
			_materialList = fadeOff.GetAllMaterials();
			_originShaders = new Shader[_materialList.Length];
			int i = 0;
			for (int num = _materialList.Length; i < num; i++)
			{
				Material material = _materialList[i];
				_originShaders[i] = material.shader;
				material.shader = Shader.Find(EFFECT_SHADER_PATH);
				material.SetVector("_ClipPlane", new Vector4(0f, 1f, 0f, 0f - _clipMaxHeight));
			}
		}

		private void EndFade()
		{
			_active = false;
			int i = 0;
			for (int num = _materialList.Length; i < num; i++)
			{
				_materialList[i].shader = _originShaders[i];
			}
			_originShaders = null;
		}

		private void LerpHeigh(float height)
		{
			Material[] materialList = _materialList;
			foreach (Material material in materialList)
			{
				material.SetVector("_ClipPlane", new Vector4(0f, 1f, 0f, height));
			}
		}

		public override bool IsActive()
		{
			return _active;
		}
	}
}
