namespace MoleMole.Config
{
	public class CylinderCollisionDetect : ConfigEntityAttackPattern
	{
		public float CenterZOffset;

		public float Radius;

		public float Height;

		public CylinderCollisionDetect()
		{
			patternMethod = AttackPattern.CylinderCollisionDetectAttack;
		}
	}
}
