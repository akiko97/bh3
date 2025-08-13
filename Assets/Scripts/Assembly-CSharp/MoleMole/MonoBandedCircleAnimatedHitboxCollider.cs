using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MonoBandedCircleAnimatedHitboxCollider : MonoAnimatedHitboxDetect
	{
		[Header("Unit Field Mesh Collider, must use 'UnitField''s mesh or it won't work.")]
		public MeshCollider unitFieldMeshCollider;

		private static Vector3 UNIT_FIELD_CENTER = new Vector3(0f, 1f, 0f);

		[Header("Off distance ratio, 0 means a full circle, 1 means no collision, this can be animated")]
		public float offDistanceRatio = 0.8f;

		private List<Tuple<BaseMonoEntity, Collider>> _offColliders;

		private RaycastHit _rayHit;

		protected override void Awake()
		{
			base.Awake();
			_offColliders = new List<Tuple<BaseMonoEntity, Collider>>();
		}

		protected override void FireTriggerCallback(Collider other, BaseMonoEntity entity)
		{
			if (!(other == null) && !(entity == null) && !IsColliderOff(other))
			{
				base.FireTriggerCallback(other, entity);
			}
		}

		public bool IsColliderOff(Collider other)
		{
			Vector3 vector = unitFieldMeshCollider.gameObject.transform.TransformPoint(UNIT_FIELD_CENTER);
			Vector3 center = other.bounds.center;
			vector.y = center.y;
			Vector3 vector2 = Vector3.Normalize(center - vector);
			Vector3 vector3 = unitFieldMeshCollider.gameObject.transform.TransformVector(vector2);
			Ray ray = new Ray(vector, vector2);
			if (other.Raycast(ray, out _rayHit, float.MaxValue) && _rayHit.distance < vector3.magnitude * offDistanceRatio)
			{
				return true;
			}
			return false;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			for (int i = 0; i < _offColliders.Count; i++)
			{
				if (_offColliders[i] != null)
				{
					if (_offColliders[i].Item1 == null || _offColliders[i].Item2 == null)
					{
						_offColliders[i] = null;
					}
					else if (!IsColliderOff(_offColliders[i].Item2) && _enteredIDs.Contains(_offColliders[i].Item1.GetRuntimeID()))
					{
						base.FireTriggerCallback(_offColliders[i].Item2, _offColliders[i].Item1);
						_offColliders[i] = null;
					}
				}
			}
		}

		protected override void OnEntityEntered(Collider other, BaseMonoEntity entity)
		{
			if (IsColliderOff(other))
			{
				int index = _offColliders.SeekAddPosition();
				_offColliders[index] = Tuple.Create(entity, other);
			}
		}

		protected override void OnEnteredReset()
		{
			_offColliders.Clear();
		}

		protected override void OnDrawGizmos()
		{
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				if (collider.enabled)
				{
					Matrix4x4 matrix = Gizmos.matrix;
					Gizmos.matrix = collider.transform.localToWorldMatrix;
					Gizmos.color = Color.blue;
					if (collider == unitFieldMeshCollider)
					{
						Gizmos.DrawWireMesh(unitFieldMeshCollider.sharedMesh);
						Gizmos.color = Color.red;
						Gizmos.DrawWireMesh(unitFieldMeshCollider.sharedMesh, Vector3.zero, Quaternion.identity, new Vector3(offDistanceRatio, 1f, offDistanceRatio));
					}
					else if (collider is BoxCollider)
					{
						BoxCollider boxCollider = (BoxCollider)collider;
						Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
					}
					else if (collider is MeshCollider)
					{
						MeshCollider meshCollider = (MeshCollider)collider;
						Gizmos.DrawWireMesh(meshCollider.sharedMesh);
					}
					else if (collider is CapsuleCollider)
					{
						CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
						Gizmos.DrawWireSphere(capsuleCollider.center, capsuleCollider.radius);
					}
					Gizmos.matrix = matrix;
				}
			}
		}

		public override Vector3 CalculateFixedRetreatDirection(Vector3 hitPoint)
		{
			Vector3 result = hitPoint - collideCenterTransform.position;
			result *= fixedRetreatDirection.z;
			result.y = 0f;
			return result;
		}
	}
}
