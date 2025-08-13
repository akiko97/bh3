using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonHoldModeMixin : BaseAbilityMixin
	{
		public AvatarSkillButtonHoldModeMixin config;

		private AvatarActor _avatarActor;

		public AbilityAvatarSkillButtonHoldModeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonHoldModeMixin)config;
			_avatarActor = (AvatarActor)actor;
		}

		public override void OnAdded()
		{
			_avatarActor.SetAttackButtonHoldMode(true);
		}

		public override void OnRemoved()
		{
			_avatarActor.SetAttackButtonHoldMode(false);
		}
	}
}
