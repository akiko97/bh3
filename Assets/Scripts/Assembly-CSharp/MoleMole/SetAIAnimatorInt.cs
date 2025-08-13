using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class SetAIAnimatorInt : Action
	{
		public string intName = string.Empty;

		public int value;

		private BaseMonoAnimatorEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			if (_entity.IsActive())
			{
				_entity.SetLocomotionInteger(intName, value);
			}
			return TaskStatus.Success;
		}
	}
}
