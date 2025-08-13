using UnityEngine;

namespace MoleMole
{
	public class SteerWithDirection : FaceToTarget
	{
		public float angle = 10f;

		public override Vector3 GetTargetFaceDir()
		{
			return Quaternion.Euler(0f, angle, 0f) * _aiEntity.FaceDirection;
		}
	}
}
