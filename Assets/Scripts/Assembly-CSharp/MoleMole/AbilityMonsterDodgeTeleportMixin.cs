using MoleMole.Config;

namespace MoleMole
{
	public class AbilityMonsterDodgeTeleportMixin : AbilityDodgeTeleportMixin
	{
		protected MonsterDodgeTeleportMixin config;

		public AbilityMonsterDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterDodgeTeleportMixin)config;
		}

		protected override void ClearTargetAttackTarget(uint sourceID)
		{
			AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(sourceID);
			if (avatarActor != null)
			{
				BaseMonoAvatar baseMonoAvatar = avatarActor.entity as BaseMonoAvatar;
				baseMonoAvatar.SetAttackTarget(null);
			}
		}
	}
}
