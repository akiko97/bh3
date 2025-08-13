using System;
using UnityEngine;

namespace MoleMole.Config
{
	public class ColorRenderingProperty : BaseRenderingProperty
	{
		public Color value;

		[NonSerialized]
		public Color _fromValue;

		[NonSerialized]
		public Color _toValue;

		public ColorRenderingProperty()
		{
		}

		public ColorRenderingProperty(string name, Color value)
		{
			propertyName = name;
			this.value = value;
		}

		public override BaseInstancedRenderingProperty CreateInstancedProperty(Material material)
		{
			ColorInstancedRenderingProperty colorInstancedRenderingProperty = new ColorInstancedRenderingProperty();
			colorInstancedRenderingProperty.material = material;
			colorInstancedRenderingProperty.propertyID = Shader.PropertyToID(propertyName);
			colorInstancedRenderingProperty.value = value;
			return colorInstancedRenderingProperty;
		}

		public override void SetupTransition(BaseRenderingProperty target)
		{
			_fromValue = value;
			_toValue = ((target == null) ? value : ((ColorRenderingProperty)target).value);
		}

		public override void LerpStep(float t)
		{
			value = Color.Lerp(_fromValue, _toValue, t);
		}

		public override void SimpleApplyOnMaterial(Material material)
		{
			material.SetColor(propertyName, value);
		}

		public override void ApplyGlobally()
		{
			Shader.SetGlobalColor(propertyName, value);
		}
	}
}
