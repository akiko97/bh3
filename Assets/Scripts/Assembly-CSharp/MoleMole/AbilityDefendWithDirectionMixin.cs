using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityDefendWithDirectionMixin : BaseAbilityMixin
	{
		private DefendWithDirectionMixin config;

		public AbilityDefendWithDirectionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DefendWithDirectionMixin)config;
		}

		public override void Core()
		{
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		protected virtual bool OnBeingHit(EvtBeingHit evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
			if (baseAbilityActor == null || baseAbilityActor.entity == null)
			{
				return false;
			}
			if (CheckSkillID(evt) && evt.attackData.attackerAniDamageRatio < config.BreakDefendAniDamageRatio)
			{
				if (CheckAngle(evt))
				{
					DefendSuccess(evt);
				}
				else
				{
					DefendFailure(evt);
				}
			}
			return true;
		}

		protected bool CheckSkillID(EvtBeingHit evt)
		{
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (actor.abilityState.ContainsState(AbilityState.Invincible) || actor.abilityState.ContainsState(AbilityState.Undamagable))
			{
				return false;
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.DefendPredicates, instancedAbility, instancedModifier, actor, evt))
			{
				return false;
			}
			if (config.AlwaysDefend)
			{
				return true;
			}
			string currentSkillID = actor.entity.CurrentSkillID;
			if (!string.IsNullOrEmpty(currentSkillID))
			{
				bool flag = false;
				for (int i = 0; i < config.DefendSkillIDs.Length; i++)
				{
					if (config.DefendSkillIDs[i] == currentSkillID)
					{
						flag = true;
						break;
					}
				}
				float num = entity.GetCurrentNormalizedTime() % 1f;
				if (flag && num > config.DefendNormalizedTimeStart && num < config.DefendNormalizedTimeStop)
				{
					return true;
				}
			}
			return false;
		}

		protected bool CheckAngle(EvtBeingHit evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
			float num = Vector3.Angle(actor.entity.transform.forward, baseAbilityActor.entity.transform.position - actor.entity.transform.position);
			bool flag = num < config.DefendAngle;
			if (config.ReverseAngle)
			{
				flag = !flag;
			}
			return flag;
		}

		protected void DefendSuccess(EvtBeingHit evt)
		{
			if (config.DefendDamageReduce >= 1f)
			{
				if (!config.DefendElemental && evt.attackData.GetElementalDamage() > 0f)
				{
					evt.attackData.damage = 0f;
					evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				}
				else
				{
					evt.attackData.Reject(AttackResult.RejectType.RejectButShowAttackEffect);
				}
			}
			else
			{
				evt.attackData.hitEffect = config.DefendSuccessHitEffect;
				evt.attackData.damage *= 1f - config.DefendDamageReduce;
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.DefendSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
		}

		protected void DefendFailure(EvtBeingHit evt)
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.DefendFailActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
		}
	}
}
