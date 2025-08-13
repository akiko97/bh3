using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityReflectBulletMixin : BaseAbilityMixin
	{
		public ReflectBulletMixin config;

		public AbilityReflectBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ReflectBulletMixin)config;
		}

		public override void OnAdded()
		{
			actor.AddAbilityState(AbilityState.ReflectBullet, false);
		}

		public override void OnRemoved()
		{
			actor.RemoveAbilityState(AbilityState.ReflectBullet);
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtAfterBulletReflected)
			{
				return OnBulletRefected((EvtAfterBulletReflected)evt);
			}
			return false;
		}

		private bool OnBulletRefected(EvtAfterBulletReflected evt)
		{
			AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(evt.bulletID);
			MonoTriggerBullet triggerBullet = abilityTriggerBullet.triggerBullet;
			abilityTriggerBullet.Setup(actor, triggerBullet.speed, MixinTargetting.All, triggerBullet.IgnoreTimeScale, triggerBullet.AliveDuration);
			evt.attackData.attackerAttackValue *= instancedAbility.Evaluate(config.DamageRatio);
			Vector3 position = Singleton<EventManager>.Instance.GetEntity(evt.launcherID).GetAttachPoint("RootNode").position;
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.launcherID);
			float num = Vector3.Angle(instancedAbility.caster.entity.transform.forward, baseAbilityActor.entity.transform.position - instancedAbility.caster.entity.transform.position);
			if (num < config.Angle)
			{
				if (config.IsReflectToLauncher)
				{
					triggerBullet.SetupTracing(position, triggerBullet._traceLerpCoef, triggerBullet._traceLerpCoefAcc);
					triggerBullet.transform.forward = -triggerBullet.transform.forward;
				}
				else
				{
					position.y += Random.Range(1f, 3f);
					Vector3 rhs = position - entity.XZPosition;
					float sqrMagnitude = rhs.sqrMagnitude;
					rhs.y = 0f;
					Vector3 vector = Random.onUnitSphere;
					if (Vector3.Dot(vector, rhs) < 0f)
					{
						vector = -vector;
					}
					vector.y = Mathf.Abs(vector.y);
					triggerBullet.transform.forward = vector;
					triggerBullet.SetupTracing(vector.normalized * sqrMagnitude * 0.8f, triggerBullet._traceLerpCoef, triggerBullet._traceLerpCoefAcc);
				}
				if (config.ResetAliveDuration)
				{
					triggerBullet.AliveDuration = config.NewAliveDuration;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.ReflectSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.launcherID), evt);
			}
			else
			{
				EvtBulletHit evtBulletHit = new EvtBulletHit(triggerBullet.GetRuntimeID(), actor.runtimeID);
				evtBulletHit.ownerID = actor.runtimeID;
				evtBulletHit.cannotBeReflected = true;
				Vector3 hitPoint = triggerBullet.transform.position - Time.deltaTime * triggerBullet.BulletTimeScale * triggerBullet.transform.GetComponent<Rigidbody>().velocity;
				evtBulletHit.hitCollision = new AttackResult.HitCollsion
				{
					hitPoint = hitPoint,
					hitDir = triggerBullet.transform.forward
				};
				Singleton<EventManager>.Instance.FireEvent(evtBulletHit);
			}
			return false;
		}
	}
}
