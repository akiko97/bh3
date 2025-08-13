using FullInspector;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityGlobalMainShieldMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			ShieldBroken = 1,
			Resuming = 2
		}

		private GlobalMainShieldMixin config;

		public float maxShield;

		public static string GLOBAL_SHIELD_KEY = "GlobalShield";

		private EntityTimer _betweenAttackResumeTimer;

		private EntityTimer _forceResumeTimer;

		private EntityTimer _minForceResumeTimer;

		private EntityTimer _shieldResumeTimer;

		private float _shieldResumeRatio = 1f;

		private float _shieldResumeStart;

		private float _shieldResumeTarget;

		private bool _showShieldBar;

		private DisplayValue<float> _shieldDisplay;

		private float _displayFloor;

		private DynamicActorValue<float> _globalShieldValue;

		[ShowInInspector]
		private State _state;

		private AbilityState _controlledAbilityState;

		public AbilityGlobalMainShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (GlobalMainShieldMixin)config;
			if (this.config.UseLevelTimeScale)
			{
				_betweenAttackResumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BetweenAttackResumeCD));
				_forceResumeTimer = new EntityTimer(this.config.ForceResumeCD);
				_minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD);
				_shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan);
			}
			else
			{
				_betweenAttackResumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BetweenAttackResumeCD), entity);
				_forceResumeTimer = new EntityTimer(this.config.ForceResumeCD, entity);
				_minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD, entity);
				_shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan, entity);
			}
		}

		public override void OnAdded()
		{
			_state = State.Idle;
			maxShield = (float)actor.baseMaxHP * instancedAbility.Evaluate(config.ShieldHPRatio) + instancedAbility.Evaluate(config.ShieldHP);
			_forceResumeTimer.SetActive(false);
			_minForceResumeTimer.SetActive(false);
			_shieldResumeTimer.SetActive(false);
			_globalShieldValue = actor.abilityPlugin.CreateOrGetDynamicFloat(GLOBAL_SHIELD_KEY, maxShield);
			_globalShieldValue.Pub(maxShield);
			BaseAbilityActor[] alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor);
			foreach (BaseAbilityActor baseAbilityActor in alliedActorsOf)
			{
				baseAbilityActor.abilityPlugin.ApplyModifier(instancedAbility, config.ChildShieldModifierName);
			}
			if (config.ControlledAbilityStates != null)
			{
				for (int j = 0; j < config.ControlledAbilityStates.Length; j++)
				{
					_controlledAbilityState |= config.ControlledAbilityStates[j];
				}
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			_globalShieldValue.Pub(0f);
			BaseAbilityActor[] alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor);
			foreach (BaseAbilityActor baseAbilityActor in alliedActorsOf)
			{
				baseAbilityActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ChildShieldModifierName);
				if (!string.IsNullOrEmpty(config.ShieldOffModifierName))
				{
					baseAbilityActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOffModifierName);
				}
			}
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(actor.runtimeID);
		}

		public override void Core()
		{
			if (!actor.isAlive)
			{
				return;
			}
			_betweenAttackResumeTimer.Core(1f);
			_forceResumeTimer.Core(_shieldResumeRatio);
			_minForceResumeTimer.Core(1f);
			_shieldResumeTimer.Core(1f);
			if (_state == State.Idle)
			{
				if (_globalShieldValue.Value <= 0f)
				{
					_state = State.ShieldBroken;
					_forceResumeTimer.SetActive(true);
					_minForceResumeTimer.SetActive(true);
					_globalShieldValue.Pub(0f);
					BaseAbilityActor[] alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor);
					foreach (BaseAbilityActor baseAbilityActor in alliedActorsOf)
					{
						if (!string.IsNullOrEmpty(config.ShieldOffModifierName))
						{
							baseAbilityActor.abilityPlugin.ApplyModifier(instancedAbility, config.ShieldOffModifierName);
						}
					}
					actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldBrokenActions, instancedAbility, instancedModifier, null, null);
				}
				else if (_betweenAttackResumeTimer.isTimeUp && _globalShieldValue.Value < maxShield)
				{
					_state = State.Resuming;
					_shieldResumeStart = _globalShieldValue.Value;
					_shieldResumeTarget = maxShield;
					_shieldResumeTimer.Reset(true);
					_betweenAttackResumeTimer.Reset(false);
				}
			}
			else if (_state == State.ShieldBroken)
			{
				_shieldResumeRatio = GetShieldResumeRatio();
				if ((!_forceResumeTimer.isTimeUp || !_minForceResumeTimer.isTimeUp) && (!_betweenAttackResumeTimer.isTimeUp || !(_globalShieldValue.Value < maxShield)))
				{
					return;
				}
				_state = State.Resuming;
				_shieldResumeStart = _globalShieldValue.Value;
				_shieldResumeTarget = maxShield;
				_shieldResumeTimer.Reset(true);
				_forceResumeTimer.Reset(false);
				_betweenAttackResumeTimer.Reset(false);
				_globalShieldValue.Pub(0f);
				BaseAbilityActor[] alliedActorsOf2 = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor);
				foreach (BaseAbilityActor baseAbilityActor2 in alliedActorsOf2)
				{
					if (!string.IsNullOrEmpty(config.ShieldOffModifierName))
					{
						baseAbilityActor2.abilityPlugin.TryRemoveModifier(instancedAbility, config.ShieldOffModifierName);
					}
				}
			}
			else if (_state == State.Resuming)
			{
				float newValue = Mathf.Lerp(_shieldResumeStart, _shieldResumeTarget, _shieldResumeTimer.GetTimingRatio());
				_globalShieldValue.Pub(newValue);
				if (_shieldResumeTimer.isTimeUp)
				{
					_state = State.Idle;
					_shieldResumeTimer.timespan = config.ShieldResumeTimeSpan;
					actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldResumeActions, instancedAbility, instancedModifier, null, null);
				}
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return OnKilled((EvtKilled)evt);
			}
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnKilled(EvtKilled evt)
		{
			_globalShieldValue.Pub(0f);
			return true;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID);
			if (baseAbilityActor == null || !(baseAbilityActor is AvatarActor))
			{
				return false;
			}
			if (baseAbilityActor.abilityState.ContainsState(AbilityState.Invincible) || baseAbilityActor.abilityState.ContainsState(AbilityState.Undamagable))
			{
				return false;
			}
			_betweenAttackResumeTimer.Reset(true);
			return false;
		}

		private float GetShieldResumeRatio()
		{
			float result = 1f;
			if ((_controlledAbilityState & actor.abilityState) != AbilityState.None)
			{
				result = config.ControlledForceResumeTimeRatio;
			}
			return result;
		}
	}
}
