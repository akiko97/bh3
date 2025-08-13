namespace MoleMole.Config
{
	public class RectCollisionDetect : ConfigEntityAttackPattern
	{
		public float CenterYOffset;

		public float OffsetZ;

		public float Width;

		public float Distance;

		public RectCollisionDetect()
		{
			patternMethod = AttackPattern.RectCollisionDetectAttack;
		}
	}
}
