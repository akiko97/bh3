using UnityEngine;

namespace MoleMole
{
	public class MonoCircleZone2D : MonoSubZone2D
	{
		public float radius = 1f;

		public Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		public override bool Contain(Vector3 pos)
		{
			float num = Vector3.Distance(pos, XZPosition);
			return num < radius;
		}
	}
}
