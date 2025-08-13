namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BlockMixin : ConfigAbilityMixin, IHashable
	{
		public DynamicFloat DamageReduce = DynamicFloat.ZERO;

		public DynamicFloat BlockChance = DynamicFloat.ZERO;

		public DynamicFloat DamageReduceRatio = DynamicFloat.ZERO;

		public DynamicFloat BlockTimer = DynamicFloat.ZERO;

		public ConfigAbilityAction[] BlockActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityPredicate[] TargetPredicates = ConfigAbilityPredicate.EMPTY;

		public ConfigAbilityPredicate[] AttackerPredicates = ConfigAbilityPredicate.EMPTY;

		public string[] BlockSkillIDs;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (DamageReduce != null)
			{
				HashUtils.ContentHashOnto(DamageReduce.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduce.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduce.dynamicKey, ref lastHash);
			}
			if (BlockChance != null)
			{
				HashUtils.ContentHashOnto(BlockChance.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BlockChance.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BlockChance.dynamicKey, ref lastHash);
			}
			if (DamageReduceRatio != null)
			{
				HashUtils.ContentHashOnto(DamageReduceRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduceRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DamageReduceRatio.dynamicKey, ref lastHash);
			}
			if (BlockTimer != null)
			{
				HashUtils.ContentHashOnto(BlockTimer.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BlockTimer.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BlockTimer.dynamicKey, ref lastHash);
			}
			if (BlockActions != null)
			{
				ConfigAbilityAction[] blockActions = BlockActions;
				foreach (ConfigAbilityAction configAbilityAction in blockActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (TargetPredicates != null)
			{
				ConfigAbilityPredicate[] targetPredicates = TargetPredicates;
				foreach (ConfigAbilityPredicate configAbilityPredicate in targetPredicates)
				{
					if (configAbilityPredicate is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityPredicate, ref lastHash);
					}
				}
			}
			if (AttackerPredicates != null)
			{
				ConfigAbilityPredicate[] attackerPredicates = AttackerPredicates;
				foreach (ConfigAbilityPredicate configAbilityPredicate2 in attackerPredicates)
				{
					if (configAbilityPredicate2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityPredicate2, ref lastHash);
					}
				}
			}
			if (BlockSkillIDs != null)
			{
				string[] blockSkillIDs = BlockSkillIDs;
				foreach (string value in blockSkillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[1][] { BlockActions };
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityBlockMixin(instancedAbility, instancedModifier, this);
		}
	}
}
