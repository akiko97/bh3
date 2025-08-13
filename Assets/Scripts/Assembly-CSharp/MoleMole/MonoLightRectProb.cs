using UnityEngine;

namespace MoleMole
{
	public class MonoLightRectProb : MonoLightProb
	{
		public float xlen = 1f;

		public float zlen = 1f;

		public override bool Evaluate(Vector3 pos, LightProbProperties defaultProperties, ref LightProbProperties ret)
		{
			Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
			float num = 0.5f * xlen;
			float num2 = 0.5f * zlen;
			if (vector.x > num || vector.x < 0f - num || vector.z > num2 || vector.z < 0f - num2)
			{
				return false;
			}
			float a = Mathf.Abs(vector.x) / num;
			float b = Mathf.Abs(vector.z) / num2;
			ret = LightProbProperties.Lerp(defaultProperties, properties, attenuateCurve.Evaluate(Mathf.Max(a, b)));
			return true;
		}

		private void OnDrawGizmosSelected()
		{
			Color bodyColor = properties.bodyColor;
			Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.matrix = matrix;
			bodyColor.a = 0.3f;
			Gizmos.color = bodyColor;
			Gizmos.DrawCube(new Vector3(0f, 0f, 0f), new Vector3(xlen, 0.1f, zlen));
		}
	}
}
