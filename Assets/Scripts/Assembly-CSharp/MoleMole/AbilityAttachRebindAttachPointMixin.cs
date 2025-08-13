using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachRebindAttachPointMixin : BaseAbilityMixin
	{
		private AttachRebindAttachPointMixin config;

		public AbilityAttachRebindAttachPointMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachRebindAttachPointMixin)config;
		}

		public override void OnAdded()
		{
			(entity as BaseMonoAnimatorEntity).RebindAttachPoint(config.PointName, config.OtherName);
		}

		public override void OnRemoved()
		{
			(entity as BaseMonoAnimatorEntity).RebindAttachPoint(config.PointName, config.OriginName);
		}
	}
}
