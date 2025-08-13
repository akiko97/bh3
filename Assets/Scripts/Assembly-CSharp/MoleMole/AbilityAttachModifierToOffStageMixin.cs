using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToOffStageMixin : BaseAbilityMixin
	{
		private AttachModifierToOffStageMixin config;

		private ActorModifier _modifier;

		public AbilityAttachModifierToOffStageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToOffStageMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Combine(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnAvatarIsOnStageChanged));
			if (!entity.gameObject.activeSelf)
			{
				OnAvatarIsOnStageChanged(false);
			}
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Combine(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnAvatarIsOnStageChanged));
			if (_modifier != null)
			{
				actor.abilityPlugin.TryRemoveModifier(_modifier);
			}
		}

		private void OnAvatarIsOnStageChanged(bool active)
		{
			if (active)
			{
				if (_modifier != null && actor.entity.gameObject.activeInHierarchy)
				{
					actor.abilityPlugin.TryRemoveModifier(_modifier);
				}
			}
			else
			{
				_modifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
		}
	}
}
