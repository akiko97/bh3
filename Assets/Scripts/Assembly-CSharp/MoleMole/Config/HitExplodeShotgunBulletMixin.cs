namespace MoleMole.Config
{
	public class HitExplodeShotgunBulletMixin : HitExplodeBulletMixin
	{
		public int BulletNum = 1;

		public int MaxHitNum = 5;

		public float ScatterAngleMinX = 10f;

		public float ScatterAngleMaxX = 10f;

		public float ScatterAngleMinY = 10f;

		public float ScatterAngleMaxY = 10f;

		public float ScatterDistanceMin = 2f;

		public float ScatterDistanceMax = 8f;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityHitExplodeShotgunBulletMixin(instancedAbility, instancedModifier, this);
		}
	}
}
