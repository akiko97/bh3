using UnityEngine;

namespace MoleMole
{
	public class ShaderProperty_Rim : ShaderProperty_Base
	{
		public Color _RGColor;

		public float _RGShininess;

		public float _RGScale;

		public float _RGBias;

		public float _RGRatio;

		public float _RGBloomFactor;

		public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_Rim shaderProperty_Rim = (ShaderProperty_Rim)to_;
			targetMat.SetColor("_RGColor", Color.Lerp(_RGColor, shaderProperty_Rim._RGColor, normalized));
			targetMat.SetFloat("_RGShininess", Mathf.Lerp(_RGShininess, shaderProperty_Rim._RGShininess, normalized));
			targetMat.SetFloat("_RGScale", Mathf.Lerp(_RGScale, shaderProperty_Rim._RGScale, normalized));
			targetMat.SetFloat("_RGBias", Mathf.Lerp(_RGBias, shaderProperty_Rim._RGBias, normalized));
			targetMat.SetFloat("_RGRatio", Mathf.Lerp(_RGRatio, shaderProperty_Rim._RGRatio, normalized));
			targetMat.SetFloat("_RGBloomFactor", Mathf.Lerp(_RGBloomFactor, shaderProperty_Rim._RGBloomFactor, normalized));
		}
	}
}
