using System;
using System.Collections;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoPropObject : BaseMonoAbilityEntity, IAttacker
	{
		protected const string TRIGGER_LIGHT_HIT_PARAM = "LightHitTrigger";

		protected const string TRIGGER_HIT_PARAM = "HitTrigger";

		protected const string TRIGGER_APPEAR = "AppearTrigger";

		protected Transform _transform;

		public ConfigPropObject config;

		[NonSerialized]
		public BaseMonoAbilityEntity owner;

		[Header("Prop Center")]
		public Transform RootNode;

		[NonSerialized]
		public Animator animator;

		protected bool _isToBeRemove;

		private float _lastTimeScale;

		private float _timeScale;

		protected FixedStack<float> _timeScaleStack;

		public override Vector3 XZPosition
		{
			get
			{
				return new Vector3(_transform.position.x, 0f, _transform.position.z);
			}
		}

		public override float TimeScale
		{
			get
			{
				return _timeScale;
			}
		}

		public override string CurrentSkillID
		{
			get
			{
				return null;
			}
		}

		public Vector3 FaceDirection
		{
			get
			{
				return base.transform.forward;
			}
		}

		public BaseMonoEntity AttackTarget
		{
			get
			{
				return null;
			}
		}

		Transform IAttacker.transform
		{
			get
			{
				return base.transform;
			}
		}

		public event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

		protected virtual void Awake()
		{
			_transform = base.transform;
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			if (Singleton<EventManager>.Instance != null && owner != null)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectCreated(owner.GetRuntimeID(), _runtimeID));
			}
		}

		public void Init(uint ownerID, uint runtimeID, string propName, bool appearAnim = false)
		{
			config = PropObjectData.GetPropObjectConfig(propName);
			commonConfig = config.CommonConfig;
			Init(runtimeID);
			_timeScaleStack = new FixedStack<float>(8);
			_timeScaleStack.Push(1f, true);
			if (Singleton<LevelManager>.Instance != null && Singleton<LevelManager>.Instance.levelEntity != null)
			{
				_timeScale = (_lastTimeScale = _timeScaleStack.value * Singleton<LevelManager>.Instance.levelEntity.TimeScale);
			}
			else
			{
				_timeScale = (_lastTimeScale = _timeScaleStack.value);
			}
			if (Singleton<EventManager>.Instance != null)
			{
				owner = (BaseMonoAbilityEntity)Singleton<EventManager>.Instance.GetEntity(ownerID);
			}
			if (config.PropArguments != null && !config.PropArguments.IsTargetable)
			{
				SetCountedDenySelect(true, true);
			}
			if (config.PropArguments != null && config.PropArguments.Duration > 0f)
			{
				StartCoroutine(WaitDestroyByDuration(config.PropArguments.Duration));
			}
			if (appearAnim)
			{
				Appear();
			}
		}

		public override bool IsActive()
		{
			return !_isToBeRemove;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemove;
		}

		public void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
		{
			if (this.onAnimatedHitBoxCreatedCallBack != null)
			{
				this.onAnimatedHitBoxCreatedCallBack(hitBox, attackPattern);
			}
		}

		private IEnumerator WaitDestroyByDuration(float duration)
		{
			while (duration > 0f)
			{
				duration -= Time.deltaTime * _timeScale;
				yield return null;
			}
			OnDurationTimeOut();
		}

		protected virtual void OnDurationTimeOut()
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectForceKilled(_runtimeID));
		}

		public override void PushTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Push(stackIx, timescale);
		}

		public override void PopTimeScale(int stackIx)
		{
			if (_timeScaleStack.IsOccupied(stackIx))
			{
				_timeScaleStack.Pop(stackIx);
			}
		}

		public override void FireEffect(string patternName)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, XZPosition, base.transform.forward, base.transform.localScale, this);
		}

		public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, base.transform.localScale, this);
		}

		public override void FireEffectTo(string patternName, BaseMonoEntity to)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, XZPosition, base.transform.forward, base.transform.localScale, this, to);
		}

		public override Transform GetAttachPoint(string name)
		{
			if (name == "RootNode")
			{
				return RootNode;
			}
			return base.transform;
		}

		public override void SetTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Set(stackIx, timescale);
		}

		public override void SteerFaceDirectionTo(Vector3 forward)
		{
			base.transform.forward = forward;
		}

		public override void SetDied(KillEffect killEffect)
		{
			_isToBeRemove = true;
		}

		protected virtual void Update()
		{
			_timeScale = ((!(owner == null)) ? owner.TimeScale : Singleton<LevelManager>.Instance.levelEntity.TimeScale) * _timeScaleStack.value * (1f + GetProperty("Entity_TimeScaleDelta"));
			if (_lastTimeScale != TimeScale)
			{
				OnTimeScaleChanged(TimeScale);
			}
			_lastTimeScale = TimeScale;
		}

		protected virtual void OnTimeScaleChanged(float newTimeScale)
		{
		}

		public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID)
		{
		}

		[AnimationCallback]
		public void AnimEventHandler(string animEventID)
		{
		}

		public override void AddAnimEventPredicate(string predicate)
		{
		}

		public override BaseMonoEntity GetAttackTarget()
		{
			return null;
		}

		public override void SetAttackTarget(BaseMonoEntity attackTarget)
		{
		}

		public override void MaskAnimEvent(string animEventName)
		{
		}

		public override void MaskTrigger(string triggerID)
		{
		}

		public override void UnmaskTrigger(string triggerID)
		{
		}

		public override float GetCurrentNormalizedTime()
		{
			return 0f;
		}

		public override void PopMaterialGroup()
		{
		}

		public override void PushMaterialGroup(string targetGroupname)
		{
		}

		public override void RemoveAnimEventPredicate(string predicate)
		{
		}

		public override void ResetTrigger(string name)
		{
			if (animator != null)
			{
				animator.ResetTrigger(name);
			}
		}

		public override void SetAdditiveVelocity(Vector3 velocity)
		{
		}

		public override int AddAdditiveVelocity(Vector3 velocity)
		{
			return -1;
		}

		public override void SetHasAdditiveVelocity(bool hasAdditiveVelocity)
		{
		}

		public override bool HasAdditiveVelocityOfIndex(int index)
		{
			return false;
		}

		public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
		{
		}

		public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
		{
		}

		public override void SetOverrideVelocity(Vector3 velocity)
		{
		}

		public override void PushHighspeedMovement()
		{
		}

		public override void PopHighspeedMovement()
		{
		}

		public override void SetTrigger(string name)
		{
			if (animator != null)
			{
				animator.SetTrigger(name);
			}
		}

		public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
		{
			ConfigPropAnimEvent configPropAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			configPropAnimEvent.AttackPattern.patternMethod(animEventID, configPropAnimEvent.AttackPattern, this, layerMask);
		}

		public override void UnmaskAnimEvent(string animEventName)
		{
		}

		public override bool ContainAnimEventPredicate(string predicate)
		{
			return false;
		}

		public virtual void FrameHalt(int frameNum)
		{
		}

		public virtual void Appear()
		{
			animator.SetTrigger("AppearTrigger");
		}

		[AnimationCallback]
		public void AppearStartTriggerEffect(string effectName)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(effectName, XZPosition, FaceDirection, Vector3.one, this);
		}

		[AnimationCallback]
		public void AppearEndTriggerEffect(string effectName)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(effectName, XZPosition, FaceDirection, Vector3.one, this);
		}

		uint IAttacker.GetRuntimeID()
		{
			return GetRuntimeID();
		}

		float IAttacker.Evaluate(DynamicFloat target)
		{
			return Evaluate(target);
		}

		int IAttacker.Evaluate(DynamicInt target)
		{
			return Evaluate(target);
		}
	}
}
