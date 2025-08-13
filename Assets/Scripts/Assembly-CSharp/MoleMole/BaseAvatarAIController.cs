using System;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAvatarAIController : BaseAvatarController, IAIController
	{
		private const float AI_CHANGE_DIR_LERP_RATIO = 40f;

		public BaseAvatarAIController(BaseMonoEntity avatar)
			: base(avatar)
		{
		}

		public void TrySteer(Vector3 dir)
		{
			base.TrySteer(dir, 40f);
		}

		public new void TrySteer(Vector3 dir, float lerpRatio)
		{
			base.TrySteer(dir, 40f);
		}

		public void TrySteerInstant(Vector3 dir)
		{
		}

		public bool TryUseSkill(string skillName)
		{
			switch (skillName)
			{
			case "ATK":
				TryAttack();
				break;
			case "SKL01":
				TryUseSkill(1);
				break;
			case "SKL02":
				TryUseSkill(2);
				break;
			default:
				throw new Exception("Invalid Type or State!");
			}
			return true;
		}

		public void TryMove(float speed)
		{
			TryOrderMove(true);
		}

		public void TryStop()
		{
			TryOrderMove(false);
		}

		public void TryMoveHorizontal(float speed)
		{
		}

		void IAIController.TrySetAttackTarget(BaseMonoEntity attackTarget)
		{
			TrySetAttackTarget(attackTarget);
		}

		void IAIController.TryClearAttackTarget()
		{
			TryClearAttackTarget();
		}
	}
}
