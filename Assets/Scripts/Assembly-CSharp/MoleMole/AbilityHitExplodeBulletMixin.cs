using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeBulletMixin : BaseAbilityMixin
	{
		protected HitExplodeBulletMixin baseConfig;

		protected Dictionary<uint, AttackData> _bulletAttackDatas;

		protected List<EvtHittingOther> _evtsLs;

		public AbilityHitExplodeBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			baseConfig = (HitExplodeBulletMixin)config;
			_bulletAttackDatas = new Dictionary<uint, AttackData>();
			_evtsLs = new List<EvtHittingOther>();
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBulletHit>(actor.runtimeID);
			_bulletAttackDatas.Clear();
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBulletHit>(actor.runtimeID);
			ClearBullets();
		}

		private void ClearBullets()
		{
			if (baseConfig.RemoveClearType == BulletClearBehavior.DoNothing)
			{
				return;
			}
			foreach (uint key in _bulletAttackDatas.Keys)
			{
				AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(key);
				if (abilityTriggerBullet != null && abilityTriggerBullet.IsActive())
				{
					if (baseConfig.RemoveClearType == BulletClearBehavior.ClearAndKillAndPlayExplodeEffect)
					{
						FireTriggerBulletHitExplodeEffect(abilityTriggerBullet, abilityTriggerBullet.triggerBullet.transform.position, abilityTriggerBullet.triggerBullet.transform.forward);
					}
					if (abilityTriggerBullet != null)
					{
						abilityTriggerBullet.Kill();
					}
				}
			}
			_bulletAttackDatas.Clear();
		}

		protected void FireTriggerBulletHitExplodeEffect(AbilityTriggerBullet bulletActor, Vector3 position, Vector3 forward, bool selfExplode = false)
		{
			if (selfExplode && baseConfig.SelfExplodeEffect != null)
			{
				FireMixinEffect(baseConfig.SelfExplodeEffect, bulletActor.triggerBullet, position, forward, true);
			}
			else if (baseConfig.ApplyDistinctHitExplodeEffectPattern && baseConfig.HitExplodeEffectAir != null && baseConfig.HitExplodeEffectGround != null)
			{
				float num = instancedAbility.Evaluate(baseConfig.DistinctHitExplodeHeight);
				if (bulletActor.triggerBullet.transform.position.y > num)
				{
					FireMixinEffect(baseConfig.HitExplodeEffectAir, bulletActor.triggerBullet, position, forward, true);
				}
				else
				{
					FireMixinEffect(baseConfig.HitExplodeEffectGround, bulletActor.triggerBullet, position, forward, true);
				}
			}
			else
			{
				FireMixinEffect(baseConfig.HitExplodeEffect, bulletActor.triggerBullet, position, forward, true);
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			HitExplodeTracingBulletMixinArgument arg = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
			CreateBullet(arg, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), evt.otherID);
		}

		protected virtual void CreateBullet(HitExplodeTracingBulletMixinArgument arg, uint bulletRuntimeID, uint otherID)
		{
			string bulletType = baseConfig.BulletTypeName;
			float speed = instancedAbility.Evaluate(baseConfig.BulletSpeed);
			if (arg != null)
			{
				if (arg.BulletName != null)
				{
					bulletType = arg.BulletName;
				}
				if (arg.RandomBulletNames != null)
				{
					bulletType = arg.RandomBulletNames[Random.Range(0, arg.RandomBulletNames.Length)];
				}
				if (arg.BulletSpeed != null)
				{
					speed = instancedAbility.Evaluate(arg.BulletSpeed);
				}
			}
			AbilityTriggerBullet abilityTriggerBullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(bulletType, actor, speed, baseConfig.Targetting, baseConfig.IgnoreTimeScale, bulletRuntimeID, instancedAbility.Evaluate(baseConfig.AliveDuration));
			if (baseConfig.BulletEffect != null && baseConfig.BulletEffect.EffectPattern != null)
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(baseConfig.BulletEffect.EffectPattern, abilityTriggerBullet.triggerBullet, baseConfig.BulletEffectGround);
			}
			InitBulletForward(abilityTriggerBullet);
			InitBulletForwardWithArgument(abilityTriggerBullet, arg, otherID);
			_bulletAttackDatas.Add(abilityTriggerBullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(actor, baseConfig.HitAnimEventID));
		}

		protected virtual void InitBulletForward(AbilityTriggerBullet bullet)
		{
			BaseMonoEntity attackTarget = entity.GetAttackTarget();
			Vector3 forward;
			if (attackTarget == null || !baseConfig.FaceTarget)
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
			if (baseConfig.IsFixedHeight)
			{
				forward.y = 0f;
			}
			bullet.triggerBullet.transform.forward = forward;
			bullet.triggerBullet.IgnoreTimeScale = baseConfig.IgnoreTimeScale;
		}

		protected virtual void InitBulletForwardWithArgument(AbilityTriggerBullet bullet, HitExplodeTracingBulletMixinArgument arg, uint otherID)
		{
			if (arg != null && arg.XZAngleOffset != 0f)
			{
				bullet.triggerBullet.transform.Rotate(new Vector3(0f, arg.XZAngleOffset, 0f));
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBulletHit)
			{
				return ListenBulletHit((EvtBulletHit)evt);
			}
			return false;
		}

		protected virtual bool ListenBulletHit(EvtBulletHit evt)
		{
			if (!_bulletAttackDatas.ContainsKey(evt.targetID))
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
			if (baseMonoEntity is MonoDummyDynamicObject)
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
				if (abilityTriggerBullet != null)
				{
					abilityTriggerBullet.Kill();
				}
				_bulletAttackDatas.Remove(evt.targetID);
			}
			else
			{
				attackData = attackData.Clone();
			}
			if (flag4 && abilityTriggerBullet != null)
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
				bool selfExplode = evt.selfExplode;
				if (abilityTriggerBullet != null)
				{
					FireTriggerBulletHitExplodeEffect(abilityTriggerBullet, hitPoint, hitDir, selfExplode);
				}
			}
			if (baseConfig.HitExplodeActions.Length > 0 && (!evt.selfExplode || !baseConfig.MuteSelfHitExplodeActions))
			{
				for (int j = 0; j < _evtsLs.Count; j++)
				{
					if (actor.abilityPlugin != null)
					{
						actor.abilityPlugin.HandleActionTargetDispatch(baseConfig.HitExplodeActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_evtsLs[j].toID), evt);
					}
				}
			}
			for (int k = 0; k < _evtsLs.Count; k++)
			{
				EvtHittingOther evtHittingOther2 = _evtsLs[k];
				if (baseConfig.IsHitChangeTargetDirection && evtHittingOther2.attackData.hitEffect >= AttackResult.AnimatorHitEffect.ThrowUp)
				{
					BaseAbilityActor baseAbilityActor2 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evtHittingOther2.toID);
					if (baseAbilityActor2 != null)
					{
						baseAbilityActor2.entity.transform.forward = -evtHittingOther2.attackData.hitCollision.hitDir;
					}
				}
				Singleton<EventManager>.Instance.FireEvent(evtHittingOther2);
			}
			return true;
		}
	}
}
