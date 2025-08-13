using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class MoveKeepDistanceWithEntity : MoveKeepDistance
	{
		public SharedEntity targetEntity;

		public float offsetRadius;

		public SharedFloat offsetAngle;

		public float steerSpeedRatio = 1f;

		protected RaycastHit _hit;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		protected override void UpdateTargetDistance()
		{
			if (updateDistance && !(targetEntity.Value == null) && targetEntity.Value.IsActive())
			{
				sharedDistance.SetValue(CalculateTargetDistance());
			}
		}

		protected override float CalculateTargetDistance()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 targetPostion = GetTargetPostion();
			return Vector3.Distance(xZPosition, targetPostion);
		}

		protected override Vector3 GetTargetDirection()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 targetPostion = GetTargetPostion();
			Vector3 vector = targetPostion - xZPosition;
			vector.y = 0f;
			vector.Normalize();
			return vector * steerSpeedRatio;
		}

		private Vector3 GetTargetPostion()
		{
			Vector3 xZPosition = targetEntity.Value.XZPosition;
			if (offsetRadius != 0f)
			{
				Vector3 vector = Quaternion.Euler(0f, offsetAngle.Value, 0f) * targetEntity.Value.transform.forward;
				Vector3 vector2 = Quaternion.Euler(0f, offsetAngle.Value, 0f) * targetEntity.Value.transform.forward;
				Debug.DrawLine(_aiEntity.RootNodePosition, _aiEntity.RootNodePosition + vector2 * distance, Color.red, distance);
				if (!Physics.Raycast(_aiEntity.RootNodePosition, vector2, out _hit, distance, (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER) | (1 << InLevelData.OBSTACLE_COLLIDER_LAYER)))
				{
					return xZPosition + vector * offsetRadius;
				}
				vector2 = Quaternion.Euler(0f, 0f - offsetAngle.Value, 0f) * targetEntity.Value.transform.forward;
				Debug.DrawLine(_aiEntity.RootNodePosition, _aiEntity.RootNodePosition + vector2 * distance, Color.red, distance);
				if (!Physics.Raycast(_aiEntity.RootNodePosition, vector2, out _hit, distance, (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER) | (1 << InLevelData.OBSTACLE_COLLIDER_LAYER)))
				{
					return xZPosition + -vector * offsetRadius;
				}
				return xZPosition += vector * offsetRadius;
			}
			return xZPosition;
		}
	}
}
