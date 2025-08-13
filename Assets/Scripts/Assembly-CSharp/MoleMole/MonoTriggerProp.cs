using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Collider))]
	public class MonoTriggerProp : BaseMonoPropObject
	{
		private const int FRAME_EXIT_COLLIDER_COUNT = 5;

		protected Collider _triggerCollider;

		private LayerMask _collisionMask;

		public List<Tuple<Collider, uint>> _insideColliders;

		private Collider[] _frameExitColliders;

		private int _frameExitIx;

		protected override void Awake()
		{
			base.Awake();
			_triggerCollider = GetComponent<Collider>();
			_collisionMask = -1;
			_insideColliders = new List<Tuple<Collider, uint>>();
			_frameExitColliders = new Collider[5];
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			if (config.PropArguments.CanAffectMonsters)
			{
				SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(_runtimeID, MixinTargetting.All));
			}
			else
			{
				SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(runtimeID, MixinTargetting.Enemy));
			}
		}

		public void SetCollisionMask(LayerMask mask)
		{
			_collisionMask = mask;
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

		public override void SetDied(KillEffect killEffect)
		{
			base.SetDied(killEffect);
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
			if (config.PropArguments.OnKillEffectPattern != null && base.gameObject.activeSelf)
			{
				FireEffect(config.PropArguments.OnKillEffectPattern);
			}
		}

		protected virtual void OnEffectiveTriggerEnter(Collider other)
		{
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

		protected virtual void OnEffectiveTriggerExit(Collider other)
		{
			if (other == null)
			{
				return;
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			if (componentInParent != null)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(_runtimeID, componentInParent.GetRuntimeID()));
			}
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

		private void OnTriggerEnter(Collider other)
		{
			if ((_collisionMask.value & (1 << other.gameObject.layer)) == 0)
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
				if (_frameExitColliders[i] == other)
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

		protected void ClearInsideColliders()
		{
			_insideColliders.Clear();
			Singleton<EventManager>.Instance.FireEvent(new EvtFieldClear(_runtimeID));
		}

		protected override void OnDurationTimeOut()
		{
			for (int i = 0; i < _insideColliders.Count; i++)
			{
				if (_insideColliders[i] != null && _insideColliders[i].Item1 != null)
				{
					Collider item = _insideColliders[i].Item1;
					BaseMonoEntity componentInParent = item.GetComponentInParent<BaseMonoEntity>();
					if (componentInParent != null)
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(_runtimeID, componentInParent.GetRuntimeID()));
					}
				}
			}
			_insideColliders.Clear();
			base.OnDurationTimeOut();
		}
	}
}
