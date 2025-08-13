using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToOnStageMixin : BaseAbilityMixin
	{
		private AttachModifierToOnStageMixin config;

		private ActorModifier _modifier;

		public AbilityAttachModifierToOnStageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToOnStageMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Combine(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnAvatarIsOnStageChanged));
			if (entity.gameObject.activeSelf)
			{
				OnAvatarIsOnStageChanged(true);
			}
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Remove(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnAvatarIsOnStageChanged));
			if (_modifier != null)
			{
				actor.abilityPlugin.TryRemoveModifier(_modifier);
			}
		}

		private void OnAvatarIsOnStageChanged(bool active)
		{
			if (!active)
			{
				actor.abilityPlugin.TryRemoveModifier(_modifier);
			}
			else
			{
				_modifier = actor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierName);
			}
		}
	}
}
