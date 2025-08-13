namespace MoleMole.Config
{
	public class RectCollisionWithHeightDetect : RectCollisionDetect
	{
		public float Height;

		public RectCollisionWithHeightDetect()
		{
			patternMethod = ComplexAttackPattern.RectCollisionWithHeightDetectAttack;
		}
	}
}
