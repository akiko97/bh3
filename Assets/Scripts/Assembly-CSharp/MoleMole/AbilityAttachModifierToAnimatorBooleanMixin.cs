using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToAnimatorBooleanMixin : BaseAbilityMixin
	{
		public AttachModifierToAnimatorBooleanMixin config;

		private BaseMonoAnimatorEntity _animatorEntity;

		private bool _lastAnimatorBoolValue;

		public AbilityAttachModifierToAnimatorBooleanMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToAnimatorBooleanMixin)config;
			_animatorEntity = (BaseMonoAnimatorEntity)entity;
		}

		public override void OnAdded()
		{
			_lastAnimatorBoolValue = _animatorEntity.GetLocomotionBool(config.AnimatorBoolean);
			if (config.IsInvert && !_lastAnimatorBoolValue)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
		}

		public override void OnRemoved()
		{
		}

		public override void Core()
		{
			bool locomotionBool = _animatorEntity.GetLocomotionBool(config.AnimatorBoolean);
			if (locomotionBool != _lastAnimatorBoolValue)
			{
				if ((!config.IsInvert) ? locomotionBool : (!locomotionBool))
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
				}
				else
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
				}
				_lastAnimatorBoolValue = locomotionBool;
			}
		}
	}
}
