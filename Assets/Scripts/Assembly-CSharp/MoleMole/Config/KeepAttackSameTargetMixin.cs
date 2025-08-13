namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class KeepAttackSameTargetMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat TargetFadeWindow;

		public ConfigAbilityAction[] OnAttackSameTarget = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnTargetFadeOrChanged = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (TargetFadeWindow != null)
			{
				HashUtils.ContentHashOnto(TargetFadeWindow.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TargetFadeWindow.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TargetFadeWindow.dynamicKey, ref lastHash);
			}
			if (OnAttackSameTarget != null)
			{
				ConfigAbilityAction[] onAttackSameTarget = OnAttackSameTarget;
				foreach (ConfigAbilityAction configAbilityAction in onAttackSameTarget)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (OnTargetFadeOrChanged == null)
			{
				return;
			}
			ConfigAbilityAction[] onTargetFadeOrChanged = OnTargetFadeOrChanged;
			foreach (ConfigAbilityAction configAbilityAction2 in onTargetFadeOrChanged)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityKeepAttackSameTargetMixin(instancedAbility, instancedModifier, this);
		}
	}
}
