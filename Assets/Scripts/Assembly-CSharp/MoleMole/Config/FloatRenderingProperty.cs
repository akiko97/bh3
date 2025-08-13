using System;
using UnityEngine;

namespace MoleMole.Config
{
	public class FloatRenderingProperty : BaseRenderingProperty
	{
		public float value;

		public float min;

		public float max;

		[NonSerialized]
		public float _fromValue;

		[NonSerialized]
		public float _toValue;

		public FloatRenderingProperty()
		{
		}

		public FloatRenderingProperty(string name, float value, float min, float max)
		{
			propertyName = name;
			this.value = value;
			this.min = min;
			this.max = max;
		}

		public override BaseInstancedRenderingProperty CreateInstancedProperty(Material material)
		{
			FloatInstancedRenderingProperty floatInstancedRenderingProperty = new FloatInstancedRenderingProperty();
			floatInstancedRenderingProperty.material = material;
			floatInstancedRenderingProperty.propertyID = Shader.PropertyToID(propertyName);
			floatInstancedRenderingProperty.value = value;
			return floatInstancedRenderingProperty;
		}

		public override void SetupTransition(BaseRenderingProperty target)
		{
			_fromValue = value;
			_toValue = ((target == null) ? value : ((FloatRenderingProperty)target).value);
		}

		public override void LerpStep(float t)
		{
			value = Mathf.Lerp(_fromValue, _toValue, t);
		}

		public override void SimpleApplyOnMaterial(Material material)
		{
			material.SetFloat(propertyName, value);
		}

		public override void ApplyGlobally()
		{
			Shader.SetGlobalFloat(propertyName, value);
		}
	}
}
