using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeTracingMultiBulletsMixin : AbilityHitExplodeBulletMixin
	{
		private HitExplodeTracingMultiBulletsMixin config;

		public AbilityHitExplodeTracingMultiBulletsMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeTracingMultiBulletsMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar == null)
			{
				return;
			}
			int count = localAvatar.SubAttackTargetList.Count;
			for (int i = 0; i < count; i++)
			{
				string bulletType = config.BulletTypeName;
				HitExplodeTracingBulletMixinArgument hitExplodeTracingBulletMixinArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
				if (hitExplodeTracingBulletMixinArgument != null)
				{
					if (hitExplodeTracingBulletMixinArgument.BulletName != null)
					{
						bulletType = hitExplodeTracingBulletMixinArgument.BulletName;
					}
					if (hitExplodeTracingBulletMixinArgument.RandomBulletNames != null)
					{
						bulletType = hitExplodeTracingBulletMixinArgument.RandomBulletNames[Random.Range(0, hitExplodeTracingBulletMixinArgument.RandomBulletNames.Length)];
					}
				}
				AbilityTriggerBullet abilityTriggerBullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(bulletType, actor, instancedAbility.Evaluate(config.BulletPostionLinearSpeed), config.Targetting, config.IgnoreTimeScale, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), -1f);
				if (config.BulletEffect != null && config.BulletEffect.EffectPattern != null)
				{
					Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(config.BulletEffect.EffectPattern, abilityTriggerBullet.triggerBullet, config.BulletEffectGround);
				}
				_bulletAttackDatas.Add(abilityTriggerBullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(actor, config.HitAnimEventID));
				float angle = 180 / (count + 1) * (i + 1);
				float num = instancedAbility.Evaluate(config.BulletPositionRadius);
				float duration = instancedAbility.Evaluate(config.BulletPositionDuration);
				Vector3 vector = Vector3.Cross(Vector3.up, entity.transform.forward);
				Vector3 vector2 = (Quaternion.AngleAxis(angle, entity.transform.forward) * vector).normalized * num;
				Vector3 position = localAvatar.SubAttackTargetList[i].transform.position;
				Vector3 up = Vector3.up;
				position += up;
				abilityTriggerBullet.triggerBullet.SetupPositioning(abilityTriggerBullet.triggerBullet.transform.position, abilityTriggerBullet.triggerBullet.transform.position + vector2, duration, instancedAbility.Evaluate(config.BulletSpeed), config.TracingLerpCoef, config.TracingLerpCoefAcc, position, config.PassBy);
				InitBulletForward(abilityTriggerBullet);
			}
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
		}

		protected override void InitBulletForward(AbilityTriggerBullet bullet)
		{
			BaseMonoEntity attackTarget = entity.GetAttackTarget();
			Vector3 forward;
			if (attackTarget == null || !config.FaceTarget)
			{
				forward = entity.transform.forward;
			}
			else
			{
				Vector3 position = attackTarget.GetAttachPoint("RootNode").position;
				forward = position - bullet.triggerBullet.transform.position;
				Quaternion quaternion = Quaternion.LookRotation(entity.transform.forward);
				Quaternion to = Quaternion.LookRotation(forward);
				Quaternion quaternion2 = Quaternion.RotateTowards(quaternion, to, 15f);
				forward = quaternion2 * Vector3.forward;
			}
			bullet.triggerBullet.transform.forward = forward;
		}

		public override void Core()
		{
			base.Core();
		}
	}
}
