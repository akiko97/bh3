using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterSkillChargeMissleMixin : AbilityMonsterSkillIDChargeAnimatorMixin
	{
		public MonsterSkillChargeMissleMixin config;

		public AbilityMonsterSkillChargeMissleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterSkillChargeMissleMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (evt.abilityArgument != null)
			{
				base.OnAbilityTriggered(evt);
				return;
			}
			int num = ((_loopIx >= _loopCount) ? (_loopCount - 1) : _loopIx);
			BaseAbilityActor[] enemyActorsOf = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(actor);
			int num2 = config.ChargeMissleAmount[num];
			int num3 = enemyActorsOf.Length;
			for (int i = 0; i < num3; i++)
			{
				BaseMonoEntity target = enemyActorsOf[i].entity;
				TriggerAbility(target, config.AbilityName);
			}
			if (num3 > 0)
			{
				for (int j = 0; j < num2 - num3; j++)
				{
					BaseMonoEntity target2 = enemyActorsOf[Random.Range(0, num3)].entity;
					TriggerAbility(target2, config.AbilityNameSub);
				}
			}
			else
			{
				for (int k = 0; k < num2 - num3; k++)
				{
					TriggerAbility(null, config.AbilityNameSub);
				}
			}
		}

		private void TriggerAbility(BaseMonoEntity target, string ability)
		{
			EvtAbilityStart evtAbilityStart = new EvtAbilityStart(entity.GetRuntimeID());
			evtAbilityStart.abilityName = ability;
			if (target != null)
			{
				evtAbilityStart.otherID = target.GetRuntimeID();
			}
			Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
		}
	}
}
