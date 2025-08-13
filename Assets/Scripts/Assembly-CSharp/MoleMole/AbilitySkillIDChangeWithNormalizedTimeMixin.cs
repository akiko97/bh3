using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilitySkillIDChangeWithNormalizedTimeMixin : BaseAbilityMixin
	{
		private SkillIDChangeWithNormalizedTimeMixin config;

		private float _fromSkillNormalizedTime;

		private bool _startActionFinish;

		public AbilitySkillIDChangeWithNormalizedTimeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (SkillIDChangeWithNormalizedTimeMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		public override void Core()
		{
			if (entity.CurrentSkillID == config.SkillIDFrom)
			{
				_fromSkillNormalizedTime = entity.GetCurrentNormalizedTime();
				if (_fromSkillNormalizedTime >= config.NormalizedTimeStart && !_startActionFinish)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.NormalizedTimeStartActions, instancedAbility, instancedModifier, actor, null);
					_startActionFinish = true;
				}
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (to == config.SkillIDFrom)
			{
				_startActionFinish = false;
			}
			if (from == config.SkillIDFrom && to == config.SkillIDTo && _fromSkillNormalizedTime < config.NormalizedTimeStop && _fromSkillNormalizedTime > config.NormalizedTimeStart)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.SkillIDChangeActions, instancedAbility, instancedModifier, actor, null);
			}
		}
	}
}
