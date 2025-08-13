using UnityEngine;

namespace MoleMole
{
	public class ColorSharedMaterialFader : RendererFader
	{
		private float _origAlpha;

		public ColorSharedMaterialFader(Renderer renderer, string property)
		{
			_renderer = renderer;
			_propertyID = Shader.PropertyToID(property);
			_origAlpha = renderer.sharedMaterial.GetColor(_propertyID).a;
		}

		public override void LerpAlpha(float t)
		{
			Color color = _renderer.sharedMaterial.GetColor(_propertyID);
			color.a = _origAlpha * t;
			_renderer.sharedMaterial.SetColor(_propertyID, color);
		}
	}
}
