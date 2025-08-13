using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskIcon("{SkinColor}WaitIcon.png")]
	[TaskDescription("Wait a specified amount of time. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
	public class Wait : Action
	{
		public float waitTime = 1f;

		public float randomAddRange;

		private float _timer;

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
			_timer = waitTime + randomAddRange * Random.value;
		}

		public override TaskStatus OnUpdate()
		{
			if (_timer < 0f)
			{
				return TaskStatus.Success;
			}
			_timer -= Time.deltaTime * _aiEntity.TimeScale;
			return TaskStatus.Running;
		}
	}
}
