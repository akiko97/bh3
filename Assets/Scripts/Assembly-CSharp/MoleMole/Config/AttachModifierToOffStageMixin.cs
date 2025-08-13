namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToOffStageMixin : ConfigAbilityMixin, IHashable
	{
		public string ModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToOffStageMixin(instancedAbility, instancedModifier, this);
		}
	}
}
