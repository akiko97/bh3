using System;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoDynamicObject : BaseMonoEntity
	{
		public enum DynamicType
		{
			Default = 0,
			Barrier = 1,
			NavigationArrow = 2,
			EvadeDummy = 3
		}

		protected float _timeScale = 1f;

		private float _lastTimeScale;

		[NonSerialized]
		public BaseMonoEntity owner;

		[NonSerialized]
		public uint ownerID;

		public DynamicType dynamicType { get; set; }

		public override float TimeScale
		{
			get
			{
				return (!owner.IsActive()) ? (Singleton<LevelManager>.Instance.levelEntity.TimeScale * _timeScale) : (owner.TimeScale * _timeScale);
			}
		}

		public override Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		public virtual void Init(uint runtimeID, uint ownerID)
		{
			_runtimeID = runtimeID;
			this.ownerID = ownerID;
			owner = Singleton<EventManager>.Instance.GetEntity(ownerID);
			_lastTimeScale = TimeScale;
			dynamicType = DynamicType.Default;
		}

		protected virtual void Start()
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtDynamicObjectCreated(owner.GetRuntimeID(), _runtimeID, dynamicType));
		}

		protected virtual void Update()
		{
			if (_lastTimeScale != TimeScale)
			{
				OnTimeScaleChanged(TimeScale);
			}
			_lastTimeScale = TimeScale;
		}

		public virtual void SetDied()
		{
		}

		protected virtual void OnTimeScaleChanged(float newTimescale)
		{
		}

		public override Transform GetAttachPoint(string name)
		{
			return base.transform;
		}

		public bool IsOwnerStaticInScene()
		{
			if (owner is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = owner as BaseMonoMonster;
				if (baseMonoMonster != null && baseMonoMonster.isStaticInScene)
				{
					return true;
				}
			}
			return false;
		}
	}
}
