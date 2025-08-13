using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeRoundBulletMixin : AbilityHitExplodeBulletMixin
	{
		private class TraceDelay
		{
			public uint bulletID;

			public float lifeTimer;

			public float holdTimer;

			public Vector3 center;

			public Vector3 centerDir;
		}

		private HitExplodeRoundBulletMixin config;

		private BaseMonoEntity _attackTarget;

		private List<TraceDelay> _traceBullets;

		public AbilityHitExplodeRoundBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeRoundBulletMixin)config;
			_traceBullets = new List<TraceDelay>();
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			base.OnAbilityTriggered(evt);
			_attackTarget = entity.GetAttackTarget();
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
			_traceBullets.Clear();
		}

		protected override void InitBulletForward(AbilityTriggerBullet bullet)
		{
			int index = _traceBullets.SeekAddPosition();
			_traceBullets[index] = new TraceDelay
			{
				bulletID = bullet.runtimeID,
				lifeTimer = config.LifeTime,
				holdTimer = config.HoldTime,
				center = entity.transform.position,
				centerDir = entity.transform.forward
			};
		}

		public override void Core()
		{
			base.Core();
			for (int i = 0; i < _traceBullets.Count; i++)
			{
				if (_traceBullets[i] == null)
				{
					continue;
				}
				TraceDelay traceDelay = _traceBullets[i];
				AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(traceDelay.bulletID);
				if (abilityTriggerBullet == null)
				{
					continue;
				}
				if (traceDelay.holdTimer > 0f)
				{
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
					abilityTriggerBullet.triggerBullet.SetupTracing(abilityTriggerBullet.triggerBullet.transform.position, 99f, 0f);
					traceDelay.holdTimer -= Time.deltaTime * entity.TimeScale;
				}
				else if (traceDelay.lifeTimer > 0f)
				{
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled();
					if (_attackTarget != null && Vector3.Distance(traceDelay.center, _attackTarget.transform.position) > config.CenterTraceRadial)
					{
						traceDelay.centerDir = (_attackTarget.transform.position - traceDelay.center).normalized;
					}
					traceDelay.center.y = abilityTriggerBullet.triggerBullet.transform.position.y;
					traceDelay.center += traceDelay.centerDir * config.CenterSpeed * Time.deltaTime * entity.TimeScale;
					Vector3 normalized = (abilityTriggerBullet.triggerBullet.transform.position - traceDelay.center).normalized;
					Vector3 vector = Quaternion.AngleAxis(config.RadAngle, Vector3.up) * normalized;
					Vector3 targetPosition = abilityTriggerBullet.triggerBullet.transform.position + vector * instancedAbility.Evaluate(config.BulletSpeed);
					abilityTriggerBullet.triggerBullet.SetupTracing(targetPosition, 99f, 0f);
					traceDelay.lifeTimer -= Time.deltaTime * entity.TimeScale;
				}
				else
				{
					_traceBullets[i] = null;
				}
			}
		}
	}
}
