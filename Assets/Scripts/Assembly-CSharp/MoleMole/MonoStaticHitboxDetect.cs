using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Rigidbody))]
	public class MonoStaticHitboxDetect : MonoBehaviour
	{
		public Action<Collider> triggerEnterCallback;

		private HashSet<uint> _enteredIDs;

		[Header("Hitpoint will be on the line of this and other collided transform")]
		public Transform collideCenterTransform;

		[Header("Use owner center for retreat direction")]
		public bool useOwnerCenterForRetreatDirection;

		private Rigidbody _rigidbody;

		[NonSerialized]
		public BaseMonoEntity owner;

		private LayerMask _collisionMask;

		private Transform _followTarget;

		private void Awake()
		{
			_enteredIDs = new HashSet<uint>();
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.detectCollisions = false;
		}

		public void Init(BaseMonoEntity owner, LayerMask mask, Transform followTarget)
		{
			this.owner = owner;
			_collisionMask = mask;
			_followTarget = ((!(followTarget != null)) ? owner.transform : followTarget);
			MonoEffect componentInChildren = GetComponentInChildren<MonoEffect>();
			if (componentInChildren != null)
			{
				componentInChildren.SetOwner(owner);
			}
		}

		private void LateUpdate()
		{
			if (owner != null && owner.IsActive())
			{
				base.transform.position = _followTarget.transform.position;
				base.transform.rotation = _followTarget.rotation;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((_collisionMask.value & (1 << other.gameObject.layer)) == 0)
			{
				return;
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			if (!componentInParent.IsActive() || _enteredIDs.Contains(componentInParent.GetRuntimeID()))
			{
				return;
			}
			if (componentInParent is MonoDummyDynamicObject)
			{
				BaseMonoDynamicObject baseMonoDynamicObject = (BaseMonoDynamicObject)componentInParent;
				if (baseMonoDynamicObject.dynamicType == BaseMonoDynamicObject.DynamicType.EvadeDummy && baseMonoDynamicObject.owner != null)
				{
					_enteredIDs.Add(baseMonoDynamicObject.owner.GetRuntimeID());
				}
			}
			_enteredIDs.Add(componentInParent.GetRuntimeID());
			if (triggerEnterCallback != null)
			{
				triggerEnterCallback(other);
			}
		}

		public void EnableHitBoxDetect(bool enable)
		{
			_rigidbody.detectCollisions = enable;
			if (!enable)
			{
				_enteredIDs.Clear();
			}
		}

		public void ResetTriggerWithoutResetInside()
		{
			_enteredIDs.Clear();
		}

		public void ResetTriggerWithResetInside()
		{
			_enteredIDs.Clear();
			_rigidbody.detectCollisions = false;
			_rigidbody.detectCollisions = true;
		}

		public void SetColliderScale(float sizeRatio, float lengthRatio)
		{
			base.transform.localScale = new Vector3(sizeRatio, sizeRatio, lengthRatio);
		}

		public void ResetColliderScale()
		{
			base.transform.localScale = Vector3.one;
		}
	}
}
