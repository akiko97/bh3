using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveToFaceTarget : BaseMove
	{
		public enum SpeedDirection
		{
			Back = 0,
			Forward = 1,
			Auto = 2
		}

		public SpeedDirection speedDirection;

		public float autoDistanceThreshold;

		private bool _isAutoDirectionSet;

		public SharedString backSpeedKey;

		private float _backSpeed;

		public float angelThreshold = 3f;

		public float steerSpeedRatio = 1f;

		public float maxMoveTime = 2f;

		private float _timer;

		public override void OnAwake()
		{
			base.OnAwake();
			if (!string.IsNullOrEmpty(backSpeedKey.Value))
			{
				BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
				_backSpeed = (component as BaseMonoMonster).GetOriginMoveSpeed(backSpeedKey.Value);
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			_timer = maxMoveTime;
		}

		protected override float GetSteerRatio()
		{
			return base.GetSteerRatio() * steerSpeedRatio;
		}

		protected override TaskStatus OnMoveUpdate()
		{
			UpdateTargetDistance();
			if (_timer < 0f)
			{
				return TaskStatus.Success;
			}
			_timer -= Time.deltaTime * _aiEntity.TimeScale;
			if (CheckCollided())
			{
				return TaskStatus.Success;
			}
			if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
			{
				return TaskStatus.Success;
			}
			if (!_isAutoDirectionSet && speedDirection == SpeedDirection.Auto)
			{
				BaseMonoEntity attackTarget = _aiEntity.AttackTarget;
				Vector3 xZPosition = _aiEntity.XZPosition;
				Vector3 xZPosition2 = attackTarget.XZPosition;
				float num = Vector3.Distance(xZPosition, xZPosition2);
				if (num <= autoDistanceThreshold)
				{
					speedDirection = SpeedDirection.Back;
				}
				else
				{
					speedDirection = SpeedDirection.Forward;
				}
				_isAutoDirectionSet = true;
			}
			Vector3 targetDirection = GetTargetDirection();
			_aiController.TrySteer(targetDirection, GetSteerRatio());
			if (speedDirection == SpeedDirection.Back)
			{
				if (!string.IsNullOrEmpty(backSpeedKey.Value))
				{
					_aiController.TryMove(0f - _backSpeed);
				}
				else
				{
					DoMoveBack();
				}
			}
			else if (speedDirection == SpeedDirection.Forward)
			{
				DoMoveForward();
			}
			float f = Miscs.AngleFromToIgnoreY(_aiEntity.FaceDirection, targetDirection);
			float num2 = Mathf.Abs(f);
			if (num2 >= angelThreshold)
			{
				_aiController.TrySteer(targetDirection, GetSteerRatio());
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}
	}
}
