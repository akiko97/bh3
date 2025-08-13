namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarShowSkillButtonMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillButtonID;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillButtonID, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarShowSkillButtonMixin(instancedAbility, instancedModifier, this);
		}

		public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			BaseAbilityActor baseAbilityActor = ((instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner);
			BaseMPIdentity identity = Singleton<MPManager>.Instance.GetIdentity<BaseMPIdentity>(baseAbilityActor.runtimeID);
			if (identity.isAuthority)
			{
				return new AbilityAvatarShowSkillButtonMixin(instancedAbility, instancedModifier, this);
			}
			return new MPAbilityStubMixin(instancedAbility, instancedModifier, this);
		}
	}
}
