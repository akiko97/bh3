namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarTiedMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat UntieSteerAmount;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (UntieSteerAmount != null)
			{
				HashUtils.ContentHashOnto(UntieSteerAmount.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(UntieSteerAmount.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(UntieSteerAmount.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarTiedMixin(instancedAbility, instancedModifier, this);
		}
	}
}
