using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class ActorAbility : BaseActorActionContext
	{
		[InspectorCollapsedFoldout]
		public BaseAbilityActor caster;

		[InspectorCollapsedFoldout]
		public ConfigAbility config;

		[ShowInInspector]
		[InspectorCollapsedFoldout]
		private Dictionary<string, object> _overrideMap;

		private BaseEvent _currentTriggerEvent;

		public float argumentSpecialValue;

		public bool argumentRecieved;

		public int instancedAbilityID;

		public BaseEvent CurrentTriggerEvent
		{
			get
			{
				return _currentTriggerEvent;
			}
			set
			{
				_currentTriggerEvent = value;
			}
		}

		public ActorAbility(BaseAbilityActor caster, ConfigAbility config, Dictionary<string, object> overrideMap)
		{
			this.caster = caster;
			this.config = config;
			_overrideMap = overrideMap;
			List<BaseAbilityMixin> list = new List<BaseAbilityMixin>();
			for (int i = 0; i < config.AbilityMixins.Length; i++)
			{
				BaseAbilityMixin baseAbilityMixin = caster.abilityPlugin.CreateInstancedAbilityMixin(this, null, config.AbilityMixins[i]);
				if (baseAbilityMixin != null)
				{
					list.Add(baseAbilityMixin);
				}
			}
			instancedMixins = list.ToArray();
			for (int j = 0; j < instancedMixins.Length; j++)
			{
				instancedMixins[j].instancedMixinID = j;
			}
		}

		public override string GetDebugContextName()
		{
			return config.AbilityName;
		}

		public override BaseAbilityActor GetDebugOwner()
		{
			return caster;
		}

		public float Evaluate(DynamicFloat dynamicFloat)
		{
			if (dynamicFloat.isDynamic)
			{
				if (argumentRecieved && config.UseAbilityArgumentAsSpecialKey == dynamicFloat.dynamicKey)
				{
					return argumentSpecialValue;
				}
				if (_overrideMap.ContainsKey(dynamicFloat.dynamicKey))
				{
					object obj = _overrideMap[dynamicFloat.dynamicKey];
					if (obj is SafeFloat)
					{
						return (SafeFloat)obj;
					}
					return (float)obj;
				}
				return (float)config.AbilitySpecials[dynamicFloat.dynamicKey];
			}
			return dynamicFloat.fixedValue;
		}

		public int Evaluate(DynamicInt dynamicInt)
		{
			if (dynamicInt.isDynamic)
			{
				if (_overrideMap.ContainsKey(dynamicInt.dynamicKey))
				{
					object obj = _overrideMap[dynamicInt.dynamicKey];
					if (obj is SafeInt32)
					{
						return (SafeInt32)obj;
					}
					if (obj is SafeFloat)
					{
						return (int)(SafeFloat)obj;
					}
					return (int)(float)obj;
				}
				return (int)(float)config.AbilitySpecials[dynamicInt.dynamicKey];
			}
			return dynamicInt.fixedValue;
		}

		public string Evaluate(DynamicString dynamicStr)
		{
			if (dynamicStr.isDynamic)
			{
				if (_overrideMap.ContainsKey(dynamicStr.dynamicKey))
				{
					return (string)_overrideMap[dynamicStr.dynamicKey];
				}
				return (string)config.AbilitySpecials[dynamicStr.dynamicKey];
			}
			return dynamicStr.fixedValue;
		}

		public void Attach()
		{
			AttachToActor(caster);
		}

		public void Detach()
		{
			DetachFromActor(caster);
		}

		public bool HasParam(string key)
		{
			return _overrideMap.ContainsKey(key);
		}

		public float GetFloatParam(string key)
		{
			if (_overrideMap.ContainsKey(key))
			{
				object obj = _overrideMap[key];
				if (obj is SafeFloat)
				{
					return (SafeFloat)obj;
				}
				return (float)obj;
			}
			return (float)config.AbilitySpecials[key];
		}

		public void SetOverrideMapValue(string key, object value)
		{
			if (value is int)
			{
				_overrideMap[key] = (SafeInt32)(int)value;
			}
			if (value is float)
			{
				_overrideMap[key] = (SafeFloat)(float)value;
			}
			else
			{
				_overrideMap[key] = value;
			}
		}

		public void SetOverrideMapValue(string key, int value)
		{
			_overrideMap[key] = (SafeInt32)value;
		}

		public void SetOverrideMapValue(string key, float value)
		{
			_overrideMap[key] = (SafeFloat)value;
		}
	}
}
