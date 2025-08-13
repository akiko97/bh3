using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class MonsterAttackWithTracingSteerByLR : MonsterAttackWithTracingSteer
	{
		public enum SteerLimitState
		{
			OnlySteerLeft = 0,
			OnlySteerRight = 1
		}

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Steer Limit Type")]
		public SteerLimitState steerType;

		protected override void SteerStep()
		{
			Vector3 faceDirection = _monster.FaceDirection;
			if (!(_monster.AttackTarget != null))
			{
				return;
			}
			faceDirection = _monster.AttackTarget.XZPosition - _monster.XZPosition;
			faceDirection.y = 0f;
			faceDirection.Normalize();
			Debug.DrawLine(_monster.transform.position, _monster.transform.position + faceDirection * 10f, Color.blue);
			Debug.DrawLine(_monster.transform.position, _monster.transform.position + _monster.FaceDirection * 10f, Color.red);
			faceDirection = _monster.FaceDirection + faceDirection;
			float num = Miscs.AngleFromToIgnoreY(_monster.FaceDirection, faceDirection);
			if (steerType == SteerLimitState.OnlySteerLeft)
			{
				if (num > 0f)
				{
					faceDirection = -faceDirection;
				}
			}
			else if (steerType == SteerLimitState.OnlySteerRight && num < 0f)
			{
				faceDirection = -faceDirection;
			}
			Debug.DrawLine(_monster.transform.position, _monster.transform.position + faceDirection * 10f, Color.yellow);
			_monster.SteerFaceDirectionTo(Vector3.Slerp(_monster.FaceDirection, faceDirection, steerRatio * _monster.TimeScale * Time.deltaTime));
		}
	}
}
