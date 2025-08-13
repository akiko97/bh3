using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyDamageTakenWithMultiMixin : AbilityModifyDamageTakenMixin
	{
		private ModifyDamageTakenWithMultiMixin config;

		public AbilityModifyDamageTakenWithMultiMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageTakenWithMultiMixin)config;
		}

		protected override void ModifyDamage(EvtBeingHit evt, float multiple = 1f)
		{
			multiple = 0f;
			if (config.MultipleType == ModifyDamageTakenWithMultiMixin.DamageMultipleType.ByTargetDistance)
			{
				BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(evt.sourceID);
				if (baseMonoEntity != null)
				{
					multiple = (baseMonoEntity.XZPosition - instancedAbility.caster.entity.XZPosition).magnitude;
				}
			}
			multiple -= config.BaseMultiple;
			if (multiple < 0f)
			{
				multiple = 0f;
			}
			if (instancedAbility.Evaluate(config.MaxMultiple) > 0f && multiple > instancedAbility.Evaluate(config.MaxMultiple))
			{
				multiple = instancedAbility.Evaluate(config.MaxMultiple);
			}
			base.ModifyDamage(evt, multiple);
		}
	}
}
