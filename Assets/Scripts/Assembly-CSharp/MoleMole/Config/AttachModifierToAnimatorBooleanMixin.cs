namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToAnimatorBooleanMixin : ConfigAbilityMixin, IHashable
	{
		public string AnimatorBoolean;

		public string ModifierName;

		public bool IsInvert;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AnimatorBoolean, ref lastHash);
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
			HashUtils.ContentHashOnto(IsInvert, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToAnimatorBooleanMixin(instancedAbility, instancedModifier, this);
		}
	}
}
