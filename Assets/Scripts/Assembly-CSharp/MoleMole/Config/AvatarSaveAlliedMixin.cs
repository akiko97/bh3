namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSaveAlliedMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicInt SaveCountLimit = DynamicInt.ONE;

		public ConfigAbilityAction[] AdditionalActions = ConfigAbilityAction.EMPTY;

		public AvatarSaveAlliedMixin()
		{
			isUnique = true;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (SaveCountLimit != null)
			{
				HashUtils.ContentHashOnto(SaveCountLimit.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(SaveCountLimit.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(SaveCountLimit.dynamicKey, ref lastHash);
			}
			if (AdditionalActions == null)
			{
				return;
			}
			ConfigAbilityAction[] additionalActions = AdditionalActions;
			foreach (ConfigAbilityAction configAbilityAction in additionalActions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSaveAlliedMixin(instancedAbility, instancedModifier, this);
		}
	}
}
