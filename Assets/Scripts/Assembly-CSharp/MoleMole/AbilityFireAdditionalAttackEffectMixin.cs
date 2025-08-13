using MoleMole.Config;

namespace MoleMole
{
	public class AbilityFireAdditionalAttackEffectMixin : BaseAbilityMixin
	{
		private FireAdditionalAttackEffectMixin config;

		public AbilityFireAdditionalAttackEffectMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (FireAdditionalAttackEffectMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtAttackLanded)
			{
				return OnAttackLanded((EvtAttackLanded)evt);
			}
			return false;
		}

		private bool OnAttackLanded(EvtAttackLanded evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
			if (baseAbilityActor == null)
			{
				return false;
			}
			AttackPattern.ActAttackEffects(config.AttackEffect, baseAbilityActor.entity, evt.attackResult.hitCollision.hitPoint, evt.attackResult.hitCollision.hitDir);
			return true;
		}
	}
}
