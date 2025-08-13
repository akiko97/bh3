namespace MoleMole.Config
{
	public class MonsterDashMixin : ConfigAbilityMixin
	{
		public string[] SkillIDs;

		public float DashTime = 0.1f;

		public float TargetDistance = 2.5f;

		public float MinDistance;

		public float MaxDistance = 100f;

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterDashMixin(instancedAbility, instancedModifier, this);
		}
	}
}
