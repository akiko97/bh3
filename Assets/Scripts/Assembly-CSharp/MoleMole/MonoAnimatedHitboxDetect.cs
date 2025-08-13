using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Rigidbody))]
	public class MonoAnimatedHitboxDetect : MonoAuxObject
	{
		public Action<Collider, string> triggerEnterCallback;

		public Action<MonoAnimatedHitboxDetect, Collider> enemyEnterCallback;

		public Action<MonoAnimatedHitboxDetect> destroyCallback;

		protected HashSet<uint> _enteredIDs;

		[Header("Hitpoint will be on the line of this and other collided transform")]
		public Transform collideCenterTransform;

		[Header("Use owner center for retreat direction")]
		public bool useOwnerCenterForRetreatDirection;

		[Header("Use fixed retreat directioon")]
		public bool useFixedReteatDirection;

		[Header("Fixed retreat direction")]
		public Vector3 fixedRetreatDirection;

		[NonSerialized]
		public BaseMonoEntity owner;

		private Animation _animation;

		private Rigidbody _rigidbody;

		private LayerMask _collisionMask;

		private float _lastTimeScale;

		private bool _follow;

		private bool _stopOnFirstContact;

		private Transform _followTarget;

		private bool _followOwnerTimeScale;

		private bool _ignoreTimeScale;

		public bool dontDestroyWhenOwnerEvade;

		private bool _enableHitWall;

		private bool _onHittingWallTriggered;

		private Vector3 _hittingWallPosition;

		private bool _lastFrameRecordRunning;

		private Vector3 _lastFramePosition;

		private RaycastHit _raycastWallHit;

		private bool _hitWallDestroy;

		private string _hitWallDestroyEffect;

		private string _overrideAnimEventID;

		public void Init(BaseMonoEntity owner, LayerMask mask, Transform followTarget, bool follow, bool stopOnFirstContact)
		{
			this.owner = owner;
			_follow = follow;
			_stopOnFirstContact = stopOnFirstContact;
			_collisionMask = mask;
			_lastTimeScale = owner.TimeScale;
			_onHittingWallTriggered = false;
			_lastFrameRecordRunning = false;
			_enableHitWall = false;
			_hitWallDestroy = false;
			_ignoreTimeScale = false;
			_followOwnerTimeScale = false;
			_followTarget = ((!(followTarget != null)) ? owner.transform : followTarget);
			MonoEffect componentInChildren = GetComponentInChildren<MonoEffect>();
			if (componentInChildren != null)
			{
				componentInChildren.SetOwner(owner);
			}
		}

		private void OnValidate()
		{
			if (!useFixedReteatDirection)
			{
			}
		}

		protected virtual void Awake()
		{
			_enteredIDs = new HashSet<uint>();
			_animation = GetComponent<Animation>();
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.detectCollisions = false;
		}

		private void Start()
		{
			_animation.Play();
			StartCoroutine(WaitEndOfFrameEnableCollision());
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((_collisionMask.value & (1 << other.gameObject.layer)) == 0)
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
			if (componentInParent is MonoDummyDynamicObject)
			{
				BaseMonoDynamicObject baseMonoDynamicObject = (BaseMonoDynamicObject)componentInParent;
				if (baseMonoDynamicObject.dynamicType == BaseMonoDynamicObject.DynamicType.EvadeDummy && baseMonoDynamicObject.owner != null)
				{
					_enteredIDs.Add(baseMonoDynamicObject.owner.GetRuntimeID());
				}
			}
			_enteredIDs.Add(componentInParent.GetRuntimeID());
			OnEntityEntered(other, componentInParent);
			if (_stopOnFirstContact)
			{
				_animation.Stop();
				_rigidbody.detectCollisions = false;
			}
			if (triggerEnterCallback != null)
			{
				if (_follow)
				{
					StartCoroutine(WaitEndOfFrameTriggerHit(other, componentInParent));
				}
				else
				{
					FireTriggerCallback(other, componentInParent);
				}
			}
		}

		protected virtual void FireTriggerCallback(Collider other, BaseMonoEntity entity)
		{
			if (other != null && owner != null)
			{
				triggerEnterCallback(other, _overrideAnimEventID);
				if (enemyEnterCallback != null)
				{
					enemyEnterCallback(this, other);
				}
			}
		}

		protected virtual void OnEntityEntered(Collider other, BaseMonoEntity entity)
		{
		}

		protected virtual void OnEnteredReset()
		{
		}

		private void RecordHittingWallPosAndRoate()
		{
			_hittingWallPosition = collideCenterTransform.transform.position;
		}

		private IEnumerator WaitEndOfFrameTriggerHit(Collider other, BaseMonoEntity entity)
		{
			yield return new WaitForEndOfFrame();
			FireTriggerCallback(other, entity);
		}

		private IEnumerator WaitEndOfFrameEnableCollision()
		{
			yield return new WaitForEndOfFrame();
			_rigidbody.detectCollisions = true;
		}

		protected virtual void LateUpdate()
		{
			if (owner != null && owner.IsActive())
			{
				if (!_ignoreTimeScale)
				{
					if (_followOwnerTimeScale)
					{
						_animation[_animation.clip.name].speed = owner.TimeScale;
					}
					else if (_lastTimeScale != owner.TimeScale)
					{
						_animation[_animation.clip.name].speed = owner.TimeScale;
					}
				}
				_lastTimeScale = owner.TimeScale;
				if (_follow)
				{
					_rigidbody.position = _followTarget.transform.position;
					_rigidbody.rotation = _followTarget.rotation;
				}
			}
			if (_enableHitWall)
			{
				WallHitCheck();
			}
			if (_onHittingWallTriggered)
			{
				collideCenterTransform.transform.SetPositionX(_hittingWallPosition.x);
				collideCenterTransform.transform.SetPositionZ(_hittingWallPosition.z);
				collideCenterTransform.transform.SetLocalEulerAnglesX(0f);
				collideCenterTransform.transform.SetLocalEulerAnglesY(0f);
				collideCenterTransform.transform.SetLocalEulerAnglesZ(0f);
				if (_hitWallDestroy)
				{
					_animation.Stop();
				}
			}
			RecordLastFramePosition();
			if (!_animation.isPlaying)
			{
				if (destroyCallback != null)
				{
					destroyCallback(this);
				}
				SetDestroy();
			}
		}

		private void WallHitCheck()
		{
			if (!_lastFrameRecordRunning || !_enableHitWall)
			{
				return;
			}
			Vector3 direction = collideCenterTransform.transform.position - _lastFramePosition;
			direction.Normalize();
			float magnitude = direction.magnitude;
			if (Physics.Raycast(_lastFramePosition, direction, out _raycastWallHit, magnitude, 1 << InLevelData.STAGE_COLLIDER_LAYER) && Vector3.Angle(_raycastWallHit.normal, Vector3.up) >= 20f && !_onHittingWallTriggered)
			{
				_onHittingWallTriggered = true;
				RecordHittingWallPosAndRoate();
				if (_hitWallDestroy && !string.IsNullOrEmpty(_hitWallDestroyEffect))
				{
					TriggerEffectPattern(_hitWallDestroyEffect);
				}
			}
		}

		private void RecordLastFramePosition()
		{
			_lastFrameRecordRunning = true;
			_lastFramePosition = collideCenterTransform.transform.position;
		}

		private void OnOwnerBeHitCancelCallback(string skillID)
		{
			SetDestroy();
		}

		private void OnDestroy()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = owner as BaseMonoAbilityEntity;
			if (!(baseMonoAbilityEntity == null))
			{
				baseMonoAbilityEntity.onBeHitCanceled = (Action<string>)Delegate.Remove(baseMonoAbilityEntity.onBeHitCanceled, new Action<string>(OnOwnerBeHitCancelCallback));
			}
		}

		public void SetFollowOwnerTimeScale(bool followOwnerTimeScale)
		{
			_followOwnerTimeScale = followOwnerTimeScale;
		}

		public void SetIgnoreTimeScale(bool ignoreTimeScale)
		{
			_ignoreTimeScale = ignoreTimeScale;
		}

		[AnimationCallback]
		private void TriggerEffectPattern(string patternName)
		{
			if (owner == null)
			{
				return;
			}
			List<MonoEffect> effects;
			Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(patternName, base.transform.position, base.transform.forward, Vector3.one, owner, out effects);
			for (int i = 0; i < effects.Count; i++)
			{
				MonoEffect monoEffect = effects[i];
				monoEffect.SetOwner(owner);
				if (monoEffect.GetComponent<MonoEffectPluginFollow>() != null)
				{
					monoEffect.GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(base.transform);
				}
				monoEffect.SetupOverride(owner);
				monoEffect.dontDestroyWhenOwnerEvade = dontDestroyWhenOwnerEvade;
			}
		}

		[AnimationCallback]
		private void TriggerAudioPattern(string name)
		{
			Singleton<WwiseAudioManager>.Instance.Post(name, base.gameObject);
		}

		[AnimationCallback]
		private void ResetTriggerWithoutResetInside()
		{
			_enteredIDs.Clear();
			OnEnteredReset();
		}

		[AnimationCallback]
		private void ResetTriggerWithResetInside()
		{
			_enteredIDs.Clear();
			OnEnteredReset();
			_rigidbody.detectCollisions = false;
			_rigidbody.detectCollisions = true;
		}

		[AnimationCallback]
		private void ResetNewTrigger(string animEventID)
		{
			_overrideAnimEventID = animEventID;
		}

		[AnimationCallback]
		private void ResetNewTriggerByMultiAnimEventID(string multiAnimEventID)
		{
			BaseMonoAvatar baseMonoAvatar = owner as BaseMonoAvatar;
			ConfigMultiAnimEvent configMultiAnimEvent = baseMonoAvatar.config.MultiAnimEvents[multiAnimEventID];
			for (int i = 0; i < configMultiAnimEvent.AnimEventNames.Length; i++)
			{
				if (baseMonoAvatar.CheckAnimEventPredicate(configMultiAnimEvent.AnimEventNames[i]))
				{
					ResetNewTrigger(configMultiAnimEvent.AnimEventNames[i]);
					break;
				}
			}
		}

		[AnimationCallback]
		private void TriggerOwnerAnimEvent(string animEventID)
		{
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = owner as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity != null && baseMonoAnimatorEntity.IsActive())
			{
				baseMonoAnimatorEntity.AnimEventHandler(animEventID);
			}
		}

		public void TriggerOwnerAnimEventIgnoreOwnerActive(string animEventID)
		{
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = owner as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity != null)
			{
				baseMonoAnimatorEntity.AnimEventHandler(animEventID);
			}
		}

		[AnimationCallback]
		public void EnableWallHitCheck()
		{
			_enableHitWall = true;
		}

		[AnimationCallback]
		public void EnableWallHitDestroy(string hitWallDestroyEffect)
		{
			_enableHitWall = true;
			_hitWallDestroy = true;
			_hitWallDestroyEffect = hitWallDestroyEffect;
		}

		public void EnableOnOwnerBeHitCanceledDestroySelf()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = owner as BaseMonoAbilityEntity;
			if (!(baseMonoAbilityEntity == null))
			{
				baseMonoAbilityEntity.onBeHitCanceled = (Action<string>)Delegate.Combine(baseMonoAbilityEntity.onBeHitCanceled, new Action<string>(OnOwnerBeHitCancelCallback));
			}
		}

		[AnimationCallback]
		private void CreateLevelProp(string propName)
		{
			ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
			float hP = propObjectConfig.PropArguments.HP;
			float attack = propObjectConfig.PropArguments.Attack;
			Vector3 position = collideCenterTransform.position;
			position.y = 0f;
			Singleton<PropObjectManager>.Instance.CreatePropObject(owner.GetRuntimeID(), propName, hP, attack, position, collideCenterTransform.forward);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("NG_HSOD_DEBUG")]
		protected virtual void OnDrawGizmos()
		{
			if (!SuperDebug.DEBUG_SWITCH[1])
			{
				return;
			}
			Gizmos.color = ((!_onHittingWallTriggered) ? Color.green : Color.red);
			Vector3 vector = collideCenterTransform.transform.position - _lastFramePosition;
			vector.Normalize();
			Gizmos.DrawLine(_lastFramePosition, _lastFramePosition + vector * 1f);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				if (collider.enabled)
				{
					Matrix4x4 matrix = Gizmos.matrix;
					Gizmos.matrix = collider.transform.localToWorldMatrix;
					Gizmos.color = Color.blue;
					if (collider is BoxCollider)
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

		public virtual Vector3 CalculateFixedRetreatDirection(Vector3 hitPoint)
		{
			return base.transform.TransformDirection(fixedRetreatDirection);
		}
	}
}
