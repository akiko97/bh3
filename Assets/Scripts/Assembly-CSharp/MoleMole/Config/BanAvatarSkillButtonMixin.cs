namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BanAvatarSkillButtonMixin : ConfigAbilityMixin, IHashable
	{
		public string SkillID;

		public string ReplaceButtonIconPath;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(SkillID, ref lastHash);
			HashUtils.ContentHashOnto(ReplaceButtonIconPath, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityBanAvatarSkillButtonMixin(instancedAbility, instancedModifier, this);
		}
	}
}
