using UnityEngine;

namespace MoleMole
{
	public class MonoLightCircleShadow : MonoLightShadow
	{
		public float radius = 1f;

		public override float Evaluate(Vector3 pos)
		{
			float num = Vector3.Distance(pos, base.XZPosition);
			if (num > radius)
			{
				return 0f;
			}
			return attenuateCurve.Evaluate(num / radius);
		}

		private void OnDrawGizmosSelected()
		{
			Color color = new Color(0.5f, 0.5f, 0.5f);
			color.a = 0.3f;
			Gizmos.color = color;
			Gizmos.DrawSphere(base.transform.position, radius);
		}
	}
}
