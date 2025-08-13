using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityMonsterAreaLastingBuffMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Buffing = 1
		}

		private MonsterAreaLastingBuffMixin config;

		private EntityTimer _buffTimer;

		private List<BaseAbilityActor> _insideActors;

		private AbilityTriggerField _fieldActor;

		private State _state;

		public AbilityMonsterAreaLastingBuffMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterAreaLastingBuffMixin)config;
			_buffTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration), entity);
			_buffTimer.SetActive(false);
			_state = State.Idle;
			_insideActors = new List<BaseAbilityActor>();
		}

		public override void OnAdded()
		{
			if (config.TriggerOnAdded)
			{
				StartAreaLastingBuff();
			}
		}

		public override void OnRemoved()
		{
			if (_state == State.Buffing)
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
				if (!string.IsNullOrEmpty(config.BuffTimeRatioAnimatorParam))
				{
					(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.BuffTimeRatioAnimatorParam, 0f);
				}
			}
			else if (!string.IsNullOrEmpty(config.BuffTimeRatioAnimatorParam))
			{
				(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.BuffTimeRatioAnimatorParam, _buffTimer.GetTimingRatio());
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

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnPostBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private void StartAreaLastingBuff()
		{
			_state = State.Buffing;
			_fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(entity.XZPosition, entity.transform.forward, actor, instancedAbility.Evaluate(config.Radius), config.Target, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), true);
			if (!config.TriggerOnAdded)
			{
				_buffTimer.Reset(true);
			}
			actor.abilityPlugin.ApplyModifier(instancedAbility, config.SelfLastingModifierName);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(actor.runtimeID);
		}

		private void StopAreaLastingBuff()
		{
			_state = State.Idle;
			_buffTimer.Reset(false);
			_fieldActor.Kill();
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.SelfLastingModifierName);
			for (int i = 0; i < _insideActors.Count; i++)
			{
				TryRemoveModifierOn(_insideActors[i]);
			}
			_insideActors.Clear();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(actor.runtimeID);
			if (!string.IsNullOrEmpty(config.BuffDurationEndTrigger))
			{
				entity.SetTrigger(config.BuffDurationEndTrigger);
			}
		}

		private void ApplyModifierOn(BaseAbilityActor actor)
		{
			if (actor != null && actor.IsActive())
			{
				for (int i = 0; i < config.ModifierNames.Length; i++)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
		}

		private void TryRemoveModifierOn(BaseAbilityActor actor)
		{
			if (actor != null)
			{
				for (int i = 0; i < config.ModifierNames.Length; i++)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (_state == State.Idle)
			{
				StartAreaLastingBuff();
			}
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			if (_state != State.Buffing || evt.targetID != _fieldActor.runtimeID || (!config.IncludeSelf && evt.otherID == actor.runtimeID))
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
			if (_state != State.Buffing || evt.targetID != _fieldActor.runtimeID)
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

		private bool OnPostBeingHit(EvtBeingHit evt)
		{
			if (_state != State.Buffing || config.HitBreakType == MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.Normal)
			{
				return false;
			}
			if (config.HitBreakType == MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.ConvertAllHitsToLightHit)
			{
				if (evt.attackData.attackerAniDamageRatio < evt.attackData.attackeeAniDefenceRatio)
				{
					evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
				}
				if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
				{
					StopAreaLastingBuff();
				}
			}
			else if (config.HitBreakType == MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.BreakingHitCancels && evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				StopAreaLastingBuff();
			}
			return false;
		}
	}
}
