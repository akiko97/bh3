using UnityEngine;

namespace MoleMole
{
	public class FloatRendererFader : RendererFader
	{
		private float _origAlpha;

		private MaterialPropertyBlock _block;

		public FloatRendererFader(Renderer renderer, string property)
		{
			_renderer = renderer;
			_propertyID = Shader.PropertyToID(property);
			_origAlpha = _renderer.sharedMaterial.GetFloat(_propertyID);
			_block = new MaterialPropertyBlock();
		}

		public override void LerpAlpha(float t)
		{
			_renderer.GetPropertyBlock(_block);
			_block.SetFloat(_propertyID, _origAlpha * t);
			_renderer.SetPropertyBlock(_block);
		}
	}
}
