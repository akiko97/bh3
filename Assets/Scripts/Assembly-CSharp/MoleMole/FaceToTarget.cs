using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class FaceToTarget : Action
	{
		public float angelThreshold = 3f;

		public float maxTurnTime = 2f;

		public float steerSpeedRatio = 1f;

		public bool forceAndInstant;

		public bool keepFacing;

		private float _timer;

		protected IAIEntity _aiEntity;

		private IAIController _aiController;

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoAvatar)
			{
				_aiEntity = (BaseMonoAvatar)component;
			}
			else if (component is BaseMonoMonster)
			{
				_aiEntity = (BaseMonoMonster)component;
			}
			_aiController = _aiEntity.GetActiveAIController();
		}

		public override void OnStart()
		{
			_timer = maxTurnTime;
		}

		public override TaskStatus OnUpdate()
		{
			if (_timer < 0f)
			{
				return TaskStatus.Success;
			}
			_timer -= Time.deltaTime * _aiEntity.TimeScale;
			if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
			{
				return TaskStatus.Success;
			}
			Vector3 targetFaceDir = GetTargetFaceDir();
			if (forceAndInstant)
			{
				BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
				component.SteerFaceDirectionTo(targetFaceDir);
				return TaskStatus.Success;
			}
			float f = Miscs.AngleFromToIgnoreY(_aiEntity.FaceDirection, targetFaceDir);
			float num = Mathf.Abs(f);
			if (num >= angelThreshold)
			{
				_aiController.TrySteer(targetFaceDir, steerSpeedRatio * (_aiEntity.GetProperty("Animator_MoveSpeedRatio") + 1f) * 1.5f);
				return TaskStatus.Running;
			}
			if (keepFacing)
			{
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}

		public virtual Vector3 GetTargetFaceDir()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = _aiEntity.AttackTarget.XZPosition;
			Vector3 result = xZPosition2 - xZPosition;
			result.y = 0f;
			result.Normalize();
			return result;
		}
	}
}
