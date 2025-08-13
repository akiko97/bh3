using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarDodgeTeleportMixin : AbilityDodgeTeleportMixin
	{
		private AvatarDodgeTeleportMixin config;

		public AbilityAvatarDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarDodgeTeleportMixin)config;
		}

		protected override void OnPostBeingHit(EvtBeingHit evtHit)
		{
			base.OnPostBeingHit(evtHit);
			if (config.DodgeMeleeAttack && config.CanHitTrigger && evtHit.attackData.hitType == AttackResult.ActorHitType.Melee && CheckAllowTeleport())
			{
				IgnoreHitDamage(evtHit);
				TeleportBack(evtHit.sourceID);
			}
		}

		private void TeleportBack(uint sourceID)
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.TeleportActions, instancedAbility, instancedModifier, null, null);
			BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(sourceID);
			Vector3 position;
			if (baseMonoEntity == null)
			{
				position = entity.transform.position;
			}
			Vector3 normalized = (entity.transform.position - baseMonoEntity.transform.position).normalized;
			position = baseMonoEntity.transform.position + normalized * config.MeleeDistance;
			TeleportTo(position, config.TeleportTime);
			if (instancedAbility.Evaluate(config.CDTime) > 0f)
			{
				_cdTimer = instancedAbility.Evaluate(config.CDTime);
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(actor.runtimeID));
		}

		protected override Vector3 GetDirection(TeleportDirectionMode mode, float angle)
		{
			if (mode == TeleportDirectionMode.UseSteerAngle)
			{
				AvatarControlData controlData = (actor.entity as BaseMonoAvatar).GetInputController().controlData;
				if (controlData.orderMove)
				{
					return controlData.steerDirection;
				}
				return -(entity as BaseMonoAvatar).FaceDirection;
			}
			return base.GetDirection(mode, angle);
		}
	}
}
