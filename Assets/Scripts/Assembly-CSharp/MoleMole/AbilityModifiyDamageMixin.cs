using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityModifiyDamageMixin : BaseAbilityMixin
	{
		private ModifyDamageMixin config;

		public AbilityModifiyDamageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			return false;
		}

		private bool OnHittingOther(EvtHittingOther evt)
		{
			if (!config.IncludeNonAnimEventAttacks && !evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
			{
				return false;
			}
			if (!(Random.value < instancedAbility.Evaluate(config.ModifyChance)))
			{
				return false;
			}
			ModifyDamage(evt, 1f);
			actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
			evt.attackData.attackerAniDamageRatio += instancedAbility.Evaluate(config.AniDamageRatio);
			actor.abilityPlugin.HandleActionTargetDispatch(config.BreakActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
			return true;
		}

		protected virtual void ModifyDamage(EvtHittingOther evt, float multiple = 1f)
		{
			evt.attackData.attackerCritChance += instancedAbility.Evaluate(config.CritChanceDelta) * multiple;
			evt.attackData.attackerCritDamageRatio += instancedAbility.Evaluate(config.CritDamageRatioDelta) * multiple;
			evt.attackData.attackerAniDamageRatio += instancedAbility.Evaluate(config.AnimDamageRatioDelta) * multiple;
			evt.attackData.addedAttackRatio += instancedAbility.Evaluate(config.AddedAttackRatio) * multiple;
			evt.attackData.addedDamageRatio += instancedAbility.Evaluate(config.AddedDamageRatio) * multiple;
			evt.attackData.attackerAddedAttackValue += instancedAbility.Evaluate(config.AddedDamageValue) * multiple;
			evt.attackData.attackerAttackPercentage += instancedAbility.Evaluate(config.AddedDamagePercentage) * multiple;
			evt.attackData.attackerNormalDamage += instancedAbility.Evaluate(config.NormalDamage) * multiple;
			evt.attackData.attackerFireDamage += instancedAbility.Evaluate(config.FireDamage) * multiple;
			evt.attackData.attackerThunderDamage += instancedAbility.Evaluate(config.ThunderDamage) * multiple;
			evt.attackData.attackerIceDamage += instancedAbility.Evaluate(config.IceDamage) * multiple;
			evt.attackData.attackerAlienDamage += instancedAbility.Evaluate(config.AllienDamage) * multiple;
			evt.attackData.attackerNormalDamagePercentage += instancedAbility.Evaluate(config.NormalDamagePercentage) * multiple;
			evt.attackData.attackerFireDamagePercentage += instancedAbility.Evaluate(config.FireDamagePercentage) * multiple;
			evt.attackData.attackerThunderDamagePercentage += instancedAbility.Evaluate(config.ThunderDamagePercentage) * multiple;
			evt.attackData.attackerIceDamagePercentage += instancedAbility.Evaluate(config.IceDamagePercentage) * multiple;
			evt.attackData.attackerAlienDamagePercentage += instancedAbility.Evaluate(config.AllienDamagePercentage) * multiple;
			evt.attackData.attackerAddedAllDamageReduceRatio += instancedAbility.Evaluate(config.AllDamageReduceRatio) * multiple;
		}

		private void CheckConfig()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < config.Predicates.Length; i++)
			{
				ConfigAbilityPredicate configAbilityPredicate = config.Predicates[i];
				if (configAbilityPredicate is ByAttackAnimEventID)
				{
					flag = true;
				}
				if (configAbilityPredicate is ByAttackInComboCount)
				{
					flag2 = true;
				}
			}
			if (!flag && !flag2)
			{
				if (instancedAbility.Evaluate(config.AddedDamageValue) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.AddedDamagePercentage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.NormalDamage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.FireDamage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.ThunderDamage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.IceDamage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.AllienDamage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.NormalDamagePercentage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.FireDamagePercentage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.ThunderDamagePercentage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.IceDamagePercentage) > 0f)
				{
					flag3 = true;
				}
				if (instancedAbility.Evaluate(config.AllienDamagePercentage) > 0f)
				{
					flag3 = true;
				}
			}
		}
	}
}
