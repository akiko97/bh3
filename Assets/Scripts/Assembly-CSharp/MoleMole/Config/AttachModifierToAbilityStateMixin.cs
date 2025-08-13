namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToAbilityStateMixin : ConfigAbilityMixin, IHashable
	{
		public AbilityState[] AbilityStates = AbilityData.EMPTY;

		public string OnModifierName;

		public string OffModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AbilityStates != null)
			{
				AbilityState[] abilityStates = AbilityStates;
				foreach (AbilityState value in abilityStates)
				{
					HashUtils.ContentHashOnto((int)value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(OnModifierName, ref lastHash);
			HashUtils.ContentHashOnto(OffModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToAbilityStateMixin(instancedAbility, instancedModifier, this);
		}
	}
}
