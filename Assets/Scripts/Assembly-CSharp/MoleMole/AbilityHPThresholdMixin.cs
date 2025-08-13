using MoleMole.Config;

namespace MoleMole
{
	public class AbilityHPThresholdMixin : BaseAbilityMixin
	{
		private HPThresholdMixin config;

		private bool _isApplied;

		public AbilityHPThresholdMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HPThresholdMixin)config;
		}

		public override void OnAdded()
		{
			_isApplied = false;
		}

		public override void Core()
		{
			float lhs = (float)actor.HP / (float)actor.maxHP;
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
