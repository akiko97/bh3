using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityPullTargetMixin : BaseAbilityMixin
	{
		private PullTargetMixin config;

		private BaseAbilityActor _pullActor;

		protected float _pullVelocity;

		public AbilityPullTargetMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (PullTargetMixin)config;
			_pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
			_pullActor = null;
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
		}

		public override void Core()
		{
			if (_pullActor != null)
			{
				Vector3 vector = entity.transform.position - _pullActor.entity.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude < instancedAbility.Evaluate(config.StopDistance))
				{
					_pullActor.entity.SetAdditiveVelocity(Vector3.zero);
					_pullActor.entity.SetHasAdditiveVelocity(false);
					_pullActor = null;
				}
				else
				{
					vector.Normalize();
					_pullActor.entity.SetAdditiveVelocity(vector * instancedAbility.Evaluate(config.PullVelocity));
				}
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			return false;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			BaseMonoEntity attackTarget = actor.entity.GetAttackTarget();
			if (attackTarget != null)
			{
				_pullActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(attackTarget.GetRuntimeID());
			}
			if (_pullActor != null)
			{
				_pullActor.entity.SetHasAdditiveVelocity(true);
				Vector3 vector = entity.transform.position - attackTarget.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude < instancedAbility.Evaluate(config.PullRadius))
				{
					vector.Normalize();
					_pullActor.entity.SetAdditiveVelocity(vector * instancedAbility.Evaluate(config.PullVelocity));
				}
			}
		}
	}
}
