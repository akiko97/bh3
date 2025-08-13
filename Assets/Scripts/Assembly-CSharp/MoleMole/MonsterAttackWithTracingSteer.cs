using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class MonsterAttackWithTracingSteer : MonsterAttack
	{
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Index of skill IDs for monster to steer.")]
		public int steerSkillIDIndex;

		public float startNormalizedTime;

		public float endNormalizedTime;

		public bool moveBack;

		public float steerRatio = 3f;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override TaskStatus OnUpdate()
		{
			TaskStatus result = base.OnUpdate();
			if (_state == State.Doing && _skillIx == steerSkillIDIndex)
			{
				float currentNormalizedTime = _monster.GetCurrentNormalizedTime();
				if (currentNormalizedTime > startNormalizedTime && currentNormalizedTime < endNormalizedTime)
				{
					SteerStep();
				}
			}
			return result;
		}

		protected virtual void SteerStep()
		{
			Vector3 b = _monster.FaceDirection;
			if (_monster.AttackTarget != null)
			{
				int num = ((!moveBack) ? 1 : (-1));
				b = num * (_monster.AttackTarget.XZPosition - _monster.XZPosition);
				b.y = 0f;
				b.Normalize();
			}
			_monster.SteerFaceDirectionTo(Vector3.Slerp(_monster.FaceDirection, b, steerRatio * _monster.TimeScale * Time.deltaTime));
		}
	}
}
