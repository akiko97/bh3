using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityLimitLoopWithMaskTriggerMixin : BaseAbilityMixin
	{
		public LimitLoopWithMaskTriggerMixin config;

		private int _countLeft;

		private EntityTimer _timer;

		private int _originLimitLoopCount;

		private int _currentLimitLoopCount;

		private EntityTimer _overCountTimer;

		private bool _isMasked;

		public AbilityLimitLoopWithMaskTriggerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (LimitLoopWithMaskTriggerMixin)config;
			_timer = new EntityTimer(this.config.MaskDuration, entity);
			if (this.config.UseOverCount)
			{
				_overCountTimer = new EntityTimer(this.config.ResetOverCountTime, entity);
			}
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_originLimitLoopCount = instancedAbility.Evaluate(config.LoopLimitCount);
			_currentLimitLoopCount = _originLimitLoopCount;
			_countLeft = _currentLimitLoopCount;
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			if (_timer.isActive)
			{
				entity.UnmaskTrigger(config.MaskTriggerID);
				_isMasked = false;
				_timer.Reset(false);
			}
		}

		public override void Core()
		{
			_timer.Core(1f);
			if (_timer.isTimeUp)
			{
				entity.UnmaskTrigger(config.MaskTriggerID);
				_isMasked = false;
				_timer.Reset(false);
			}
			if (!config.UseOverCount)
			{
				return;
			}
			_overCountTimer.Core(1f);
			if (_overCountTimer.isTimeUp)
			{
				_overCountTimer.Reset(true);
				int num = ((_currentLimitLoopCount >= _originLimitLoopCount) ? _originLimitLoopCount : (_currentLimitLoopCount + 1));
				if (_countLeft == _currentLimitLoopCount)
				{
					_countLeft = num;
				}
				_currentLimitLoopCount = num;
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (to == config.SkillID)
			{
				_countLeft--;
				if (_countLeft > 0 || _isMasked)
				{
					return;
				}
				entity.MaskTrigger(config.MaskTriggerID);
				_isMasked = true;
				_timer.Reset(true);
				if (config.UseOverCount)
				{
					_overCountTimer.Reset(true);
					if (_currentLimitLoopCount > 1)
					{
						_currentLimitLoopCount--;
					}
				}
			}
			else if (from == config.SkillID && to != config.SkillID)
			{
				_countLeft = _currentLimitLoopCount;
			}
		}
	}
}
