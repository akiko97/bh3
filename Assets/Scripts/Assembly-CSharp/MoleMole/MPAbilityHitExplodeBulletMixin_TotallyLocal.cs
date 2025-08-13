using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MPAbilityHitExplodeBulletMixin_TotallyLocal : AbilityHitExplodeBulletMixin
	{
		public MPAbilityHitExplodeBulletMixin_TotallyLocal(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			HitExplodeTracingBulletMixinArgument arg = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
			uint nextNonSyncedRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(6);
			CreateBullet(arg, nextNonSyncedRuntimeID, evt.otherID);
		}

		protected override bool ListenBulletHit(EvtBulletHit evt)
		{
			if (!_bulletAttackDatas.ContainsKey(evt.targetID))
			{
				return false;
			}
			BaseMPIdentity baseMPIdentity = Singleton<MPManager>.Instance.TryGetIdentity(evt.otherID);
			if (baseMPIdentity != null && !baseMPIdentity.isAuthority && !baseMPIdentity.remoteMode.IsRemoteReceive())
			{
				return false;
			}
			AttackData attackData = _bulletAttackDatas[evt.targetID];
			attackData.isFromBullet = true;
			bool flag = baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage;
			bool flag2 = baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage || baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDamageHitTarget;
			bool flag3 = true;
			bool flag4 = baseConfig.BulletHitType == BulletHitBehavior.NoDestroyAndRefresh;
			BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(evt.otherID);
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor(evt.otherID) as BaseAbilityActor;
			if (baseMonoEntity is MonoDummyDynamicObject || (baseMPIdentity != null && !baseMPIdentity.remoteMode.IsRemoteReceive()))
			{
				flag2 = false;
				flag = false;
				flag3 = false;
				flag4 = false;
			}
			else if (evt.hitEnvironment)
			{
				flag2 = true;
				flag4 = false;
			}
			else if (!evt.cannotBeReflected && baseAbilityActor != null && baseAbilityActor.abilityState.ContainsState(AbilityState.ReflectBullet))
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtAfterBulletReflected(evt.otherID, evt.targetID, actor.runtimeID, _bulletAttackDatas[evt.targetID]));
				return false;
			}
			AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(evt.targetID);
			if (flag2)
			{
				abilityTriggerBullet.Kill();
				_bulletAttackDatas.Remove(evt.targetID);
			}
			else
			{
				attackData = attackData.Clone();
			}
			if (flag4)
			{
				abilityTriggerBullet.triggerBullet.ResetInside(baseConfig.ResetTime);
			}
			_evtsLs.Clear();
			if (evt.hitEnvironment)
			{
				if (!evt.hitGround)
				{
					return true;
				}
				EvtHittingOther evtHittingOther = new EvtHittingOther(actor.runtimeID, evt.otherID, attackData);
				evtHittingOther.hitCollision = evt.hitCollision;
				_evtsLs.Add(evtHittingOther);
			}
			else
			{
				attackData.hitCollision = evt.hitCollision;
				_evtsLs.Add(new EvtHittingOther(actor.runtimeID, evt.otherID, baseConfig.HitAnimEventID, attackData));
			}
			if (flag)
			{
				List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(evt.hitCollision.hitPoint, evt.hitCollision.hitPoint, instancedAbility.Evaluate(baseConfig.HitExplodeRadius), 1f, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, baseConfig.Targetting));
				float y = evt.hitCollision.hitPoint.y;
				for (int i = 0; i < list.Count; i++)
				{
					CollisionResult collisionResult = list[i];
					BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(collisionResult.entity);
					if (!(collisionResultEntity == null) && collisionResultEntity.GetRuntimeID() != evt.otherID)
					{
						collisionResult.hitPoint.y = y;
						AttackData attackData2 = attackData.Clone();
						attackData2.hitCollision = new AttackResult.HitCollsion
						{
							hitDir = collisionResult.hitForward,
							hitPoint = collisionResult.hitPoint
						};
						_evtsLs.Add(new EvtHittingOther(actor.runtimeID, collisionResultEntity.GetRuntimeID(), baseConfig.HitAnimEventID, attackData2));
					}
				}
			}
			if (flag3)
			{
				Vector3 hitPoint = evt.hitCollision.hitPoint;
				if (baseConfig.ExplodeEffectGround)
				{
					hitPoint.y = 0f;
				}
				Vector3 hitDir = evt.hitCollision.hitDir;
				hitDir.y = 0f;
				FireTriggerBulletHitExplodeEffect(abilityTriggerBullet, hitPoint, hitDir);
			}
			if (baseConfig.HitExplodeActions.Length > 0)
			{
				for (int j = 0; j < _evtsLs.Count; j++)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(baseConfig.HitExplodeActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_evtsLs[j].toID), evt);
				}
			}
			for (int k = 0; k < _evtsLs.Count; k++)
			{
				EvtHittingOther evtHittingOther2 = _evtsLs[k];
				AttackPattern.SendHitEvent(actor.runtimeID, evtHittingOther2.toID, evtHittingOther2.animEventID, evtHittingOther2.hitCollision, evtHittingOther2.attackData, false, MPEventDispatchMode.CheckRemoteMode);
			}
			return true;
		}
	}
}
