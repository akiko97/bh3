using UnityEngine;

namespace MoleMole
{
	public class MaterialPropertyBlockFader
	{
		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		private string _propertyName;

		private Color _originColor;

		public MaterialPropertyBlockFader(Renderer renderer, string name)
		{
			_renderer = renderer;
			_mpb = new MaterialPropertyBlock();
			_propertyName = name;
			_originColor = _renderer.sharedMaterial.GetColor(name);
		}

		public void LerpAlpha(float t)
		{
			Color originColor = _originColor;
			originColor.a *= t;
			_renderer.GetPropertyBlock(_mpb);
			_mpb.SetColor(_propertyName, originColor);
			_renderer.SetPropertyBlock(_mpb);
		}
	}
}
