namespace MoleMole.Config
{
	public class CylinderCollisionDetectTargetLocked : ConfigEntityAttackPattern
	{
		public float Radius;

		public DynamicFloat RadiusRatio = DynamicFloat.ZERO;

		public float Height;

		public CylinderCollisionDetectTargetLocked()
		{
			patternMethod = AttackPattern.CylinderCollisionDetectTargetLockedAttack;
		}
	}
}
