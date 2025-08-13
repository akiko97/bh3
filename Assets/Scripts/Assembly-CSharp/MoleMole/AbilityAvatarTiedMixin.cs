using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarTiedMixin : BaseAbilityMixin
	{
		private AvatarTiedMixin config;

		private BaseMonoAvatar _avatar;

		private int _animatorMoveStackIx;

		private Vector3 _lastSteer;

		private float _accumulatedSteerAmount;

		public AbilityAvatarTiedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarTiedMixin)config;
			_avatar = (BaseMonoAvatar)actor.entity;
		}

		public override void OnAdded()
		{
			_animatorMoveStackIx = _avatar.PushProperty("Animator_RigidBodyVelocityRatio", -1000f);
			_lastSteer = Vector3.zero;
			_accumulatedSteerAmount = 0f;
		}

		public override void OnRemoved()
		{
			_avatar.PopProperty("Animator_RigidBodyVelocityRatio", _animatorMoveStackIx);
		}

		public override void Core()
		{
			AvatarControlData activeControlData = _avatar.GetActiveControlData();
			if (activeControlData.hasAnyControl)
			{
				Vector3 steerDirection = activeControlData.steerDirection;
				_accumulatedSteerAmount += (steerDirection - _lastSteer).sqrMagnitude;
				_lastSteer = steerDirection;
				if (_accumulatedSteerAmount > instancedAbility.Evaluate(config.UntieSteerAmount))
				{
					actor.abilityPlugin.TryRemoveModifier(instancedModifier);
				}
			}
		}
	}
}
