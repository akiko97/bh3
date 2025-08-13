namespace MoleMole.Config
{
	public class FanCollisionDetect : ConfigEntityAttackPattern
	{
		public float CenterYOffset;

		public float OffsetZ;

		public float FanAngle;

		public float Radius;

		public DynamicFloat RadiusRatio = DynamicFloat.ZERO;

		public float MeleeRadius;

		public float MeleeFanAngle;

		public bool FollowRootNodeY;

		public FanCollisionDetect()
		{
			patternMethod = AttackPattern.FanCollisionDetectAttack;
		}
	}
}
