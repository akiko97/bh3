namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ApplyLevelBuff : ConfigAbilityAction, IHashable, IOnLoaded
	{
		public LevelBuffType LevelBuff;

		public LevelBuffSpecial LevelBuffSpecial;

		public bool LevelBuffAllowRefresh;

		public bool EnteringTimeSlow = true;

		public DynamicFloat Duration = DynamicFloat.ONE;

		public AttachModifier[] AttachModifiers = new AttachModifier[0];

		public string AttachLevelEffectPattern;

		public bool UseOverrideCurSide;

		public LevelBuffSide OverrideCurSide;

		public bool NotStartEffect;

		public ApplyLevelBuff()
		{
			Target = AbilityTargetting.Other;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto((int)LevelBuff, ref lastHash);
			HashUtils.ContentHashOnto((int)LevelBuffSpecial, ref lastHash);
			HashUtils.ContentHashOnto(LevelBuffAllowRefresh, ref lastHash);
			HashUtils.ContentHashOnto(EnteringTimeSlow, ref lastHash);
			if (Duration != null)
			{
				HashUtils.ContentHashOnto(Duration.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(Duration.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(Duration.dynamicKey, ref lastHash);
			}
			if (AttachModifiers != null)
			{
				AttachModifier[] attachModifiers = AttachModifiers;
				foreach (AttachModifier attachModifier in attachModifiers)
				{
					if (attachModifier is IHashable)
					{
						HashUtils.ContentHashOnto(attachModifier, ref lastHash);
					}
				}
			}
			HashUtils.ContentHashOnto(AttachLevelEffectPattern, ref lastHash);
			HashUtils.ContentHashOnto(UseOverrideCurSide, ref lastHash);
			HashUtils.ContentHashOnto((int)OverrideCurSide, ref lastHash);
			HashUtils.ContentHashOnto(NotStartEffect, ref lastHash);
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

		public override void OnLoaded()
		{
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			ConfigAbilityAction[] array = new ConfigAbilityAction[AttachModifiers.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = AttachModifiers[i];
			}
			return new ConfigAbilityAction[1][] { array };
		}

		public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			abilityPlugin.ApplyLevelBuffHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
		{
			output = string.Format("{0} 触发LevelBuff {1}, 持续时间 {2}, 附带挂 Modifier 数量 {3}", Miscs.GetDebugActorName(instancedAbility.caster), LevelBuff, instancedAbility.Evaluate(Duration), AttachModifiers.Length);
			return true;
		}

		public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.ApplyLevelBuff_AuthorityHandler;
		}

		public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
		{
			return mpAbilityPlugin.ApplyLevelBuff_RemoteHandler;
		}
	}
}
