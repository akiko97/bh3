using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class ColorInstancedRenderingProperty : BaseInstancedRenderingProperty
	{
		public Color value;

		public Color _fromColor;

		public Color _toColor;

		public override BaseRenderingProperty CreateBaseRenderingProperty(string name)
		{
			return new ColorRenderingProperty(name, value);
		}

		public override void ApplyProperty()
		{
			material.SetColor(propertyID, value);
		}

		public override void SetupTransition(BaseRenderingProperty target)
		{
			_fromColor = value;
			_toColor = ((target == null) ? value : ((ColorRenderingProperty)target).value);
		}

		public override void LerpStep(float t)
		{
			value = Color.Lerp(_fromColor, _toColor, t);
		}

		public override void CopyFrom(BaseRenderingProperty target)
		{
			if (target != null)
			{
				value = ((ColorRenderingProperty)target).value;
			}
		}
	}
}
