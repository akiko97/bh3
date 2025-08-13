using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityPositionDirectionHitMixin : BaseAbilityMixin
	{
		private PositionDirectionHitMixin config;

		private string[] _animEventIDs;

		private float _forwardAngleRangeMax;

		private float _forwardAngleRangeMin;

		private float _posAngleRangeMin;

		private float _posAngleRangeMax;

		private float _backHitRange = 1f;

		public AbilityPositionDirectionHitMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (PositionDirectionHitMixin)config;
			_forwardAngleRangeMax = instancedAbility.Evaluate(this.config.ForwardAngleRangeMax);
			_forwardAngleRangeMin = instancedAbility.Evaluate(this.config.ForwardAngleRangeMin);
			_posAngleRangeMin = instancedAbility.Evaluate(this.config.PosAngleRangeMin);
			_posAngleRangeMax = instancedAbility.Evaluate(this.config.PosAngleRangeMax);
			_backHitRange = instancedAbility.Evaluate(this.config.HitRange);
			_animEventIDs = this.config.AnimEventIDs;
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
			bool flag = false;
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if (_animEventIDs != null)
			{
				if (Miscs.ArrayContains(_animEventIDs, evt.animEventID))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				return true;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID);
			if (baseAbilityActor == null || !baseAbilityActor.IsActive())
			{
				return false;
			}
			Vector3 vector = actor.entity.transform.position - baseAbilityActor.entity.transform.position;
			float num = Vector3.Angle(actor.entity.transform.forward, baseAbilityActor.entity.transform.forward);
			float num2 = Vector3.Angle(vector, baseAbilityActor.entity.transform.forward);
			bool flag2 = num < _forwardAngleRangeMax && num > _forwardAngleRangeMin;
			bool flag3 = num2 < _posAngleRangeMax && num2 > _posAngleRangeMin;
			bool flag4 = vector.magnitude < _backHitRange;
			if (flag2 && flag3 && flag4)
			{
				evt.attackData.addedDamageRatio += instancedAbility.Evaluate(config.BackDamageRatio);
				actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, actor, evt);
			}
			return true;
		}
	}
}
