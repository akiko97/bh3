using MoleMole.Config;

namespace MoleMole
{
	public class AbilityModifiyDamageWithMultiMixin : AbilityModifiyDamageMixin
	{
		private ModifyDamageWithMultiMixin config;

		public AbilityModifiyDamageWithMultiMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (ModifyDamageWithMultiMixin)config;
		}

		protected override void ModifyDamage(EvtHittingOther evt, float multiple = 1f)
		{
			multiple = 0f;
			switch (config.MultipleType)
			{
			case ModifyDamageWithMultiMixin.DamageMultipleType.ByTargetAbilityState:
				switch (config.Targetting)
				{
				case MixinTargetting.Enemy:
				{
					BaseAbilityActor[] actorByCategory = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(actor);
					multiple = GetTargetCountWithAbilityState(actorByCategory);
					break;
				}
				case MixinTargetting.Allied:
				{
					BaseAbilityActor[] actorByCategory = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor);
					multiple = GetTargetCountWithAbilityState(actorByCategory);
					break;
				}
				case MixinTargetting.All:
				{
					BaseAbilityActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<BaseAbilityActor>(3);
					multiple = GetTargetCountWithAbilityState(actorByCategory);
					actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<BaseAbilityActor>(4);
					multiple += (float)GetTargetCountWithAbilityState(actorByCategory);
					break;
				}
				}
				break;
			case ModifyDamageWithMultiMixin.DamageMultipleType.BySelfCurrentSPAmount:
				multiple = actor.SP;
				if (config.ClearAllSP)
				{
					actor.HealSP(0f - (float)actor.SP);
				}
				break;
			case ModifyDamageWithMultiMixin.DamageMultipleType.BySelfMaxSPAmount:
				multiple = actor.maxSP;
				break;
			case ModifyDamageWithMultiMixin.DamageMultipleType.ByLevelCurrentCombo:
				multiple = (int)Singleton<LevelManager>.Instance.levelActor.levelCombo;
				break;
			case ModifyDamageWithMultiMixin.DamageMultipleType.ByTargetDistance:
			{
				BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
				if (baseMonoEntity != null)
				{
					multiple = (baseMonoEntity.XZPosition - instancedAbility.caster.entity.XZPosition).magnitude;
				}
				break;
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

		private int GetTargetCountWithAbilityState(BaseAbilityActor[] targets)
		{
			int num = 0;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i].abilityState.ContainsState(config.TargetAbilityState))
				{
					num++;
				}
			}
			return num;
		}
	}
}
