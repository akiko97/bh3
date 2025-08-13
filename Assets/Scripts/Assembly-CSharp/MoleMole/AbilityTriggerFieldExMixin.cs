using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityTriggerFieldExMixin : BaseAbilityMixin
	{
		private TriggerFieldExMixin config;

		private EntityTimer _timer;

		private EntityTimer _createEffectDelayTimer;

		private List<BaseAbilityActor> _insideActors;

		private AbilityTriggerField _fieldActor;

		private int _durationEffectIx;

		private int _creationEffectIx;

		public AbilityTriggerFieldExMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (TriggerFieldExMixin)config;
			_timer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration));
			_timer.SetActive(false);
			_createEffectDelayTimer = new EntityTimer(this.config.CreateEffectDelay);
			_insideActors = new List<BaseAbilityActor>();
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(actor.runtimeID);
			if (config.TriggerOnAdded)
			{
				CreateField(null);
				StartCreateEffect();
			}
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(actor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapOutStart>(actor.runtimeID);
			if ((_timer.isActive && !_timer.isTimeUp) || config.TriggerOnAdded)
			{
				StopField();
			}
		}

		public override void Core()
		{
			if (_createEffectDelayTimer.isActive && !_createEffectDelayTimer.isTimeUp)
			{
				_createEffectDelayTimer.Core(1f);
				if (_createEffectDelayTimer.isTimeUp)
				{
					StartField();
				}
			}
			if (_timer.isActive && !_timer.isTimeUp)
			{
				_timer.Core(1f);
				if (_timer.isTimeUp)
				{
					StopField();
				}
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
			if (evt is EvtAvatarSwapOutStart)
			{
				return ListenAvatarSwapOut((EvtAvatarSwapOutStart)evt);
			}
			return false;
		}

		private bool ListenAvatarSwapOut(EvtAvatarSwapOutStart evt)
		{
			if (evt.targetID == actor.runtimeID && _timer.isActive && !_timer.isTimeUp && config.DestoryAfterSwitch)
			{
				StopField();
			}
			return false;
		}

		private void StartCreateEffect()
		{
			_createEffectDelayTimer.Reset(true);
			if (config.CreationEffect != null)
			{
				_creationEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(config.CreationEffect.EffectPattern, _fieldActor.triggerField);
			}
		}

		private void StartDestroyEffect()
		{
			if (config.DestroyEffect != null)
			{
				Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(config.DestroyEffect.EffectPattern, _fieldActor.triggerField);
			}
		}

		private void StartField()
		{
			if (!config.TriggerOnAdded)
			{
				_timer.Reset(true);
			}
			if (config.DurationEffect != null && config.DurationEffect.EffectPattern != null)
			{
				_durationEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(config.DurationEffect.EffectPattern, _fieldActor.triggerField);
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.OnStartCasterActions, instancedAbility, instancedModifier, actor, null);
		}

		private void StopField()
		{
			_timer.Reset(false);
			StartDestroyEffect();
			_fieldActor.Kill();
			for (int i = 0; i < _insideActors.Count; i++)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.OnDestroyTargetActions, instancedAbility, instancedModifier, _insideActors[i], null);
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.OnDestroyCasterActions, instancedAbility, instancedModifier, actor, null);
			for (int j = 0; j < _insideActors.Count; j++)
			{
				TryRemoveModifierOn(_insideActors[j]);
			}
			_insideActors.Clear();
			if (config.DurationEffect != null && config.DurationEffect.EffectPattern != null)
			{
				Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(_durationEffectIx);
			}
			if (config.CreationEffect != null && config.CreationEffect.EffectPattern != null)
			{
				Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(_creationEffectIx);
			}
		}

		private void ApplyModifierOn(BaseAbilityActor actor)
		{
			if (actor != null && actor.IsActive())
			{
				for (int i = 0; i < config.TargetModifiers.Length; i++)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.TargetModifiers[i]);
				}
			}
		}

		private void TryRemoveModifierOn(BaseAbilityActor actor)
		{
			if (actor != null)
			{
				for (int i = 0; i < config.TargetModifiers.Length; i++)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.TargetModifiers[i]);
				}
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			CreateField(evt);
			StartCreateEffect();
			if (!config.ApplyAttackerWitchTimeRatio || evt.TriggerEvent == null)
			{
				return;
			}
			EvtEvadeSuccess evtEvadeSuccess = evt.TriggerEvent as EvtEvadeSuccess;
			if (evtEvadeSuccess == null)
			{
				return;
			}
			MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evtEvadeSuccess.attackerID);
			if (monsterActor != null)
			{
				ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(monsterActor.config, evtEvadeSuccess.skillID);
				if (configMonsterAnimEvent != null)
				{
					_timer.timespan = instancedAbility.Evaluate(config.Duration) * configMonsterAnimEvent.AttackProperty.WitchTimeRatio;
				}
			}
		}

		private void CreateField(EvtAbilityStart evt)
		{
			if (_timer.isActive)
			{
				StopField();
			}
			BaseMonoEntity baseMonoEntity = null;
			float num = instancedAbility.Evaluate(config.CreationZOffset);
			float num2 = instancedAbility.Evaluate(config.CreationXOffset);
			switch (config.TriggerPositionType)
			{
			case TriggerFieldExMixin.PositionType.Target:
				if (evt.otherID != 0)
				{
					BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
					if (baseAbilityActor != null && baseAbilityActor.entity != null)
					{
						baseMonoEntity = baseAbilityActor.entity;
					}
				}
				break;
			case TriggerFieldExMixin.PositionType.AttackTarget:
				baseMonoEntity = entity.GetAttackTarget();
				if (baseMonoEntity == null)
				{
					num += instancedAbility.Evaluate(config.NoAttackTargetZOffset);
				}
				break;
			case TriggerFieldExMixin.PositionType.Caster:
				baseMonoEntity = actor.entity;
				break;
			}
			Vector3 vector = Vector3.Cross(Vector3.up, entity.transform.forward).normalized * num2;
			Vector3 initPos;
			if (baseMonoEntity != null)
			{
				initPos = baseMonoEntity.XZPosition + new Vector3(0f, 0.5f, 0f) + vector;
			}
			else
			{
				baseMonoEntity = actor.entity;
				Vector3 origin = entity.XZPosition + new Vector3(0f, 0.5f, 0f) + vector;
				initPos = CollisionDetectPattern.GetRaycastPoint(origin, entity.transform.forward, num, 0.2f, 1 << InLevelData.STAGE_COLLIDER_LAYER);
			}
			_fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(initPos, baseMonoEntity.transform.forward, actor, instancedAbility.Evaluate(config.Radius), config.Targetting, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), config.Follow);
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			if (_fieldActor == null || _fieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (baseAbilityActor != null)
			{
				ApplyModifierOn(baseAbilityActor);
				_insideActors.Add(baseAbilityActor);
			}
			return true;
		}

		private bool ListenFieldExit(EvtFieldExit evt)
		{
			if (_fieldActor == null || _fieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (baseAbilityActor != null)
			{
				TryRemoveModifierOn(baseAbilityActor);
				_insideActors.Remove(baseAbilityActor);
			}
			return false;
		}
	}
}
