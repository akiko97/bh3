using System.Collections.Generic;
using UniRx;

namespace MoleMole
{
	public class ShaderLerpPlugin : BaseEntityFuncPlugin
	{
		public const int EXECUTION_ORDER = 8001;

		private List<LerpInstance> _lerpInstances = new List<LerpInstance>();

		private List<Tuple<E_ShaderData, int>> _newShaderEntries = new List<Tuple<E_ShaderData, int>>();

		private BaseMonoAnimatorEntity _animatorEntity;

		public ShaderLerpPlugin(BaseMonoAnimatorEntity entity)
			: base(entity)
		{
			_animatorEntity = entity;
		}

		public override void FixedCore()
		{
		}

		public override void Core()
		{
			for (int num = _lerpInstances.Count - 1; num >= 0; num--)
			{
				LerpInstance lerpInstance = _lerpInstances[num];
				lerpInstance.Core();
				if (!lerpInstance.IsActive())
				{
					_lerpInstances.Remove(lerpInstance);
				}
			}
		}

		public void StartLerp(E_ShaderData dataType, List<BaseMonoAnimatorEntity.SpecialStateMaterialData> list, MonoBuffShader_Lerp shaderData, bool dir, int shaderIx)
		{
			LerpInstance lerpInstance = new LerpInstance(this, _animatorEntity, dataType, list, shaderData, dir);
			if (shaderIx != -1)
			{
				int index = _newShaderEntries.SeekAddPosition();
				_newShaderEntries[index] = Tuple.Create(dataType, shaderIx);
			}
			_lerpInstances.Add(lerpInstance);
			lerpInstance.StartLerping();
		}

		public override bool IsActive()
		{
			return _lerpInstances.Count > 0;
		}

		public int PopFirstNewShaderEntryByShaderDataType(E_ShaderData dataType)
		{
			for (int i = 0; i < _newShaderEntries.Count; i++)
			{
				Tuple<E_ShaderData, int> tuple = _newShaderEntries[i];
				if (tuple != null && tuple.Item1 == dataType)
				{
					_newShaderEntries[i] = null;
					return tuple.Item2;
				}
			}
			return -1;
		}
	}
}
