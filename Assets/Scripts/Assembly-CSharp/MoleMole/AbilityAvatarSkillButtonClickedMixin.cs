using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonClickedMixin : BaseAbilityMixin
	{
		private AvatarSkillButtonClickedMixin config;

		private MonoSkillButton _skillButton;

		public AbilityAvatarSkillButtonClickedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonClickedMixin)config;
		}

		public override void OnAdded()
		{
			_skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID);
			MonoSkillButton skillButton = _skillButton;
			skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>)Delegate.Combine(skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(OnSkillButtonClicked));
		}

		public override void OnRemoved()
		{
			MonoSkillButton skillButton = _skillButton;
			skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>)Delegate.Remove(skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(OnSkillButtonClicked));
		}

		private bool OnSkillButtonClicked(MonoSkillButton.PointerState pointerState)
		{
			if (pointerState == MonoSkillButton.PointerState.PointerUp)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.OnClickedActions, instancedAbility, instancedModifier, null, null);
			}
			return !config.ConsumeClick;
		}
	}
}
