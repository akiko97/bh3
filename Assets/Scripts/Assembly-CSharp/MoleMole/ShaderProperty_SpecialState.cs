using UnityEngine;

namespace MoleMole
{
	public class ShaderProperty_SpecialState : ShaderProperty_Base
	{
		public float _SPNoiseScaler;

		public float _SPIntensity;

		public float _SPTransition;

		public Color _SPTransitionColor;

		public Color _SPOutlineColor;

		public float _SPTransitionEmissionScaler;

		public float _SPTransitionBloomFactor;

		public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_SpecialState shaderProperty_SpecialState = (ShaderProperty_SpecialState)to_;
			targetMat.SetFloat("_SPNoiseScaler", Mathf.Lerp(_SPNoiseScaler, shaderProperty_SpecialState._SPNoiseScaler, normalized));
			targetMat.SetFloat("_SPIntensity", Mathf.Lerp(_SPIntensity, shaderProperty_SpecialState._SPIntensity, normalized));
			targetMat.SetFloat("_SPTransition", Mathf.Lerp(_SPTransition, shaderProperty_SpecialState._SPTransition, normalized));
			targetMat.SetColor("_SPTransitionColor", Color.Lerp(_SPTransitionColor, shaderProperty_SpecialState._SPTransitionColor, normalized));
			targetMat.SetColor("_SPOutlineColor", Color.Lerp(_SPOutlineColor, shaderProperty_SpecialState._SPOutlineColor, normalized));
			targetMat.SetFloat("_SPTransitionEmissionScaler", Mathf.Lerp(_SPTransitionEmissionScaler, shaderProperty_SpecialState._SPTransitionEmissionScaler, normalized));
			targetMat.SetFloat("_SPTransitionBloomFactor", Mathf.Lerp(_SPTransitionBloomFactor, shaderProperty_SpecialState._SPTransitionBloomFactor, normalized));
		}
	}
}
