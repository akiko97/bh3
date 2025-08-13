namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterForceFollowMixin : ConfigAbilityMixin, IHashable
	{
		public string[] SkillIDs;

		public float NormalizeTimeEnd = 1f;

		public float TargetDistance = 1.5f;

		public float MinDistance;

		public float MaxDistance = 100f;

		public float FollowSpeed = 10f;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (SkillIDs != null)
			{
				string[] skillIDs = SkillIDs;
				foreach (string value in skillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(NormalizeTimeEnd, ref lastHash);
			HashUtils.ContentHashOnto(TargetDistance, ref lastHash);
			HashUtils.ContentHashOnto(MinDistance, ref lastHash);
			HashUtils.ContentHashOnto(MaxDistance, ref lastHash);
			HashUtils.ContentHashOnto(FollowSpeed, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterForceFollowMixin(instancedAbility, instancedModifier, this);
		}
	}
}
