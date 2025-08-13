using UnityEngine;

namespace MoleMole
{
	public class MonoLightRectShadow : MonoLightShadow
	{
		public float xlen = 1f;

		public float zlen = 1f;

		public override float Evaluate(Vector3 pos)
		{
			Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
			float num = 0.5f * xlen;
			float num2 = 0.5f * zlen;
			if (vector.x > num || vector.x < 0f - num || vector.z > num2 || vector.z < 0f - num2)
			{
				return 0f;
			}
			float a = Mathf.Abs(vector.x) / num;
			float b = Mathf.Abs(vector.z) / num2;
			return attenuateCurve.Evaluate(Mathf.Max(a, b));
		}

		private void OnDrawGizmosSelected()
		{
			Color color = new Color(0.5f, 0.5f, 0.5f);
			Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.matrix = matrix;
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawCube(new Vector3(0f, 0f, 0f), new Vector3(xlen, 0.1f, zlen));
		}
	}
}
