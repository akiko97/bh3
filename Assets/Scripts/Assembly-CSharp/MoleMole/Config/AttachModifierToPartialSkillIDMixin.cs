namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToPartialSkillIDMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillID;

		public float NormalizedTimeStart;

		public float NormalizedTimeStop;

		public string ModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStart, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStop, ref lastHash);
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToPartialSkillIDMixin(instancedAbility, instancedModifier, this);
		}
	}
}
