namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BrokenEnemyDraggedByHitBoxMixin : ConfigAbilityMixin, IHashable
	{
		public string ColliderEntryName;

		public DynamicFloat PullVelocity;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ColliderEntryName, ref lastHash);
			if (PullVelocity != null)
			{
				HashUtils.ContentHashOnto(PullVelocity.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PullVelocity.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityBrokenEnemyDraggedByHitBoxMixin(instancedAbility, instancedModifier, this);
		}
	}
}
