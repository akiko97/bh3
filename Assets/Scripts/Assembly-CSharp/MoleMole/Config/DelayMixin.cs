namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DelayMixin : ConfigAbilityMixin, IHashable
	{
		public ConfigAbilityAction[] OnTimeUp = ConfigAbilityAction.EMPTY;

		public DynamicFloat Delay = new DynamicFloat
		{
			fixedValue = 60f
		};

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (OnTimeUp != null)
			{
				ConfigAbilityAction[] onTimeUp = OnTimeUp;
				foreach (ConfigAbilityAction configAbilityAction in onTimeUp)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (Delay != null)
			{
				HashUtils.ContentHashOnto(Delay.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Delay.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Delay.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDelayMixin(instancedAbility, instancedModifier, this);
		}
	}
}
