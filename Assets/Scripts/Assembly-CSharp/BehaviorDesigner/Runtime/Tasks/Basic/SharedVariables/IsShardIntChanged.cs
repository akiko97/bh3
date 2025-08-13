namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is changed.")]
	public class IsShardIntChanged : Conditional
	{
		[Tooltip("The first variable to check")]
		public SharedInt variable;

		private int _originalValue;

		public override void OnAwake()
		{
			_originalValue = variable.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (_originalValue != variable.Value)
			{
				_originalValue = variable.Value;
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			variable = 0;
			_originalValue = 0;
		}
	}
}
