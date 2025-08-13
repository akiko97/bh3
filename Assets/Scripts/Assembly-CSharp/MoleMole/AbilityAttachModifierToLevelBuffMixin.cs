using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAttachModifierToLevelBuffMixin : BaseAbilityMixin
	{
		private AttachModifierToLevelBuffMixin config;

		public AbilityAttachModifierToLevelBuffMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToLevelBuffMixin)config;
		}

		public override void OnAdded()
		{
			if (Singleton<LevelManager>.Instance.levelActor.levelBuffs[(int)config.LevelBuff].isActive)
			{
				if (config.OnModifierName != null)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.OnModifierName);
				}
			}
			else if (config.OffModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			if (config.OnModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OnModifierName);
			}
			if (config.OffModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OffModifierName);
			}
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelBuffState)
			{
				return ListenLevelBuffState((EvtLevelBuffState)evt);
			}
			return false;
		}

		private bool ListenLevelBuffState(EvtLevelBuffState evt)
		{
			if (evt.levelBuff != config.LevelBuff)
			{
				return false;
			}
			if (evt.state == LevelBuffState.Start)
			{
				if (config.OnModifierName != null)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.OnModifierName);
				}
				if (config.OffModifierName != null)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OffModifierName);
				}
			}
			else if (evt.state == LevelBuffState.Stop)
			{
				if (config.OnModifierName != null)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OnModifierName);
				}
				if (config.OffModifierName != null)
				{
					actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
				}
			}
			return true;
		}
	}
}
