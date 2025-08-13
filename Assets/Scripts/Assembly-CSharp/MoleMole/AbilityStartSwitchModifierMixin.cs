using MoleMole.Config;

namespace MoleMole
{
	public class AbilityStartSwitchModifierMixin : BaseAbilityMixin
	{
		private OnStartSwitchModifierMixin config;

		private bool _state;

		private float _originSpCost;

		private float _originSpNeed;

		private string _originIconPath;

		private EntityTimer _timer;

		public AbilityStartSwitchModifierMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (OnStartSwitchModifierMixin)config;
			_timer = new EntityTimer(this.config.MaxDuration);
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (config.AlwaysSwitchOn)
			{
				TurnOnModifier();
			}
			else if (!_state)
			{
				TurnOnModifier();
			}
			else
			{
				TurnOffModifier();
			}
		}

		public override void Core()
		{
			base.Core();
			if (_state && config.UseLowSPForceOff && (float)actor.SP <= 0f)
			{
				TurnOffModifier();
			}
			if (_state && config.UseLowHPForceOff && (float)actor.HP <= 0f)
			{
				TurnOffModifier();
			}
			if (config.MaxDuration > 0f)
			{
				_timer.Core(1f);
				if (_timer.isTimeUp)
				{
					TurnOffModifier();
				}
			}
		}

		public override void OnAdded()
		{
			if (config.OffModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
			}
			AvatarActor avatarActor = actor as AvatarActor;
			if (!string.IsNullOrEmpty(config.SkillButtonID))
			{
				_originSpCost = avatarActor.GetSkillInfo(config.SkillButtonID).costSP;
				_originSpNeed = avatarActor.GetSkillInfo(config.SkillButtonID).needSP;
				_originIconPath = avatarActor.GetSkillInfo(config.SkillButtonID).iconPath;
			}
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
		}

		private void TurnOnModifier()
		{
			actor.abilityPlugin.ApplyModifier(instancedAbility, config.OnModifierName);
			if (config.OffModifierName != null)
			{
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OffModifierName);
			}
			if (!string.IsNullOrEmpty(config.SkillButtonID))
			{
				AvatarActor avatarActor = actor as AvatarActor;
				avatarActor.GetSkillInfo(config.SkillButtonID).costSP = config.OnModifierReplaceCostSP;
				avatarActor.GetSkillInfo(config.SkillButtonID).needSP = config.OnModifierReplaceCostSP;
				avatarActor.GetSkillInfo(config.SkillButtonID).iconPath = config.OnModifierReplaceIconPath;
				avatarActor.GetSkillInfo(config.SkillButtonID).muteHighlighted = true;
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID).RefreshSkillInfo();
				if (config.OnModifierSwitchToInstantTrigger)
				{
					avatarActor.config.Skills[config.SkillButtonID].IsInstantTrigger = true;
					avatarActor.config.Skills[config.SkillButtonID].InstantTriggerEvent = config.OnModifierInstantTriggerEvent;
				}
			}
			_timer.Reset(true);
			_state = true;
		}

		private void TurnOffModifier()
		{
			if (config.OffModifierName != null)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.OffModifierName);
			}
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.OnModifierName);
			if (!string.IsNullOrEmpty(config.SkillButtonID))
			{
				AvatarActor avatarActor = actor as AvatarActor;
				if (config.OnModifierSwitchToInstantTrigger)
				{
					avatarActor.config.Skills[config.SkillButtonID].IsInstantTrigger = false;
					avatarActor.config.Skills[config.SkillButtonID].InstantTriggerEvent = null;
				}
				avatarActor.GetSkillInfo(config.SkillButtonID).costSP = _originSpCost;
				avatarActor.GetSkillInfo(config.SkillButtonID).needSP = _originSpNeed;
				avatarActor.GetSkillInfo(config.SkillButtonID).iconPath = _originIconPath;
				avatarActor.GetSkillInfo(config.SkillButtonID).muteHighlighted = false;
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID).RefreshSkillInfo();
			}
			_timer.Reset(false);
			_state = false;
		}
	}
}
