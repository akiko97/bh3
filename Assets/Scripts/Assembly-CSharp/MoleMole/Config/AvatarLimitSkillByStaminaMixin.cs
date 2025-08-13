namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarLimitSkillByStaminaMixin : ConfigAbilityMixin, IHashable
	{
		public float StaminaMax;

		public float ResumeSpeed;

		public string SkillID;

		public string MaskTriggerID;

		public float SkillHeatCost;

		public bool ShowStaminaBar;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(StaminaMax, ref lastHash);
			HashUtils.ContentHashOnto(ResumeSpeed, ref lastHash);
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			HashUtils.ContentHashOnto(MaskTriggerID, ref lastHash);
			HashUtils.ContentHashOnto(SkillHeatCost, ref lastHash);
			HashUtils.ContentHashOnto(ShowStaminaBar, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarLimitSkillByStaminaMixin(instancedAbility, instancedModifier, this);
		}
	}
}
