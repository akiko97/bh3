using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToSkillIDMixin : BaseAbilityMixin
	{
		private AttachModifierToSkillIDMixin config;

		public AbilityAttachModifierToSkillIDMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToSkillIDMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			if (Miscs.ArrayContains(config.SkillIDs, entity.CurrentSkillID))
			{
				if (!config.Inverse)
				{
					AddModifier();
				}
			}
			else if (config.Inverse)
			{
				AddModifier();
			}
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
		}

		private void AddModifier()
		{
			if (actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, actor, null))
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
		}

		private void RemoveModifier()
		{
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < config.SkillIDs.Length; i++)
			{
				if (config.SkillIDs[i] == from)
				{
					flag = true;
				}
				if (config.SkillIDs[i] == to)
				{
					flag2 = true;
				}
			}
			if (!flag && flag2)
			{
				if (!config.Inverse)
				{
					AddModifier();
				}
				else
				{
					RemoveModifier();
				}
			}
			else if (flag && !flag2)
			{
				if (!config.Inverse)
				{
					RemoveModifier();
				}
				else
				{
					AddModifier();
				}
			}
		}
	}
}
