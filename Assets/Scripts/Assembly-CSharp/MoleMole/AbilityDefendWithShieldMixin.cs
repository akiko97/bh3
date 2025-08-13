using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityDefendWithShieldMixin : AbilityDefendWithDirectionMixin
	{
		private DefendWithShieldMixin config;

		public float maxShield;

		public float shield;

		public AbilityDefendWithShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DefendWithShieldMixin)config;
		}

		public override void OnAdded()
		{
			shield = (maxShield = (float)actor.baseMaxHP * instancedAbility.Evaluate(config.ShieldHPRatio));
			(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.ShieldRatioAnimatorParam, shield / maxShield);
		}

		public override void Core()
		{
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			shield = maxShield;
			(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.ShieldRatioAnimatorParam, shield / maxShield);
		}

		protected override bool OnBeingHit(EvtBeingHit evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
			if (baseAbilityActor == null || baseAbilityActor.entity == null)
			{
				return false;
			}
			float num = ((!config.DefendElemental) ? evt.attackData.damage : evt.attackData.GetTotalDamage());
			float num2 = num * Mathf.Pow(evt.attackData.attackerAniDamageRatio, config.ShieldAniDamageRatioPow);
			if (CheckSkillID(evt))
			{
				if (CheckAngle(evt))
				{
					shield -= num2;
					if (shield <= 0f)
					{
						actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldBrokenActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
						shield = maxShield;
					}
					else
					{
						DefendSuccess(evt);
					}
					(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.ShieldRatioAnimatorParam, shield / maxShield);
				}
				else
				{
					DefendFailure(evt);
				}
			}
			return true;
		}
	}
}
