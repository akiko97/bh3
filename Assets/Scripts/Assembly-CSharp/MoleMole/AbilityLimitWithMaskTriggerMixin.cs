using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityLimitWithMaskTriggerMixin : BaseAbilityMixin
	{
		public LimitTimeWithMaskTriggerMixin config;

		private int _originEvaLimitCount;

		private int _countLeft;

		private EntityTimer _maskTimer;

		private EntityTimer _countTimer;

		private bool _isMasked;

		public AbilityLimitWithMaskTriggerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (LimitTimeWithMaskTriggerMixin)config;
			_maskTimer = new EntityTimer(this.config.MaskDuration, entity);
			_countTimer = new EntityTimer(this.config.CountTime, entity);
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_originEvaLimitCount = instancedAbility.Evaluate(config.EvadeLimitCount);
			_countLeft = _originEvaLimitCount;
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			if (_maskTimer.isActive)
			{
				entity.UnmaskTrigger(config.MaskTriggerID);
				_isMasked = false;
				_maskTimer.Reset(false);
			}
		}

		public override void Core()
		{
			_maskTimer.Core(1f);
			_countTimer.Core(1f);
			if (_maskTimer.isTimeUp)
			{
				entity.UnmaskTrigger(config.MaskTriggerID);
				_isMasked = false;
				_maskTimer.Reset(false);
			}
			if (_countTimer.isTimeUp)
			{
				_countLeft = _originEvaLimitCount;
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (to == config.SkillID)
			{
				if (_countLeft == _originEvaLimitCount)
				{
					_countTimer.Reset(true);
				}
				_countLeft--;
				if (_countLeft <= 0 && !_isMasked)
				{
					entity.MaskTrigger(config.MaskTriggerID);
					_isMasked = true;
					_maskTimer.Reset(true);
				}
			}
		}
	}
}
