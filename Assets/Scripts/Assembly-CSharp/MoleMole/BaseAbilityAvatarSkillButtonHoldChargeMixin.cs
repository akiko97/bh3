using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAbilityAvatarSkillButtonHoldChargeMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Before = 1,
			InLoop = 2,
			After = 3
		}

		private BaseAvatarSkillButtonHoldChargeAnimatorMixin config;

		private BaseMonoAvatar _avatar;

		private EntityTimer _switchTimer;

		private List<BaseMonoEntity> _subSelectTargetList;

		private MonoSkillButton _skillButton;

		private State _state;

		protected int _loopIx;

		protected int _loopCount;

		protected float _chargeTimeRatio;

		private EntityTimer _triggeredChargeTimer;

		private bool _useTriggerTimeControl;

		private int _chargeEffectPatternIx;

		private string _chargeAudioLoopName;

		private bool _checkPointerDownInBS = true;

		private State _oldState;

		private string _lastFrom;

		public BaseAbilityAvatarSkillButtonHoldChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BaseAvatarSkillButtonHoldChargeAnimatorMixin)config;
			_avatar = (BaseMonoAvatar)entity;
			_triggeredChargeTimer = new EntityTimer();
			_switchTimer = new EntityTimer(this.config.ChargeSwitchWindow, entity);
			_loopCount = this.config.ChargeLoopSkillIDs.Length;
			_subSelectTargetList = new List<BaseMonoEntity>();
			_chargeTimeRatio = 1f;
		}

		public override void OnAdded()
		{
			BaseMonoAvatar avatar = _avatar;
			avatar.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(avatar.onCurrentSkillIDChanged, new Action<string, string>(WithTransientSkillIDChangedCallback));
			_state = State.Idle;
			_switchTimer.Reset(false);
			_loopIx = 0;
			_chargeEffectPatternIx = -1;
			_skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID);
			MonoSkillButton skillButton = _skillButton;
			skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>)Delegate.Combine(skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(SkillButtonStateChangedCallback));
			if (_avatar.IsAIActive() && !string.IsNullOrEmpty(config.ChargeTimeRatioAIKey))
			{
				(_avatar.GetActiveAIController() as BTreeAvatarAIController).SetBehaviorVariable(config.ChargeTimeRatioAIKey, _chargeTimeRatio);
			}
			if (config.ChargeLoopEffects == null || config.ChargeSwitchEffects != null)
			{
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLocalAvatarChanged>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			BaseMonoAvatar avatar = _avatar;
			avatar.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(avatar.onCurrentSkillIDChanged, new Action<string, string>(WithTransientSkillIDChangedCallback));
			MonoSkillButton skillButton = _skillButton;
			skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>)Delegate.Remove(skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(SkillButtonStateChangedCallback));
			if (_chargeEffectPatternIx != -1)
			{
				entity.DetachEffectImmediately(_chargeEffectPatternIx);
			}
			if (_chargeAudioLoopName != null)
			{
				entity.StopAudio(_chargeAudioLoopName);
			}
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLocalAvatarChanged>(actor.runtimeID);
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			float num = (float)evt.abilityArgument;
			if (num != 0f)
			{
				_triggeredChargeTimer.timespan = num;
				_triggeredChargeTimer.Reset(true);
				_useTriggerTimeControl = true;
			}
			else
			{
				_useTriggerTimeControl = false;
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLocalAvatarChanged)
			{
				return OnLocalAvatarChange((EvtLocalAvatarChanged)evt);
			}
			return false;
		}

		private bool OnLocalAvatarChange(EvtLocalAvatarChanged evt)
		{
			if (evt.localAvatarID == _avatar.GetRuntimeID())
			{
				_triggeredChargeTimer.isTimeUp = true;
			}
			return true;
		}

		private bool IsTriggerCharging()
		{
			return _triggeredChargeTimer.isActive && !_triggeredChargeTimer.isTimeUp;
		}

		public override void Core()
		{
			if (_oldState != _state)
			{
				_oldState = _state;
			}
			if (_triggeredChargeTimer.isActive)
			{
				_triggeredChargeTimer.Core(1f);
				if (_state == State.Before)
				{
					if (_triggeredChargeTimer.isTimeUp)
					{
						_avatar.ResetTrigger(config.NextLoopTriggerID);
						_avatar.SetTrigger(config.AfterSkillTriggerID);
					}
					else
					{
						_avatar.ResetTrigger(config.AfterSkillTriggerID);
						_avatar.SetTrigger(config.NextLoopTriggerID);
					}
				}
				else if (_state == State.InLoop && _triggeredChargeTimer.isTimeUp)
				{
					_avatar.SetTrigger(config.AfterSkillTriggerID);
				}
				if (_triggeredChargeTimer.isTimeUp)
				{
					_triggeredChargeTimer.Reset(false);
					_useTriggerTimeControl = false;
				}
			}
			if (_state != State.InLoop)
			{
				return;
			}
			UpdateInLoop();
			if (ShouldMoveToNextLoop())
			{
				_loopIx++;
				if (_loopIx == _loopCount)
				{
					OnMoveingToNextLoop(true);
					_avatar.SetTrigger(config.AfterSkillTriggerID);
					_avatar.IsLockDirection = false;
					ClearSubTargets();
				}
				else
				{
					OnMoveingToNextLoop(false);
					_avatar.SetTrigger(config.NextLoopTriggerID);
					if (config.ChargeSubTargetAmount != null)
					{
						int targetAmount = config.ChargeSubTargetAmount[_loopIx];
						SelectSubTargets(targetAmount);
					}
				}
			}
			_switchTimer.Core(1f);
			if (_switchTimer.isTimeUp)
			{
				_switchTimer.Reset(false);
			}
		}

		private void SelectSubTargets(int targetAmount)
		{
			BaseMonoEntity attackTarget = actor.entity.GetAttackTarget();
			if (attackTarget != null && !_subSelectTargetList.Contains(attackTarget))
			{
				_subSelectTargetList.Add(attackTarget);
				AddSubAttackTarget(attackTarget);
				if (attackTarget is BaseMonoMonster)
				{
					MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(attackTarget.GetRuntimeID());
					if (monsterActor != null && !string.IsNullOrEmpty(config.SubTargetModifierName))
					{
						monsterActor.abilityPlugin.ApplyModifier(instancedAbility, config.SubTargetModifierName);
					}
				}
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			allMonsters.Sort(NearestTargetCompare);
			for (int i = 0; i < allMonsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[i];
				MonsterActor monsterActor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoMonster.GetRuntimeID());
				if (baseMonoMonster != attackTarget && monsterActor2 != null && _subSelectTargetList.Count < targetAmount && !_subSelectTargetList.Contains(baseMonoMonster))
				{
					if (!string.IsNullOrEmpty(config.SubTargetModifierName))
					{
						monsterActor2.abilityPlugin.ApplyModifier(instancedAbility, config.SubTargetModifierName);
					}
					_subSelectTargetList.Add(baseMonoMonster);
					AddSubAttackTarget(baseMonoMonster);
				}
			}
		}

		private void AddSubAttackTarget(BaseMonoEntity target)
		{
			BaseMonoAvatar baseMonoAvatar = actor.entity as BaseMonoAvatar;
			if (baseMonoAvatar != null)
			{
				baseMonoAvatar.AddTargetToSubAttackList(target);
			}
		}

		private void ClearAllSubAttackTargets()
		{
			BaseMonoAvatar baseMonoAvatar = actor.entity as BaseMonoAvatar;
			if (baseMonoAvatar != null)
			{
				baseMonoAvatar.ClearSubAttackList();
			}
		}

		private int NearestTargetCompare(BaseMonoMonster monsterA, BaseMonoMonster monsterB)
		{
			float num = Vector3.Distance(entity.transform.position, monsterA.transform.position);
			float num2 = Vector3.Distance(entity.transform.position, monsterB.transform.position);
			return (int)(num - num2);
		}

		private void ClearSubTargets()
		{
			if (_subSelectTargetList.Count > 0)
			{
				for (int i = 0; i < _subSelectTargetList.Count; i++)
				{
					BaseMonoEntity baseMonoEntity = _subSelectTargetList[i];
					if (!(baseMonoEntity == null))
					{
						MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(baseMonoEntity.GetRuntimeID());
						if (monsterActor != null && !string.IsNullOrEmpty(config.SubTargetModifierName))
						{
							monsterActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.SubTargetModifierName);
						}
					}
				}
				_subSelectTargetList.Clear();
			}
			ClearAllSubAttackTargets();
		}

		private bool SkillButtonStateChangedCallback(MonoSkillButton.PointerState pointerState)
		{
			if (_useTriggerTimeControl)
			{
				return true;
			}
			if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(_avatar.GetRuntimeID()))
			{
				return true;
			}
			BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(actor.runtimeID);
			bool allowHoldLockDirection = config.AllowHoldLockDirection;
			if (avatarByRuntimeID != null && allowHoldLockDirection)
			{
				switch (pointerState)
				{
				case MonoSkillButton.PointerState.PointerUp:
					avatarByRuntimeID.IsLockDirection = false;
					break;
				case MonoSkillButton.PointerState.PointerDown:
					avatarByRuntimeID.IsLockDirection = true;
					break;
				}
			}
			if (_state == State.Before)
			{
				switch (pointerState)
				{
				case MonoSkillButton.PointerState.PointerUp:
					_avatar.ResetTrigger(config.NextLoopTriggerID);
					_avatar.SetTrigger(config.AfterSkillTriggerID);
					break;
				case MonoSkillButton.PointerState.PointerDown:
					if (!config.DisallowReleaseButtonInBS)
					{
						_avatar.ResetTrigger(config.AfterSkillTriggerID);
						_avatar.SetTrigger(config.NextLoopTriggerID);
					}
					else if (_checkPointerDownInBS)
					{
						_avatar.ResetTrigger(config.AfterSkillTriggerID);
						_avatar.SetTrigger(config.NextLoopTriggerID);
						_checkPointerDownInBS = false;
					}
					break;
				}
			}
			else if (_state == State.InLoop && pointerState == MonoSkillButton.PointerState.PointerUp)
			{
				_avatar.SetTrigger(config.AfterSkillTriggerID);
			}
			return true;
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

		private bool IsControlHold()
		{
			if (_useTriggerTimeControl)
			{
				return IsTriggerCharging();
			}
			return _skillButton.IsPointerHold();
		}

		protected abstract void OnBeforeToInLoop();

		protected abstract void OnInLoopToAfter();

		protected abstract void UpdateInLoop();

		protected abstract bool ShouldMoveToNextLoop();

		protected abstract void OnMoveingToNextLoop(bool endLoop);

		private void SkillIDChangedToBefore()
		{
			_state = State.Before;
			_loopIx = 0;
			if (IsControlHold())
			{
				if (config.DisallowReleaseButtonInBS)
				{
					_checkPointerDownInBS = false;
				}
				else
				{
					_checkPointerDownInBS = true;
				}
				_avatar.ResetTrigger(config.AfterSkillTriggerID);
				_avatar.SetTrigger(config.NextLoopTriggerID);
			}
			else
			{
				_checkPointerDownInBS = true;
				_avatar.ResetTrigger(config.NextLoopTriggerID);
				_avatar.SetTrigger(config.AfterSkillTriggerID);
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (_state == State.Idle)
			{
				if (Miscs.ArrayContains(config.BeforeSkillIDs, to))
				{
					SkillIDChangedToBefore();
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
					OnBeforeToInLoop();
					if (config.ChargeSubTargetAmount != null)
					{
						int targetAmount = config.ChargeSubTargetAmount[_loopIx];
						SelectSubTargets(targetAmount);
					}
				}
				else if (Miscs.ArrayContains(config.AfterSkillIDs, to))
				{
					_avatar.ResetTrigger(config.AfterSkillTriggerID);
					_avatar.ResetTrigger(config.NextLoopTriggerID);
					_state = State.After;
				}
				else if (Miscs.ArrayContains(config.BeforeSkillIDs, to))
				{
					if (Miscs.ArrayContains(config.BeforeSkillIDs, from))
					{
						SkillIDChangedToBefore();
					}
				}
				else
				{
					_avatar.ResetTrigger(config.AfterSkillTriggerID);
					_avatar.ResetTrigger(config.NextLoopTriggerID);
					_state = State.Idle;
				}
			}
			else if (_state == State.InLoop)
			{
				if (Miscs.ArrayContains(config.ChargeLoopSkillIDs, to))
				{
					if (config.ChargeLoopEffects != null)
					{
						if (config.ImmediatelyDetachLoopEffect)
						{
							entity.DetachEffectImmediately(_chargeEffectPatternIx);
						}
						else
						{
							entity.DetachEffect(_chargeEffectPatternIx);
						}
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
					_switchTimer.Reset(true);
					return;
				}
				if (Miscs.ArrayContains(config.AfterSkillIDs, to))
				{
					if (config.ChargeLoopEffects != null)
					{
						if (config.ImmediatelyDetachLoopEffect)
						{
							entity.DetachEffectImmediately(_chargeEffectPatternIx);
						}
						else
						{
							entity.DetachEffect(_chargeEffectPatternIx);
						}
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
					_state = State.After;
					OnInLoopToAfter();
					return;
				}
				if (config.ChargeLoopEffects != null)
				{
					if (config.ImmediatelyDetachLoopEffect)
					{
						entity.DetachEffectImmediately(_chargeEffectPatternIx);
					}
					else
					{
						entity.DetachEffect(_chargeEffectPatternIx);
					}
					_chargeEffectPatternIx = -1;
					if (_chargeAudioLoopName != null)
					{
						entity.StopAudio(_chargeAudioLoopName);
						_chargeAudioLoopName = null;
					}
				}
				_avatar.ResetTrigger(config.AfterSkillTriggerID);
				_avatar.ResetTrigger(config.NextLoopTriggerID);
				_state = State.Idle;
				ClearSubTargets();
			}
			else if (_state == State.After)
			{
				if (Miscs.ArrayContains(config.BeforeSkillIDs, to))
				{
					SkillIDChangedToBefore();
				}
				else
				{
					_avatar.ResetTrigger(config.AfterSkillTriggerID);
					_avatar.ResetTrigger(config.NextLoopTriggerID);
					_state = State.Idle;
				}
				ClearSubTargets();
			}
		}
	}
}
