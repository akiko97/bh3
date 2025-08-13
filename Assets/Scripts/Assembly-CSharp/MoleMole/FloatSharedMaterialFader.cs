using UnityEngine;

namespace MoleMole
{
	public class FloatSharedMaterialFader : RendererFader
	{
		private float _origAlpha;

		private MaterialPropertyBlock _block;

		public FloatSharedMaterialFader(Renderer renderer, string property)
		{
			_renderer = renderer;
			_propertyID = Shader.PropertyToID(property);
			_origAlpha = _renderer.sharedMaterial.GetFloat(_propertyID);
		}

		public override void LerpAlpha(float t)
		{
			_renderer.sharedMaterial.SetFloat(_propertyID, _origAlpha * t);
		}
	}
}
