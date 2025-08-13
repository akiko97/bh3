using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class CheckNotBlockedByAllied : Action
	{
		private IAIEntity _aiEntity;

		public float distance;

		private RaycastHit _hit;

		public override void OnAwake()
		{
			_aiEntity = (IAIEntity)GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			Debug.DrawLine(_aiEntity.RootNodePosition, _aiEntity.RootNodePosition + _aiEntity.FaceDirection * distance, Color.red, 1f);
			if (Physics.Raycast(_aiEntity.RootNodePosition, _aiEntity.FaceDirection, out _hit, distance, (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER)))
			{
				if (_hit.collider.gameObject.layer != _aiEntity.transform.gameObject.layer)
				{
					return TaskStatus.Success;
				}
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}
	}
}
