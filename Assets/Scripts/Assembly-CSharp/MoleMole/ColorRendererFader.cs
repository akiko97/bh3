using UnityEngine;

namespace MoleMole
{
	public class ColorRendererFader : RendererFader
	{
		private Color _origColor;

		private float _origAlpha;

		private MaterialPropertyBlock _block;

		public ColorRendererFader(Renderer renderer, string property)
		{
			_renderer = renderer;
			_propertyID = Shader.PropertyToID(property);
			_origColor = renderer.sharedMaterial.GetColor(_propertyID);
			_origAlpha = _origColor.a;
			_block = new MaterialPropertyBlock();
		}

		public override void LerpAlpha(float t)
		{
			_renderer.GetPropertyBlock(_block);
			Vector4 vector = _block.GetVector(_propertyID);
			Color value = ((!(vector == Vector4.zero)) ? ((Color)vector) : _origColor);
			value.a = _origAlpha * t;
			_block.SetColor(_propertyID, value);
			_renderer.SetPropertyBlock(_block);
		}
	}
}
