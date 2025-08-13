namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToLevelBuffMixin : ConfigAbilityMixin, IHashable
	{
		public LevelBuffType LevelBuff;

		public string OnModifierName;

		public string OffModifierName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)LevelBuff, ref lastHash);
			HashUtils.ContentHashOnto(OnModifierName, ref lastHash);
			HashUtils.ContentHashOnto(OffModifierName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToLevelBuffMixin(instancedAbility, instancedModifier, this);
		}
	}
}
