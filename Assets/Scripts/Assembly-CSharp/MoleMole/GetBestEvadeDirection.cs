using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class GetBestEvadeDirection : Action
	{
		private IAIEntity _aiEntity;

		public float distance;

		public int TryAngleNum = 6;

		public SharedFloat DirectionAngle;

		protected RaycastHit _hit;

		public override void OnAwake()
		{
			_aiEntity = (IAIEntity)GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			if (_aiEntity.AttackTarget != null)
			{
				Vector3 vector = _aiEntity.XZPosition - _aiEntity.AttackTarget.XZPosition;
				int num = 1;
				for (int i = 0; i < TryAngleNum; i++)
				{
					num = -num;
					Vector3 vector2 = Quaternion.Euler(0f, (float)num * (360f / (float)TryAngleNum) * Mathf.Floor((i + 1) / 2), 0f) * vector;
					Debug.DrawLine(_aiEntity.RootNodePosition, _aiEntity.RootNodePosition + vector2 * distance, Color.red, distance);
					if (!Physics.Raycast(_aiEntity.RootNodePosition, vector2, out _hit, distance, (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER) | (1 << InLevelData.OBSTACLE_COLLIDER_LAYER)))
					{
						if (Vector3.Cross(_aiEntity.FaceDirection, vector2).y > 0f)
						{
							DirectionAngle.SetValue(Vector3.Angle(_aiEntity.FaceDirection, vector2));
						}
						else
						{
							DirectionAngle.SetValue(0f - Vector3.Angle(_aiEntity.FaceDirection, vector2));
						}
						return TaskStatus.Success;
					}
				}
			}
			DirectionAngle.SetValue(Random.Range(0f, 360f));
			return TaskStatus.Success;
		}
	}
}
