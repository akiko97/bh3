using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeShotgunBulletMixin : AbilityHitExplodeBulletMixin
	{
		private HitExplodeShotgunBulletMixin config;

		private BaseMonoEntity _attackTarget;

		private int _hitCount;

		public AbilityHitExplodeShotgunBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeShotgunBulletMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			for (int i = 0; i < config.BulletNum; i++)
			{
				base.OnAbilityTriggered(evt);
			}
			_attackTarget = entity.GetAttackTarget();
			_hitCount = config.MaxHitNum;
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
		}

		protected override void InitBulletForward(AbilityTriggerBullet bullet)
		{
			float value = 10f;
			if (_attackTarget != null)
			{
				value = Vector3.Distance(_attackTarget.XZPosition, actor.entity.XZPosition);
			}
			float t = Mathf.InverseLerp(config.ScatterDistanceMax, config.ScatterDistanceMin, value);
			bullet.triggerBullet.transform.forward = actor.entity.transform.forward;
			Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
			insideUnitCircle.x *= Mathf.Tan((float)Math.PI / 180f * Mathf.Lerp(config.ScatterAngleMinX, config.ScatterAngleMaxX, t));
			insideUnitCircle.y *= Mathf.Tan((float)Math.PI / 180f * Mathf.Lerp(config.ScatterAngleMinY, config.ScatterAngleMaxY, t));
			bullet.triggerBullet.transform.forward += Quaternion.FromToRotation(Vector3.forward, bullet.triggerBullet.transform.forward) * insideUnitCircle;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBulletHit)
			{
				if (((EvtBulletHit)evt).hitEnvironment)
				{
					return base.ListenEvent(evt);
				}
				if (_hitCount == 0)
				{
					return false;
				}
				_hitCount--;
				return base.ListenEvent(evt);
			}
			return false;
		}

		public override void Core()
		{
			base.Core();
		}
	}
}
