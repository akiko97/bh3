using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class UpdateCD : Action
	{
		public SharedFloat CD;

		public float defaultTime;

		public bool isRandom;

		public float minRandTime;

		public float maxRandTime;

		public bool keepUpdating;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("if NoUpdate < 0, CD will never update (for unique monster)")]
		public SharedFloat NoUpdate;

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
			if (isRandom)
			{
				CD.Value = Random.Range(minRandTime, maxRandTime);
			}
			else
			{
				CD.Value = defaultTime;
			}
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if ((keepUpdating || CD.Value >= 0f) && NoUpdate.Value >= 0f)
			{
				CD.Value -= Time.deltaTime * _aiEntity.TimeScale;
			}
			return TaskStatus.Running;
		}
	}
}
