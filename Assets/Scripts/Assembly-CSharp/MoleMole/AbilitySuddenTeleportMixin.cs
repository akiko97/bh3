using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilitySuddenTeleportMixin : BaseAbilityMixin
	{
		public SuddenTeleportMixin config;

		public AbilitySuddenTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (SuddenTeleportMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (baseAbilityActor != null && !(baseAbilityActor.entity == null))
			{
				Vector3 position = baseAbilityActor.entity.XZPosition + Quaternion.AngleAxis(instancedAbility.Evaluate(config.Angle), Vector3.up) * baseAbilityActor.entity.transform.forward * baseAbilityActor.commonConfig.CommonArguments.CollisionRadius;
				position.y = 0f;
				entity.transform.position = position;
				Vector3 forward = baseAbilityActor.entity.XZPosition - entity.XZPosition;
				forward.y = 0f;
				entity.transform.forward = forward;
			}
		}
	}
}
