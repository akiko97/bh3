namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachAdditiveVelocityMixin : ConfigAbilityMixin, IHashable
	{
		public float MoveSpeed;

		public float MoveAngle;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(MoveSpeed, ref lastHash);
			HashUtils.ContentHashOnto(MoveAngle, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachAdditiveVelocityMixin(instancedAbility, instancedModifier, this);
		}
	}
}
