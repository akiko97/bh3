using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilitySmokeBombMixin : BaseAbilityMixin
	{
		private SmokeBombMixin config;

		private EntityTimer _buffTimer;

		private Vector3 _position;

		private Vector3 _dir;

		private List<BaseAbilityActor> _insideAlliedActors;

		private List<BaseAbilityActor> _insideEnemyActors;

		private AbilityTriggerField _alliedFieldActor;

		private AbilityTriggerField _enemyFieldActor;

		private bool _isSmokeOn;

		private bool _isSmokeAvaliable;

		private List<MonoEffect> _smokeOnEffects;

		private List<MonoEffect> _smokeOffEffects;

		public AbilitySmokeBombMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (SmokeBombMixin)config;
			_buffTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration), entity);
			_buffTimer.SetActive(false);
			_isSmokeOn = false;
			_isSmokeAvaliable = false;
			_insideAlliedActors = new List<BaseAbilityActor>();
			_insideEnemyActors = new List<BaseAbilityActor>();
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
			if (_isSmokeOn)
			{
				StopAreaLastingBuff();
			}
		}

		public override void Core()
		{
			_buffTimer.Core(1f);
			if (_buffTimer.isTimeUp)
			{
				StopAreaLastingBuff();
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter)
			{
				return ListenFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtFieldExit)
			{
				return ListenFieldExit((EvtFieldExit)evt);
			}
			return false;
		}

		private void StartAreaLastingBuff()
		{
			_isSmokeOn = true;
			_isSmokeAvaliable = true;
			_position = entity.XZPosition;
			_dir = entity.transform.forward;
			_insideAlliedActors.Clear();
			_insideEnemyActors.Clear();
			_alliedFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(_position, entity.transform.forward, actor, instancedAbility.Evaluate(config.Radius), MixinTargetting.Allied, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID());
			_enemyFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(_position, entity.transform.forward, actor, instancedAbility.Evaluate(config.Radius), MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID());
			SetSmokeOnEffects();
			_buffTimer.Reset(true);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(actor.runtimeID);
		}

		private void StopAreaLastingBuff()
		{
			_isSmokeOn = false;
			_isSmokeAvaliable = false;
			_buffTimer.Reset(false);
			_alliedFieldActor.Kill();
			_enemyFieldActor.Kill();
			for (int i = 0; i < _insideAlliedActors.Count; i++)
			{
				TryRemoveModifierOn(_insideAlliedActors[i], config.InSmokeModifiers);
				TryRemoveModifierOn(_insideAlliedActors[i], config.Modifiers);
			}
			_insideAlliedActors.Clear();
			_insideEnemyActors.Clear();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(actor.runtimeID);
			DestroyEffects(_smokeOnEffects);
			DestroyEffects(_smokeOffEffects);
		}

		private void ApplyModifierOn(BaseAbilityActor actor, string[] modifiers)
		{
			if (actor != null && actor.IsActive())
			{
				for (int i = 0; i < modifiers.Length; i++)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, modifiers[i]);
				}
			}
		}

		private void TryRemoveModifierOn(BaseAbilityActor actor, string[] modifiers)
		{
			if (actor != null)
			{
				for (int i = 0; i < modifiers.Length; i++)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, modifiers[i]);
				}
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (!_isSmokeOn)
			{
				StartAreaLastingBuff();
			}
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			if (!_isSmokeOn)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (evt.targetID == _alliedFieldActor.runtimeID)
			{
				OnAlliedEnter(baseAbilityActor);
				return true;
			}
			if (evt.targetID == _enemyFieldActor.runtimeID)
			{
				OnEnemyEnter(baseAbilityActor);
				return true;
			}
			return false;
		}

		private bool ListenFieldExit(EvtFieldExit evt)
		{
			if (!_isSmokeOn)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (evt.targetID == _alliedFieldActor.runtimeID)
			{
				OnAlliedExit(baseAbilityActor);
				return true;
			}
			if (evt.targetID == _enemyFieldActor.runtimeID)
			{
				OnEnemyExit(baseAbilityActor);
				return true;
			}
			return false;
		}

		private void OnAlliedEnter(BaseAbilityActor actor)
		{
			_insideAlliedActors.Add(actor);
			ApplyModifierOn(actor, config.Modifiers);
			if (_isSmokeAvaliable)
			{
				ApplyModifierOn(actor, config.InSmokeModifiers);
			}
		}

		private void OnAlliedExit(BaseAbilityActor actor)
		{
			_insideAlliedActors.Remove(actor);
			TryRemoveModifierOn(actor, config.Modifiers);
			TryRemoveModifierOn(actor, config.InSmokeModifiers);
		}

		private void OnEnemyEnter(BaseAbilityActor actor)
		{
			_insideEnemyActors.Add(actor);
			if (_isSmokeAvaliable)
			{
				_isSmokeAvaliable = false;
				for (int i = 0; i < _insideAlliedActors.Count; i++)
				{
					TryRemoveModifierOn(_insideAlliedActors[i], config.InSmokeModifiers);
				}
				SetSmokeOffEffects();
			}
		}

		private void OnEnemyExit(BaseAbilityActor actor)
		{
			_insideEnemyActors.Remove(actor);
			if (!_isSmokeAvaliable && _insideEnemyActors.Count == 0)
			{
				_isSmokeAvaliable = true;
				for (int i = 0; i < _insideAlliedActors.Count; i++)
				{
					ApplyModifierOn(_insideAlliedActors[i], config.InSmokeModifiers);
				}
				SetSmokeOnEffects();
			}
		}

		private void SetSmokeOnEffects()
		{
			if (_smokeOnEffects == null || _smokeOnEffects.Count == 0)
			{
				_smokeOnEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.SmokeOnEffect.EffectPattern, _position, _dir, Vector3.one, entity);
			}
			DestroyEffects(_smokeOffEffects);
		}

		private void SetSmokeOffEffects()
		{
			if (_smokeOffEffects == null || _smokeOffEffects.Count == 0)
			{
				_smokeOffEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.SmokeOffEffect.EffectPattern, _position, _dir, Vector3.one, entity);
			}
			DestroyEffects(_smokeOnEffects);
		}

		private void DestroyEffects(List<MonoEffect> _effects)
		{
			if (_effects == null)
			{
				return;
			}
			foreach (MonoEffect _effect in _effects)
			{
				ParticleSystem componentInChildren = _effect.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren != null)
				{
					componentInChildren.Stop(true);
				}
			}
			_effects.Clear();
		}
	}
}
