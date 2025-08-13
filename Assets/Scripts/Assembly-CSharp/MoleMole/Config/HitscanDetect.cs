namespace MoleMole.Config
{
	public class HitscanDetect : ConfigEntityAttackPattern
	{
		public float CenterYOffset;

		public float OffsetZ;

		public float MaxHitDistance;

		public HitscanDetect()
		{
			patternMethod = ComplexAttackPattern.HitscanDetectAttack;
		}
	}
}
