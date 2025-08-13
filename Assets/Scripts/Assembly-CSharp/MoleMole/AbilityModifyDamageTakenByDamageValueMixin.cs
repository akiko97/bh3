using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifyDamageTakenByDamageValueMixin : AbilityModifyDamageTakenMixin
	{
		private ModifyDamageTakenByDamageValueMixin config;

		public AbilityModifyDamageTakenByDamageValueMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageTakenByDamageValueMixin)config;
		}

		protected override bool OnPostBeingHit(EvtBeingHit evt)
		{
			base.OnPostBeingHit(evt);
			float replaceDamage = GetReplaceDamage(evt.attackData.damage);
			if (replaceDamage != evt.attackData.damage)
			{
				evt.attackData.damage = GetReplaceDamage(evt.attackData.damage);
				if (config.UseReplaceAniDamageRatio)
				{
					evt.attackData.attackerAniDamageRatio = config.ReplaceAniDamageRatio;
				}
			}
			evt.attackData.fireDamage = GetReplaceDamage(evt.attackData.fireDamage);
			evt.attackData.thunderDamage = GetReplaceDamage(evt.attackData.thunderDamage);
			evt.attackData.alienDamage = GetReplaceDamage(evt.attackData.alienDamage);
			evt.attackData.plainDamage = GetReplaceDamage(evt.attackData.plainDamage);
			return true;
		}

		private float GetReplaceDamage(float damage)
		{
			switch (config.CompareType)
			{
			case ModifyDamageTakenByDamageValueMixin.LogicType.MoreThan:
				if (damage > instancedAbility.Evaluate(config.ByDamageValue))
				{
					damage = instancedAbility.Evaluate(config.ReplaceDamageValue);
				}
				break;
			case ModifyDamageTakenByDamageValueMixin.LogicType.LessThanOrEqual:
				if (damage <= instancedAbility.Evaluate(config.ByDamageValue) && damage > instancedAbility.Evaluate(config.ReplaceDamageValue))
				{
					damage = instancedAbility.Evaluate(config.ReplaceDamageValue);
				}
				break;
			}
			return damage;
		}
	}
}
