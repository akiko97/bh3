using UnityEngine;

namespace MoleMole
{
	public class MonoLightShadowGroup : MonoBehaviour
	{
		public LightProbProperties baseProperties;

		public LightProbProperties shadowProperties;

		public float radius = 1f;

		private MonoLightShadow[] _probs;

		public Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Color bodyColor = baseProperties.bodyColor;
			bodyColor.a = 0.5f;
			Gizmos.color = bodyColor;
			Gizmos.DrawSphere(base.transform.position, radius);
		}

		public void Init()
		{
			_probs = GetComponentsInChildren<MonoLightShadow>();
		}

		public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
		{
			float num = Vector3.Distance(pos, XZPosition);
			if (num > radius)
			{
				return false;
			}
			ret = default(LightProbProperties);
			float num2 = 0f;
			MonoLightShadow[] probs = _probs;
			foreach (MonoLightShadow monoLightShadow in probs)
			{
				float num3 = monoLightShadow.Evaluate(pos);
				if (num3 > num2)
				{
					num2 = num3;
				}
			}
			ret = LightProbProperties.Lerp(baseProperties, shadowProperties, num2);
			return true;
		}
	}
}
