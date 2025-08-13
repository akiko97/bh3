using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAttachAdditiveVelocityMixin : BaseAbilityMixin
	{
		private AttachAdditiveVelocityMixin config;

		private BaseMonoAnimatorEntity _animatorEntity;

		public AbilityAttachAdditiveVelocityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachAdditiveVelocityMixin)config;
			_animatorEntity = entity as BaseMonoAnimatorEntity;
		}

		public override void OnAdded()
		{
			SetVelocity();
		}

		public override void OnRemoved()
		{
			ResetVelocity();
		}

		private void SetVelocity()
		{
			Vector3 additiveVelocity = config.MoveSpeed * _animatorEntity.FaceDirection;
			_animatorEntity.SetHasAdditiveVelocity(true);
			_animatorEntity.SetAdditiveVelocity(additiveVelocity);
			_animatorEntity.PushHighspeedMovement();
		}

		private void ResetVelocity()
		{
			_animatorEntity.SetHasAdditiveVelocity(false);
			_animatorEntity.SetAdditiveVelocity(Vector3.zero);
			_animatorEntity.PopHighspeedMovement();
		}
	}
}
