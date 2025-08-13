namespace MoleMole.Config
{
	public class FanCollisionWithHeightDetect : FanCollisionDetect
	{
		public float Height;

		public FanCollisionWithHeightDetect()
		{
			patternMethod = ComplexAttackPattern.FanCollisionWithHeightDetectAttack;
		}
	}
}
