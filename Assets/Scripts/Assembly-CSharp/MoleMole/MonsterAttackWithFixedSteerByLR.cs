using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class MonsterAttackWithFixedSteerByLR : MonsterAttackWithFixedSteer
	{
		public string attackTypeL;

		private bool _isAttackTypeL;

		protected override void DoCalcSteer()
		{
			CalcTargetForward();
			CalcIsAttackTypeL();
		}

		private void CalcIsAttackTypeL()
		{
			Vector3 rhs = new Vector3(_targetForward.x, 0f, _targetForward.z);
			Vector3 lhs = new Vector3(_monster.FaceDirection.x, 0f, _monster.FaceDirection.z);
			float num = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(lhs, rhs)));
			_isAttackTypeL = ((num <= 0f) ? true : false);
		}

		protected override void OnTransit(State from, State to)
		{
		}

		protected override bool DoAttack()
		{
			if (_isAttackTypeL)
			{
				return _controller.TryUseSkill(attackTypeL);
			}
			return _controller.TryUseSkill(attackType);
		}
	}
}
