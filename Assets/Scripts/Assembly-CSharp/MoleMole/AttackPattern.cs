using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class AttackPattern
	{
		private const float ATTACK_RANDOM_RANGE_H = 0.1f;

		private const float ATTACK_RANDOM_RANGE_V = 0.25f;

		public const float ATTACK_EFFECT_BIG_DAMAGE_RATIO = 0.8f;

		private static void TriggerAttackEffectsTo(string patternName, Vector3 initPos, Vector3 initForward, Vector3 initScale, BaseMonoEntity entity)
		{
			List<MonoEffect> effects;
			Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(patternName, initPos, initForward, initScale, entity, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(Singleton<LevelManager>.Instance.levelEntity);
				monoEffect.SetupPluginFromTo(entity);
			}
		}

		public static void ActAttackEffects(ConfigEntityAttackEffect attackEffectConfig, BaseMonoEntity entity, Vector3 hitPoint, Vector3 hitForward)
		{
			if (attackEffectConfig == null || attackEffectConfig.MuteAttackEffect)
			{
				return;
			}
			if (attackEffectConfig.EffectPattern != null)
			{
				if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtHitPoint)
				{
					TriggerAttackEffectsTo(attackEffectConfig.EffectPattern, hitPoint, hitForward, Vector3.one, entity);
				}
				else if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtHitPointRandom)
				{
					float num = 0.25f;
					float num2 = 0.1f;
					Vector3 vector = Vector3.Cross(Vector3.up, new Vector3(hitForward.x, 0f, hitForward.y));
					Vector3.Normalize(vector);
					vector *= UnityEngine.Random.Range(0f - num2, num2);
					TriggerAttackEffectsTo(initPos: hitPoint + new Vector3(0f, UnityEngine.Random.Range(0f - num, num), 0f) + vector, patternName: attackEffectConfig.EffectPattern, initForward: hitForward, initScale: Vector3.one, entity: entity);
				}
				else if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtEntity)
				{
					TriggerAttackEffectsTo(attackEffectConfig.EffectPattern, entity.XZPosition, entity.transform.forward, entity.transform.localScale, entity);
				}
			}
			if (string.IsNullOrEmpty(attackEffectConfig.SwitchName))
			{
				return;
			}
			string text = attackEffectConfig.SwitchName;
			BaseMonoMonster baseMonoMonster = entity as BaseMonoMonster;
			if (baseMonoMonster != null && baseMonoMonster.hasArmor)
			{
				switch (text)
				{
				case "Bullet":
					text = "Bullet_Armor";
					break;
				case "Punch":
					text = "Shield_Default";
					break;
				case "Sword":
					text = "Shield_Sword";
					break;
				}
			}
			Singleton<WwiseAudioManager>.Instance.SetSwitch("Damage_Type", text, entity.gameObject);
			Singleton<WwiseAudioManager>.Instance.Post("DR_IMP", entity.gameObject);
		}

		public static void FanCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			FanCollisionDetect fanCollisionDetect = (FanCollisionDetect)patternConfig;
			Vector3 fanCenterPoint = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + fanCollisionDetect.CenterYOffset + ((!fanCollisionDetect.FollowRootNodeY) ? 0f : attacker.GetAttachPoint("RootNode").transform.position.y), attacker.XZPosition.z);
			List<CollisionResult> results = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, fanCollisionDetect.OffsetZ, attacker.FaceDirection, fanCollisionDetect.Radius * (1f + attacker.Evaluate(fanCollisionDetect.RadiusRatio)), fanCollisionDetect.FanAngle, fanCollisionDetect.MeleeRadius, fanCollisionDetect.MeleeFanAngle, layerMask);
			TestAndActHit(attackName, patternConfig, attacker, results);
		}

		public static void RectCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			RectCollisionDetect rectCollisionDetect = (RectCollisionDetect)patternConfig;
			Vector3 recCenterPoint = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + rectCollisionDetect.CenterYOffset, attacker.XZPosition.z);
			List<CollisionResult> results = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, rectCollisionDetect.OffsetZ, attacker.FaceDirection, rectCollisionDetect.Width, rectCollisionDetect.Distance, layerMask);
			TestAndActHit(attackName, patternConfig, attacker, results);
		}

		public static void CylinderCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			CylinderCollisionDetect cylinderCollisionDetect = (CylinderCollisionDetect)patternConfig;
			Vector3 xZPosition = attacker.XZPosition;
			xZPosition += attacker.FaceDirection * cylinderCollisionDetect.CenterZOffset;
			List<CollisionResult> results = CollisionDetectPattern.CylinderCollisionDetectBySphere(attacker.XZPosition, xZPosition, cylinderCollisionDetect.Radius, cylinderCollisionDetect.Height, layerMask);
			TestAndActHit(attackName, patternConfig, attacker, results);
		}

		public static void CylinderCollisionDetectTargetLockedAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			CylinderCollisionDetectTargetLocked cylinderCollisionDetectTargetLocked = (CylinderCollisionDetectTargetLocked)patternConfig;
			List<CollisionResult> results = CollisionDetectPattern.CylinderCollisionDetectBySphere(cylinderCenterPoint: (!(attacker.AttackTarget != null) || !attacker.AttackTarget.IsActive()) ? attacker.XZPosition : attacker.AttackTarget.XZPosition, hitCastFromCenter: attacker.XZPosition, radius: cylinderCollisionDetectTargetLocked.Radius * (1f + attacker.Evaluate(cylinderCollisionDetectTargetLocked.RadiusRatio)), height: cylinderCollisionDetectTargetLocked.Height, mask: layerMask);
			TestAndActHit(attackName, patternConfig, attacker, results);
		}

		public static LayerMask GetTargetLayerMask(uint runtimeID)
		{
			return Singleton<LevelManager>.Instance.gameMode.GetAttackPatternDefaultLayerMask(runtimeID);
		}

		public static LayerMask GetLayerMask(IAttacker attacker)
		{
			return GetTargetLayerMask(attacker.GetRuntimeID());
		}

		public static void SendHitEvent(uint attackerID, uint beHitID, string attackName, AttackResult.HitCollsion hitCollision, AttackData attackData, bool forceSkipAttackerResolve = false, MPEventDispatchMode mode = MPEventDispatchMode.Normal)
		{
			if (forceSkipAttackerResolve || Singleton<LevelManager>.Instance.gameMode.ShouldAttackPatternSendBeingHit(beHitID))
			{
				EvtBeingHit evtBeingHit = new EvtBeingHit();
				evtBeingHit.targetID = beHitID;
				evtBeingHit.sourceID = attackerID;
				evtBeingHit.animEventID = attackName;
				if (attackData != null)
				{
					if (attackData.hitCollision == null)
					{
						attackData.hitCollision = hitCollision;
					}
					evtBeingHit.attackData = attackData;
				}
				else
				{
					evtBeingHit.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(Singleton<EventManager>.Instance.GetActor(attackerID), attackName);
					evtBeingHit.attackData.resolveStep = AttackData.AttackDataStep.AttackerResolved;
					evtBeingHit.attackData.hitCollision = hitCollision;
				}
				Singleton<EventManager>.Instance.FireEvent(evtBeingHit, mode);
				return;
			}
			EvtHittingOther evtHittingOther = new EvtHittingOther();
			evtHittingOther.hitCollision = hitCollision;
			evtHittingOther.targetID = attackerID;
			evtHittingOther.toID = beHitID;
			evtHittingOther.animEventID = attackName;
			if (attackData != null)
			{
				if (attackData.hitCollision == null)
				{
					attackData.hitCollision = hitCollision;
				}
				evtHittingOther.attackData = attackData;
			}
			else
			{
				evtHittingOther.hitCollision = hitCollision;
			}
			Singleton<EventManager>.Instance.FireEvent(evtHittingOther, mode);
		}

		public static void TestAndActHit(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, List<CollisionResult> results)
		{
			List<CollisionResult> list = FilterRealAttackeesByBodyParts(attackName, patternConfig, attacker, results);
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(attacker.GetRuntimeID()))
			{
			case 3:
			{
				foreach (CollisionResult item in list)
				{
					BaseMonoEntity entity3 = item.entity;
					if (entity3 == null || entity3.GetRuntimeID() == attacker.GetRuntimeID() || (!entity3.IsActive() && Singleton<RuntimeIDManager>.Instance.ParseCategory(entity3.GetRuntimeID()) != 4))
					{
						continue;
					}
					switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity3.GetRuntimeID()))
					{
					case 3:
					case 4:
					case 6:
					case 7:
						if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity3.GetRuntimeID()))
						{
							AttackResult.HitCollsion hitCollsion = new AttackResult.HitCollsion();
							hitCollsion.hitDir = item.hitForward;
							hitCollsion.hitPoint = item.hitPoint;
							AttackResult.HitCollsion hitCollision3 = hitCollsion;
							SendHitEvent(attacker.GetRuntimeID(), entity3.GetRuntimeID(), attackName, hitCollision3, null, false, MPEventDispatchMode.CheckRemoteMode);
						}
						break;
					}
				}
				break;
			}
			case 4:
			{
				foreach (CollisionResult item2 in list)
				{
					BaseMonoEntity entity2 = item2.entity;
					if (entity2 == null || entity2.GetRuntimeID() == attacker.GetRuntimeID() || !entity2.IsActive())
					{
						continue;
					}
					switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity2.GetRuntimeID()))
					{
					case 3:
					case 4:
					case 6:
						if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity2.GetRuntimeID()))
						{
							AttackResult.HitCollsion hitCollsion = new AttackResult.HitCollsion();
							hitCollsion.hitDir = item2.hitForward;
							hitCollsion.hitPoint = item2.hitPoint;
							AttackResult.HitCollsion hitCollision2 = hitCollsion;
							SendHitEvent(attacker.GetRuntimeID(), entity2.GetRuntimeID(), attackName, hitCollision2, null, false, MPEventDispatchMode.CheckRemoteMode);
						}
						break;
					}
				}
				break;
			}
			case 7:
			{
				foreach (CollisionResult item3 in list)
				{
					BaseMonoEntity entity = item3.entity;
					if (entity == null || entity.GetRuntimeID() == attacker.GetRuntimeID() || !entity.IsActive())
					{
						continue;
					}
					switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity.GetRuntimeID()))
					{
					case 3:
					case 4:
					case 6:
						if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity.GetRuntimeID()))
						{
							AttackResult.HitCollsion hitCollsion = new AttackResult.HitCollsion();
							hitCollsion.hitDir = item3.hitForward;
							hitCollsion.hitPoint = item3.hitPoint;
							AttackResult.HitCollsion hitCollision = hitCollsion;
							SendHitEvent(attacker.GetRuntimeID(), entity.GetRuntimeID(), attackName, hitCollision, null, false, MPEventDispatchMode.CheckRemoteMode);
						}
						break;
					}
				}
				break;
			}
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public static BaseMonoEntity GetCollisionResultEntity(BaseMonoEntity entity)
		{
			if (entity is MonoBodyPartEntity)
			{
				return ((MonoBodyPartEntity)entity).owner;
			}
			if (entity is BaseMonoAbilityEntity && ((BaseMonoAbilityEntity)entity).isGhost)
			{
				return null;
			}
			return entity;
		}

		private static List<CollisionResult> FilterRealAttackeesByBodyParts(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, List<CollisionResult> results)
		{
			Dictionary<BaseMonoEntity, List<CollisionResult>> dictionary = new Dictionary<BaseMonoEntity, List<CollisionResult>>();
			List<CollisionResult> list = new List<CollisionResult>();
			foreach (CollisionResult result in results)
			{
				BaseMonoEntity entity = result.entity;
				if (entity == null)
				{
					continue;
				}
				if (entity is MonoBodyPartEntity)
				{
					BaseMonoEntity owner = ((MonoBodyPartEntity)entity).owner;
					if (!dictionary.ContainsKey(owner))
					{
						dictionary[owner] = new List<CollisionResult>();
					}
					List<CollisionResult> list2 = dictionary[owner];
					list2.Add(result);
				}
				else
				{
					list.Add(result);
				}
			}
			List<CollisionResult> list3 = new List<CollisionResult>();
			foreach (CollisionResult item2 in list)
			{
				if (!dictionary.ContainsKey(item2.entity))
				{
					list3.Add(item2);
				}
			}
			foreach (BaseMonoEntity key in dictionary.Keys)
			{
				CollisionResult collisionResult = null;
				if (patternConfig is FanCollisionDetect || patternConfig is CylinderCollisionDetect || patternConfig is CylinderCollisionDetectTargetLocked || patternConfig is RectCollisionDetect)
				{
					Vector3 vector = default(Vector3);
					if (patternConfig is RectCollisionDetect)
					{
						RectCollisionDetect rectCollisionDetect = (RectCollisionDetect)patternConfig;
						Vector3 vector2 = new Vector3(attacker.XZPosition.x, 0f, attacker.XZPosition.z);
						Vector3 faceDirection = attacker.FaceDirection;
						faceDirection.y = 0f;
						vector = vector2 + rectCollisionDetect.OffsetZ * faceDirection.normalized;
					}
					else
					{
						vector = attacker.XZPosition;
					}
					float num = float.MaxValue;
					foreach (CollisionResult item3 in dictionary[key])
					{
						float num2 = Vector3.Distance(item3.entity.XZPosition, vector);
						if (num2 < num)
						{
							collisionResult = item3;
							num = num2;
						}
					}
				}
				else
				{
					collisionResult = dictionary[key][0];
				}
				CollisionResult item = new CollisionResult(key, collisionResult.hitPoint, collisionResult.hitForward);
				list3.Add(item);
			}
			return list3;
		}

		public static void ActCameraShake(ConfigEntityCameraShake cameraShake)
		{
			if (cameraShake != null)
			{
				bool hasValue = cameraShake.ShakeAngle.HasValue;
				float angle = ((!cameraShake.ShakeAngle.HasValue) ? 0f : cameraShake.ShakeAngle.Value);
				Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(cameraShake.ShakeTime, cameraShake.ShakeRange * 1.5f, angle, cameraShake.ShakeStepFrame, hasValue, cameraShake.ClearPreviousShake);
			}
		}
	}
}
