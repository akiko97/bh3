using MoleMole.Config;

namespace MoleMole
{
	public class AbilityBanAvatarSkillButtonMixin : BaseAbilityMixin
	{
		private BanAvatarSkillButtonMixin config;

		public AbilityBanAvatarSkillButtonMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BanAvatarSkillButtonMixin)config;
		}

		public override void OnAdded()
		{
			AvatarActor avatarActor = actor as AvatarActor;
			if (avatarActor.GetSkillInfo(config.SkillID) != null)
			{
				avatarActor.GetSkillInfo(config.SkillID).muted = true;
				avatarActor.GetSkillInfo(config.SkillID).maskIconPath = config.ReplaceButtonIconPath;
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.entity.GetRuntimeID()))
				{
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillID).RefreshSkillInfo();
				}
			}
		}

		public override void OnRemoved()
		{
			AvatarActor avatarActor = actor as AvatarActor;
			if (avatarActor.GetSkillInfo(config.SkillID) != null)
			{
				avatarActor.GetSkillInfo(config.SkillID).muted = false;
				avatarActor.GetSkillInfo(config.SkillID).maskIconPath = null;
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.entity.GetRuntimeID()))
				{
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(config.SkillID).RefreshSkillInfo();
				}
			}
		}
	}
}
