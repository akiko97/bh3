using UnityEngine;

namespace MoleMole
{
	public abstract class ShaderProperty_Base : MonoBehaviour
	{
		public abstract void LerpTo(Material targetMat, ShaderProperty_Base to, float normalized);

		public virtual void LerpTo(MaterialColorModifier.Multiplier multiplier, ShaderProperty_Base to_, float normalized)
		{
		}
	}
}
