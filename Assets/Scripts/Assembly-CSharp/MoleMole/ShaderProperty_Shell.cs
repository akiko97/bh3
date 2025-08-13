using UnityEngine;

namespace MoleMole
{
	public class ShaderProperty_Shell : ShaderProperty_Base
	{
		public float _ShellNormalDisplacment;

		public Color _ShellColor;

		public float _ShellEmission;

		public Vector2 Tiling;

		public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
		{
			ShaderProperty_Shell shaderProperty_Shell = (ShaderProperty_Shell)to_;
			targetMat.SetFloat("_ShellNormalDisplacment", Mathf.Lerp(_ShellNormalDisplacment, shaderProperty_Shell._ShellNormalDisplacment, normalized));
			targetMat.SetColor("_ShellColor", Color.Lerp(_ShellColor, shaderProperty_Shell._ShellColor, normalized));
			targetMat.SetFloat("_ShellEmission", Mathf.Lerp(_ShellEmission, shaderProperty_Shell._ShellEmission, normalized));
			targetMat.SetTextureScale("_ShellTex", Vector2.Lerp(Tiling, shaderProperty_Shell.Tiling, normalized));
		}
	}
}
