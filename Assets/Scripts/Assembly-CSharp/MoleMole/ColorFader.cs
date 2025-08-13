using UnityEngine;

namespace MoleMole
{
	public class ColorFader : MaterialFader
	{
		private Color _origColor;

		public ColorFader(Material material, string property)
		{
			_material = material;
			_propertyID = Shader.PropertyToID(property);
			_origColor = _material.GetColor(_propertyID);
		}

		public override void LerpAlpha(float t)
		{
			Color origColor = _origColor;
			origColor.a *= t;
			_material.SetColor(_propertyID, origColor);
		}
	}
}
