namespace MoleMole.Config
{
	public class StaticHitBoxDetect : RectCollisionDetect
	{
		public enum HitBoxResetType
		{
			None = 0,
			WithInside = 1,
			WithoutInside = 2
		}

		public bool Enable = true;

		public HitBoxResetType ResetType;

		public float SizeRatio = 1f;

		public float LengthRatio = 1f;

		public bool UseOwnerCenterForRetreatDirection;

		public StaticHitBoxDetect()
		{
			patternMethod = ComplexAttackPattern.StaticHitBoxDetectAttack;
		}
	}
}
