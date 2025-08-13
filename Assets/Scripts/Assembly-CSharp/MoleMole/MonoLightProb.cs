using UnityEngine;

namespace MoleMole
{
	public class MonoLightProb : MonoLightProbEntityBase
	{
		public AnimationCurve attenuateCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		public virtual bool Evaluate(Vector3 pos, LightProbProperties defaultProperties, ref LightProbProperties ret)
		{
			float num = Vector3.Distance(pos, base.XZPosition);
			if (num > radius)
			{
				return false;
			}
			ret = LightProbProperties.Lerp(defaultProperties, properties, attenuateCurve.Evaluate(num / radius));
			return true;
		}
	}
}
