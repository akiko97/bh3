namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarAutoUltraModeMixin : ConfigAbilityMixin, IHashable
	{
		public float AutoUltraSPRatio;

		public float EndUltarSPRatio;

		public float CostSPSpeed;

		public ConfigAbilityAction[] BeginActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] EndActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AutoUltraSPRatio, ref lastHash);
			HashUtils.ContentHashOnto(EndUltarSPRatio, ref lastHash);
			HashUtils.ContentHashOnto(CostSPSpeed, ref lastHash);
			if (BeginActions != null)
			{
				ConfigAbilityAction[] beginActions = BeginActions;
				foreach (ConfigAbilityAction configAbilityAction in beginActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (EndActions == null)
			{
				return;
			}
			ConfigAbilityAction[] endActions = EndActions;
			foreach (ConfigAbilityAction configAbilityAction2 in endActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarAutoUltraModeMixin(instancedAbility, instancedModifier, this);
		}
	}
}
