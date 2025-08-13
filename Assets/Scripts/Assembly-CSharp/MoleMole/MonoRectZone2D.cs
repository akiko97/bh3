using UnityEngine;

namespace MoleMole
{
	public class MonoRectZone2D : MonoSubZone2D
	{
		public float xlen = 1f;

		public float zlen = 1f;

		public override bool Contain(Vector3 pos)
		{
			Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
			float num = 0.5f * xlen;
			float num2 = 0.5f * zlen;
			if (vector.x > num || vector.x < 0f - num || vector.z > num2 || vector.z < 0f - num2)
			{
				return false;
			}
			return true;
		}

		private void OnDrawGizmosSelected()
		{
			Color green = Color.green;
			Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.matrix = matrix;
			green.a = 0.3f;
			Gizmos.color = green;
			Gizmos.DrawCube(new Vector3(0f, 0f, 0f), new Vector3(xlen, 0.1f, zlen));
		}
	}
}
