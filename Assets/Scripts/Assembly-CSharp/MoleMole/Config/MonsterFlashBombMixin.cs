namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterFlashBombMixin : ConfigAbilityMixin, IHashable
	{
		public float Angle = 120f;

		public float DelayTime;

		public MixinEffect TriggerEffect;

		public string[] ModifierNames;

		public ConfigAbilityAction[] SuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] FailActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Angle, ref lastHash);
			HashUtils.ContentHashOnto(DelayTime, ref lastHash);
			if (TriggerEffect != null)
			{
				HashUtils.ContentHashOnto(TriggerEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(TriggerEffect.AudioPattern, ref lastHash);
			}
			if (ModifierNames != null)
			{
				string[] modifierNames = ModifierNames;
				foreach (string value in modifierNames)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (SuccessActions != null)
			{
				ConfigAbilityAction[] successActions = SuccessActions;
				foreach (ConfigAbilityAction configAbilityAction in successActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (FailActions == null)
			{
				return;
			}
			ConfigAbilityAction[] failActions = FailActions;
			foreach (ConfigAbilityAction configAbilityAction2 in failActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterFlashBombMixin(instancedAbility, instancedModifier, this);
		}
	}
}
