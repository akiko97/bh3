using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityMonsterSkillIDChargeAnimatorMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Before = 1,
			InLoop = 2,
			After = 3
		}

		private MonsterSkillIDChargeAnimatorMixin config;

		private BaseMonoMonster _monster;

		private EntityTimer _chargeTimer;

		private EntityTimer _switchTimer;

		private EntityTimer _triggeredChargeTimer;

		private State _state;

		protected int _loopIx;

		protected int _loopCount;

		private float _chargeTimeRatio;

		private int _chargeEffectPatternIx;

		private string _chargeAudioLoopName;

		private string _lastFrom;

		public AbilityMonsterSkillIDChargeAnimatorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterSkillIDChargeAnimatorMixin)config;
			_monster = (BaseMonoMonster)entity;
			_chargeTimer = new EntityTimer();
			_triggeredChargeTimer = new EntityTimer();
			_switchTimer = new EntityTimer(this.config.ChargeSwitchWindow, entity);
			_loopCount = this.config.ChargeLoopSkillIDs.Length;
			_chargeTimeRatio = instancedAbility.Evaluate(this.config.ChargeTimeRatio);
		}

		public override void OnAdded()
		{
			BaseMonoMonster monster = _monster;
			monster.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(monster.onCurrentSkillIDChanged, new Action<string, string>(WithTransientSkillIDChangedCallback));
			_state = State.Idle;
			_chargeTimer.Reset(false);
			_switchTimer.Reset(false);
			_loopIx = 0;
			_chargeEffectPatternIx = -1;
			if (config.ChargeLoopEffects != null && config.ChargeSwitchEffects == null)
			{
			}
		}

		public override void OnRemoved()
		{
			BaseMonoMonster monster = _monster;
			monster.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(monster.onCurrentSkillIDChanged, new Action<string, string>(WithTransientSkillIDChangedCallback));
			if (_chargeEffectPatternIx != -1)
			{
				entity.DetachEffectImmediately(_chargeEffectPatternIx);
			}
			if (_chargeAudioLoopName != null)
			{
				entity.StopAudio(_chargeAudioLoopName);
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			float timespan = (float)evt.abilityArgument;
			_triggeredChargeTimer.timespan = timespan;
			_triggeredChargeTimer.Reset(true);
		}

		public override void Core()
		{
			if (_triggeredChargeTimer.isActive && _state != State.Idle)
			{
				_triggeredChargeTimer.Core(1f);
				if (_state == State.Before)
				{
					if (_triggeredChargeTimer.isTimeUp)
					{
						_monster.ResetTrigger(config.NextLoopTriggerID);
						_monster.SetTrigger(config.AfterSkillTriggerID);
					}
					else
					{
						_monster.ResetTrigger(config.AfterSkillTriggerID);
						_monster.SetTrigger(config.NextLoopTriggerID);
					}
				}
				else if (_state == State.InLoop && _triggeredChargeTimer.isTimeUp)
				{
					_monster.SetTrigger(config.AfterSkillTriggerID);
				}
			}
			if (_state != State.InLoop)
			{
				return;
			}
			_chargeTimer.Core(1f);
			if (_chargeTimer.isTimeUp)
			{
				_loopIx++;
				if (_loopIx == _loopCount)
				{
					_monster.SetTrigger(config.AfterSkillTriggerID);
					_chargeTimer.Reset(false);
				}
				else
				{
					_monster.SetTrigger(config.NextLoopTriggerID);
					_chargeTimer.timespan = config.ChargeLoopDurations[_loopIx] * _chargeTimeRatio;
					_chargeTimer.Reset(true);
					_switchTimer.Reset(true);
				}
			}
			_switchTimer.Core(1f);
			if (_switchTimer.isTimeUp)
			{
				_switchTimer.Reset(false);
			}
		}

		private void WithTransientSkillIDChangedCallback(string from, string to)
		{
			if (Miscs.ArrayContains(config.TransientSkillIDs, to))
			{
				_lastFrom = from;
			}
			else if (Miscs.ArrayContains(config.TransientSkillIDs, from))
			{
				SkillIDChangedCallback(_lastFrom, to);
			}
			else
			{
				SkillIDChangedCallback(from, to);
			}
		}

		private bool IsTriggerCharging()
		{
			return _triggeredChargeTimer.isActive && !_triggeredChargeTimer.isTimeUp;
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_state == State.Idle)
			{
				if (Miscs.ArrayContains(config.BeforeSkillIDs, to))
				{
					if (IsTriggerCharging())
					{
						_monster.ResetTrigger(config.AfterSkillTriggerID);
						_monster.SetTrigger(config.NextLoopTriggerID);
						_state = State.Before;
						_loopIx = 0;
					}
					else
					{
						_monster.ResetTrigger(config.NextLoopTriggerID);
						_monster.SetTrigger(config.AfterSkillTriggerID);
					}
				}
			}
			else if (_state == State.Before)
			{
				if (to == config.ChargeLoopSkillIDs[_loopIx])
				{
					if (config.ChargeLoopEffects != null)
					{
						MixinEffect mixinEffect = config.ChargeLoopEffects[_loopIx];
						if (mixinEffect.EffectPattern != null)
						{
							_chargeEffectPatternIx = entity.AttachEffect(mixinEffect.EffectPattern);
						}
						if (mixinEffect.AudioPattern != null)
						{
							_chargeAudioLoopName = mixinEffect.AudioPattern;
							entity.PlayAudio(mixinEffect.AudioPattern);
						}
					}
					_state = State.InLoop;
					_chargeTimer.timespan = config.ChargeLoopDurations[_loopIx] * _chargeTimeRatio;
					_chargeTimer.Reset(true);
				}
				else if (Miscs.ArrayContains(config.AfterSkillIDs, to))
				{
					_monster.ResetTrigger(config.AfterSkillTriggerID);
					_monster.ResetTrigger(config.NextLoopTriggerID);
					_state = State.After;
				}
				else if (!Miscs.ArrayContains(config.BeforeSkillIDs, to))
				{
					_monster.ResetTrigger(config.AfterSkillTriggerID);
					_monster.ResetTrigger(config.NextLoopTriggerID);
					_state = State.Idle;
				}
			}
			else if (_state == State.InLoop)
			{
				if (Miscs.ArrayContains(config.ChargeLoopSkillIDs, to))
				{
					if (config.ChargeLoopEffects != null)
					{
						entity.DetachEffect(_chargeEffectPatternIx);
						if (_chargeAudioLoopName != null)
						{
							entity.StopAudio(_chargeAudioLoopName);
							_chargeAudioLoopName = null;
						}
						MixinEffect mixinEffect2 = config.ChargeLoopEffects[_loopIx];
						if (mixinEffect2.EffectPattern != null)
						{
							_chargeEffectPatternIx = entity.AttachEffect(mixinEffect2.EffectPattern);
						}
						if (mixinEffect2.AudioPattern != null)
						{
							_chargeAudioLoopName = mixinEffect2.AudioPattern;
							entity.PlayAudio(mixinEffect2.AudioPattern);
						}
						if (config.ChargeSwitchEffects != null)
						{
							FireMixinEffect(config.ChargeSwitchEffects[_loopIx - 1], entity);
						}
					}
					return;
				}
				if (Miscs.ArrayContains(config.AfterSkillIDs, to))
				{
					if (config.ChargeLoopEffects != null)
					{
						entity.DetachEffectImmediately(_chargeEffectPatternIx);
						_chargeEffectPatternIx = -1;
						if (_chargeAudioLoopName != null)
						{
							entity.StopAudio(_chargeAudioLoopName);
							_chargeAudioLoopName = null;
						}
					}
					EvtChargeRelease evtChargeRelease = new EvtChargeRelease(actor.runtimeID, to);
					evtChargeRelease.isSwitchRelease = _switchTimer.isActive && !_switchTimer.isTimeUp;
					Singleton<EventManager>.Instance.FireEvent(evtChargeRelease);
					_switchTimer.Reset(false);
					_chargeTimer.Reset(false);
					_state = State.After;
					return;
				}
				if (config.ChargeLoopEffects != null)
				{
					entity.DetachEffectImmediately(_chargeEffectPatternIx);
					_chargeEffectPatternIx = -1;
					if (_chargeAudioLoopName != null)
					{
						entity.StopAudio(_chargeAudioLoopName);
						_chargeAudioLoopName = null;
					}
				}
				_monster.ResetTrigger(config.AfterSkillTriggerID);
				_monster.ResetTrigger(config.NextLoopTriggerID);
				_state = State.Idle;
			}
			else if (_state == State.After)
			{
				if (Miscs.ArrayContains(config.BeforeSkillIDs, to) && IsTriggerCharging())
				{
					_monster.ResetTrigger(config.AfterSkillTriggerID);
					_monster.SetTrigger(config.NextLoopTriggerID);
					_state = State.Before;
					_loopIx = 0;
				}
				else
				{
					_monster.ResetTrigger(config.AfterSkillTriggerID);
					_monster.ResetTrigger(config.NextLoopTriggerID);
					_state = State.Idle;
				}
			}
		}
	}
}
