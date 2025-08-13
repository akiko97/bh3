using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class FloatInstancedRenderingProperty : BaseInstancedRenderingProperty
	{
		private const float DEFAULT_MIN = 0f;

		private const float DEFAULT_MAX = 100f;

		public float value;

		private float _fromFloat;

		private float _toFloat;

		public override void ApplyProperty()
		{
			material.SetFloat(propertyID, value);
		}

		public override BaseRenderingProperty CreateBaseRenderingProperty(string name)
		{
			return new FloatRenderingProperty(name, value, 0f, 100f);
		}

		public override void SetupTransition(BaseRenderingProperty target)
		{
			_fromFloat = value;
			_toFloat = ((target == null) ? value : ((FloatRenderingProperty)target).value);
		}

		public override void LerpStep(float t)
		{
			value = Mathf.Lerp(_fromFloat, _toFloat, t);
		}

		public override void CopyFrom(BaseRenderingProperty target)
		{
			if (target != null)
			{
				value = ((FloatRenderingProperty)target).value;
			}
		}
	}
}
