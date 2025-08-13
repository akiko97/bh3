using UnityEngine;

namespace MoleMole
{
	public class MonoTriangleZone2D : MonoSubZone2D
	{
		public float zlen = 1f;

		public float pXlen = 1f;

		public float nXlen = 1f;

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

		public override bool Contain(Vector3 pos)
		{
			Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
			validateInput();
			float num = 0f;
			if (vector.z < 0f)
			{
				return false;
			}
			if (vector.x >= 0f)
			{
				num = (pXlen * vector.z + vector.x * zlen) / (pXlen * zlen);
				if (num > 1f)
				{
					return false;
				}
			}
			else
			{
				num = (nXlen * vector.z - vector.x * zlen) / (nXlen * zlen);
				if (num > 1f)
				{
					return false;
				}
			}
			return true;
		}
	}
}
