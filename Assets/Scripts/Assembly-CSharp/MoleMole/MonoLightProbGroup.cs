using UnityEngine;

namespace MoleMole
{
	public class MonoLightProbGroup : MonoLightProbEntityBase
	{
		private MonoLightProb[] _probs;

		public void Init()
		{
			_probs = GetComponentsInChildren<MonoLightProb>();
		}

		public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
		{
			float num = Vector3.Distance(pos, base.XZPosition);
			if (num > radius)
			{
				return false;
			}
			ret = default(LightProbProperties);
			int num2 = 0;
			MonoLightProb[] probs = _probs;
			foreach (MonoLightProb monoLightProb in probs)
			{
				LightProbProperties ret2 = default(LightProbProperties);
				if (monoLightProb.Evaluate(pos, properties, ref ret2))
				{
					num2++;
					ret += ret2;
				}
			}
			if (num2 != 0)
			{
				ret /= (float)num2;
			}
			else
			{
				ret = properties;
			}
			return true;
		}
	}
}
