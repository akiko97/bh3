namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillButtonHoldModeMixin : ConfigAbilityMixin, IHashable
	{
		public void ObjectContentHashOnto(ref int lastHash)
		{
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSkillButtonHoldModeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
