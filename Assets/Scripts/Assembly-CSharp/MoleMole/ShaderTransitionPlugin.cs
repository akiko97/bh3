using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class ShaderTransitionPlugin : BaseEntityFuncPlugin
	{
		public const int EXECUTION_ORDER = 8000;

		private List<BaseMonoAnimatorEntity.SpecialStateMaterialData> _mats = new List<BaseMonoAnimatorEntity.SpecialStateMaterialData>();

		private MonoBuffShader_SpecialTransition _shaderData;

		private bool _dir = true;

		private float _from;

		private float _to = 1f;

		private float _duration = 1f;

		private bool _active;

		private float _time;

		public ShaderTransitionPlugin(BaseMonoEntity entity)
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
				_time += Time.deltaTime * _entity.TimeScale;
				float value = Mathf.Lerp(_from, _to, _time / _duration);
				for (int i = 0; i < _mats.Count; i++)
				{
					Material material = _mats[i].material;
					material.SetFloat(_shaderData.TransitionName, value);
				}
				if (_time > _duration)
				{
					EndTransition();
				}
			}
		}

		public void StartTransition(List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_SpecialTransition shaderData, bool dir)
		{
			_mats = list;
			_shaderData = shaderData;
			_dir = dir;
			_from = ((!_dir) ? 1f : 0f);
			_to = ((!_dir) ? 0f : 1f);
			_duration = ((!_dir) ? _shaderData.SPExitDuration : _shaderData.SPEnterDuration);
			_active = true;
			_time = 0f;
			for (int i = 0; i < _mats.Count; i++)
			{
				Material mat = _mats[i].material;
				mat.EnableKeyword(MonoBuffShader_SpecialTransition.DefaultShaderKeyword);
				_shaderData.PushValue(ref mat);
				mat.SetFloat(_shaderData.TransitionName, _from);
			}
		}

		private void EndTransition()
		{
			_active = false;
			if (!_dir)
			{
				for (int i = 0; i < _mats.Count; i++)
				{
					Material material = _mats[i].material;
					material.DisableKeyword(MonoBuffShader_SpecialTransition.DefaultShaderKeyword);
				}
			}
		}

		public override bool IsActive()
		{
			return _active;
		}
	}
}
