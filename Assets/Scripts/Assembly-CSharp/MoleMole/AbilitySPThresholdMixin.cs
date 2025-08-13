using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilitySPThresholdMixin : BaseAbilityMixin
	{
		private SPTHresholdMixin config;

		private bool _isApplied;

		public AbilitySPThresholdMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (SPTHresholdMixin)config;
		}

		public override void OnAdded()
		{
			_isApplied = false;
			OnSPChangedCallback(actor.SP, actor.SP, 0f);
			BaseAbilityActor baseAbilityActor = actor;
			baseAbilityActor.onSPChanged = (Action<float, float, float>)Delegate.Combine(baseAbilityActor.onSPChanged, new Action<float, float, float>(OnSPChangedCallback));
		}

		public override void OnRemoved()
		{
			if (_isApplied)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
			BaseAbilityActor baseAbilityActor = actor;
			baseAbilityActor.onSPChanged = (Action<float, float, float>)Delegate.Remove(baseAbilityActor.onSPChanged, new Action<float, float, float>(OnSPChangedCallback));
		}

		private void OnSPChangedCallback(float from, float to, float delta)
		{
			float lhs = to / (float)actor.maxSP;
			bool flag = EvaluatePredicate(lhs, instancedAbility.Evaluate(config.Threshold), config.Predicate);
			if (_isApplied)
			{
				if (!flag)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
					_isApplied = false;
				}
			}
			else if (flag)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
				_isApplied = true;
			}
		}
	}
}
