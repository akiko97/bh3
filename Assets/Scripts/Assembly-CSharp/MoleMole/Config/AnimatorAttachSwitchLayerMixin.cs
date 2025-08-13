namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AnimatorAttachSwitchLayerMixin : ConfigAbilityMixin, IHashable
	{
		public int FromLayer;

		public int ToLayer;

		public float Duration = 0.3f;

		public bool NoEndResume;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(FromLayer, ref lastHash);
			HashUtils.ContentHashOnto(ToLayer, ref lastHash);
			HashUtils.ContentHashOnto(Duration, ref lastHash);
			HashUtils.ContentHashOnto(NoEndResume, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAnimatorAttachSwitchLayerMixin(instancedAbility, instancedModifier, this);
		}
	}
}
