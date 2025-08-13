using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToAbilityStateMixin : BaseAbilityMixin
	{
		public AttachModifierToAbilityStateMixin config;

		public AbilityAttachModifierToAbilityStateMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToAbilityStateMixin)config;
		}

		public override void OnAdded()
		{
			BaseAbilityActor baseAbilityActor = actor;
			baseAbilityActor.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Combine(baseAbilityActor.onAbilityStateAdd, new Action<AbilityState, bool>(OnAbilityStateAdd));
			BaseAbilityActor baseAbilityActor2 = actor;
			baseAbilityActor2.onAbilityStateRemove = (Action<AbilityState>)Delegate.Combine(baseAbilityActor2.onAbilityStateRemove, new Action<AbilityState>(OnAbilityStateRemove));
			bool flag = IsTargetState(actor.abilityState);
			if (flag && config.OnModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OnModifierName);
			}
			if (!flag && config.OffModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
			}
		}

		public override void OnRemoved()
		{
			BaseAbilityActor baseAbilityActor = actor;
			baseAbilityActor.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Remove(baseAbilityActor.onAbilityStateAdd, new Action<AbilityState, bool>(OnAbilityStateAdd));
			BaseAbilityActor baseAbilityActor2 = actor;
			baseAbilityActor2.onAbilityStateRemove = (Action<AbilityState>)Delegate.Remove(baseAbilityActor2.onAbilityStateRemove, new Action<AbilityState>(OnAbilityStateRemove));
			if (config.OnModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OnModifierName);
			}
			if (config.OffModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OffModifierName);
			}
		}

		private void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
			bool flag = IsTargetState(state);
			if (flag && config.OnModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OnModifierName);
			}
			if (flag && config.OffModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OffModifierName);
			}
		}

		private void OnAbilityStateRemove(AbilityState state)
		{
			bool flag = IsTargetState(state);
			if (flag && config.OnModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OnModifierName);
			}
			if (flag && config.OffModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
			}
		}

		private bool IsTargetState(AbilityState target)
		{
			for (int i = 0; i < config.AbilityStates.Length; i++)
			{
				if (target.ContainsState(config.AbilityStates[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
