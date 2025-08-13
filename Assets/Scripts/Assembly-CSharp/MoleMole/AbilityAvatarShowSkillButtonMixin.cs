using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarShowSkillButtonMixin : BaseAbilityMixin
	{
		private AvatarShowSkillButtonMixin config;

		private AvatarActor _avatarActor;

		private bool _removedFromMask;

		public AbilityAvatarShowSkillButtonMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarShowSkillButtonMixin)config;
			_avatarActor = (AvatarActor)actor;
		}

		public override void OnAdded()
		{
			if (_avatarActor.maskedSkillButtons.Contains(config.SkillButtonID))
			{
				_avatarActor.maskedSkillButtons.Remove(config.SkillButtonID);
				_removedFromMask = true;
			}
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
			{
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID).gameObject.SetActive(true);
				if (config.SkillButtonID == "SKL02")
				{
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSPBar().gameObject.SetActive(true);
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.RefreshSPView(_avatarActor.SP, _avatarActor.SP, 0f);
				}
			}
		}

		public override void OnRemoved()
		{
			if (_removedFromMask && Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
			{
				_avatarActor.maskedSkillButtons.Add(config.SkillButtonID);
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillButtonID).gameObject.SetActive(false);
				if (config.SkillButtonID == "SKL02")
				{
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSPBar().gameObject.SetActive(false);
				}
			}
		}
	}
}
