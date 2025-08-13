namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AnimatorParamLerpMixin : ConfigAbilityMixin, IHashable
	{
		public string AnimatorParamName;

		public float LerpStartNormalizedTime;

		public float LerpEndNormalizedTime = 1f;

		public float LerpStartValue = -1f;

		public float LerpEndValue;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AnimatorParamName, ref lastHash);
			HashUtils.ContentHashOnto(LerpStartNormalizedTime, ref lastHash);
			HashUtils.ContentHashOnto(LerpEndNormalizedTime, ref lastHash);
			HashUtils.ContentHashOnto(LerpStartValue, ref lastHash);
			HashUtils.ContentHashOnto(LerpEndValue, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAnimatorParamLerpMixin(instancedAbility, instancedModifier, this);
		}
	}
}
