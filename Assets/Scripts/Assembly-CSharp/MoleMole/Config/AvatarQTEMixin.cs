namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarQTEMixin : ConfigAbilityMixin, IHashable
	{
		public string[] SkillIDs;

		public string ModifierName;

		public string QTEName;

		public DynamicFloat QTEMaxTimeSpan = DynamicFloat.ONE;

		public DynamicFloat DelayQTETimeSpan = new DynamicFloat
		{
			fixedValue = 0.2f
		};

		public QTECondition[] TriggerConditions = QTECondition.EMPTY;

		public QTECondition[] Conditions = QTECondition.EMPTY;

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
			HashUtils.ContentHashOnto(QTEName, ref lastHash);
			if (QTEMaxTimeSpan != null)
			{
				HashUtils.ContentHashOnto(QTEMaxTimeSpan.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(QTEMaxTimeSpan.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(QTEMaxTimeSpan.dynamicKey, ref lastHash);
			}
			if (DelayQTETimeSpan != null)
			{
				HashUtils.ContentHashOnto(DelayQTETimeSpan.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(DelayQTETimeSpan.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(DelayQTETimeSpan.dynamicKey, ref lastHash);
			}
			if (TriggerConditions != null)
			{
				QTECondition[] triggerConditions = TriggerConditions;
				foreach (QTECondition qTECondition in triggerConditions)
				{
					HashUtils.ContentHashOnto(qTECondition.QTERange, ref lastHash);
					HashUtils.ContentHashOnto((int)qTECondition.QTEType, ref lastHash);
					if (qTECondition.QTEValues != null)
					{
						string[] qTEValues = qTECondition.QTEValues;
						foreach (string value2 in qTEValues)
						{
							HashUtils.ContentHashOnto(value2, ref lastHash);
						}
					}
				}
			}
			if (Conditions != null)
			{
				QTECondition[] conditions = Conditions;
				foreach (QTECondition qTECondition2 in conditions)
				{
					HashUtils.ContentHashOnto(qTECondition2.QTERange, ref lastHash);
					HashUtils.ContentHashOnto((int)qTECondition2.QTEType, ref lastHash);
					if (qTECondition2.QTEValues != null)
					{
						string[] qTEValues2 = qTECondition2.QTEValues;
						foreach (string value3 in qTEValues2)
						{
							HashUtils.ContentHashOnto(value3, ref lastHash);
						}
					}
				}
			}
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
			return new AbilityAvatarQTEMixin(instancedAbility, instancedModifier, this);
		}
	}
}
