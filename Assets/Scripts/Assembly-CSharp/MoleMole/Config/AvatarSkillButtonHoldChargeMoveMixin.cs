namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillButtonHoldChargeMoveMixin : ConfigAbilityMixin, IHashable
	{
		public string[] ChargeLoopSkillIDs;

		public DynamicFloat MoveSpeed = DynamicFloat.ONE;

		public bool IsSteer;

		public float SteerSpeed = 1f;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (ChargeLoopSkillIDs != null)
			{
				string[] chargeLoopSkillIDs = ChargeLoopSkillIDs;
				foreach (string value in chargeLoopSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (MoveSpeed != null)
			{
				HashUtils.ContentHashOnto(MoveSpeed.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(MoveSpeed.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(MoveSpeed.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(IsSteer, ref lastHash);
			HashUtils.ContentHashOnto(SteerSpeed, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarSkillButtonChargeMoveMixin(instancedAbility, instancedModifier, this);
		}
	}
}
