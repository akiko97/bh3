using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityRangeAttackProtectShieldMixin : BaseAbilityMixin
	{
		private RangeAttackProtectShieldMixin config;

		private float _damageReduceRatio;

		private float _protectRange;

		public AbilityRangeAttackProtectShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (RangeAttackProtectShieldMixin)config;
			_damageReduceRatio = instancedAbility.Evaluate(this.config.DamageReduceRatio);
			_protectRange = instancedAbility.Evaluate(this.config.ProtectRange);
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
			if (baseAbilityActor != null)
			{
				float num = Vector3.Distance(baseAbilityActor.entity.transform.position, actor.entity.transform.position);
				if (num > _protectRange)
				{
					evt.attackData.damage *= 1f - _damageReduceRatio;
					actor.abilityPlugin.HandleActionTargetDispatch(config.OnRangeAttackProtectShieldSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
				}
			}
			return true;
		}
	}
}
