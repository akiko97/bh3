namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MirrorMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat MirrorLastingTime;

		public bool ApplyAttackerWitchTimeRatio;

		public DynamicInt MirrorAmount;

		public float AheadTime;

		public float DelayTime;

		public float PerMirrorDelayTime;

		public DynamicFloat HPRatioOfParent = DynamicFloat.ONE;

		public string MirrorAIName;

		public string[] MirrorAbilities;

		public string MirrorAbilitiesOverrideName;

		public int RemoveMirrorAfterSkillCount;

		public string[] SelfModifiers = Miscs.EMPTY_STRINGS;

		public ConfigAbilityAction[] MirrorCreateActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] MirrorDestroyActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] MirrorAheadDestroyActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (MirrorLastingTime != null)
			{
				HashUtils.ContentHashOnto(MirrorLastingTime.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MirrorLastingTime.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MirrorLastingTime.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(ApplyAttackerWitchTimeRatio, ref lastHash);
			if (MirrorAmount != null)
			{
				HashUtils.ContentHashOnto(MirrorAmount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MirrorAmount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MirrorAmount.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(AheadTime, ref lastHash);
			HashUtils.ContentHashOnto(DelayTime, ref lastHash);
			HashUtils.ContentHashOnto(PerMirrorDelayTime, ref lastHash);
			if (HPRatioOfParent != null)
			{
				HashUtils.ContentHashOnto(HPRatioOfParent.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HPRatioOfParent.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HPRatioOfParent.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(MirrorAIName, ref lastHash);
			if (MirrorAbilities != null)
			{
				string[] mirrorAbilities = MirrorAbilities;
				foreach (string value in mirrorAbilities)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(MirrorAbilitiesOverrideName, ref lastHash);
			HashUtils.ContentHashOnto(RemoveMirrorAfterSkillCount, ref lastHash);
			if (SelfModifiers != null)
			{
				string[] selfModifiers = SelfModifiers;
				foreach (string value2 in selfModifiers)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			if (MirrorCreateActions != null)
			{
				ConfigAbilityAction[] mirrorCreateActions = MirrorCreateActions;
				foreach (ConfigAbilityAction configAbilityAction in mirrorCreateActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (MirrorDestroyActions != null)
			{
				ConfigAbilityAction[] mirrorDestroyActions = MirrorDestroyActions;
				foreach (ConfigAbilityAction configAbilityAction2 in mirrorDestroyActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			if (MirrorAheadDestroyActions == null)
			{
				return;
			}
			ConfigAbilityAction[] mirrorAheadDestroyActions = MirrorAheadDestroyActions;
			foreach (ConfigAbilityAction configAbilityAction3 in mirrorAheadDestroyActions)
			{
				if (configAbilityAction3 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[3][] { MirrorCreateActions, MirrorDestroyActions, MirrorAheadDestroyActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMirrorMixin(instancedAbility, instancedModifier, this);
		}
	}
}
