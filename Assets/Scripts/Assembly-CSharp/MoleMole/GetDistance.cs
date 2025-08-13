using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class GetDistance : Action
	{
		public SharedFloat Distance;

		private IAIEntity _aiEntity;

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
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
			{
				return TaskStatus.Success;
			}
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = _aiEntity.AttackTarget.XZPosition;
			Distance.SetValue(Vector3.Distance(xZPosition, xZPosition2));
			return TaskStatus.Success;
		}
	}
}
