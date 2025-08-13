using UnityEngine;

namespace MoleMole
{
	public class AlphaLerpMaterialPropetyBlock
	{
		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		private string _propertyName;

		private float _little_alpha;

		private float _large_alpha;

		private Color _color;

		private E_AlphaLerpDir _dir;

		public AlphaLerpMaterialPropetyBlock(Renderer renderer, string colorPropertyName, float littleA, float largeA)
		{
			_renderer = renderer;
			_mpb = new MaterialPropertyBlock();
			_propertyName = colorPropertyName;
			_little_alpha = littleA;
			_large_alpha = largeA;
			_color = _renderer.sharedMaterial.GetColor(_propertyName);
		}

		public void SetAlpha(float alpha)
		{
			_color.a = alpha;
			_renderer.GetPropertyBlock(_mpb);
			_mpb.SetColor(_propertyName, _color);
			_renderer.SetPropertyBlock(_mpb);
		}

		public void SetDir(E_AlphaLerpDir dir)
		{
			_dir = dir;
		}

		public void LerpAlpha(float t)
		{
			float a = ((_dir != E_AlphaLerpDir.ToLarge) ? _large_alpha : _little_alpha);
			float b = ((_dir != E_AlphaLerpDir.ToLarge) ? _little_alpha : _large_alpha);
			_color.a = Mathf.Lerp(a, b, t);
			_renderer.GetPropertyBlock(_mpb);
			_mpb.SetColor(_propertyName, _color);
			_renderer.SetPropertyBlock(_mpb);
		}
	}
}
