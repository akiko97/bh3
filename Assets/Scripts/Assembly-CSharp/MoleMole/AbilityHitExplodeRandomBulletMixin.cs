using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeRandomBulletMixin : AbilityHitExplodeBulletMixin
	{
		private class TraceDelay
		{
			public uint bulletID;

			public float holdTimer;

			public float lifeTimer;

			public Vector3 startPos;

			public Vector3 dummyPos;

			public Vector3 targetOffset;
		}

		private HitExplodeRandomBulletMixin config;

		private BaseMonoEntity _attackTarget;

		private List<TraceDelay> _traceBullets;

		private float _offsetAngle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);

		public AbilityHitExplodeRandomBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeRandomBulletMixin)config;
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
			Vector3 position = new Vector3(UnityEngine.Random.Range(0f - config.RandomPosX, config.RandomPosX), UnityEngine.Random.Range(0f - config.RandomPosY, config.RandomPosY), UnityEngine.Random.Range(0f - config.RandomPosZ, config.RandomPosZ));
			bullet.triggerBullet.transform.position += bullet.triggerBullet.transform.TransformPoint(position) - bullet.triggerBullet.transform.localPosition;
			bullet.triggerBullet.transform.localRotation *= Quaternion.Euler(10f, 0f, (float)UnityEngine.Random.Range(-2, 3) * 10f);
			_offsetAngle += 1.7951958f;
			Vector3 targetOffset = new Vector3(config.TargetOffset * Mathf.Sin(_offsetAngle), 0f, config.TargetOffset * Mathf.Cos(_offsetAngle));
			_traceBullets[index] = new TraceDelay
			{
				bulletID = bullet.runtimeID,
				holdTimer = config.HoldTime,
				lifeTimer = config.LifeTime,
				startPos = bullet.triggerBullet.transform.position,
				dummyPos = bullet.triggerBullet.transform.position,
				targetOffset = targetOffset
			};
			if (_attackTarget != null)
			{
				_traceBullets[index].dummyPos = _attackTarget.transform.position;
			}
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
				if (abilityTriggerBullet != null)
				{
					if (_attackTarget != null)
					{
						traceDelay.dummyPos += Vector3.ClampMagnitude(_attackTarget.transform.position - traceDelay.dummyPos, config.TraceSpeed * entity.TimeScale);
					}
					if (traceDelay.holdTimer > 0f)
					{
						abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
						abilityTriggerBullet.triggerBullet.SetupTracing(abilityTriggerBullet.triggerBullet.transform.position, config.SteerCoef, 0f);
						abilityTriggerBullet.triggerBullet.IgnoreTimeScale = config.IgnoreTimeScale;
						traceDelay.holdTimer -= Time.deltaTime * entity.TimeScale;
					}
					else if (traceDelay.lifeTimer > 0f)
					{
						abilityTriggerBullet.triggerBullet.SetCollisionEnabled();
						Vector3 normalized = (traceDelay.dummyPos - traceDelay.startPos).normalized;
						Vector3 targetPosition = abilityTriggerBullet.triggerBullet.transform.position + normalized * instancedAbility.Evaluate(config.BulletSpeed) * config.LifeTime;
						targetPosition.y = 0f;
						targetPosition += traceDelay.targetOffset;
						abilityTriggerBullet.triggerBullet.SetupTracing(targetPosition, config.SteerCoef, 0f);
						abilityTriggerBullet.triggerBullet.IgnoreTimeScale = config.IgnoreTimeScale;
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
}
