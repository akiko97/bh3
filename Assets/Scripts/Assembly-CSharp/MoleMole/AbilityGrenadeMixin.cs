using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityGrenadeMixin : AbilityHitExplodeBulletMixin
	{
		private class TraceDelay
		{
			public uint bulletID;

			public float delayTime;

			public Vector3 tarPos;

			public float SpeedY;

			public bool isTriggered;

			public bool isStuck;

			public int hitGroundTime;
		}

		private GrenadeMixin config;

		private List<TraceDelay> _traceBullets;

		public AbilityGrenadeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (GrenadeMixin)config;
			_traceBullets = new List<TraceDelay>();
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			base.OnAbilityTriggered(evt);
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
			_traceBullets.Clear();
		}

		protected override void InitBulletForward(AbilityTriggerBullet bullet)
		{
			int index = _traceBullets.SeekAddPosition();
			bullet.triggerBullet.transform.forward = entity.transform.forward;
			bullet.triggerBullet.SetCollisionEnabled(false);
			_traceBullets[index] = new TraceDelay
			{
				bulletID = bullet.runtimeID,
				delayTime = config.DelayTime,
				tarPos = bullet.triggerBullet.transform.position,
				SpeedY = 0f,
				isTriggered = false,
				isStuck = false,
				hitGroundTime = 0
			};
			BaseMonoEntity attackTarget = entity.GetAttackTarget();
			if (attackTarget != null)
			{
				Vector3 vector = new Vector3(Random.Range(0f - config.Offset, config.Offset), 0f, Random.Range(0f - config.Offset, config.Offset));
				_traceBullets[index].tarPos = attackTarget.transform.position + vector;
				bullet.triggerBullet.transform.forward = (_traceBullets[index].tarPos - entity.transform.position).normalized;
			}
			float num = actor.Evaluate(config.BulletSpeed);
			float num2 = Vector3.Distance(_traceBullets[index].tarPos, entity.transform.position);
			float y = bullet.triggerBullet.transform.position.y;
			float gravity = config.Gravity;
			while (num2 < 4.2f * Mathf.Sqrt(2f * y / gravity) * num)
			{
				bullet.triggerBullet.speed *= 0.8f;
				num = bullet.triggerBullet.speed;
				if (num < 3f)
				{
					break;
				}
			}
			_traceBullets[index].SpeedY = num2 * gravity / (6f * num);
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
				if (!traceDelay.isStuck)
				{
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
					if (abilityTriggerBullet.triggerBullet.transform.position.y < 0.03f && traceDelay.SpeedY < 0f)
					{
						traceDelay.SpeedY *= 0f - config.Elasticity;
						traceDelay.hitGroundTime++;
						traceDelay.isTriggered = true;
						if (traceDelay.SpeedY < 0.1f || traceDelay.hitGroundTime >= 3)
						{
							traceDelay.isStuck = true;
							traceDelay.SpeedY = 0f;
						}
					}
					abilityTriggerBullet.triggerBullet.speedAdd = new Vector3(0f, traceDelay.SpeedY, 0f);
					abilityTriggerBullet.triggerBullet.SetupTracing();
					traceDelay.SpeedY -= config.Gravity * Time.deltaTime * entity.TimeScale;
				}
				else
				{
					abilityTriggerBullet.triggerBullet.speedAdd = Vector3.zero;
					abilityTriggerBullet.triggerBullet.SetupTracing(abilityTriggerBullet.triggerBullet.transform.position, 100f, 0f);
				}
				if (traceDelay.isTriggered)
				{
					if (traceDelay.delayTime > 0f)
					{
						traceDelay.delayTime -= Time.deltaTime * entity.TimeScale;
					}
					else
					{
						abilityTriggerBullet.triggerBullet.SetCollisionEnabled();
						if (traceDelay.isStuck)
						{
							_traceBullets[i] = null;
						}
					}
				}
				Debug.DrawLine(traceDelay.tarPos, abilityTriggerBullet.triggerBullet.transform.position, Color.blue);
			}
		}
	}
}
