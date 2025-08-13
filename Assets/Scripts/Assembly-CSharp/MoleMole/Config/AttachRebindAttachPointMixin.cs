namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachRebindAttachPointMixin : ConfigAbilityMixin, IHashable
	{
		public string PointName;

		public string OriginName;

		public string OtherName;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(PointName, ref lastHash);
			HashUtils.ContentHashOnto(OriginName, ref lastHash);
			HashUtils.ContentHashOnto(OtherName, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachRebindAttachPointMixin(instancedAbility, instancedModifier, this);
		}
	}
}
