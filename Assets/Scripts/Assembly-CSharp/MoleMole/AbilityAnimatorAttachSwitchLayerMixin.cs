using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAnimatorAttachSwitchLayerMixin : BaseAbilityMixin
	{
		public AnimatorAttachSwitchLayerMixin config;

		private BaseMonoAnimatorEntity _animatorEntity;

		public AbilityAnimatorAttachSwitchLayerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AnimatorAttachSwitchLayerMixin)config;
			_animatorEntity = (BaseMonoAnimatorEntity)entity;
		}

		public override void OnAdded()
		{
			_animatorEntity.StartFadeAnimatorLayerWeight(config.FromLayer, 0f, config.Duration);
			_animatorEntity.StartFadeAnimatorLayerWeight(config.ToLayer, 1f, config.Duration);
		}

		public override void OnRemoved()
		{
			if (!config.NoEndResume)
			{
				_animatorEntity.StartFadeAnimatorLayerWeight(config.FromLayer, 1f, config.Duration);
				_animatorEntity.StartFadeAnimatorLayerWeight(config.ToLayer, 0f, config.Duration);
			}
		}
	}
}
