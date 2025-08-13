using MoleMole.Config;

namespace MoleMole
{
	public class AbilityTriggerMultiBulletMixin : BaseAbilityMixin
	{
		private TriggerMultiBulletMixin config;

		public AbilityTriggerMultiBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (TriggerMultiBulletMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar == null)
			{
				return;
			}
			int count = localAvatar.SubAttackTargetList.Count;
			for (int i = 0; i < count; i++)
			{
				BaseMonoEntity baseMonoEntity = localAvatar.SubAttackTargetList[i];
				EvtAbilityStart evtAbilityStart = new EvtAbilityStart(entity.GetRuntimeID());
				evtAbilityStart.abilityName = config.AbilityName;
				if (baseMonoEntity != null)
				{
					evtAbilityStart.otherID = baseMonoEntity.GetRuntimeID();
				}
				Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			}
		}

		public override void OnRemoved()
		{
			base.OnRemoved();
		}

		public override void Core()
		{
			base.Core();
		}
	}
}
