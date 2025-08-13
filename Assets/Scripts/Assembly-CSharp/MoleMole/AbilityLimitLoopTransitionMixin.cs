using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityLimitLoopTransitionMixin : BaseAbilityMixin
	{
		private LimitLoopTransitionMixin config;

		private int _maxCount;

		private int _countLeft;

		public AbilityLimitLoopTransitionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (LimitLoopTransitionMixin)config;
			_maxCount = instancedAbility.Evaluate(this.config.LoopLimitCount);
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_countLeft = (_maxCount = Mathf.FloorToInt((float)evt.abilityArgument));
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_countLeft = instancedAbility.Evaluate(config.LoopLimitCount);
			entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			entity.RemovePersistentAnimatorBool(config.AllowLoopBoolID);
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (to == config.SkillID)
			{
				_countLeft--;
				if (_countLeft <= 0)
				{
					entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, false);
				}
			}
			else if (from == config.SkillID && to != config.SkillID)
			{
				entity.SetPersistentAnimatorBool(config.AllowLoopBoolID, true);
				_countLeft = _maxCount;
			}
		}
	}
}
