using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class MonsterAttackWithFixedSteer : MonsterAttack
	{
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Index of skill IDs for monster to steer.")]
		public int steerSkillIDIndex;

		public float startNormalizedTime;

		public float endNormalizedTime;

		public bool moveBack;

		public float steerRatio = 3f;

		protected Vector3 _targetForward;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnTransit(State from, State to)
		{
			if (to == State.Doing)
			{
				CalcTargetForward();
			}
		}

		protected void CalcTargetForward()
		{
			if (_monster.AttackTarget != null)
			{
				int num = ((!moveBack) ? 1 : (-1));
				_targetForward = num * (_monster.AttackTarget.XZPosition - _monster.XZPosition);
				_targetForward.Normalize();
			}
			else
			{
				_targetForward = _monster.FaceDirection;
			}
		}

		public override TaskStatus OnUpdate()
		{
			TaskStatus result = base.OnUpdate();
			if (_state == State.Doing && _skillIx == steerSkillIDIndex)
			{
				float currentNormalizedTime = _monster.GetCurrentNormalizedTime();
				if (currentNormalizedTime > startNormalizedTime && currentNormalizedTime < endNormalizedTime)
				{
					_monster.SteerFaceDirectionTo(Vector3.Slerp(_monster.FaceDirection, _targetForward, steerRatio * _monster.TimeScale * Time.deltaTime));
				}
			}
			return result;
		}
	}
}
