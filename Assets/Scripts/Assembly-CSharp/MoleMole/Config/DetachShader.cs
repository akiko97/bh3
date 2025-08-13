namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class DetachShader : ConfigAbilityAction, IHashable
	{
		public E_ShaderData ShaderType;

		public bool UsePrefabDisableDurtion;

		public float Duration;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)ShaderType, ref lastHash);
			HashUtils.ContentHashOnto(UsePrefabDisableDurtion, ref lastHash);
			HashUtils.ContentHashOnto(Duration, ref lastHash);
			HashUtils.ContentHashOnto((int)Target, ref lastHash);
			if (TargetOption != null && TargetOption.Range != null)
			{
				HashUtils.ContentHashOnto(TargetOption.Range.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(TargetOption.Range.dynamicKey, ref lastHash);
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

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.DetachShaderHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
