using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeTracingBulletMixin : AbilityHitExplodeBulletMixin
	{
		private class TraceDelay
		{
			public float lineTimer;

			public float turnTimer;

			public uint bulletID;

			public BaseMonoEntity subAttackTarget;
		}

		private HitExplodeTracingBulletMixin config;

		private BaseMonoEntity _attackTarget;

		private List<TraceDelay> _traceBullets;

		public AbilityHitExplodeTracingBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeTracingBulletMixin)config;
			_traceBullets = new List<TraceDelay>();
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			base.OnAbilityTriggered(evt);
			if (config.IsRandomTarget)
			{
				SelectRandomTarget();
			}
			else
			{
				_attackTarget = entity.GetAttackTarget();
			}
		}

		private void SelectRandomTarget()
		{
			if (entity is BaseMonoAvatar)
			{
				List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
				if (allMonsters.Count > 0)
				{
					_attackTarget = allMonsters[Random.Range(0, allMonsters.Count)];
				}
			}
			else if (entity is BaseMonoMonster)
			{
				_attackTarget = entity.GetAttackTarget();
			}
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
			_traceBullets.Clear();
		}

		protected override void InitBulletForward(AbilityTriggerBullet bullet)
		{
		}

		protected override void InitBulletForwardWithArgument(AbilityTriggerBullet bullet, HitExplodeTracingBulletMixinArgument arg, uint otherID)
		{
			base.InitBulletForwardWithArgument(bullet, arg, otherID);
			if (config.IsRandomInit)
			{
				if (config.IsRandomInitCone)
				{
					bullet.triggerBullet.transform.forward = Quaternion.AngleAxis(Random.Range(-40, 40), entity.transform.up) * Quaternion.AngleAxis(-Random.Range(-10, 40), entity.transform.right) * entity.transform.forward;
				}
				else
				{
					bullet.triggerBullet.transform.forward = Quaternion.AngleAxis(-Random.Range(20, 45), entity.transform.right) * entity.transform.forward;
				}
			}
			BaseMonoEntity subAttackTarget = null;
			if (otherID != 0)
			{
				subAttackTarget = Singleton<EventManager>.Instance.GetEntity(otherID);
			}
			int index = _traceBullets.SeekAddPosition();
			_traceBullets[index] = new TraceDelay
			{
				lineTimer = config.TurnStartDelay,
				turnTimer = config.TraceStartDelay,
				bulletID = bullet.runtimeID,
				subAttackTarget = subAttackTarget
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
				BaseMonoEntity baseMonoEntity = ((!(traceDelay.subAttackTarget != null)) ? _attackTarget : traceDelay.subAttackTarget);
				if (traceDelay.lineTimer > 0f)
				{
					traceDelay.lineTimer -= Time.deltaTime * entity.TimeScale;
					continue;
				}
				if (traceDelay.turnTimer > 0f)
				{
					if (Mathf.Approximately(traceDelay.turnTimer, config.TraceStartDelay) && baseMonoEntity != null)
					{
						Vector3 position = abilityTriggerBullet.triggerBullet.transform.position;
						Vector3 onUnitSphere = Random.onUnitSphere;
						Vector2 lhs = new Vector2(onUnitSphere.x, onUnitSphere.z);
						if (Vector2.Dot(lhs, new Vector2(abilityTriggerBullet.triggerBullet.transform.forward.x, abilityTriggerBullet.triggerBullet.transform.forward.z)) < 0f)
						{
							lhs *= -1f;
						}
						onUnitSphere = new Vector3(lhs.x, onUnitSphere.y, lhs.y);
						Vector3 vector = onUnitSphere * Vector3.Distance(abilityTriggerBullet.triggerBullet.transform.position, baseMonoEntity.transform.position);
						position += vector;
						Mathf.Clamp(position.y, 0f - config.RandomHeight, config.RandomHeight);
						abilityTriggerBullet.triggerBullet.SetupTracing(position, config.TracingLerpCoef, 0f);
					}
					traceDelay.turnTimer -= Time.deltaTime * entity.TimeScale;
					continue;
				}
				if (baseMonoEntity != null)
				{
					Vector3 targetPosition = ((!config.TraceRootNode) ? baseMonoEntity.transform.position : baseMonoEntity.GetAttachPoint("RootNode").position);
					if (config.RandomOffsetDistance > 0f)
					{
						Vector3 vector2 = new Vector3(Random.value - 0.5f, Random.value - 0.5f).normalized * config.RandomOffsetDistance;
						vector2 += Vector3.up * Random.Range(0f, config.RandomHeight);
						targetPosition += vector2;
					}
					if (!config.TraceY)
					{
						targetPosition.y = abilityTriggerBullet.triggerBullet.transform.position.y;
					}
					abilityTriggerBullet.triggerBullet.SetupTracing(targetPosition, config.TracingLerpCoef, 0f, config.PassBy);
				}
				_traceBullets[i] = null;
			}
		}
	}
}
