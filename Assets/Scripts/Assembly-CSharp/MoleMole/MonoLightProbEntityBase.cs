using UnityEngine;

namespace MoleMole
{
	public abstract class MonoLightProbEntityBase : MonoBehaviour
	{
		public LightProbProperties properties;

		public float radius = 1f;

		public Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Color bodyColor = properties.bodyColor;
			bodyColor.a = 1f;
			Gizmos.color = bodyColor;
			Gizmos.DrawSphere(base.transform.position, 0.5f);
			bodyColor.a = 0.3f;
			Gizmos.color = bodyColor;
			Gizmos.DrawSphere(base.transform.position, radius);
		}
	}
}
