using UnityEngine;

namespace MoleMole
{
	public interface IAIController
	{
		void SetActive(bool active);

		void TrySteer(Vector3 dir);

		void TrySteer(Vector3 dir, float lerpRatio);

		void TrySteerInstant(Vector3 dir);

		bool TryUseSkill(string skillName);

		void TryMove(float speed);

		void TryMoveHorizontal(float speed);

		void TryStop();

		void TrySetAttackTarget(BaseMonoEntity attackTarget);

		void TryClearAttackTarget();
	}
}
