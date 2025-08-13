using UnityEngine;

namespace MoleMole.Config
{
	public abstract class BaseRenderingProperty
	{
		public string propertyName;

		public abstract BaseInstancedRenderingProperty CreateInstancedProperty(Material material);

		public abstract void SetupTransition(BaseRenderingProperty target);

		public abstract void LerpStep(float t);

		public abstract void SimpleApplyOnMaterial(Material material);

		public abstract void ApplyGlobally();

		public BaseRenderingProperty Clone()
		{
			return (BaseRenderingProperty)MemberwiseClone();
		}
	}
}
