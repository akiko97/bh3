using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoAbilityEntity : BaseMonoEntity
	{
		public Action<string, string> onCurrentSkillIDChanged;

		public Action<bool> onActiveChanged;

		public Action onAnimatorBoolChanged;

		public Action onAnimatorIntChanged;

		public Action<bool> onIsGhostChanged;

		public Action<bool> onHasAdditiveVelocityChanged;

		public Action<string> onBeHitCanceled;

		public Dictionary<string, FixedSafeFloatStack> _entityProperties;

		protected Dictionary<string, bool> _animatorBoolParams;

		protected Dictionary<string, int> _animatorIntParams;

		private Dictionary<string, Action> _propertyChangedCallbacks;

		public ConfigCommonEntity commonConfig;

		private int _isGhostCount;

		private int _denySelectCount;

		public bool isGhost
		{
			get
			{
				return _isGhostCount > 0;
			}
		}

		public bool denySelect
		{
			get
			{
				return _denySelectCount > 0;
			}
		}

		public abstract string CurrentSkillID { get; }

		public virtual void Init(uint runtimeID)
		{
			_runtimeID = runtimeID;
			_entityProperties = new Dictionary<string, FixedSafeFloatStack>();
			_animatorBoolParams = new Dictionary<string, bool>();
			_animatorIntParams = new Dictionary<string, int>();
			_propertyChangedCallbacks = new Dictionary<string, Action>();
		}

		public virtual float GetProperty(string propertyKey)
		{
			if (!_entityProperties.ContainsKey(propertyKey))
			{
				if (commonConfig.EntityProperties.ContainsKey(propertyKey))
				{
					return commonConfig.EntityProperties[propertyKey].Default;
				}
				return AbilityData.PROPERTIES[propertyKey].Default;
			}
			return _entityProperties[propertyKey].value;
		}

		public virtual int PushProperty(string propertyKey, float value)
		{
			if (!_entityProperties.ContainsKey(propertyKey))
			{
				bool flag = false;
				if (AbilityData.PROPERTIES.ContainsKey(propertyKey))
				{
					flag = true;
					_entityProperties.Add(propertyKey, AbilityData.PROPERTIES[propertyKey].CreatePropertySafeStack());
				}
				else
				{
					flag = true;
					_entityProperties.Add(propertyKey, commonConfig.EntityProperties[propertyKey].CreatePropertySafeStack());
				}
				if (flag && _propertyChangedCallbacks.ContainsKey(propertyKey))
				{
					Action callback = _propertyChangedCallbacks[propertyKey];
					FixedSafeFloatStack fixedSafeFloatStack = _entityProperties[propertyKey];
					fixedSafeFloatStack.onChanged = (Action<SafeFloat, int, SafeFloat, int>)Delegate.Combine(fixedSafeFloatStack.onChanged, (Action<SafeFloat, int, SafeFloat, int>)delegate
					{
						callback();
					});
					_propertyChangedCallbacks.Remove(propertyKey);
				}
			}
			return _entityProperties[propertyKey].Push(value);
		}

		public virtual void PopProperty(string propertyKey, int stackIx)
		{
			_entityProperties[propertyKey].Pop(stackIx);
		}

		public virtual float GetPropertyByStackIndex(string propertyKey, int stackIx)
		{
			return _entityProperties[propertyKey].Get(stackIx);
		}

		public virtual void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
		{
			_entityProperties[propertyKey].Set(stackIx, value);
		}

		public virtual void SetPersistentAnimatorBool(string key, bool value)
		{
			if (!_animatorBoolParams.ContainsKey(key))
			{
				_animatorBoolParams.Add(key, value);
			}
			else
			{
				_animatorBoolParams[key] = value;
			}
			if (onAnimatorBoolChanged != null)
			{
				onAnimatorBoolChanged();
			}
		}

		public virtual void RemovePersistentAnimatorBool(string key)
		{
			_animatorBoolParams.Remove(key);
			if (onAnimatorBoolChanged != null)
			{
				onAnimatorBoolChanged();
			}
		}

		public virtual void SetPersistentAnimatoInt(string key, int value)
		{
			if (!_animatorIntParams.ContainsKey(key))
			{
				_animatorIntParams.Add(key, value);
			}
			else
			{
				_animatorIntParams[key] = value;
			}
			if (onAnimatorIntChanged != null)
			{
				onAnimatorIntChanged();
			}
		}

		public virtual void RemovePersistentAnimatorInt(string key)
		{
			_animatorIntParams.Remove(key);
			if (onAnimatorIntChanged != null)
			{
				onAnimatorIntChanged();
			}
		}

		public virtual int AttachEffect(string effectPattern)
		{
			return Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(effectPattern, this);
		}

		public virtual void DetachEffect(int patternIx)
		{
			Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(patternIx);
		}

		public virtual void DetachEffectImmediately(int patternIx)
		{
			Singleton<EffectManager>.Instance.SetDestroyImmediatelyIndexedEffectPattern(patternIx);
		}

		public abstract void FireEffect(string patternName);

		public abstract void FireEffect(string patternName, Vector3 initPos, Vector3 initDir);

		public abstract void FireEffectTo(string patternName, BaseMonoEntity to);

		public abstract void AddAnimEventPredicate(string predicate);

		public abstract void RemoveAnimEventPredicate(string predicate);

		public abstract bool ContainAnimEventPredicate(string predicate);

		public abstract void MaskAnimEvent(string animEventName);

		public abstract void UnmaskAnimEvent(string animEventName);

		public abstract void MaskTrigger(string triggerID);

		public abstract void UnmaskTrigger(string triggerID);

		public abstract void PushMaterialGroup(string targetGroupname);

		public abstract void PopMaterialGroup();

		public abstract void SetDied(KillEffect killEffect);

		public abstract void PushTimeScale(float timescale, int stackIx);

		public abstract void SetTimeScale(float timescale, int stackIx);

		public abstract void PopTimeScale(int stackIx);

		public abstract void SetNeedOverrideVelocity(bool needOverrideVelocity);

		public abstract void SetOverrideVelocity(Vector3 velocity);

		public abstract void SetHasAdditiveVelocity(bool hasAdditiveVelocity);

		public abstract void SetAdditiveVelocity(Vector3 velocity);

		public abstract int AddAdditiveVelocity(Vector3 velocity);

		public abstract bool HasAdditiveVelocityOfIndex(int index);

		public abstract void SetAdditiveVelocityOfIndex(Vector3 velocity, int index);

		public abstract void PushHighspeedMovement();

		public abstract void PopHighspeedMovement();

		public virtual void PushNoCollision()
		{
		}

		public virtual void PopNoCollision()
		{
		}

		public abstract BaseMonoEntity GetAttackTarget();

		public abstract void SetAttackTarget(BaseMonoEntity attackTarget);

		public abstract float GetCurrentNormalizedTime();

		public abstract void TriggerAttackPattern(string animEventID, LayerMask layerMask);

		public abstract void SetTrigger(string name);

		public abstract void ResetTrigger(string name);

		public abstract void SteerFaceDirectionTo(Vector3 forward);

		public void SetCountedIsGhost(bool value)
		{
			bool flag = isGhost;
			_isGhostCount += (value ? 1 : (-1));
			if (flag != value && onIsGhostChanged != null)
			{
				onIsGhostChanged(value);
			}
		}

		public void SetCountedDenySelect(bool value, bool pernament = false)
		{
			bool flag = denySelect;
			if (pernament)
			{
				_denySelectCount += ((!value) ? (-1000) : 1000);
			}
			else
			{
				_denySelectCount += (value ? 1 : (-1));
			}
			if (!flag && denySelect)
			{
				Singleton<LevelManager>.Instance.levelActor.UntargetEntity(this);
			}
		}

		public void PlayAudio(string audioPattern)
		{
		}

		public void PlayAudio(string audioPattern, Transform target)
		{
		}

		public void StopAudio(string audioPattern)
		{
		}

		public void AddEffectOverride(string effectOverrideKey, string effectPattern)
		{
			MonoEffectOverride monoEffectOverride = GetComponent<MonoEffectOverride>();
			if (monoEffectOverride == null)
			{
				monoEffectOverride = base.gameObject.AddComponent<MonoEffectOverride>();
			}
			monoEffectOverride.effectOverrides.Add(effectOverrideKey, effectPattern);
		}

		public void RemoveEffectOverride(string effectOverrideKey)
		{
			MonoEffectOverride component = GetComponent<MonoEffectOverride>();
			component.effectOverrides.Remove(effectOverrideKey);
		}

		public float Evaluate(DynamicFloat target)
		{
			if (target.isDynamic)
			{
				return GetProperty(target.dynamicKey);
			}
			return target.fixedValue;
		}

		public int Evaluate(DynamicInt target)
		{
			if (target.isDynamic)
			{
				return (int)GetProperty(target.dynamicKey);
			}
			return target.fixedValue;
		}

		protected void RegisterPropertyChangedCallback(string propertyKey, Action callback)
		{
			_propertyChangedCallbacks.Add(propertyKey, callback);
		}

		protected virtual void OnEnable()
		{
			if (onActiveChanged != null)
			{
				onActiveChanged(true);
			}
		}

		protected virtual void OnDisable()
		{
			if (onActiveChanged != null)
			{
				onActiveChanged(false);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			onCurrentSkillIDChanged = null;
			onActiveChanged = null;
			onAnimatorBoolChanged = null;
			onAnimatorIntChanged = null;
		}

		public virtual void SetShaderData(E_ShaderData dataType, bool bEnable)
		{
		}

		public virtual void SetShaderDataLerp(E_ShaderData dataType, bool bEnable, float enableDuration = -1f, float disableDuration = -1f, bool bUseNewTexture = false)
		{
		}
	}
}
