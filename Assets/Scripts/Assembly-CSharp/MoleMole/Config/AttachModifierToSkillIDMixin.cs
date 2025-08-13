namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachModifierToSkillIDMixin : ConfigAbilityMixin, IHashable
	{
		public string[] SkillIDs;

		public string ModifierName;

		public bool Inverse;

		public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

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
			HashUtils.ContentHashOnto(ModifierName, ref lastHash);
			HashUtils.ContentHashOnto(Inverse, ref lastHash);
			if (Predicates == null)
			{
				return;
			}
			ConfigAbilityPredicate[] predicates = Predicates;
			foreach (ConfigAbilityPredicate configAbilityPredicate in predicates)
			{
				if (configAbilityPredicate is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAttachModifierToSkillIDMixin(instancedAbility, instancedModifier, this);
		}
	}
}
