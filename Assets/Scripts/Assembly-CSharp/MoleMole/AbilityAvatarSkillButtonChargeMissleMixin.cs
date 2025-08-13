using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonChargeMissleMixin : AbilityAvatarSkillButtonHoldChargeMixin
	{
		public AvatarSkillButtonHoldChargeMissleMixin config;

		public AbilityAvatarSkillButtonChargeMissleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonHoldChargeMissleMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (evt.abilityArgument != null)
			{
				base.OnAbilityTriggered(evt);
				return;
			}
			BaseMonoAvatar baseMonoAvatar = entity as BaseMonoAvatar;
			if (baseMonoAvatar == null)
			{
				return;
			}
			int num = ((_loopIx >= _loopCount) ? (_loopCount - 1) : _loopIx);
			int num2 = config.ChargeMissleAmount[num];
			int count = baseMonoAvatar.SubAttackTargetList.Count;
			for (int i = 0; i < count; i++)
			{
				BaseMonoEntity target = baseMonoAvatar.SubAttackTargetList[i];
				TriggerAbility(target, config.AbilityName);
			}
			if (count > 0)
			{
				for (int j = 0; j < num2 - count; j++)
				{
					BaseMonoEntity target2 = baseMonoAvatar.SubAttackTargetList[Random.Range(0, count)];
					TriggerAbility(target2, config.AbilityNameSub);
				}
			}
			else
			{
				for (int k = 0; k < num2 - count; k++)
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
