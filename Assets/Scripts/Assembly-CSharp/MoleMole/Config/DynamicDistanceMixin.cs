namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DynamicDistanceMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillID;

		public string TypeName;

		public float DefaultDistance;

		public float NoTargetDistance;

		public float MinDynamicDistanceDistance;

		public float MaxDynamicDistanceDistance = 20f;

		public float NormalizedTimeStart;

		public float NormalizedTimeStop = 1f;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			HashUtils.ContentHashOnto(TypeName, ref lastHash);
			HashUtils.ContentHashOnto(DefaultDistance, ref lastHash);
			HashUtils.ContentHashOnto(NoTargetDistance, ref lastHash);
			HashUtils.ContentHashOnto(MinDynamicDistanceDistance, ref lastHash);
			HashUtils.ContentHashOnto(MaxDynamicDistanceDistance, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStart, ref lastHash);
			HashUtils.ContentHashOnto(NormalizedTimeStop, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityDynamicDistanceMixin(instancedAbility, instancedModifier, this);
		}
	}
}
