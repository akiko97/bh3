using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAvatarController : BaseController
	{
		public BaseMonoAvatar avatar { get; private set; }

		public bool active { get; protected set; }

		public AvatarControlData controlData { get; private set; }

		public BaseAvatarController(BaseMonoEntity avatar)
			: base(2u, avatar)
		{
			this.avatar = (BaseMonoAvatar)avatar;
			controlData = new AvatarControlData();
		}

		public virtual void SetActive(bool isActive)
		{
			active = isActive;
		}

		protected void TrySteer(Vector3 dir, float lerpRatio)
		{
			controlData.hasSteer = true;
			controlData.lerpRatio = lerpRatio;
			controlData.steerDirection = dir.normalized;
			controlData.hasAnyControl = true;
		}

		protected void TryAttack()
		{
			controlData.useAttack = true;
			controlData.hasAnyControl = true;
		}

		protected void TryHoldAttack()
		{
			controlData.useHoldAttack = true;
			controlData.hasAnyControl = true;
		}

		protected void TryUseSkill(int skillIx)
		{
			controlData.useSkills[skillIx] = true;
			controlData.hasAnyControl = true;
		}

		protected void TryOrderMove(bool orderMove)
		{
			controlData.hasOrderMove = true;
			controlData.orderMove = orderMove;
			controlData.hasAnyControl = true;
		}

		public void TrySetAttackTarget(BaseMonoEntity attackTarget)
		{
			controlData.hasSetAttackTarget = true;
			controlData.attackTarget = attackTarget;
			controlData.hasAnyControl = true;
		}

		public void TryClearAttackTarget()
		{
			controlData.hasSetAttackTarget = true;
			controlData.attackTarget = null;
			controlData.hasAnyControl = true;
		}
	}
}
