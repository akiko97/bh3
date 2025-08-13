using UnityEngine;

namespace MoleMole
{
	public abstract class RendererFader : IAlphaFader
	{
		protected Renderer _renderer;

		protected int _propertyID;

		public abstract void LerpAlpha(float t);
	}
}
