using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class SetAIAnimatorBool : Action
	{
		public string boolName = string.Empty;

		public bool value;

		private BaseMonoAnimatorEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			if (_entity.IsActive())
			{
				_entity.SetLocomotionBool(boolName, value);
			}
			return TaskStatus.Success;
		}
	}
}
