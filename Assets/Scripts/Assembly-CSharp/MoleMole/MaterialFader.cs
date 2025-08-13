using UnityEngine;

namespace MoleMole
{
	public abstract class MaterialFader : IAlphaFader
	{
		protected Material _material;

		protected int _propertyID;

		public abstract void LerpAlpha(float t);
	}
}
