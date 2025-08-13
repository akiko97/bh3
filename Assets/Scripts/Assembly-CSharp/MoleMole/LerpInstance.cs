using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class LerpInstance
	{
		private ShaderLerpPlugin _shaderLerpPlugin;

		private BaseMonoAnimatorEntity _entity;

		private E_ShaderData _dataType;

		private List<BaseMonoAnimatorEntity.SpecialStateMaterialData> _mats;

		private MonoBuffShader_Lerp _shaderData;

		private bool isEnableTransition;

		private float _duration = 1f;

		private bool _active;

		private float _time;

		public LerpInstance(ShaderLerpPlugin plugin, BaseMonoAnimatorEntity entity, E_ShaderData dataType, List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_Lerp shaderData, bool dir)
		{
			_shaderLerpPlugin = plugin;
			_entity = entity;
			_dataType = dataType;
			_mats = list;
			_shaderData = shaderData;
			isEnableTransition = dir;
		}

		public void Core()
		{
			if (!_active)
			{
				return;
			}
			_time += Time.deltaTime * _entity.TimeScale;
			float normalized = Mathf.Clamp01(_time / _duration);
			for (int i = 0; i < _mats.Count; i++)
			{
				Material material = _mats[i].material;
				MaterialColorModifier.Multiplier colorMultiplier = _mats[i].colorMultiplier;
				if (_dataType == E_ShaderData.InverseTimeSpace)
				{
					_shaderData.Lerp<ShaderProperty_Shell>(material, normalized, isEnableTransition);
				}
				else if (_dataType == E_ShaderData.Transparent)
				{
					_shaderData.Lerp<ShaderProperty_SpecialState>(material, normalized, isEnableTransition);
				}
				else if (_dataType == E_ShaderData.Distortion)
				{
					_shaderData.Lerp<ShaderProperty_Distortion>(material, normalized, isEnableTransition);
				}
				else if (_dataType == E_ShaderData.ColorBias)
				{
					_shaderData.Lerp<ShaderProperty_ColorBias>(colorMultiplier, normalized, isEnableTransition);
				}
				else
				{
					_shaderData.Lerp<ShaderProperty_Rim>(material, normalized, isEnableTransition);
				}
			}
			if (_time > _duration)
			{
				EndTransition();
			}
		}

		public void StartLerping()
		{
			_duration = ((!isEnableTransition) ? _shaderData.DisableDuration : _shaderData.EnableDuration);
			_active = true;
			_time = 0f;
			if (_dataType != E_ShaderData.ColorBias)
			{
				return;
			}
			for (int i = 0; i < _mats.Count; i++)
			{
				Material material = _mats[i].material;
				if (isEnableTransition)
				{
					Color originalColor = ((!material.HasProperty("_MainColor")) ? material.color : material.GetColor("_MainColor"));
					(_shaderData.FromProperty as ShaderProperty_ColorBias).SetOriginalColor(originalColor);
					(_shaderData.ToProperty as ShaderProperty_ColorBias).SetOriginalColor(originalColor);
				}
			}
		}

		private void EndTransition()
		{
			_active = false;
			if (isEnableTransition)
			{
				return;
			}
			for (int i = 0; i < _mats.Count; i++)
			{
				Material material = _mats[i].material;
				if (_shaderData.Keyword == "DISTORTION")
				{
					material.SetOverrideTag("Distortion", "None");
				}
				else
				{
					material.DisableKeyword(_shaderData.Keyword);
				}
			}
			if (!string.IsNullOrEmpty(_shaderData.NewShaderName))
			{
				int index = _shaderLerpPlugin.PopFirstNewShaderEntryByShaderDataType(_dataType);
				_entity.PopShaderStackByIndex(index);
			}
		}

		public bool IsActive()
		{
			return _active;
		}
	}
}
