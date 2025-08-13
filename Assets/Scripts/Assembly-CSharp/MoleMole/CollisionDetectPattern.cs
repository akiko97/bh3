using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public static class CollisionDetectPattern
	{
		private const int NON_ALLOC_CACHE_SIZE = 64;

		private static RaycastHit[] _raycastHitsBuffer = new RaycastHit[64];

		private static Collider[] _collidersBuffer = new Collider[64];

		private static RaycastHit _hitscanHit;

		private static RaycastHit _castToAtMost = default(RaycastHit);

		private static bool IsEntityAlreadyInResults(List<CollisionResult> results, BaseMonoEntity entity)
		{
			for (int i = 0; i < results.Count; i++)
			{
				if (results[i].entity.GetRuntimeID() == entity.GetRuntimeID())
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckStartingPointInsideness(Collider collider, Vector3 startingPoint, Vector3 forward, List<CollisionResult> outResults, out RaycastHit hit)
		{
			Vector3 direction = collider.bounds.center - startingPoint;
			Ray ray = new Ray(startingPoint, direction);
			if (!collider.Raycast(ray, out hit, direction.magnitude))
			{
				BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
				if (!IsEntityAlreadyInResults(outResults, componentInParent))
				{
					outResults.Add(new CollisionResult(componentInParent, collider.ClosestPointOnBounds(startingPoint), forward));
				}
				return true;
			}
			return false;
		}

		public static List<CollisionResult> CircleCollisionDetectBySphere(Vector3 circleCenterPoint, float offsetZ, Vector3 direction, float radius, LayerMask mask)
		{
			return FanCollisionDetectBySphere(circleCenterPoint, offsetZ, radius, direction, 360f, mask);
		}

		public static List<CollisionResult> FanCollisionDetectBySphere(Vector3 fanCenterPoint, float offsetZ, float radius, Vector3 direction, float fanAngle, LayerMask mask)
		{
			List<CollisionResult> list = new List<CollisionResult>();
			if ((double)radius <= 0.0)
			{
				return list;
			}
			Vector3 vector = direction;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = fanCenterPoint + offsetZ * vector.normalized;
			int num = Physics.OverlapSphereNonAlloc(vector2, radius, _collidersBuffer, mask);
			for (int i = 0; i < num; i++)
			{
				Collider collider = _collidersBuffer[i];
				BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
				RaycastHit hitInfo;
				if (IsEntityAlreadyInResults(list, componentInParent) || CheckStartingPointInsideness(collider, vector2, vector, list, out hitInfo))
				{
					continue;
				}
				Vector3 vector3 = collider.bounds.center - vector2;
				vector3.y = 0f;
				Ray ray = new Ray(vector2, vector3);
				if (collider.Raycast(ray, out hitInfo, radius))
				{
					float num2 = Vector3.Angle(vector, vector3);
					if (num2 <= fanAngle * 0.5f)
					{
						Vector3 hitForward = -hitInfo.normal;
						hitForward.y = 0f;
						list.Add(new CollisionResult(componentInParent, hitInfo.point, hitForward));
						continue;
					}
				}
				int num3 = ((radius < InLevelData.MIN_COLLIDER_RADIUS) ? 1 : (Mathf.CeilToInt(fanAngle * ((float)Math.PI / 180f) / (2f * Mathf.Asin(InLevelData.MIN_COLLIDER_RADIUS / radius))) + 1));
				float angle = fanAngle / (float)(num3 - 1);
				Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
				Vector3 vector4 = Quaternion.AngleAxis(0.5f * (0f - fanAngle), Vector3.up) * vector;
				for (int j = 0; j < num3; j++)
				{
					ray.direction = vector4;
					if (collider.Raycast(ray, out hitInfo, radius))
					{
						RaycastHit castHit;
						if (!CenterHit(vector2, collider, out castHit))
						{
							castHit = hitInfo;
						}
						list.Add(new CollisionResult(componentInParent, castHit.point, vector4));
						break;
					}
					vector4 = quaternion * vector4;
				}
			}
			return list;
		}

		public static List<CollisionResult> MeleeFanCollisionDetectBySphere(Vector3 fanCenterPoint, float offsetZ, Vector3 direction, float radius, float fanAngle, float meleeRadius, float meleeFanAgele, LayerMask mask)
		{
			List<CollisionResult> list = null;
			List<CollisionResult> list2 = null;
			list = FanCollisionDetectBySphere(fanCenterPoint, offsetZ, radius, direction, fanAngle, mask);
			list2 = FanCollisionDetectBySphere(fanCenterPoint, offsetZ, meleeRadius, direction, meleeFanAgele, mask);
			List<CollisionResult> list3 = list;
			for (int i = 0; i < list2.Count; i++)
			{
				CollisionResult collisionResult = list2[i];
				bool flag = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].entity.GetRuntimeID() == collisionResult.entity.GetRuntimeID())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list3.Add(collisionResult);
				}
			}
			return list3;
		}

		public static List<CollisionResult> RectCollisionDetectByRay(Vector3 recCenterPoint, float offsetZ, Vector3 direction, float width, float distance, LayerMask mask)
		{
			Vector3 vector = direction;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = recCenterPoint + offsetZ * vector.normalized;
			List<CollisionResult> list = new List<CollisionResult>();
			int num = Physics.OverlapSphereNonAlloc(vector2, width, _collidersBuffer, mask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit hit;
				CheckStartingPointInsideness(_collidersBuffer[i], vector2, direction, list, out hit);
			}
			int num2 = Mathf.CeilToInt(width / (InLevelData.MIN_COLLIDER_RADIUS * 2f)) + 1;
			float num3 = width / (float)(num2 - 1);
			Vector3 normalized = new Vector3(0f - vector.z, 0f, vector.x).normalized;
			Vector3 vector3 = 0.5f * (0f - width) * normalized + vector2;
			Vector3 vector4 = normalized * num3;
			int[] array = MakeCenteredIndices(num2);
			for (int j = 0; j < num2; j++)
			{
				Vector3 vector5 = vector3 + array[j] * vector4;
				num = Physics.RaycastNonAlloc(vector5, vector, _raycastHitsBuffer, distance, mask);
				if (num == 0)
				{
					num = Physics.RaycastNonAlloc(vector5 + vector * distance, -vector, _raycastHitsBuffer, distance, mask);
				}
				for (int k = 0; k < num; k++)
				{
					RaycastHit raycastHit = _raycastHitsBuffer[k];
					Collider collider = raycastHit.collider;
					BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
					if (componentInParent != null && !IsEntityAlreadyInResults(list, componentInParent))
					{
						RaycastHit castHit;
						if (!CenterHit(recCenterPoint, collider, out castHit))
						{
							castHit = raycastHit;
						}
						list.Add(new CollisionResult(componentInParent, castHit.point, vector));
					}
				}
			}
			return list;
		}

		public static List<CollisionResult> CylinderCollisionDetectBySphere(Vector3 hitCastFromCenter, Vector3 cylinderCenterPoint, float radius, float height, LayerMask mask)
		{
			List<CollisionResult> list = new List<CollisionResult>();
			if (radius <= 0f)
			{
				return list;
			}
			int num = Physics.SphereCastNonAlloc(cylinderCenterPoint - radius * Vector3.up, radius, Vector3.up, _raycastHitsBuffer, radius + height, mask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = _raycastHitsBuffer[i];
				Collider collider = raycastHit.collider;
				BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
				if (componentInParent != null && !IsEntityAlreadyInResults(list, componentInParent))
				{
					Vector3 origin = hitCastFromCenter;
					origin.y = height * 0.5f;
					RaycastHit castHit;
					if (!CenterHit(origin, collider, out castHit))
					{
						castHit = raycastHit;
						Vector3 normal = hitCastFromCenter - castHit.point;
						normal.y = 0f;
						castHit.normal = normal;
					}
					list.Add(new CollisionResult(componentInParent, castHit.point, -castHit.normal));
				}
			}
			return list;
		}

		public static bool CheckCapsule(Vector3 startPt, Vector3 endPt, float radius, LayerMask mask)
		{
			return Physics.CheckCapsule(startPt, endPt, radius, mask);
		}

		public static bool SphereOverlapWithEntity(Vector3 centerPoint, float radius, LayerMask mask, GameObject gameobjet)
		{
			int num = Physics.OverlapSphereNonAlloc(centerPoint, radius, _collidersBuffer, mask);
			for (int i = 0; i < num; i++)
			{
				Collider collider = _collidersBuffer[i];
				if (!(collider.gameObject == gameobjet))
				{
					return true;
				}
			}
			return false;
		}

		public static List<CollisionResult> HitscanSingleDetect(Vector3 rayCenterPoint, Vector3 rayForward, float maxDistance, LayerMask mask)
		{
			List<CollisionResult> list = new List<CollisionResult>();
			rayForward.Normalize();
			if (Physics.Raycast(rayCenterPoint, rayForward, out _hitscanHit, maxDistance, mask))
			{
				BaseMonoEntity componentInParent = _hitscanHit.collider.GetComponentInParent<BaseMonoEntity>();
				list.Add(new CollisionResult(componentInParent, _hitscanHit.point, rayForward));
			}
			else if (Physics.Raycast(rayCenterPoint + rayForward * maxDistance, -rayForward, out _hitscanHit, maxDistance, mask))
			{
				BaseMonoEntity componentInParent2 = _hitscanHit.collider.GetComponentInParent<BaseMonoEntity>();
				list.Add(new CollisionResult(componentInParent2, rayCenterPoint, rayForward));
			}
			return list;
		}

		public static int[] MakeCenteredIndices(int count)
		{
			int[] array = new int[count];
			int num = count % 2;
			int num2 = count / 2;
			if (num == 1)
			{
				array[0] = num2;
				for (int i = 0; i < num2; i++)
				{
					array[2 * i + 1] = num2 - i - 1;
					array[2 * i + 2] = num2 + i + 1;
				}
			}
			else
			{
				for (int j = 0; j < num2; j++)
				{
					array[2 * j] = num2 - 1 - j;
					array[2 * j + 1] = num2 + j;
				}
			}
			return array;
		}

		public static bool CenterHit(Vector3 origin, Collider collider, out RaycastHit castHit)
		{
			Vector3 vector = collider.ClosestPointOnBounds(origin);
			if (vector == origin)
			{
				castHit = default(RaycastHit);
				return false;
			}
			Vector3 vector2 = vector - origin;
			vector2.y = 0f;
			if (vector2 == Vector3.zero)
			{
				castHit = default(RaycastHit);
				return false;
			}
			Ray ray = new Ray(origin, vector2);
			return collider.Raycast(ray, out castHit, float.MaxValue);
		}

		public static Vector3 GetRaycastPoint(Vector3 origin, Vector3 forward, float maxDistance, float minClipOff, LayerMask mask)
		{
			forward.Normalize();
			if (Physics.Raycast(origin, forward, out _castToAtMost, maxDistance + minClipOff, mask))
			{
				return origin + forward * (_castToAtMost.distance - minClipOff);
			}
			return origin + forward * maxDistance;
		}

		public static float GetRaycastDistance(Vector3 origin, Vector3 forward, float maxDistance, float minClipOff, LayerMask mask)
		{
			float num = maxDistance;
			if (Physics.Raycast(origin, forward, out _castToAtMost, maxDistance + minClipOff, mask))
			{
				return _castToAtMost.distance - minClipOff;
			}
			return maxDistance;
		}

		public static Vector3 GetNearestHitPoint(BaseMonoEntity entity, Vector3 center)
		{
			if (entity is BaseMonoAvatar)
			{
				BaseMonoAvatar baseMonoAvatar = (BaseMonoAvatar)entity;
				return baseMonoAvatar.hitbox.ClosestPointOnBounds(center);
			}
			if (entity is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = (BaseMonoMonster)entity;
				return baseMonoMonster.hitbox.ClosestPointOnBounds(center);
			}
			Vector3 xZPosition = entity.XZPosition;
			xZPosition.y = 1f;
			return xZPosition;
		}
	}
}
