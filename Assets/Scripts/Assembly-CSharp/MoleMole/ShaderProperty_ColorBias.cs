using UnityEngine;

namespace MoleMole
{
	public class ShaderProperty_ColorBias : ShaderProperty_Base
	{
		public Color BiasColor;

		private Color _originalColor = Color.white;

		public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_ColorBias shaderProperty_ColorBias = (ShaderProperty_ColorBias)to_;
			Color color = _originalColor * Color.Lerp(BiasColor, shaderProperty_ColorBias.BiasColor, normalized);
			if (targetMat.HasProperty("_MainColor"))
			{
				targetMat.SetColor("_MainColor", color);
			}
			else
			{
				targetMat.color = color;
			}
		}

		public override void LerpTo(MaterialColorModifier.Multiplier multiplier, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_ColorBias shaderProperty_ColorBias = (ShaderProperty_ColorBias)to_;
			if (multiplier != null)
			{
				multiplier.mulColor = Color.Lerp(BiasColor, shaderProperty_ColorBias.BiasColor, normalized);
			}
		}

		public void SetOriginalColor(Color color)
		{
			_originalColor = color;
		}
	}
}
