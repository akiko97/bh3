using UnityEngine;

namespace MoleMole
{
	public class MonoLightTriangleShadow : MonoLightShadow
	{
		public float zlen = 1f;

		public float pXlen = 1f;

		public float nXlen = 1f;

		private Mesh meshUp;

		private Mesh meshDown;

		private void validateInput()
		{
			if (zlen < 0f)
			{
				zlen = 0f - zlen;
			}
			if (pXlen < 0f)
			{
				pXlen = 0f - pXlen;
			}
			if (nXlen < 0f)
			{
				nXlen = 0f - nXlen;
			}
		}

		private void UpdateMesh()
		{
			if (meshUp == null)
			{
				meshUp = new Mesh();
			}
			if (meshDown == null)
			{
				meshDown = new Mesh();
			}
			validateInput();
			meshUp.vertices = new Vector3[3]
			{
				new Vector3(pXlen, 0f, 0f),
				new Vector3(0f, 0f, zlen),
				new Vector3(0f - nXlen, 0f, 0f)
			};
			meshUp.triangles = new int[3] { 0, 1, 2 };
			meshUp.normals = new Vector3[3]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
			meshDown.vertices = new Vector3[3]
			{
				new Vector3(pXlen, 0f, 0f),
				new Vector3(0f, 0f, zlen),
				new Vector3(0f - nXlen, 0f, 0f)
			};
			meshDown.triangles = new int[3] { 0, 2, 1 };
			meshDown.normals = new Vector3[3]
			{
				Vector3.down,
				Vector3.down,
				Vector3.down
			};
		}

		public override float Evaluate(Vector3 pos)
		{
			Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
			validateInput();
			float num = 0f;
			float num2 = 0f;
			if (vector.z < 0f)
			{
				return 0f;
			}
			num2 = (zlen - 2f * vector.z) / zlen;
			if (vector.x >= 0f)
			{
				num = (pXlen * vector.z + vector.x * zlen) / (pXlen * zlen);
				if (num > 1f)
				{
					return 0f;
				}
			}
			else
			{
				num = (nXlen * vector.z - vector.x * zlen) / (nXlen * zlen);
				if (num > 1f)
				{
					return 0f;
				}
			}
			return attenuateCurve.Evaluate(Mathf.Max(num, num2));
		}

		private void OnDrawGizmosSelected()
		{
			Color color = new Color(0.5f, 0.5f, 0.5f);
			Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			Gizmos.matrix = matrix;
			UpdateMesh();
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawMesh(meshUp);
			Gizmos.DrawMesh(meshDown);
		}
	}
}
