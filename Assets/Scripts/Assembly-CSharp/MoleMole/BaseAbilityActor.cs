using System;
using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAbilityActor : BasePluggedActor
	{
		private class AbilityStateEntry
		{
			public AbilityState state;

			public int count;
		}

		private const float PARALYZE_ANI_DEFENCE_DEFENCE_DOWN = -0.2f;

		[HideInInspector]
		public ActorAbilityPlugin abilityPlugin;

		public SafeFloat baseMaxHP = 0f;

		public SafeFloat baseMaxSP = 0f;

		public SafeFloat maxHP = 0f;

		public SafeFloat maxSP = 0f;

		public SafeFloat HP = 0f;

		public SafeFloat SP = 0f;

		public SafeInt32 level = 0;

		public SafeFloat attack = 0f;

		public SafeFloat defense = 0f;

		public SafeBool isAlive = false;

		public bool isInLevelAnim;

		[HideInInspector]
		public ConfigCommonEntity commonConfig;

		public Action<float, float, float> onHPChanged;

		public Action<float, float, float> onSPChanged;

		public Action<float, float> onMaxHPChanged;

		public Action<float, float> onMaxSPChanged;

		public Action<AbilityState, bool> onAbilityStateAdd;

		public Action<AbilityState> onAbilityStateRemove;

		public Action onPostInitialized;

		[ShowInInspector]
		protected Dictionary<string, FixedSafeFloatStack> _actorProperties;

		public BaseMonoAbilityEntity entity;

		private Dictionary<string, Action> _propertyChangedCallbacks;

		public Dictionary<string, string> abilityIDMap;

		public List<Tuple<ConfigAbility, Dictionary<string, object>>> appliedAbilities;

		[ShowInInspector]
		private List<ConfigBuffDebuffResistance> _resistanceBuffDebuffs;

		public AbilityState abilityState;

		private List<AbilityStateEntry> _abilityStatePushCount = new List<AbilityStateEntry>();

		private List<AbilityStateEntry> _abilityStateImmuneCount = new List<AbilityStateEntry>();

		private int _immuneCount;

		private static Dictionary<AbilityState, string> ABILITY_EFFECT_MAP = new Dictionary<AbilityState, string>
		{
			{
				AbilityState.MoveSpeedUp,
				"Ability_MoveSpeedUp"
			},
			{
				AbilityState.AttackSpeedUp,
				"Ability_AttackSpeedUp"
			},
			{
				AbilityState.PowerUp,
				"Ability_PowerUp"
			},
			{
				AbilityState.Shielded,
				"Ability_Shielded"
			},
			{
				AbilityState.CritUp,
				"Ability_CritUp"
			},
			{
				AbilityState.Bleed,
				"Ability_Bleed"
			},
			{
				AbilityState.Burn,
				"Ability_Burn"
			},
			{
				AbilityState.Poisoned,
				"Ability_Poisoned"
			},
			{
				AbilityState.Stun,
				"Ability_Stun"
			},
			{
				AbilityState.Paralyze,
				"Ability_Paralyze"
			},
			{
				AbilityState.MoveSpeedDown,
				"Ability_MoveSpeedDown"
			},
			{
				AbilityState.AttackSpeedDown,
				"Ability_AttackSpeedDown"
			},
			{
				AbilityState.Fragile,
				"Ability_Fragile"
			},
			{
				AbilityState.Weak,
				"Ability_Weak"
			},
			{
				AbilityState.TargetLocked,
				"Ability_TargetLocked"
			}
		};

		private List<Tuple<AbilityState, int>> _abilityStateEffectIxLs = new List<Tuple<AbilityState, int>>();

		private int _maxSpeedPropertyIx;

		private int _paralyzeAniDefenceStackIx;

		private EntityTimer _witchTimeResumeTimer;

		public Action<uint, string, KillEffect> onJustKilled;

		[HideInInspector]
		public MPActorAbilityPlugin mpAbilityPlugin;

		public override void Init(BaseMonoEntity entity)
		{
			this.entity = (BaseMonoAbilityEntity)entity;
			_actorProperties = new Dictionary<string, FixedSafeFloatStack>();
			_propertyChangedCallbacks = new Dictionary<string, Action>();
			abilityIDMap = new Dictionary<string, string>();
			appliedAbilities = new List<Tuple<ConfigAbility, Dictionary<string, object>>>();
			_resistanceBuffDebuffs = new List<ConfigBuffDebuffResistance>();
			isAlive = true;
			onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Combine(onAbilityStateAdd, new Action<AbilityState, bool>(OnAbilityStateAdd));
			onAbilityStateRemove = (Action<AbilityState>)Delegate.Combine(onAbilityStateRemove, new Action<AbilityState>(OnAbilityStateRemove));
			RegisterPropertyChangedCallback("Actor_MaxHPRatio", HPPropertyChangedCallback);
			RegisterPropertyChangedCallback("Actor_MaxHPDelta", HPPropertyChangedCallback);
			RegisterPropertyChangedCallback("Actor_MaxSPRatio", SPPropertyChangedCallback);
			RegisterPropertyChangedCallback("Actor_MaxSPDelta", SPPropertyChangedCallback);
			_witchTimeResumeTimer = new EntityTimer(0.5f, Singleton<LevelManager>.Instance.levelEntity);
			_witchTimeResumeTimer.SetActive(false);
		}

		protected void HPPropertyChangedCallback()
		{
			float num = (float)HP / (float)maxHP;
			float num2 = ((float)baseMaxHP + GetProperty("Actor_MaxHPDelta")) * (1f + GetProperty("Actor_MaxHPRatio"));
			bool flag = num2 > (float)maxHP;
			DelegateUtils.UpdateField(ref maxHP, num2, onMaxHPChanged);
			if (flag)
			{
				float num3 = num * (float)maxHP;
				DelegateUtils.UpdateField(ref HP, num3, num3 - (float)HP, onHPChanged);
			}
			else
			{
				float num4 = Mathf.Min(maxHP, HP);
				DelegateUtils.UpdateField(ref HP, num4, num4 - (float)HP, onHPChanged);
			}
		}

		protected void SPPropertyChangedCallback()
		{
			float num = (float)SP / (float)maxSP;
			float num2 = ((float)baseMaxSP + GetProperty("Actor_MaxSPDelta")) * (1f + GetProperty("Actor_MaxSPRatio"));
			bool flag = num2 > (float)maxSP;
			DelegateUtils.UpdateField(ref maxSP, num2, onMaxSPChanged);
			if (flag)
			{
				float num3 = num * (float)maxSP;
				DelegateUtils.UpdateField(ref SP, num3, num3 - (float)SP, onSPChanged);
			}
			else
			{
				float num4 = Mathf.Min(maxSP, SP);
				DelegateUtils.UpdateField(ref SP, num4, num4 - (float)SP, onSPChanged);
			}
		}

		public virtual void PostInit()
		{
			ActorAbilityPlugin.PostInitAbilityActorPlugin(this);
			if (onPostInitialized != null)
			{
				onPostInitialized();
			}
		}

		public abstract void ForceKill(uint killerID, KillEffect killEffect);

		public float GetProperty(string propertyKey)
		{
			if (!_actorProperties.ContainsKey(propertyKey))
			{
				if (commonConfig.EntityProperties.ContainsKey(propertyKey) && commonConfig.EntityProperties[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Actor)
				{
					return commonConfig.EntityProperties[propertyKey].Default;
				}
				return AbilityData.PROPERTIES[propertyKey].Default;
			}
			return _actorProperties[propertyKey].value;
		}

		public int PushProperty(string propertyKey, float value)
		{
			if (!commonConfig.EntityProperties.ContainsKey(propertyKey) && !AbilityData.PROPERTIES.ContainsKey(propertyKey))
			{
				return -1;
			}
			if ((commonConfig.EntityProperties.ContainsKey(propertyKey) && commonConfig.EntityProperties[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Actor) || (AbilityData.PROPERTIES.ContainsKey(propertyKey) && AbilityData.PROPERTIES[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Actor))
			{
				bool flag = false;
				if (!_actorProperties.ContainsKey(propertyKey))
				{
					if (AbilityData.PROPERTIES.ContainsKey(propertyKey))
					{
						flag = true;
						_actorProperties.Add(propertyKey, AbilityData.PROPERTIES[propertyKey].CreatePropertySafeStack());
					}
					else
					{
						flag = true;
						_actorProperties.Add(propertyKey, commonConfig.EntityProperties[propertyKey].CreatePropertySafeStack());
					}
				}
				if (flag && _propertyChangedCallbacks.ContainsKey(propertyKey))
				{
					Action callback = _propertyChangedCallbacks[propertyKey];
					FixedSafeFloatStack fixedSafeFloatStack = _actorProperties[propertyKey];
					fixedSafeFloatStack.onChanged = (Action<SafeFloat, int, SafeFloat, int>)Delegate.Combine(fixedSafeFloatStack.onChanged, (Action<SafeFloat, int, SafeFloat, int>)delegate
					{
						callback();
					});
					_propertyChangedCallbacks.Remove(propertyKey);
				}
				return _actorProperties[propertyKey].Push(value);
			}
			return entity.PushProperty(propertyKey, value);
		}

		public void PopProperty(string propertyKey, int stackIx)
		{
			if (commonConfig.EntityProperties.ContainsKey(propertyKey) || AbilityData.PROPERTIES.ContainsKey(propertyKey))
			{
				if ((commonConfig.EntityProperties.ContainsKey(propertyKey) && commonConfig.EntityProperties[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Entity) || (AbilityData.PROPERTIES.ContainsKey(propertyKey) && AbilityData.PROPERTIES[propertyKey].Type != ConfigAbilityPropertyEntry.PropertyType.Actor))
				{
					entity.PopProperty(propertyKey, stackIx);
				}
				else
				{
					_actorProperties[propertyKey].Pop(stackIx);
				}
			}
		}

		public float GetPropertyByStackIndex(string propertyKey, int stackIx)
		{
			if (_actorProperties.ContainsKey(propertyKey))
			{
				return _actorProperties[propertyKey].Get(stackIx);
			}
			return entity.GetPropertyByStackIndex(propertyKey, stackIx);
		}

		public void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
		{
			if (_actorProperties.ContainsKey(propertyKey))
			{
				_actorProperties[propertyKey].Set(stackIx, value);
			}
			else
			{
				entity.SetPropertyByStackIndex(propertyKey, stackIx, value);
			}
		}

		private AbilityStateEntry GetStateEntry(List<AbilityStateEntry> ls, AbilityState state, bool createIfNotFound = false)
		{
			for (int i = 0; i < ls.Count; i++)
			{
				if (ls[i].state == state)
				{
					return ls[i];
				}
			}
			if (createIfNotFound)
			{
				AbilityStateEntry abilityStateEntry = new AbilityStateEntry();
				abilityStateEntry.state = state;
				abilityStateEntry.count = 0;
				AbilityStateEntry abilityStateEntry2 = abilityStateEntry;
				ls.Add(abilityStateEntry2);
				return abilityStateEntry2;
			}
			return null;
		}

		public void AddAbilityState(AbilityState state, bool muteDisplayEffect)
		{
			AbilityStateEntry stateEntry = GetStateEntry(_abilityStatePushCount, state, true);
			if (stateEntry.count == 0)
			{
				abilityState |= state;
				if (onAbilityStateAdd != null)
				{
					onAbilityStateAdd(state, muteDisplayEffect);
				}
			}
			stateEntry.count++;
		}

		public void RemoveAbilityState(AbilityState state)
		{
			AbilityStateEntry stateEntry = GetStateEntry(_abilityStatePushCount, state);
			if (stateEntry == null)
			{
				return;
			}
			stateEntry.count--;
			if (stateEntry.count == 0)
			{
				abilityState &= ~state;
				if (onAbilityStateRemove != null)
				{
					onAbilityStateRemove(state);
				}
			}
		}

		public void SetAbilityStateImmune(AbilityState state, bool immune)
		{
			AbilityStateEntry stateEntry = GetStateEntry(_abilityStateImmuneCount, state, true);
			stateEntry.count += (immune ? 1 : (-1));
			if (stateEntry.count > 0 && (abilityState & state) != AbilityState.None)
			{
				abilityPlugin.RemoveModifierByState(state);
			}
		}

		public void SetImmuneDebuff(bool immune)
		{
			_immuneCount += (immune ? 1 : (-1));
			abilityPlugin.IsImmuneDebuff = _immuneCount > 0;
		}

		public bool IsImmuneAbilityState(AbilityState state)
		{
			AbilityStateEntry stateEntry = GetStateEntry(_abilityStateImmuneCount, state);
			return stateEntry != null && stateEntry.count > 0;
		}

		public void AddBuffDebuffResistance(ConfigBuffDebuffResistance resistance)
		{
			if (resistance != null && _resistanceBuffDebuffs != null)
			{
				_resistanceBuffDebuffs.Add(resistance);
			}
		}

		public void RemoveBuffDebuffResistance(ConfigBuffDebuffResistance resistance)
		{
			if (resistance != null && _resistanceBuffDebuffs != null && _resistanceBuffDebuffs.Contains(resistance))
			{
				_resistanceBuffDebuffs.Remove(resistance);
			}
		}

		public float GetResistanceRatio(AbilityState abilityState)
		{
			float num = 1f;
			if (_resistanceBuffDebuffs != null)
			{
				for (int i = 0; i < _resistanceBuffDebuffs.Count; i++)
				{
					if (_resistanceBuffDebuffs[i].ResistanceBuffDebuffs.Contains(abilityState))
					{
						num *= 1f - _resistanceBuffDebuffs[i].ResistanceRatio;
					}
				}
			}
			return num;
		}

		public float GetAbilityStateDurationRatio(AbilityState abilityState)
		{
			float num = 1f;
			if (_resistanceBuffDebuffs != null)
			{
				for (int i = 0; i < _resistanceBuffDebuffs.Count; i++)
				{
					if (_resistanceBuffDebuffs[i].ResistanceBuffDebuffs.Contains(abilityState))
					{
						num *= 1f - _resistanceBuffDebuffs[i].DurationRatio;
					}
				}
			}
			return num;
		}

		public virtual void HealHP(float amount)
		{
			if ((bool)isAlive)
			{
				DelegateUtils.UpdateField(ref HP, Mathf.Clamp((float)HP + amount, 0f, maxHP), amount, onHPChanged);
			}
		}

		public virtual void HealSP(float amount)
		{
			if ((bool)isAlive)
			{
				DelegateUtils.UpdateField(ref SP, Mathf.Clamp((float)SP + amount, 0f, maxSP), amount, onSPChanged);
			}
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

		protected virtual void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
			bool flag = false;
			switch (state)
			{
			case AbilityState.Invincible:
				abilityPlugin.RemoveAllDebuffModifiers();
				SetImmuneDebuff(true);
				if (!muteDisplayEffect)
				{
					flag = true;
					entity.SetShaderDataLerp(E_ShaderData.Invincible, true);
				}
				break;
			case AbilityState.Immune:
				abilityPlugin.RemoveAllDebuffModifiers();
				SetImmuneDebuff(true);
				break;
			case AbilityState.Endure:
			{
				for (int i = 0; i < AbilityData.ABILITY_STATE_CONTROL_DEBUFFS.Length; i++)
				{
					AbilityState state2 = AbilityData.ABILITY_STATE_CONTROL_DEBUFFS[i];
					abilityPlugin.RemoveModifierByState(state2);
					SetAbilityStateImmune(state2, true);
				}
				if (!muteDisplayEffect)
				{
					flag = true;
					entity.SetShaderDataLerp(E_ShaderData.Endure, true);
				}
				break;
			}
			case AbilityState.Paralyze:
				_paralyzeAniDefenceStackIx = PushProperty("Actor_AniDefenceDelta", -0.2f);
				break;
			case AbilityState.WitchTimeSlowed:
				entity.PushTimeScale(0.1f, 1);
				if (!muteDisplayEffect)
				{
					flag = true;
					entity.SetShaderDataLerp(E_ShaderData.ColorBias, true);
				}
				break;
			case AbilityState.MaxMoveSpeed:
				_maxSpeedPropertyIx = entity.PushProperty("Animator_MoveSpeedRatio", 999999f);
				break;
			case AbilityState.Frozen:
				if (!muteDisplayEffect)
				{
					flag = true;
					entity.SetShaderData(E_ShaderData.Frozon, true);
				}
				break;
			}
			if (!muteDisplayEffect && (flag || ABILITY_EFFECT_MAP.ContainsKey(state)))
			{
				int index = _abilityStateEffectIxLs.SeekAddPosition();
				_abilityStateEffectIxLs[index] = Tuple.Create(state, (!flag) ? entity.AttachEffect(ABILITY_EFFECT_MAP[state]) : (-1));
			}
		}

		protected virtual void OnAbilityStateRemove(AbilityState state)
		{
			bool flag = false;
			for (int i = 0; i < _abilityStateEffectIxLs.Count; i++)
			{
				if (_abilityStateEffectIxLs[i] != null && _abilityStateEffectIxLs[i].Item1 == state)
				{
					if (_abilityStateEffectIxLs[i].Item2 == -1)
					{
						flag = true;
					}
					else
					{
						entity.DetachEffect(_abilityStateEffectIxLs[i].Item2);
					}
					_abilityStateEffectIxLs[i] = null;
				}
			}
			switch (state)
			{
			case AbilityState.Invincible:
				SetImmuneDebuff(false);
				if (flag)
				{
					entity.SetShaderDataLerp(E_ShaderData.Invincible, false);
				}
				break;
			case AbilityState.Immune:
				SetImmuneDebuff(false);
				break;
			case AbilityState.Endure:
			{
				for (int j = 0; j < AbilityData.ABILITY_STATE_CONTROL_DEBUFFS.Length; j++)
				{
					AbilityState state2 = AbilityData.ABILITY_STATE_CONTROL_DEBUFFS[j];
					SetAbilityStateImmune(state2, false);
				}
				if (flag)
				{
					entity.SetShaderDataLerp(E_ShaderData.Endure, false);
				}
				break;
			}
			case AbilityState.Paralyze:
				PopProperty("Actor_AniDefenceDelta", _paralyzeAniDefenceStackIx);
				break;
			case AbilityState.WitchTimeSlowed:
				entity.PopTimeScale(1);
				_witchTimeResumeTimer.Reset(false);
				if (flag)
				{
					entity.SetShaderDataLerp(E_ShaderData.ColorBias, false);
				}
				break;
			case AbilityState.MaxMoveSpeed:
				entity.PopProperty("Animator_MoveSpeedRatio", _maxSpeedPropertyIx);
				break;
			case AbilityState.Frozen:
				if (flag)
				{
					entity.SetShaderData(E_ShaderData.Frozon, false);
				}
				break;
			}
		}

		public string GetAbilityNameByID(string abilityID)
		{
			return abilityIDMap[abilityID];
		}

		public bool HasAppliedAbilityName(string abilityName)
		{
			foreach (Tuple<ConfigAbility, Dictionary<string, object>> appliedAbility in appliedAbilities)
			{
				if (appliedAbility.Item1.AbilityName == abilityName)
				{
					return true;
				}
			}
			return false;
		}

		public override bool OnEventResolves(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHitResolve((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnBeingHitResolve(EvtBeingHit evt)
		{
			AbilityBeingHit(evt);
			return false;
		}

		public override void Core()
		{
			base.Core();
			_witchTimeResumeTimer.Core(1f);
			if (_witchTimeResumeTimer.isTimeUp)
			{
				entity.SetTimeScale(0.1f, 1);
				_witchTimeResumeTimer.Reset(false);
				_witchTimeResumeTimer.timespan = 0.5f;
			}
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			onHPChanged = null;
			onSPChanged = null;
			onMaxHPChanged = null;
			onMaxSPChanged = null;
			onAbilityStateAdd = null;
			onAbilityStateRemove = null;
		}

		public void AbilityBeingHit(EvtBeingHit evt)
		{
			if ((abilityState & AbilityState.WitchTimeSlowed) == 0)
			{
				return;
			}
			float timespan = 0.5f;
			if (evt.animEventID != null && Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) == 3)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.sourceID);
				if (actor != null && actor.config != null)
				{
					ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(actor.config, evt.animEventID);
					if (configAvatarAnimEvent != null && configAvatarAnimEvent.WitchTimeResume != null)
					{
						timespan = configAvatarAnimEvent.WitchTimeResume.ResumeTime;
					}
				}
			}
			if (evt.attackData.isAnimEventAttack && evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				entity.SetTimeScale(1f, 1);
				_witchTimeResumeTimer.timespan = timespan;
				_witchTimeResumeTimer.Reset(true);
			}
		}
	}
}
