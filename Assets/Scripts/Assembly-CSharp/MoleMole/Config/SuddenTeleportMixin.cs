namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class SuddenTeleportMixin : ConfigAbilityMixin, IHashable
	{
		public TeleportDirectionMode DirectionMode = TeleportDirectionMode.FromTarget;

		public DynamicFloat Angle = DynamicFloat.ZERO;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)DirectionMode, ref lastHash);
			if (Angle != null)
			{
				HashUtils.ContentHashOnto(Angle.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Angle.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Angle.dynamicKey, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilitySuddenTeleportMixin(instancedAbility, instancedModifier, this);
		}
	}
}
