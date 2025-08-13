using UnityEngine;

namespace MoleMole
{
	public class FloatFader : MaterialFader
	{
		private float _origAlpha;

		public FloatFader(Material material, string property)
		{
			_material = material;
			_propertyID = Shader.PropertyToID(property);
			_origAlpha = _material.GetFloat(_propertyID);
		}

		public override void LerpAlpha(float t)
		{
			_material.SetFloat(_propertyID, _origAlpha * t);
		}
	}
}
