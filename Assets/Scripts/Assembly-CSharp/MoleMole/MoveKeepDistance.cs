using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveKeepDistance : BaseMove
	{
		public SharedString backSpeedKey;

		private float _backSpeed;

		public float distance;

		public float distancePadding;

		public float distanceRandomAdd;

		private float _distance;

		public override void OnAwake()
		{
			base.OnAwake();
			if (!string.IsNullOrEmpty(backSpeedKey.Value))
			{
				BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
				_backSpeed = (component as BaseMonoMonster).GetOriginMoveSpeed(backSpeedKey.Value);
			}
			_distance = distance + Random.Range(0f, distanceRandomAdd);
		}

		protected override TaskStatus OnMoveUpdate()
		{
			UpdateTargetDistance();
			if (CheckCollided())
			{
				return TaskStatus.Success;
			}
			if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
			{
				return TaskStatus.Success;
			}
			DoFaceToTarget();
			float targetDistance = GetTargetDistance();
			if (targetDistance > _distance + distancePadding)
			{
				if (_aiEntity.GetProperty("AI_CanTeleport") > 0f)
				{
					TriggerEliteTeleport(targetDistance - _distance);
				}
				DoMoveForward();
				return TaskStatus.Running;
			}
			if (targetDistance < _distance - distancePadding)
			{
				if (_aiEntity.GetProperty("AI_CanTeleport") > 0f)
				{
					TriggerEliteTeleport(targetDistance - _distance);
				}
				if (!string.IsNullOrEmpty(backSpeedKey.Value))
				{
					_aiController.TryMove(0f - _backSpeed);
				}
				else
				{
					DoMoveBack();
				}
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}
	}
}
