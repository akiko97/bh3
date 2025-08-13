using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveFarThanDistance : BaseMove
	{
		public float distance;

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
			if (targetDistance > distance)
			{
				return TaskStatus.Success;
			}
			if (_aiEntity.GetProperty("AI_CanTeleport") > 0f)
			{
				TriggerEliteTeleport(targetDistance - distance);
				return TaskStatus.Success;
			}
			DoMoveBack();
			return TaskStatus.Running;
		}
	}
}
