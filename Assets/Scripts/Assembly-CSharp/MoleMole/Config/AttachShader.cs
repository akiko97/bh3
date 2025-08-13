namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AttachShader : ConfigAbilityAction, IHashable
	{
		public E_ShaderData ShaderType;

		public bool UseNewTexture;

		public bool UsePrefabEnableDurtion;

		public DynamicFloat Duration;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)ShaderType, ref lastHash);
			HashUtils.ContentHashOnto(UseNewTexture, ref lastHash);
			HashUtils.ContentHashOnto(UsePrefabEnableDurtion, ref lastHash);
			if (Duration != null)
			{
				HashUtils.ContentHashOnto(Duration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Duration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Duration.dynamicKey, ref lastHash);
			}
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
			abilityPlugin.AttachShaderHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}
	}
}
