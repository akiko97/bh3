using MoleMole.Config;

namespace MoleMole
{
	public class AbilityIntervalModifierMixin : BaseAbilityMixin
	{
		private IntervalModifierMixin config;

		private EntityTimer _intervalTimer;

		public AbilityIntervalModifierMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (IntervalModifierMixin)config;
			_intervalTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Interval));
		}

		public override void OnAdded()
		{
			_intervalTimer.Reset(true);
		}

		public override void OnRemoved()
		{
			_intervalTimer.Reset(false);
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
		}

		public override void Core()
		{
			if (_intervalTimer.isActive)
			{
				if (!GetPredicateResult())
				{
					_intervalTimer.Core(1f);
				}
				if (_intervalTimer.isTimeUp)
				{
					AddModifier();
					_intervalTimer.Reset(true);
				}
			}
		}

		private bool GetPredicateResult()
		{
			bool result = false;
			if (config.Predicates.Length > 0)
			{
				result = actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, actor, null);
			}
			return result;
		}

		private void AddModifier()
		{
			actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
		}
	}
}
