using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Rigidbody))]
	public class MonoTriggerBullet : BaseMonoDynamicObject
	{
		private enum BulletState
		{
			None = 0,
			Linear = 1,
			TracePosition = 2,
			Placing = 3
		}

		[Header("Addtional offset from launch point")]
		public Vector3 offset;

		[Header("Parent Attach Point/Transform Path")]
		public string parentTransform;

		[Header("Is NOT Attach Point")]
		public bool isNotAttachPoint;

		[Header("Follow Rotation")]
		public bool followRotation;

		private BulletState _state;

		private float _aliveDuration = -1f;

		private bool _ignoreTimeScale;

		private EntityTimer _aliveTimer;

		public float speed;

		public Vector3 speedAdd;

		public float acceleration;

		public float targetReachThreshold;

		private LayerMask _collisionMask;

		private bool _isToBeRemoved;

		private Rigidbody _rigidbody;

		private bool _collisionEnabled = true;

		private Vector3 _targetPosition;

		public float _traceLerpCoef;

		public float _traceLerpCoefAcc;

		private bool _passBy;

		private Vector3 _originalPosition;

		private Vector3 _placingPosition;

		private EntityTimer _placingTimer;

		private float _resumeSpeed;

		private HashSet<uint> _enteredIDs;

		private EntityTimer _resetTimer;

		public float AliveDuration
		{
			get
			{
				return _aliveDuration;
			}
			set
			{
				_aliveDuration = value;
				if (_aliveTimer != null)
				{
					_aliveTimer.timespan = value;
				}
			}
		}

		public bool IgnoreTimeScale
		{
			get
			{
				return _ignoreTimeScale;
			}
			set
			{
				_ignoreTimeScale = value;
			}
		}

		public float BulletTimeScale
		{
			get
			{
				if (_ignoreTimeScale)
				{
					return _timeScale;
				}
				return TimeScale;
			}
		}

		public Vector3 RigidbodyPos
		{
			get
			{
				return _rigidbody.position;
			}
			set
			{
				_rigidbody.position = value;
			}
		}

		public override float TimeScale
		{
			get
			{
				return (!owner.IsActive()) ? (Singleton<LevelManager>.Instance.levelEntity.TimeScale * Singleton<LevelManager>.Instance.levelEntity.AuxTimeScale * _timeScale) : (owner.TimeScale * Singleton<LevelManager>.Instance.levelEntity.AuxTimeScale * _timeScale);
			}
		}

		protected void Awake()
		{
			_collisionMask = -1;
			_rigidbody = GetComponent<Rigidbody>();
			_enteredIDs = new HashSet<uint>();
			_aliveTimer = new EntityTimer(_aliveDuration);
		}

		public override void Init(uint runtimeID, uint ownerID)
		{
			base.Init(runtimeID, ownerID);
			Transform transform = ((!string.IsNullOrEmpty(parentTransform) && !isNotAttachPoint) ? owner.GetAttachPoint(parentTransform) : owner.transform.Find(parentTransform));
			base.transform.position = transform.TransformPoint(offset);
			if (followRotation)
			{
				base.transform.rotation = transform.rotation;
			}
			_aliveTimer.Reset(false);
		}

		protected override void Start()
		{
			base.Start();
			_rigidbody.velocity = (base.transform.forward * speed + speedAdd) * BulletTimeScale;
			if (_state == BulletState.None)
			{
				_state = BulletState.Linear;
			}
			if (_aliveDuration > 0f)
			{
				_aliveTimer.Reset(true);
			}
		}

		public void SetCollisionMask(LayerMask mask)
		{
			_collisionMask = mask;
		}

		public void SetupLinear()
		{
			_rigidbody.velocity = (base.transform.forward * speed + speedAdd) * BulletTimeScale;
			_state = BulletState.Linear;
		}

		public void SetupAtReset()
		{
			_rigidbody.velocity = Vector3.zero;
			_state = BulletState.None;
		}

		public void SetupTracing(Vector3 targetPosition, float steerCoef, float steerCoefAcc, bool passBy = false)
		{
			_state = BulletState.TracePosition;
			_targetPosition = targetPosition;
			_traceLerpCoef = steerCoef;
			_traceLerpCoefAcc = steerCoefAcc;
			_passBy = passBy;
		}

		public void SetupTracing()
		{
			_state = BulletState.TracePosition;
			_targetPosition = new Vector3(0f, 100f, 0f);
		}

		public void SetupPositioning(Vector3 originalPosition, Vector3 targetPosition, float duration, float resumeSpeed, float steerCoef, float steerCoefAcc, Vector3 tracingPosition, bool passBy = false)
		{
			_state = BulletState.Placing;
			_placingTimer = new EntityTimer(duration);
			_placingTimer.Reset(true);
			_originalPosition = originalPosition;
			_placingPosition = targetPosition;
			_resumeSpeed = resumeSpeed;
			_traceLerpCoef = steerCoef;
			_traceLerpCoefAcc = steerCoefAcc;
			_targetPosition = tracingPosition;
			_passBy = passBy;
		}

		private float GetBulletTimeScale()
		{
			if (_ignoreTimeScale)
			{
				return _timeScale;
			}
			return TimeScale;
		}

		protected override void Update()
		{
			base.Update();
			if (_resetTimer != null)
			{
				_resetTimer.Core(1f);
				if (_resetTimer.isTimeUp)
				{
					_enteredIDs.Clear();
					_resetTimer.Reset(false);
				}
			}
			if (_aliveTimer.isActive)
			{
				_aliveTimer.Core(BulletTimeScale);
				if (_aliveTimer.isTimeUp)
				{
					EvtBulletHit evtBulletHit = new EvtBulletHit(_runtimeID);
					evtBulletHit.ownerID = ownerID;
					evtBulletHit.hitCollision = new AttackResult.HitCollsion
					{
						hitDir = CreateHitForward(),
						hitPoint = base.transform.position
					};
					evtBulletHit.hitGround = true;
					evtBulletHit.selfExplode = true;
					Singleton<EventManager>.Instance.FireEvent(evtBulletHit);
					_aliveTimer.Reset(false);
				}
			}
			if (InLevelData.IsOutOfStage(XZPosition))
			{
				EvtBulletHit evtBulletHit2 = new EvtBulletHit(_runtimeID);
				evtBulletHit2.ownerID = ownerID;
				evtBulletHit2.hitCollision = new AttackResult.HitCollsion
				{
					hitDir = CreateHitForward(),
					hitPoint = base.transform.position
				};
				Singleton<EventManager>.Instance.FireEvent(evtBulletHit2);
			}
			else if (base.transform.position.y < 0.05f && _collisionEnabled)
			{
				EvtBulletHit evtBulletHit3 = new EvtBulletHit(_runtimeID);
				evtBulletHit3.ownerID = ownerID;
				evtBulletHit3.hitCollision = new AttackResult.HitCollsion
				{
					hitDir = CreateHitForward(),
					hitPoint = base.transform.position
				};
				evtBulletHit3.hitGround = true;
				Singleton<EventManager>.Instance.FireEvent(evtBulletHit3);
			}
			else if (_state == BulletState.Linear)
			{
				speed += acceleration * Time.deltaTime * BulletTimeScale;
				_rigidbody.velocity = speed * base.transform.forward * BulletTimeScale;
			}
			else if (_state == BulletState.TracePosition)
			{
				_traceLerpCoef += _traceLerpCoefAcc * Time.deltaTime * BulletTimeScale;
				Vector3 forward = base.transform.forward;
				if ((_targetPosition - _rigidbody.position).magnitude >= targetReachThreshold)
				{
					forward = Vector3.Normalize(_targetPosition - _rigidbody.position);
					float num = Vector3.Angle(forward, base.transform.forward);
					if (!_passBy || (double)num < 90.0)
					{
						base.transform.forward = Vector3.Slerp(base.transform.forward, forward, Time.deltaTime * BulletTimeScale * _traceLerpCoef);
					}
					speed += acceleration * Time.deltaTime * BulletTimeScale;
					_rigidbody.velocity = (speed * base.transform.forward + speedAdd) * BulletTimeScale;
					return;
				}
				_rigidbody.velocity = base.transform.forward * 0f;
				if (_collisionEnabled)
				{
					EvtBulletHit evtBulletHit4 = new EvtBulletHit(_runtimeID);
					evtBulletHit4.ownerID = ownerID;
					evtBulletHit4.hitCollision = new AttackResult.HitCollsion
					{
						hitDir = CreateHitForward(),
						hitPoint = base.transform.position
					};
					evtBulletHit4.hitGround = true;
					Singleton<EventManager>.Instance.FireEvent(evtBulletHit4);
				}
			}
			else if (_state == BulletState.Placing && _placingTimer != null && _placingTimer.isActive)
			{
				_placingTimer.Core(BulletTimeScale);
				if (!_placingTimer.isTimeUp)
				{
					base.transform.position = Vector3.Slerp(_originalPosition, _placingPosition, _placingTimer.timer / _placingTimer.timespan);
					_collisionEnabled = false;
					_collisionMask = -1;
					_rigidbody.velocity = speed * base.transform.forward * BulletTimeScale;
				}
				else
				{
					_state = BulletState.TracePosition;
					_placingTimer.Reset(false);
					speed = _resumeSpeed;
					_collisionEnabled = true;
				}
			}
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}

		public void SetCollisionEnabled(bool _enabled = true)
		{
			_collisionEnabled = _enabled;
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((_collisionMask.value & (1 << other.gameObject.layer)) == 0 || !_collisionEnabled)
			{
				return;
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(componentInParent.GetRuntimeID()) == 4)
			{
				if (_enteredIDs.Contains(componentInParent.GetRuntimeID()))
				{
					return;
				}
			}
			else if (!componentInParent.IsActive() || _enteredIDs.Contains(componentInParent.GetRuntimeID()))
			{
				return;
			}
			BaseMonoEntity baseMonoEntity = componentInParent;
			if (componentInParent is BaseMonoDynamicObject)
			{
				BaseMonoDynamicObject baseMonoDynamicObject = (BaseMonoDynamicObject)componentInParent;
				if (baseMonoDynamicObject.dynamicType == DynamicType.EvadeDummy && baseMonoDynamicObject.owner != null)
				{
					_enteredIDs.Add(baseMonoDynamicObject.owner.GetRuntimeID());
				}
			}
			else if (componentInParent is MonoBodyPartEntity)
			{
				baseMonoEntity = ((MonoBodyPartEntity)componentInParent).owner;
			}
			if (!(baseMonoEntity is BaseMonoAbilityEntity) || !((BaseMonoAbilityEntity)baseMonoEntity).isGhost)
			{
				_enteredIDs.Add(baseMonoEntity.GetRuntimeID());
				EvtBulletHit evtBulletHit = new EvtBulletHit(_runtimeID, baseMonoEntity.GetRuntimeID());
				evtBulletHit.ownerID = ownerID;
				Vector3 position = base.transform.position - Time.deltaTime * BulletTimeScale * _rigidbody.velocity;
				evtBulletHit.hitCollision = new AttackResult.HitCollsion
				{
					hitPoint = other.ClosestPointOnBounds(position),
					hitDir = CreateHitForward()
				};
				Singleton<EventManager>.Instance.FireEvent(evtBulletHit);
			}
		}

		private Vector3 CreateHitForward()
		{
			Vector3 vector = _rigidbody.velocity;
			if (vector == Vector3.zero)
			{
				vector = base.transform.forward;
			}
			vector.Normalize();
			return vector;
		}

		protected override void OnTimeScaleChanged(float newTimescale)
		{
			if (Time.timeScale < 1f)
			{
				_rigidbody.velocity = base.transform.forward * 2f;
			}
			else
			{
				_rigidbody.velocity = newTimescale * (speed * base.transform.forward + speedAdd);
			}
		}

		public void ResetInside(float resetTime = 0.4f)
		{
			if (_resetTimer == null)
			{
				_resetTimer = new EntityTimer(resetTime, this);
			}
			_resetTimer.Reset(true);
		}
	}
}
