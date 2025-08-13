using UnityEngine;

namespace MoleMole
{
	public class ShaderProperty_Distortion : ShaderProperty_Base
	{
		public float _DTIntensity;

		public float _DTPlaySpeed;

		public float _DTNormalDisplacment;

		public float _DTUVScaleInX;

		public float _DTUVScaleInY;

		public Vector4 _DTFresnel;

		public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_Distortion shaderProperty_Distortion = (ShaderProperty_Distortion)to_;
			targetMat.SetFloat("_DTIntensity", Mathf.Lerp(_DTIntensity, shaderProperty_Distortion._DTIntensity, normalized));
			targetMat.SetFloat("_DTPlaySpeed", Mathf.Lerp(_DTPlaySpeed, shaderProperty_Distortion._DTPlaySpeed, normalized));
			targetMat.SetFloat("_DTNormalDisplacment", Mathf.Lerp(_DTNormalDisplacment, shaderProperty_Distortion._DTNormalDisplacment, normalized));
			targetMat.SetFloat("_DTUVScaleInX", Mathf.Lerp(_DTUVScaleInX, shaderProperty_Distortion._DTUVScaleInX, normalized));
			targetMat.SetFloat("_DTUVScaleInY", Mathf.Lerp(_DTUVScaleInY, shaderProperty_Distortion._DTUVScaleInY, normalized));
			targetMat.SetVector("_DTFresnel", Vector4.Lerp(_DTFresnel, shaderProperty_Distortion._DTFresnel, normalized));
		}
	}
}
