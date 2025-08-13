using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Collider))]
	public class MonoTriggerField : BaseMonoDynamicObject
	{
		private const int FRAME_EXIT_COLLIDER_COUNT = 5;

		[NonSerialized]
		public Collider triggerCollider;

		private LayerMask _collisionMask;

		private bool _isToBeRemoved;

		public List<Tuple<Collider, uint>> _insideColliders;

		private Collider[] _frameExitColliders;

		private int _frameExitIx;

		protected void Awake()
		{
			triggerCollider = GetComponent<Collider>();
			_collisionMask = -1;
			_insideColliders = new List<Tuple<Collider, uint>>();
			_frameExitColliders = new Collider[5];
		}

		public void SetCollisionMask(LayerMask mask)
		{
			_collisionMask = mask;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return triggerCollider.enabled;
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}

		protected override void Update()
		{
			base.Update();
			for (int i = 0; i < _insideColliders.Count; i++)
			{
				if (_insideColliders[i] != null)
				{
					Collider item = _insideColliders[i].Item1;
					if (item == null || !item.enabled || !item.gameObject.activeInHierarchy)
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(_runtimeID, _insideColliders[i].Item2));
						_insideColliders[i] = null;
					}
				}
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < _frameExitIx; i++)
			{
				if (_frameExitColliders[i] != null)
				{
					OnEffectiveTriggerExit(_frameExitColliders[i]);
				}
			}
			_frameExitIx = 0;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other == null || other.gameObject == null || (_collisionMask.value & (1 << other.gameObject.layer)) == 0)
			{
				return;
			}
			if (_frameExitIx > 5)
			{
				OnEffectiveTriggerEnter(other);
				return;
			}
			for (int i = 0; i < _frameExitIx; i++)
			{
				if (_frameExitColliders[i] != null && _frameExitColliders[i] == other)
				{
					_frameExitColliders[i] = null;
					return;
				}
			}
			OnEffectiveTriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			if ((_collisionMask.value & (1 << other.gameObject.layer)) != 0)
			{
				if (_frameExitIx >= 5)
				{
					OnEffectiveTriggerExit(other);
					return;
				}
				_frameExitColliders[_frameExitIx] = other;
				_frameExitIx++;
			}
		}

		private void OnEffectiveTriggerEnter(Collider other)
		{
			if (other == null)
			{
				return;
			}
			for (int i = 0; i < _insideColliders.Count; i++)
			{
				if (_insideColliders[i] != null && !(_insideColliders[i].Item1 == null) && _insideColliders[i].Item1.gameObject == other.gameObject)
				{
					return;
				}
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			if (!(componentInParent == null) && componentInParent.IsActive())
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtFieldEnter(_runtimeID, componentInParent.GetRuntimeID()));
				int index = _insideColliders.SeekAddPosition();
				_insideColliders[index] = Tuple.Create(other, componentInParent.GetRuntimeID());
			}
		}

		private void OnEffectiveTriggerExit(Collider other)
		{
			if (other == null)
			{
				return;
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			if (componentInParent == null)
			{
				return;
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(_runtimeID, componentInParent.GetRuntimeID()));
			int num = -1;
			for (int i = 0; i < _insideColliders.Count; i++)
			{
				if (_insideColliders[i] != null && _insideColliders[i].Item1 != null && _insideColliders[i].Item1.gameObject == other.gameObject)
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				_insideColliders[num] = null;
			}
		}
	}
}
