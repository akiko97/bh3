using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeTracingPositionBulletMixin : AbilityHitExplodeBulletMixin
	{
		private class TraceDelay
		{
			public float traceTimer;

			public uint bulletID;
		}

		private HitExplodeTracePositionBulletMixin config;

		private List<TraceDelay> _traceBullets;

		public AbilityHitExplodeTracingPositionBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeTracePositionBulletMixin)config;
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
			_traceBullets[index] = new TraceDelay
			{
				traceTimer = config.TraceStartDelay,
				bulletID = bullet.runtimeID
			};
		}

		private Vector3 CalculateTraceTargetPosition(bool baseOnTarget, float distance, float angle)
		{
			Vector3 vector = entity.XZPosition;
			BaseMonoEntity baseMonoEntity = null;
			baseMonoEntity = ((!baseOnTarget) ? entity : entity.GetAttackTarget());
			if (baseMonoEntity != null)
			{
				vector = AdjustLevelCollision(baseMonoEntity.XZPosition, Quaternion.Euler(0f, angle, 0f) * baseMonoEntity.transform.forward * distance);
			}
			Debug.DrawLine(vector, vector + Vector3.up * 5f, Color.red, 3f);
			return vector;
		}

		private Vector3 AdjustLevelCollision(Vector3 origin, Vector3 offset)
		{
			float num = 0.2f;
			Vector3 vector = offset;
			int num2 = 4;
			for (int i = 0; i < num2; i++)
			{
				Ray ray = new Ray(origin + Vector3.up * num, vector.normalized);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, vector.magnitude, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
				{
					vector = Quaternion.AngleAxis(360f / (float)num2, Vector3.up) * vector;
					continue;
				}
				break;
			}
			return origin + vector;
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
				traceDelay.traceTimer -= Time.deltaTime * entity.TimeScale;
				if (!(traceDelay.traceTimer > 0f))
				{
					_traceBullets[i] = null;
					AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(traceDelay.bulletID);
					if (abilityTriggerBullet != null && abilityTriggerBullet.triggerBullet != null && abilityTriggerBullet.triggerBullet.IsActive())
					{
						abilityTriggerBullet.triggerBullet.SetupTracing(CalculateTraceTargetPosition(config.BaseOnTarget, config.Distance, config.Angle), config.TracingLerpCoef, config.TracingLerpCoefAcc);
					}
				}
			}
		}
	}
}
