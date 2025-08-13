namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class OnStartSwitchModifierMixin : ConfigAbilityMixin, IHashable
	{
		public string OnModifierName;

		public string OffModifierName;

		public bool UseLowSPForceOff;

		public bool UseLowHPForceOff = true;

		public bool AlwaysSwitchOn;

		public float MaxDuration = -1f;

		public string SkillButtonID;

		public string OnModifierReplaceIconPath;

		public int OnModifierReplaceCostSP;

		public bool OnModifierSwitchToInstantTrigger;

		public string OnModifierInstantTriggerEvent;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(OnModifierName, ref lastHash);
			HashUtils.ContentHashOnto(OffModifierName, ref lastHash);
			HashUtils.ContentHashOnto(UseLowSPForceOff, ref lastHash);
			HashUtils.ContentHashOnto(UseLowHPForceOff, ref lastHash);
			HashUtils.ContentHashOnto(AlwaysSwitchOn, ref lastHash);
			HashUtils.ContentHashOnto(MaxDuration, ref lastHash);
			HashUtils.ContentHashOnto(SkillButtonID, ref lastHash);
			HashUtils.ContentHashOnto(OnModifierReplaceIconPath, ref lastHash);
			HashUtils.ContentHashOnto(OnModifierReplaceCostSP, ref lastHash);
			HashUtils.ContentHashOnto(OnModifierSwitchToInstantTrigger, ref lastHash);
			HashUtils.ContentHashOnto(OnModifierInstantTriggerEvent, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityStartSwitchModifierMixin(instancedAbility, instancedModifier, this);
		}
	}
}
