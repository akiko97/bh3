using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterSuicideAttack : BaseAbilityMixin
	{
		private const float WARNING_SPEED_RATIO = 0.9f;

		private const float BASIC_WARNING_SPEED_RATIO = 0.15f;

		private const float MIN_WARNING_INTERVAL = 0.1f;

		private const float MAX_BRIGHTNESS = 6f;

		private MonsterSuicideAttackMixin config;

		private EntityTimer _suicideTimer;

		private EntityTimer _warningTimer;

		private bool _isTouchExplode;

		private string _onTouchTriggerID;

		private float _basicBrightness = 1f;

		private string _brightnessPropertyName = "_Emission";

		private SkinnedMeshRenderer _skinedMeshRenderer;

		public AbilityMonsterSuicideAttack(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterSuicideAttackMixin)config;
			_suicideTimer = new EntityTimer(instancedAbility.Evaluate(this.config.SuicideCountDownDuration));
			_warningTimer = new EntityTimer(instancedAbility.Evaluate(this.config.SuicideCountDownDuration) * 0.15f);
			_isTouchExplode = this.config.IsTouchExplode;
			_onTouchTriggerID = this.config.OnTouchTriggerID;
			_skinedMeshRenderer = actor.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
			if (_skinedMeshRenderer != null)
			{
				_basicBrightness = _skinedMeshRenderer.sharedMaterial.GetFloat(_brightnessPropertyName);
			}
		}

		public override void Core()
		{
			_suicideTimer.Core(1f);
			_warningTimer.Core(1f);
			if (_skinedMeshRenderer != null)
			{
				float t = _warningTimer.timer / _warningTimer.timespan;
				float value = Mathf.Lerp(_basicBrightness, 6f, t);
				_skinedMeshRenderer.sharedMaterial.SetFloat(_brightnessPropertyName, value);
			}
			if (_warningTimer.isTimeUp)
			{
				_warningTimer.timespan *= 0.9f;
				_warningTimer.timespan = Mathf.Max(0.1f, _warningTimer.timespan);
				_warningTimer.Reset(true);
			}
			if (_suicideTimer.isTimeUp)
			{
				MonsterActor monsterActor = actor as MonsterActor;
				monsterActor.needDropReward = false;
				monsterActor.ForceKill(actor.runtimeID, KillEffect.KillImmediately);
				_suicideTimer.Reset(false);
				_warningTimer.Reset(false);
			}
			if (!string.IsNullOrEmpty(_onTouchTriggerID))
			{
				actor.entity.ResetTrigger(_onTouchTriggerID);
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return OnKilled((EvtKilled)evt);
			}
			if (evt is EvtTouch)
			{
				return OnTouch((EvtTouch)evt);
			}
			return false;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (evt.abilityArgument != null)
			{
				MonsterSuicideAttackMixinArgument monsterSuicideAttackMixinArgument = evt.abilityArgument as MonsterSuicideAttackMixinArgument;
				if (monsterSuicideAttackMixinArgument != null)
				{
					_suicideTimer.timespan = monsterSuicideAttackMixinArgument.SuicideCountDown;
					_warningTimer.timespan = monsterSuicideAttackMixinArgument.BeapInterval;
					_isTouchExplode = monsterSuicideAttackMixinArgument.SuicideOnTouch;
					if (!string.IsNullOrEmpty(monsterSuicideAttackMixinArgument.OnTouchTriggerID))
					{
						_onTouchTriggerID = monsterSuicideAttackMixinArgument.OnTouchTriggerID;
					}
				}
			}
			_suicideTimer.Reset(true);
			_warningTimer.Reset(true);
		}

		private bool OnKilled(EvtKilled evt)
		{
			_suicideTimer.Reset(false);
			if (evt.killerID == actor.runtimeID)
			{
				entity.TriggerAttackPattern(config.SuicideHitAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Enemy));
				if (config.SuicideHitAlliedAnimEventID != null)
				{
					entity.TriggerAttackPattern(config.SuicideHitAlliedAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Allied));
				}
				FireMixinEffect(config.SuicideEffect, entity);
			}
			else
			{
				entity.TriggerAttackPattern(config.KilledHitAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Enemy));
				if (config.KilledHitAlliedAnimEventID != null)
				{
					entity.TriggerAttackPattern(config.KilledHitAlliedAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Allied));
				}
				FireMixinEffect(config.KilledEffect, entity);
			}
			return true;
		}

		private bool OnTouch(EvtTouch evt)
		{
			if (_suicideTimer.isActive && _isTouchExplode)
			{
				actor.ForceKill(actor.runtimeID, KillEffect.KillImmediately);
				_suicideTimer.Reset(false);
				_warningTimer.Reset(false);
			}
			if (_suicideTimer.isActive && !string.IsNullOrEmpty(_onTouchTriggerID))
			{
				actor.entity.SetTrigger(_onTouchTriggerID);
			}
			return true;
		}
	}
}
