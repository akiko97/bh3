using UnityEngine;

namespace MoleMole
{
	public class MonoBuffShader_Lerp : MonoBuffShader_Base
	{
		public ShaderProperty_Base FromProperty;

		public ShaderProperty_Base ToProperty;

		public float EnableDuration = 0.5f;

		public float DisableDuration = 0.5f;

		public string Keyword = string.Empty;

		public AnimationCurve LerpCurve;

		public string NewShaderName = string.Empty;

		public Texture NewTexture;

		public string TexturePropertyName = string.Empty;

		public void Lerp<T>(Material targetMat, float normalized, bool dir) where T : ShaderProperty_Base
		{
			T val = ((!dir) ? ((T)ToProperty) : ((T)FromProperty));
			T to = ((!dir) ? ((T)FromProperty) : ((T)ToProperty));
			val.LerpTo(targetMat, to, LerpCurve.Evaluate(normalized));
		}

		public void Lerp<T>(MaterialColorModifier.Multiplier multiplier, float normalized, bool dir) where T : ShaderProperty_Base
		{
			T val = ((!dir) ? ((T)ToProperty) : ((T)FromProperty));
			T to_ = ((!dir) ? ((T)FromProperty) : ((T)ToProperty));
			val.LerpTo(multiplier, to_, LerpCurve.Evaluate(normalized));
		}
	}
}
