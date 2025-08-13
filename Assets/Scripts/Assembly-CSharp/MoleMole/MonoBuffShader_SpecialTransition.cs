using UnityEngine;

namespace MoleMole
{
	public class MonoBuffShader_SpecialTransition : MonoBuffShader_Base
	{
		public static string DefaultShaderKeyword = "SPECIAL_STATE";

		public Texture SPTex;

		public Texture SPNoiseTex;

		public float SPNoiseScalar = 1f;

		public float SPIntensity;

		public Color SPTransitionColor = Color.white;

		public Color SPOutlineColor = Color.white;

		public float SPTransitionEmissionScalar = 1f;

		public float SPTransitionBloomFactor = 1f;

		public float SPEnterDuration = 0.2f;

		public float SPExitDuration = 0.5f;

		[HideInInspector]
		public string TransitionName = "_SPTransition";

		public void PushValue(ref Material mat)
		{
			mat.SetTexture("_SPTex", SPTex);
			mat.SetTexture("_SPNoiseTex", SPNoiseTex);
			mat.SetFloat("_SPNoiseScaler", SPNoiseScalar);
			mat.SetFloat("_SPIntensity", SPIntensity);
			mat.SetColor("_SPTransitionColor", SPTransitionColor);
			mat.SetColor("_SPOutlineColor", SPOutlineColor);
			mat.SetFloat("_SPTransitionEmissionScaler", SPTransitionEmissionScalar);
			mat.SetFloat("_SPTransitionBloomFactor", SPTransitionBloomFactor);
		}
	}
}
