using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class StopMove : Action
	{
		protected IAIEntity _aiEntity;

		protected IAIController _aiController;

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
		}

		public override TaskStatus OnUpdate()
		{
			_aiController.TryStop();
			return TaskStatus.Success;
		}
	}
}
