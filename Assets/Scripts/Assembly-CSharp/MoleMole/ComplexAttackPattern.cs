using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class ComplexAttackPattern
	{
		private static RaycastHit _wallHit;

		public static void AnimatedColliderDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			AnimatedColliderDetect animatedColliderDetect = (AnimatedColliderDetect)patternConfig;
			MonoAnimatedHitboxDetect hitboxCollider = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAnimatedHitboxDetect>(animatedColliderDetect.ColliderEntryName, attacker.GetRuntimeID());
			Transform followTarget = attacker.transform;
			if (!string.IsNullOrEmpty(animatedColliderDetect.FollowAttachPoint) && attacker is BaseMonoAnimatorEntity)
			{
				BaseMonoAnimatorEntity baseMonoAnimatorEntity = (BaseMonoAnimatorEntity)attacker;
				followTarget = baseMonoAnimatorEntity.GetAttachPoint(animatedColliderDetect.FollowAttachPoint);
			}
			hitboxCollider.Init((BaseMonoEntity)attacker, layerMask, followTarget, animatedColliderDetect.Follow, false);
			hitboxCollider.SetIgnoreTimeScale(animatedColliderDetect.IgnoreTimeScale);
			hitboxCollider.SetFollowOwnerTimeScale(animatedColliderDetect.FollowOwnerTimeScale);
			hitboxCollider.dontDestroyWhenOwnerEvade = animatedColliderDetect.dontDestroyWhenEvade;
			if (animatedColliderDetect.brokenEnemyDragged)
			{
				attacker.onAnimatedHitBoxCreated(hitboxCollider, patternConfig);
			}
			if (animatedColliderDetect.DestroyOnOwnerBeHitCanceled)
			{
				hitboxCollider.EnableOnOwnerBeHitCanceledDestroySelf();
			}
			if (animatedColliderDetect.EnableHitWallStop)
			{
				hitboxCollider.EnableWallHitCheck();
			}
			if (animatedColliderDetect.DestroyOnHitWall)
			{
				hitboxCollider.EnableWallHitDestroy(animatedColliderDetect.HitWallDestroyEffect);
			}
			hitboxCollider.transform.position = attacker.XZPosition;
			hitboxCollider.transform.forward = attacker.transform.forward;
			hitboxCollider.triggerEnterCallback = delegate(Collider other, string overrideAnimEventID)
			{
				BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
				Vector3 vector;
				if (hitboxCollider.useOwnerCenterForRetreatDirection && hitboxCollider.owner != null)
				{
					vector = hitboxCollider.owner.XZPosition;
					vector.y = hitboxCollider.collideCenterTransform.position.y;
				}
				else
				{
					vector = hitboxCollider.collideCenterTransform.position;
				}
				Vector3 vector2 = other.ClosestPointOnBounds(vector);
				Vector3 vector3 = ((!hitboxCollider.useFixedReteatDirection) ? (vector2 - vector) : hitboxCollider.CalculateFixedRetreatDirection(vector2));
				vector3.y = 0f;
				vector3.Normalize();
				if (vector3 == Vector3.zero)
				{
					vector3 = other.transform.position - attacker.XZPosition;
					vector3.y = 0f;
					vector3.Normalize();
				}
				CollisionResult item = new CollisionResult(componentInParent, vector2, vector3);
				List<CollisionResult> results = new List<CollisionResult> { item };
				string attackName2 = ((overrideAnimEventID == null) ? attackName : overrideAnimEventID);
				AttackPattern.TestAndActHit(attackName2, patternConfig, attacker, results);
			};
		}

		public static void TargetLockedAnimatedColliderDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			TargetLockedAnimatedColliderDetect targetLockedAnimatedColliderDetect = (TargetLockedAnimatedColliderDetect)patternConfig;
			MonoAnimatedHitboxDetect hitboxCollider = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAnimatedHitboxDetect>(targetLockedAnimatedColliderDetect.ColliderEntryName, attacker.GetRuntimeID());
			hitboxCollider.Init((BaseMonoEntity)attacker, layerMask, null, false, targetLockedAnimatedColliderDetect.StopOnFirstContact);
			hitboxCollider.dontDestroyWhenOwnerEvade = targetLockedAnimatedColliderDetect.dontDestroyWhenEvade;
			if (targetLockedAnimatedColliderDetect.brokenEnemyDragged)
			{
				attacker.onAnimatedHitBoxCreated(hitboxCollider, patternConfig);
			}
			float maxLockDistance = targetLockedAnimatedColliderDetect.MaxLockDistance;
			Vector3 vector = attacker.transform.forward;
			if (targetLockedAnimatedColliderDetect.DestroyOnOwnerBeHitCanceled)
			{
				hitboxCollider.EnableOnOwnerBeHitCanceledDestroySelf();
			}
			Vector3 direction;
			if (attacker.AttackTarget != null && attacker.AttackTarget.IsActive())
			{
				if (targetLockedAnimatedColliderDetect.LockX)
				{
					maxLockDistance = Miscs.DistancForVec3IgnoreY(attacker.XZPosition, attacker.AttackTarget.XZPosition);
					maxLockDistance = Mathf.Min(maxLockDistance, targetLockedAnimatedColliderDetect.MaxLockDistance);
					vector = Vector3.Normalize(attacker.AttackTarget.XZPosition - attacker.XZPosition);
					direction = attacker.transform.InverseTransformDirection(vector) * maxLockDistance;
				}
				else
				{
					maxLockDistance = Miscs.DistancForVec3IgnoreY(attacker.XZPosition, attacker.AttackTarget.XZPosition);
					direction = new Vector3(0f, 0f, Mathf.Min(maxLockDistance, targetLockedAnimatedColliderDetect.MaxLockDistance));
				}
			}
			else
			{
				float num = targetLockedAnimatedColliderDetect.MaxLockDistance * 0.6f;
				maxLockDistance = num;
				if (Physics.Raycast(attacker.XZPosition, attacker.transform.forward, out _wallHit, maxLockDistance, 1 << InLevelData.STAGE_COLLIDER_LAYER))
				{
					maxLockDistance = _wallHit.distance;
				}
				maxLockDistance = Mathf.Min(maxLockDistance, num);
				direction = new Vector3(0f, 0f, Mathf.Min(maxLockDistance, targetLockedAnimatedColliderDetect.MaxLockDistance));
			}
			float num2 = Random.Range(0f, targetLockedAnimatedColliderDetect.ScatteringDistance);
			float f = Random.Range(0f, 360f);
			direction.x += num2 * Mathf.Sin(f);
			direction.z += num2 * Mathf.Cos(f);
			hitboxCollider.transform.forward = vector;
			hitboxCollider.transform.position = attacker.transform.position + attacker.transform.TransformDirection(direction);
			hitboxCollider.triggerEnterCallback = delegate(Collider other, string overrideAnimEventID)
			{
				BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
				Vector3 vector2 = other.ClosestPointOnBounds(hitboxCollider.collideCenterTransform.position);
				Vector3 vector3 = (vector2 - hitboxCollider.collideCenterTransform.position).normalized;
				if (vector3 == Vector3.zero)
				{
					vector3 = other.transform.position - attacker.XZPosition;
					vector3.y = 0f;
				}
				CollisionResult item = new CollisionResult(componentInParent, vector2, vector3);
				List<CollisionResult> results = new List<CollisionResult> { item };
				string attackName2 = ((overrideAnimEventID == null) ? attackName : overrideAnimEventID);
				AttackPattern.TestAndActHit(attackName2, patternConfig, attacker, results);
			};
		}

		public static void HitscanDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			HitscanDetect hitscanDetect = (HitscanDetect)patternConfig;
			Vector3 xZPosition = attacker.XZPosition;
			xZPosition.y = hitscanDetect.CenterYOffset;
			xZPosition.z += hitscanDetect.OffsetZ;
			List<CollisionResult> results = CollisionDetectPattern.HitscanSingleDetect(xZPosition, attacker.FaceDirection, hitscanDetect.MaxHitDistance, layerMask);
			AttackPattern.TestAndActHit(attackName, patternConfig, attacker, results);
		}

		public static void FanCollisionWithHeightDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			FanCollisionWithHeightDetect fanCollisionWithHeightDetect = (FanCollisionWithHeightDetect)patternConfig;
			Vector3 vector = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + fanCollisionWithHeightDetect.CenterYOffset + ((!fanCollisionWithHeightDetect.FollowRootNodeY) ? 0f : attacker.GetAttachPoint("RootNode").transform.position.y), attacker.XZPosition.z);
			Vector3 fanCenterPoint = vector;
			fanCenterPoint.y += fanCollisionWithHeightDetect.Height * 0.5f;
			List<CollisionResult> list = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, fanCollisionWithHeightDetect.OffsetZ, attacker.FaceDirection, fanCollisionWithHeightDetect.Radius, fanCollisionWithHeightDetect.FanAngle, fanCollisionWithHeightDetect.MeleeRadius, fanCollisionWithHeightDetect.MeleeFanAngle, layerMask);
			fanCenterPoint = vector;
			fanCenterPoint.y -= fanCollisionWithHeightDetect.Height * 0.5f;
			List<CollisionResult> list2 = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, fanCollisionWithHeightDetect.OffsetZ, attacker.FaceDirection, fanCollisionWithHeightDetect.Radius, fanCollisionWithHeightDetect.FanAngle, fanCollisionWithHeightDetect.MeleeRadius, fanCollisionWithHeightDetect.MeleeFanAngle, layerMask);
			List<CollisionResult> list3 = list2;
			for (int i = 0; i < list.Count; i++)
			{
				CollisionResult collisionResult = list[i];
				bool flag = false;
				for (int j = 0; j < list2.Count; j++)
				{
					CollisionResult collisionResult2 = list2[j];
					if (collisionResult2.entity.GetRuntimeID() == collisionResult.entity.GetRuntimeID())
					{
						flag = true;
						collisionResult2.hitPoint.y = vector.y;
						break;
					}
				}
				if (!flag)
				{
					list3.Add(collisionResult);
				}
			}
			AttackPattern.TestAndActHit(attackName, patternConfig, attacker, list3);
		}

		public static void RectCollisionWithHeightDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			RectCollisionWithHeightDetect rectCollisionWithHeightDetect = (RectCollisionWithHeightDetect)patternConfig;
			Vector3 vector = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + rectCollisionWithHeightDetect.CenterYOffset, attacker.XZPosition.z);
			Vector3 recCenterPoint = vector;
			recCenterPoint.y += rectCollisionWithHeightDetect.Height * 0.5f;
			List<CollisionResult> list = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, rectCollisionWithHeightDetect.OffsetZ, attacker.FaceDirection, rectCollisionWithHeightDetect.Width, rectCollisionWithHeightDetect.Distance, layerMask);
			recCenterPoint = vector;
			recCenterPoint.y -= rectCollisionWithHeightDetect.Height * 0.5f;
			List<CollisionResult> list2 = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, rectCollisionWithHeightDetect.OffsetZ, attacker.FaceDirection, rectCollisionWithHeightDetect.Width, rectCollisionWithHeightDetect.Distance, layerMask);
			List<CollisionResult> list3 = list2;
			for (int i = 0; i < list.Count; i++)
			{
				CollisionResult collisionResult = list[i];
				bool flag = false;
				for (int j = 0; j < list2.Count; j++)
				{
					CollisionResult collisionResult2 = list2[j];
					if (collisionResult2.entity.GetRuntimeID() == collisionResult.entity.GetRuntimeID())
					{
						flag = true;
						collisionResult2.hitPoint.y = vector.y;
						break;
					}
				}
				if (!flag)
				{
					list3.Add(collisionResult);
				}
			}
			AttackPattern.TestAndActHit(attackName, patternConfig, attacker, list3);
		}

		public static void StaticHitBoxDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
		{
			StaticHitBoxDetect staticHitBoxDetect = (StaticHitBoxDetect)patternConfig;
			IStaticHitBox staticHitBox = attacker as IStaticHitBox;
			MonoStaticHitboxDetect hitboxCollider = staticHitBox.GetStaticHitBox();
			hitboxCollider.useOwnerCenterForRetreatDirection = staticHitBoxDetect.UseOwnerCenterForRetreatDirection;
			if (staticHitBoxDetect.Enable)
			{
				hitboxCollider.SetColliderScale(staticHitBoxDetect.SizeRatio, staticHitBoxDetect.LengthRatio);
				switch (staticHitBoxDetect.ResetType)
				{
				case StaticHitBoxDetect.HitBoxResetType.WithoutInside:
					hitboxCollider.ResetTriggerWithoutResetInside();
					break;
				case StaticHitBoxDetect.HitBoxResetType.WithInside:
					hitboxCollider.ResetTriggerWithResetInside();
					break;
				}
				hitboxCollider.EnableHitBoxDetect(true);
				hitboxCollider.triggerEnterCallback = delegate(Collider other)
				{
					BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
					Vector3 vector;
					if (hitboxCollider.useOwnerCenterForRetreatDirection && hitboxCollider.owner != null)
					{
						vector = hitboxCollider.owner.XZPosition;
						vector.y = hitboxCollider.collideCenterTransform.position.y;
					}
					else
					{
						vector = hitboxCollider.collideCenterTransform.position;
					}
					Vector3 vector2 = other.ClosestPointOnBounds(vector);
					Vector3 vector3 = vector2 - vector;
					vector3.y = 0f;
					vector3.Normalize();
					if (vector3 == Vector3.zero)
					{
						vector3 = other.transform.position - attacker.XZPosition;
						vector3.y = 0f;
						vector3.Normalize();
					}
					CollisionResult item = new CollisionResult(componentInParent, vector2, vector3);
					List<CollisionResult> results = new List<CollisionResult> { item };
					string attackName2 = attackName;
					AttackPattern.TestAndActHit(attackName2, patternConfig, attacker, results);
				};
			}
			else
			{
				hitboxCollider.EnableHitBoxDetect(false);
				hitboxCollider.ResetColliderScale();
			}
		}
	}
}
